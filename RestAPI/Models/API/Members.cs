namespace RestAPI.Models.API
{
    public class Members
    {
        public int id { get; set; }
        public string title { get; set; }
        public string name { get; set; }
        public string first_nric { get; set; }
        public string no_nric { get; set; }
        public string last_nric { get; set; }
        public string no_mobile { get; set; }
        public string email { get; set; }
        public string postal_code { get; set; }
        public string promo_code { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }
}