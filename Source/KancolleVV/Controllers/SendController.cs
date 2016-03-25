using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using KancolleVV.Logics.BlobStrage;
using KancolleVV.Models;

namespace KancolleVV.Controllers
{
    public class SendController : ApiController
    {
        private AzuleBlobService _blobService;

        private static Object lockObj = new Object();

        public SendController()
        {
        }

        // テスト用
        public IEnumerable<string> Get()
        {
            //_blobService.Add("114515", "api_start2", "json文字列ココに入る");
            //_blobService.Add("123456789", "api_start2/dddd", "RequestValue", "ResponseValue", "ほっぽあるふぁなの");

            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public void Post([FromBody]KancolleApiSendModel model)
        {
            lock(lockObj)
            {
                _blobService = new AzuleBlobService();
                _blobService.Add(model.LoginSessionId, model.Path, model.RequestValue, model.ResponseValue, model.AgentId, model.StatusCode, model.HttpDate, model.LocalTime);
            }
        }
    }
}
