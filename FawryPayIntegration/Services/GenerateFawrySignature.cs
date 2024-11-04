using FawryPayIntegration.Dto;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace FawryPayIntegration.Services
{
    public class GenerateFawrySignature
    {
        public static string GenerateFawrySign(OnlineOrder order, Plan plan, string pathLang)
        {
            string data = plan.Merchant + order.order_reference;
            string returnUrl = $"http://localhost:32453/{pathLang}/eshop.aspx?RN={order.order_reference}&action=r";
            data += returnUrl;

            var items = new List<Tuple<int, string, string>>
            {
                new Tuple<int, string, string>(order.package_id, "1", order.package_price.ToString("0.00")),
                new Tuple<int, string, string>(order.decoder_id, "1", order.hardware_price.ToString("0.00"))
            }.OrderBy(item => item.Item1).ToList();

            foreach (var item in items)
            {
                data += item.Item1 + item.Item2 + item.Item3;
            }

            data += plan.SecretKey;

            using var sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));

            return string.Concat(hash.Select(b => b.ToString("x2")));
        }
    }
}
