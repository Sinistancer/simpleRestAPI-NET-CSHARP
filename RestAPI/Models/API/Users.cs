using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestAPI.Models.API
{
    public class Users
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string remember_token { get; set; }
        public int is_active { get; set; }
    }
}