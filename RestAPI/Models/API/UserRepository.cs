using Payslip.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RestAPI.Models.API
{
    public class UserRepository
    {
        private UserRepository() { }
        private static UserRepository instance = null;

        public static UserRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UserRepository();
                }
                return instance;
            }
        }

        public IEnumerable<Users> GetUser(string email)
        {
            string query = string.Format("SELECT * FROM users WHERE email = '{0}'", email);
            DataContextFetch dcf = new DataContextFetch();
            DataRow dr = dcf.Get(query);

            var user = new Users
            {
                id = Int32.Parse(dr["id"].ToString()),
                name = dr["name"].ToString(),
                email = dr["email"].ToString(),
                password = dr["password"].ToString(),
                remember_token = dr["remember_token"].ToString(),
                is_active = Int32.Parse(dr["is_active"].ToString())
            };
            
            yield return user;
        }

        public IEnumerable<Users> GetUserbyHeader(string email, string token)
        {
            object[] args = new object[] { email, token };

            string query = string.Format("SELECT * FROM users WHERE email = '{0}' AND remember_token = '{1}'", args);
            DataContextFetch dcf = new DataContextFetch();
            DataRow dr = dcf.Get(query);

            var id = Int32.Parse(dr["id"].ToString());

            var user = new Users { };
            
            if(id > 0)
            {
                user = new Users
                {
                    id = Int32.Parse(dr["id"].ToString()),
                    name = dr["name"].ToString(),
                    email = dr["email"].ToString(),
                    password = dr["password"].ToString(),
                    remember_token = dr["remember_token"].ToString(),
                    is_active = Int32.Parse(dr["is_active"].ToString())
                };
            }         
            
            yield return user;
        }
    }
}