using Newtonsoft.Json.Linq;
using Payslip.Utils;
using RazHeaderAttribute.Attributes;
using RestAPI.Models.API;
using RM.Models.API;
using System;
using System.Linq;
using System.Web.Http;

namespace RestAPI.Controllers.API
{
    public class MembershipController : ApiController
    {
        [Route("api/v1/member/register")]
        [HttpPost]
        public IHttpActionResult register([FromHeader("token")] string token, [FromHeader("email")] string email, [FromBody] object form)
        {
            var users = UserRepository.Instance.GetUserbyHeader(email, token);
            var getData = users.Where(x => x.email == email && x.remember_token == token).ToList();
            
            string[] data = new string[] { };
            string messages = "";

            if (getData.Count() > 0 && getData[0].is_active > 0)
            {
                JObject obj = JObject.Parse(form.ToString());                
                try
                {
                    DataContextFetch dcf = new DataContextFetch();
                    dcf.tableName = "members";
                    dcf.AddColumnValues("title", (string)obj["title"], false);
                    dcf.AddColumnValues("name", (string)obj["name"], false);
                    dcf.AddColumnValues("first_nric", (string)obj["first_nric"], false);
                    dcf.AddColumnValues("no_nric", (string)obj["no_nric"], false);
                    dcf.AddColumnValues("last_nric", (string)obj["last_nric"], false);
                    dcf.AddColumnValues("no_mobile", (string)obj["no_mobile"], false);
                    dcf.AddColumnValues("email", (string)obj["email"], false);
                    dcf.AddColumnValues("postal_code", (string)obj["postal_code"], false);
                    dcf.AddColumnValues("promo_code", (string)obj["promo_code"], false);
                    dcf.AddColumnValues("created_at", DateTime.Now, false);
                    dcf.ExecuteInsert();

                    messages = "Success Insert Data";
                }
                catch (Exception e)
                {
                    messages = "Failed Insert Data " + e.Message;
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
                messages = "Not Authorized";

                object jsons = new OutputsModel(
                    System.Net.HttpStatusCode.OK.GetHashCode(),
                    System.Net.HttpStatusCode.OK.ToString(),
                    messages,
                    data
                );

                return Json(jsons);
            }
        }

        [Route("api/v1/member/update/{id}")]
        [HttpPut]
        public IHttpActionResult update([FromHeader("token")] string token, [FromHeader("email")] string email, [FromBody] object form, string id)
        {
            var users = UserRepository.Instance.GetUserbyHeader(email, token);
            var getData = users.Where(x => x.email == email && x.remember_token == token).ToList();

            string[] data = new string[] { };
            string messages = "";

            if (getData.Count() > 0 && getData[0].is_active > 0)
            {
                JObject obj = JObject.Parse(form.ToString());
                try
                {
                    DataContextFetch dcf = new DataContextFetch();
                    dcf.tableName = "members";
                    dcf.AddColumnValues("id", id, true);
                    dcf.AddColumnValues("title", (string)obj["title"], false);
                    dcf.AddColumnValues("name", (string)obj["name"], false);
                    dcf.AddColumnValues("first_nric", (string)obj["first_nric"], false);
                    dcf.AddColumnValues("no_nric", (string)obj["no_nric"], false);
                    dcf.AddColumnValues("last_nric", (string)obj["last_nric"], false);
                    dcf.AddColumnValues("no_mobile", (string)obj["no_mobile"], false);
                    dcf.AddColumnValues("email", (string)obj["email"], false);
                    dcf.AddColumnValues("postal_code", (string)obj["postal_code"], false);
                    dcf.AddColumnValues("promo_code", (string)obj["promo_code"], false);
                    dcf.AddColumnValues("updated_at", DateTime.Now, false);
                    dcf.ExecuteInsertOrUpdatebyExist();

                    messages = "Success Update Data";
                }
                catch (Exception e)
                {
                    messages = "Failed Update Data " + e.Message;
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
                messages = "Not Authorized";

                object jsons = new OutputsModel(
                    System.Net.HttpStatusCode.OK.GetHashCode(),
                    System.Net.HttpStatusCode.OK.ToString(),
                    messages,
                    data
                );

                return Json(jsons);
            }
        }

        [Route("api/v1/member/delete/{id}")]
        [HttpDelete]
        public IHttpActionResult delete([FromHeader("token")] string token, [FromHeader("email")] string email, string id)
        {
            var users = UserRepository.Instance.GetUserbyHeader(email, token);
            var getData = users.Where(x => x.email == email && x.remember_token == token).ToList();

            string[] data = new string[] { };
            string messages = "";

            if (getData.Count() > 0 && getData[0].is_active > 0)
            {
                try
                {
                    DataContextFetch dcf = new DataContextFetch();
                    dcf.tableName = "members";
                    dcf.AddColumnValues("id", id, true);
                    dcf.ExecuteDelete();

                    messages = "Success Delete Data";
                }
                catch (Exception e)
                {
                    messages = "Failed Delete Data " + e.Message;
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
                messages = "Not Authorized";

                object jsons = new OutputsModel(
                    System.Net.HttpStatusCode.OK.GetHashCode(),
                    System.Net.HttpStatusCode.OK.ToString(),
                    messages,
                    data
                );

                return Json(jsons);
            }
        }

        [Route("api/v1/member/{id}")]
        [HttpGet]
        public IHttpActionResult GetMember([FromHeader("token")] string token, [FromHeader("email")] string email, string id)
        {
            var users = UserRepository.Instance.GetUserbyHeader(email, token);
            var getData = users.Where(x => x.email == email && x.remember_token == token).ToList();

            string[] data = new string[] { };
            string messages = "";

            if (getData.Count() > 0 && getData[0].is_active > 0)
            {
                var member = MemberRepository.Instance.GetMemberbyId(id);
                messages = member.Count() > 0 ? "Success Get Data" : "Data Not Found";

                object jsons = new OutputsModel(
                    System.Net.HttpStatusCode.OK.GetHashCode(),
                    System.Net.HttpStatusCode.OK.ToString(),
                    messages,
                    member
                );

                return Json(jsons);
            }
            else
            {
                messages = "Not Authorized";

                object jsons = new OutputsModel(
                    System.Net.HttpStatusCode.OK.GetHashCode(),
                    System.Net.HttpStatusCode.OK.ToString(),
                    messages,
                    data
                );

                return Json(jsons);
            }
        }
    }
}
