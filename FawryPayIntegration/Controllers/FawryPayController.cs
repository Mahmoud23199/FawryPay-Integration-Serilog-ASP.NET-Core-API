using FawryPayIntegration.Contract;
using FawryPayIntegration.Dto;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.Http;
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
        private readonly ILogger<FawryPayController> _logger;
        public FawryPayController(IOptions<Plan> fawrySettings,IGenerateFawrySignature generateFawrySignature, ILogger<FawryPayController> logger)
        {
            _fawrySettings = fawrySettings.Value;
            _generateFawrySignature = generateFawrySignature;
            _logger = logger;
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

            try
            {
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
                _logger.LogError("Payment posted successfully. at {Time}\n customer_name: {customer_name}\n customer_phone: {customer_phone}\n order_reference: {order_reference}\n package_id: {package_id}\n decoder_id: {decoder_id}",
                                 DateTime.UtcNow,
                                 order.customer_name,
                                 order.customer_phone,
                                 order.order_reference, order.package_id, order.decoder_id);
                // Return the charge request data as JSON
                return Ok(new
                {
                    success = true,
                    statusCode = StatusCodes.Status200OK,
                    message = "Payment posted successfully.",
                    data = chargeRequest
                });

            } catch (Exception ex)
            {
                
                _logger.LogError(ex.Message, "Exception occurred at {Time}\n customer_name: {customer_name}\n customer_phone: {customer_phone}\n order_reference: {order_reference}\n package_id: {package_id}\n decoder_id: {decoder_id}",
                                    DateTime.UtcNow,
                                    order.customer_name,
                                    order.customer_phone,
                                    order.order_reference,order.package_id,order.decoder_id);

                return BadRequest(new
                {
                    success = false,
                    statusCode = StatusCodes.Status500InternalServerError,
                    message = "Payment failed. Our call center team will contact you as soon as possible.",
                    errorDetails = new
                    {
                        errorCode = "PAYMENT_FAILURE",
                        timestamp = DateTime.UtcNow
                    }
                });
            }
           
        }


    }
}
