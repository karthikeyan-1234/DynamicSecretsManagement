##Architectural Summary##
This project is a .NET 9 ASP.NET Core Web API that demonstrates dynamic secret management patterns on Kubernetes, progressing from basic to production-leaning approaches.

At system level, the core objective is to prove how application configuration (specifically StripeOptions) behaves under different secret delivery mechanisms, and how quickly the app can reflect rotations at runtime.

#What The Solution Achieves
Validates three secret delivery models
Env-var injection (deployment-minimal.yaml) for baseline simplicity.
Kubernetes Secret volume mount (deployment-volumeMount.yml) for file-based secret refresh.
HashiCorp Vault Agent sidecar (deployment-vault-agent.yml) for dynamic, continuously rendered secrets.
Demonstrates runtime config reloading semantics
Uses IOptions, IOptionsSnapshot, and IOptionsMonitor in PaymentController to show stale vs refreshed values.
Uses polling file provider in Program.cs to detect mounted secret file updates.
Shows platform bootstrap for Vault on Kubernetes
Infra + RBAC + init automation in vault-infrastructure.yml, vault-rbac.yml, vault-init-job.yml.
Establishes Vault Kubernetes auth path and policy-based access to secret paths.
Provides containerized deployment path
Docker multi-stage build (Dockerfile) and K8s manifests for different runtime modes.
Functional Scope (Current)
API capability
/api/payment: returns effective Stripe retry config through different options lifetimes.
/weatherforecast: template endpoint (non-core).
Configuration behavior
Layered config from appsettings.json + mounted /app/secrets/appsettings.json.
Startup validation via options binding and data annotations.
Deployment variants
Supports learning/demo progression from static secret injection to dynamic sidecar-driven rotation.
