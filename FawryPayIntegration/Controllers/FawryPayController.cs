using FawryPayIntegration.Dto;
using FawryPayIntegration.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FawryPayIntegration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FawryPayController : ControllerBase
    {
        private readonly Plan _fawrySettings;
        private readonly IGenerateFawrySignature _generateFawrySignature;
        public FawryPayController(IOptions<Plan> fawrySettings,IGenerateFawrySignature generateFawrySignature)
        {
            _fawrySettings = fawrySettings.Value;
            _generateFawrySignature= generateFawrySignature;
        }
        [HttpPost("PostFawry")]
        public IActionResult PostFawry([FromBody] OnlineOrder order)
        {
            string lang = Request.Path.Value.Contains("/ar/") ? "AR" : "EN";
            string pathLang = order.lang == "AR" ? "ar/" : "";

            var plan = new Plan
            {
                Merchant = _fawrySettings.Merchant,
                SecretKey = _fawrySettings.SecretKey,
            };

            // Generate the FawryPay signature
            string signature = _generateFawrySignature.GenerateFawrySign(order, plan, pathLang);
            string returnUrl = $"http://localhost:32453/{pathLang}/eshop.aspx?RN={order.order_reference}&action=r";

            // Build the FawryPay charge request payload
            var chargeRequest = new
            {
                merchantCode = _fawrySettings.Merchant,
                merchantRefNum = order.order_reference,
                customerMobile = order.customer_phone,
                customerEmail = order.customer_email,
                customerName = order.customer_name,
                customerProfileId = string.Empty,
                paymentExpiry = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 3600000,
                chargeItems = new[]
                {
                    new
                    {
                        itemId = order.package_id.ToString(),
                        description = "Package Description",
                        price = order.package_price.ToString("0.00"),
                        quantity = 1
                    },
                    new
                    {
                        itemId = order.decoder_id.ToString(),
                        description = "Hardware Description",
                        price = order.hardware_price.ToString("0.00"),
                        quantity = 1
                    }
                },
                paymentMethod = "CARD",
                returnUrl = returnUrl,
                authCaptureModePayment = false,
                signature = signature
            };

            // Return the charge request data as JSON
            return Ok(chargeRequest);
        }


    }
}
