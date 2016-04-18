using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace KCVDB.Services
{
    public class SessinEntity : TableEntity
    {
        public SessinEntity(string sessionId)
        {
            this.PartitionKey = sessionId;
            this.RowKey = sessionId;
        }

        public SessinEntity() { }

        public DateTime BeforeAccessTime { get; set; }
    }
}