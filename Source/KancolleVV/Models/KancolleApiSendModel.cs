using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KancolleVV.Models
{
    public class KancolleApiSendModel
    {
        public string LoginSessionId { get; set; }

        public string AgentId { get; set; }

        public string Path { get; set; }

        public string RequestValue { get; set; }

        public string ResponseValue { get; set; }

        public int? StatusCode { get; set; }

        public string HttpDate { get; set; }

        public string LocalTime { get; set; }
    }
}