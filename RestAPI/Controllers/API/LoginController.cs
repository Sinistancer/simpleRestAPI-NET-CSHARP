using RestAPI.Models.API;
using System;
using System.Linq;
using System.Web.Http;
using RM.Models.API;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using Payslip.Utils;
using RazHeaderAttribute.Attributes;

namespace RestAPI.Controllers.API
{
    public class LoginController : ApiController
    {
        [Route("api/v1/login")]
        [HttpPost]
        public IHttpActionResult login([FromBody] object form)
        {
            JObject obj = JObject.Parse(form.ToString());

            string email = (string)obj["email"];
            var users = UserRepository.Instance.GetUser(email);
            var getData = users.Where(x => x.email == email).ToList();
            
            string passwordEncrypt = getData[0].password;

            string password = (string)obj["password"];
            password = GenerateSHA256String(password);

            if (password == passwordEncrypt)
            {
                // Define const Key this should be private secret key  stored in some safe place
                string key = "C419rghdDDEbc7xvGIlfy8vgWjyPL0Li";

                // Create Security key  using private key above:
                // not that latest version of JWT using Microsoft namespace instead of System
                var securityKey = new Microsoft
                   .IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

                // Also note that securityKey length should be >256b
                // so you have to make sure that your private key has a proper length
                //
                var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials
                                  (securityKey, SecurityAlgorithms.HmacSha256Signature);

                //  Finally create a Token
                var header = new JwtHeader(credentials);

                //Some PayLoad that contain information about the  customer
                string name = getData[0].name;
                string is_active = getData[0].is_active.ToString();
                string date = DateTime.Now.ToString();

                var payload = new JwtPayload
                {
                    { name, email },
                    { is_active, date },
                };
                
                var secToken = new JwtSecurityToken(header, payload);
                var handler = new JwtSecurityTokenHandler();

                // Token to String so you can use it in your client
                var tokenString = handler.WriteToken(secToken);

                //// And finally when  you received token from client
                //// you can  either validate it or try to  read
                //var token = handler.ReadJwtToken(tokenString);

                DataContextFetch dcf = new DataContextFetch();
                dcf.tableName = "users";
                dcf.AddColumnValues("email", email, false);
                dcf.AddColumnValues("remember_token", tokenString.ToString(), false);
                dcf.AddColumnValues("is_active", 1, false);
                dcf.AddColumnValues("updated_at", DateTime.Now, false);
                dcf.ExecuteInsertOrUpdatebyExist();

                string messages = "Success login";
                Users data = new Users
                {
                    name = name,
                    email = email,
                    remember_token = tokenString,
                    is_active = Int32.Parse(is_active)
                };


                object jsons = new OutputsModel(
                    System.Net.HttpStatusCode.OK.GetHashCode(),
                    System.Net.HttpStatusCode.OK.ToString(),
                    messages,
                    data
                );

                return Json(jsons);
            }
            else
            {
                string[] data = new string[] { };

                string messages = "wrong password";

                object jsons = new OutputsModel(
                    System.Net.HttpStatusCode.OK.GetHashCode(),
                    System.Net.HttpStatusCode.OK.ToString(),
                    messages,
                    data
                );

                return Json(jsons);
            }
        }

        [Route("api/v1/logout")]
        [HttpPost]
        public IHttpActionResult logout([FromHeader("email")] string email, [FromHeader("token")] string token)
        {
            string[] data = new string[] { };

            var users = UserRepository.Instance.GetUserbyHeader(email, token);
            var getData = users.Where(x => x.email == email && x.remember_token == token).ToList();
            string messages = "";

            if(getData.Count() > 0)
            {
                try
                {
                    DataContextFetch dcf = new DataContextFetch();
                    dcf.tableName = "users";
                    dcf.AddColumnValues("id", getData[0].id, true);
                    dcf.AddColumnValues("is_active", 0, false);
                    dcf.AddColumnValues("updated_at", DateTime.Now, false);
                    dcf.ExecuteInsertOrUpdatebyExist();

                    messages = "Logout Success";
                }
                catch (Exception e)
                {
                    messages = "Logout Failed " + e.Message;
                }

                object jsons = new OutputsModel(
                    System.Net.HttpStatusCode.OK.GetHashCode(),
                    System.Net.HttpStatusCode.OK.ToString(),
                    messages,
                    data
                );

                return Json(jsons);
            }
            else
            {
                messages = "Data Not Found";

                object jsons = new OutputsModel(
                    System.Net.HttpStatusCode.OK.GetHashCode(),
                    System.Net.HttpStatusCode.OK.ToString(),
                    messages,
                    data
                );

                return Json(jsons);
            }
        }

        public static string GenerateSHA256String(string inputString)
        {
            SHA256 sha256 = SHA256Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha256.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }

        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }
    }
}
