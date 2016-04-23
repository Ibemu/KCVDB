using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace KCVDB.Services.BlobStorage
{
    public class SessionEntity : TableEntity
    {
        public SessionEntity(string sessionId)
        {
            this.PartitionKey = "sessionId";
            this.RowKey = sessionId;
        }

        public SessionEntity() { }

        public DateTime BlobCreated { get; set; }

        public string BlobName { get; set; }
    }
}