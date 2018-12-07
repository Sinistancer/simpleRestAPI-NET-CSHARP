using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RM.Models.API
{
    public class OutputsModel
    {
        public int code { get; set; }
        public string status { get; set; }
        public object message { get; set; }
        public object data { get; set; }

        public OutputsModel() { }

        public OutputsModel(int _code = 0, string _status = null, object _message = null, object _data = null)
        {
            code = _code;
            status = _status;
            message = _message;
            data = _data;            
        }

        public void SetOutput(int _code = 0, string _status = null, object _message = null, object _data = null)
        {
            code = _code;
            status = _status;
            message = _message;
            data = _data;           
        }
        
    }
}