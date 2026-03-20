using System.ComponentModel.DataAnnotations;

namespace DynamicSecretsManagement
{
    // Options class
    public class StripeOptions
    {
        public const string SectionName = "StripeOptions";

        [Required, MinLength(10)]
        public string SecretKey { get; set; } = string.Empty;

        [Required]
        public string PublishableKey { get; set; } = string.Empty;

        [Range(1, 100)]
        public int MaxRetries { get; set; }
    }
}
