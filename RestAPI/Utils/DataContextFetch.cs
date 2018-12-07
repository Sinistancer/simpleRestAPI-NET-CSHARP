using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Diagnostics;

namespace Payslip.Utils
{
    public class DataContextFetch
    {
        string connectionString;

        public string tableName { get; set; }
        public string procedureName { get; set; }

        //public List<Account> AccountColumn { get; set; }
        public List<DCColumn> ListColumn { get; set; }
        public List<DCStoreParam> ListStoreParam { get; set; }
        private Dictionary<string, string> ListFilter { get; set; }

        public DataContextFetch(string Database="Database")
        {
            connectionString = ConfigurationManager.ConnectionStrings[Database].ConnectionString;
            ListColumn = new List<DCColumn>();
            ListStoreParam = new List<DCStoreParam>();
        }

        public void AddColumnValues(string ColumnName, object ColumnValue, bool isPK)
        {
            if (ColumnValue != null)
            {
                DCColumn temp = new DCColumn();
                temp.ColumnName = ColumnName;
                temp.ColumnValueObj = ColumnValue;
                temp.isPK = isPK;
                ListColumn.Add(temp);
            }
        }

        public void AddStoreParam(string ParamName, object ParamValue)
        {
            DCStoreParam temp = new DCStoreParam();
            temp.ParamName = ParamName;
            temp.ParamValueObj = ParamValue;
            ListStoreParam.Add(temp);
        }

        public void ExecuteQueryInsertUpdate()
        {
            DCColumn colModified = ListColumn.Where(s => s.ColumnName == "modifiedOn").FirstOrDefault();
            if (colModified != null)
            {
                string queryWhere = "";
                string queryInsert = GetInsertQuery();
                string queryUpdate = GetUpdateQuery();

                foreach (var a in ListColumn.Where(s => s.isPK == true))
                    queryWhere = (string.IsNullOrEmpty(queryWhere) ? "WHERE" : "AND") + string.Format(" {0} = '{1}' ", a.ColumnName, a.ColumnValue);

                string query = "declare @oldmoddate datetime = null " + Environment.NewLine;
                query += string.Format("select top 1 @oldmoddate = modifiedOn from {0} {1}", this.tableName, queryWhere);
                query += "IF(@oldmoddate IS NULL)" + Environment.NewLine;
                query += "  BEGIN" + Environment.NewLine;
                query += "      " + queryInsert + Environment.NewLine;
                query += "  END" + Environment.NewLine;
                query += "ELSE IF(@oldmoddate < '" + colModified.ColumnValue + "' )" + Environment.NewLine;
                query += "  BEGIN" + Environment.NewLine;
                query += "      " + queryUpdate + Environment.NewLine;
                query += "  END" + Environment.NewLine;
                Debug.WriteLine("Query is "+ query);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string strQuery = GetInsertQuery();
                    SqlCommand cmd = new SqlCommand(query);
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }

        }

        public void ExecuteInsert()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string strQuery = GetInsertQuery();
                SqlCommand cmd = new SqlCommand(strQuery);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void ExecuteDelete()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string strQuery = GetDeleteQuery();
                SqlCommand cmd = new SqlCommand(strQuery);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void ExecuteInsertOrUpdatebyExist()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string strQuery = "";
                string strQuerySelect = GetSelectQuery();
                string strQueryUpdate = GetUpdateQuery();
                string strQueryInsert = GetInsertQuery();

                #region queryWhere
                string queryWhere = "";
                foreach (var a in ListColumn.Where(s => s.isPK == true))
                    queryWhere += (!string.IsNullOrEmpty(queryWhere) ? " AND " : "") + string.Format(" {0} = '{1}' ", a.ColumnName, a.ColumnValue);
                queryWhere = (string.IsNullOrEmpty(queryWhere) ? "" : " WHERE " + queryWhere);
                #endregion

