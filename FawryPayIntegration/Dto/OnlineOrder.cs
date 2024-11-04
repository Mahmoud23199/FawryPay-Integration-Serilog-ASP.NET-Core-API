namespace FawryPayIntegration.Dto
{
    public class OnlineOrder
    {
        public string order_reference { get; set; }
        public string lang { get; set; }
        public string customer_phone { get; set; }
        public string customer_email { get; set; }
        public string customer_name { get; set; }
        public int package_id { get; set; }
        public decimal package_price { get; set; }
        public int decoder_id { get; set; }
        public decimal hardware_price { get; set; }
    }
}
