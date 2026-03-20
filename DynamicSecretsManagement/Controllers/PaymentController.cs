using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DynamicSecretsManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly StripeOptions _options;
        private readonly StripeOptions _snapshot;
        private readonly IOptionsMonitor<StripeOptions> _monitor;

        public PaymentController(
            IOptions<StripeOptions> options,
            IOptionsSnapshot<StripeOptions> snapshot,
            IOptionsMonitor<StripeOptions> monitor)
        {
            _options = options.Value;
            _snapshot = snapshot.Value;
            _monitor = monitor;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                StartupValue = _options.MaxRetries,
                RequestValue = _snapshot.MaxRetries,
                RealTimeValue = _monitor.CurrentValue.MaxRetries
            });
        }
    }
}