                strQuery += "declare @count int" + Environment.NewLine;
                strQuery += "select @count = count(*) from " + tableName + queryWhere + Environment.NewLine;
                strQuery += "if(@count > 0 )" + Environment.NewLine;
                strQuery += "BEGIN" + Environment.NewLine;
                strQuery += "   " + strQueryUpdate + Environment.NewLine;
                strQuery += "END" + Environment.NewLine;
                strQuery += "ELSE" + Environment.NewLine;
                strQuery += "BEGIN" + Environment.NewLine;
                strQuery += "   " + strQueryInsert + Environment.NewLine;
                strQuery += "END" + Environment.NewLine;
                Debug.WriteLine("Query is " + strQuery);
                SqlCommand cmd = new SqlCommand(strQuery);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void ExecuteProcedure()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(procedureName);
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (var a in ListStoreParam)
                    cmd.Parameters.AddWithValue(a.ParamName, a.ParamValue);
                cmd.Connection = connection;
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void ExecuteNoReturn(string strQuery)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(strQuery);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public DataTable GetAll(string Query)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(Query);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                connection.Open();
                dt.Load(cmd.ExecuteReader());
            }
            return dt;
        }

        public DataRow Get(string Query)
        {
            DataRow dr = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(Query);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;
                connection.Open();
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                if (dt.Rows.Count > 0)
                    dr = dt.Rows[0];
                else
                    dr = dt.NewRow();
            }
            return dr;
        }

        private string GetSelectQuery()
        {
            string query = "";
            string queryWhere = "";
            foreach (var a in ListColumn.Where(s => s.isPK == true))
            {
                queryWhere += (!string.IsNullOrEmpty(queryWhere) ? " AND " : "") + string.Format(" {0} = '{1}' ", a.ColumnName, a.ColumnValue);
            }
            query = "SELECT * FROM " + tableName + (string.IsNullOrEmpty(queryWhere) ? "" : " WHERE " + queryWhere);
            return query;
        }

        private string GetInsertQuery()
        {
            string query = "";
            string queryCol = "";
            string queryVal = "";
            foreach (var a in ListColumn.Where(s => !string.IsNullOrEmpty(s.ColumnValue)))
            {
                queryCol += (string.IsNullOrEmpty(queryCol) ? "" : ",") + a.ColumnName;
                queryVal += (string.IsNullOrEmpty(queryVal) ? "" : ",") + string.Format("'{0}'", a.ColumnValue);
            }
            query = string.Format("INSERT INTO {0} ({1}) values ({2}) ", tableName, queryCol, queryVal);
            return query;
        }

        private string GetUpdateQuery()
        {
            string query = "";
            string queryVal = "";
            foreach (var a in ListColumn.Where(s => s.isPK == false))
                queryVal += (string.IsNullOrEmpty(queryVal) ? "" : ",") + string.Format(" {0} = '{1}' ", a.ColumnName, a.ColumnValue);

            string queryWhere = "";
            foreach (var a in ListColumn.Where(s => s.isPK == true))
                queryWhere = (string.IsNullOrEmpty(queryWhere) ? "WHERE" : "AND") + string.Format(" {0} = '{1}' ", a.ColumnName, a.ColumnValue);

            query = string.Format("UPDATE {0} SET {1} {2} ", tableName, queryVal, queryWhere);
            return query;
        }

        private string GetDeleteQuery()
        {
            string query = "";

            string queryWhere = "";
            foreach (var a in ListColumn.Where(s => s.isPK == true))
                queryWhere += (!string.IsNullOrEmpty(queryWhere) ? " AND " : "") + string.Format(" {0} = '{1}' ", a.ColumnName, a.ColumnValue);

            query = string.Format("DELETE FROM {0} WHERE {1} ", tableName, queryWhere);
            return query;
        }


        public class DCColumn
        {
            private string _ColumnName;
            public string ColumnName
            {
                get { return _ColumnName; }
                set
                {
                    _ColumnName = value;
                }
            }

            private string _ColumnValue;
            public string ColumnValue
            {
                get { return _ColumnValue; }
                set
                {
                    _ColumnValue = value;
                }
            }

            private object _ColumnValueObj;
            public object ColumnValueObj
            {
                get { return _ColumnValueObj; }
                set
                {
                    _ColumnValueObj = value;
                    ColumnValue = GetStringColumnValue(_ColumnValueObj);
                }
            }

            private string GetStringColumnValue(object ColumnValue)
            {
                if (ColumnValue != null)
                {
                    if (ColumnValue.GetType() == typeof(string))
                        return ColumnValue.ToString();
                    if (ColumnValue.GetType() == typeof(int))
                        return ColumnValue.ToString();
                    if (ColumnValue.GetType() == typeof(DateTime))
                        return Convert.ToDateTime(ColumnValue).ToString("yyyy-MM-dd HH:mm:ss");
                    if (ColumnValue.GetType() == typeof(Guid))
                        return ColumnValue.ToString();
                    if (ColumnValue.GetType() == typeof(decimal))
                        return ColumnValue.ToString();
                    if (ColumnValue.GetType() == typeof(bool))
                        return ColumnValue.ToString();
                }
                return "";
            }

            public bool isPK { get; set; }
        }

        public class DCStoreParam
        {
            private string _ParamName;
            public string ParamName
            {
                get { return _ParamName; }
                set
                {
                    _ParamName = value;
                }
            }

            private string _ParamValue;
            public string ParamValue
            {
                get { return _ParamValue; }
                set
                {
                    _ParamValue = value;
                }
            }

            private object _ParamValueObj;
            public object ParamValueObj
            {
                get { return _ParamValueObj; }
                set
                {
                    _ParamValueObj = value;
                    ParamValue = GetStringColumnValue(_ParamValueObj);
                }
            }

            private string GetStringColumnValue(object ColumnValue)
            {
                if (ColumnValue != null)
                {
                    if (ColumnValue.GetType() == typeof(string))
                        return ColumnValue.ToString();
                    if (ColumnValue.GetType() == typeof(int))
                        return ColumnValue.ToString();
                    if (ColumnValue.GetType() == typeof(DateTime))
                        try
                        {
                            return Convert.ToDateTime(ColumnValue).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        catch
                        {
                            return "";
                        }
                    if (ColumnValue.GetType() == typeof(Guid))
                        return ColumnValue.ToString();
                    if (ColumnValue.GetType() == typeof(decimal))
                        return ColumnValue.ToString();
                }
                return "";
            }
        }
    }
}