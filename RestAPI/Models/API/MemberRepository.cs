using Payslip.Utils;
using System;
using System.Collections.Generic;
using System.Data;

namespace RestAPI.Models.API
{
    public class MemberRepository
    {
        private MemberRepository() { }
        private static MemberRepository instance = null;

        public static MemberRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MemberRepository();
                }
                return instance;
            }
        }

        public IEnumerable<Members> GetMemberbyId(string id)
        {
            string query = string.Format("SELECT * FROM members WHERE id = {0}", id);
            DataContextFetch dcf = new DataContextFetch();
            DataRow dr = dcf.Get(query);

            var member = new Members
            {
                id = Int32.Parse(dr["id"].ToString()),
                title = dr["title"].ToString(),
                name = dr["name"].ToString(),
                first_nric = dr["first_nric"].ToString(),
                no_nric = dr["no_nric"].ToString(),
                last_nric = dr["last_nric"].ToString(),
                no_mobile = dr["no_mobile"].ToString(),
                email = dr["email"].ToString(),
                postal_code = dr["postal_code"].ToString(),
                promo_code = dr["promo_code"].ToString(),
                created_at = dr["created_at"].ToString(),
                updated_at = dr["updated_at"].ToString()
            };
            
            yield return member;
        }
    }
}