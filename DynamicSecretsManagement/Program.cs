using DynamicSecretsManagement;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DynamicSecrets API", Version = "v1" });
});

// -------------------------------------------------------------------------
// Layer 1 – base appsettings.json (baked into the image)
// -------------------------------------------------------------------------
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// -------------------------------------------------------------------------
// Layer 2 – secret volume mount  (/app/secrets/appsettings.json)
//
// WHY a polling provider?
//   Kubernetes secret volume updates work by swapping a symlink
//   (..data → new timestamped directory) rather than modifying the file
//   in-place.  .NET's default FileSystemWatcher listens for inotify
//   "modify" events and therefore misses symlink swaps entirely, so
//   reloadOnChange:true silently does nothing with the default provider.
//
//   Switching to UsePollingFileWatcher + UseActivePolling makes the
//   PhysicalFileProvider stat the file on a timer (default 4 s) and push
//   change tokens when the mtime/size differs – which works correctly
//   with the symlink-swap strategy.
//
// FUTURE:  When HashiCorp Vault Agent is introduced it will write a real
//   file to this same mount path, so no further changes will be needed
//   here – the polling provider will pick up those writes too.
// -------------------------------------------------------------------------
const string SecretMountPath = "/app/secrets";
const string SecretFileName = "appsettings.json";

var secretsFileProvider = new PhysicalFileProvider(SecretMountPath)
{
    // Poll the filesystem instead of relying on inotify/FSW
    UsePollingFileWatcher = true,
    // Actively push change notifications rather than waiting for consumers
    UseActivePolling = true
};

builder.Configuration.AddJsonFile(
    provider: secretsFileProvider,
    path: SecretFileName,
    optional: true,   // pod still starts even if the mount is absent
    reloadOnChange: true);

// -------------------------------------------------------------------------
// Bind StripeOptions – validated eagerly so a bad secret fails fast
// -------------------------------------------------------------------------
builder.Services.AddOptions<StripeOptions>()
    .Bind(builder.Configuration.GetSection("StripeOptions"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();