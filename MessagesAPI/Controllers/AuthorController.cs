using Brokers.DAL.Elastic;
using Brokers.DAL.Interfaces;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MessagesAPI.Controllers
{
    public class AuthorController : ApiController
    {
        readonly IMessagesReportBuilder messagesReportBuilder;

        public AuthorController(/*IMessagesReportBuilder messagesReportBuilder*/)
        {
            //this.messagesReportBuilder = messagesReportBuilder;
        }

        //Template
        //http://localhost:54413/api/GetTopAuthorsByViews?fromDate=01.01.2020&toDate=14.01.2020&limit=1

        [HttpGet]
        public HttpResponseMessage GetTopAuthorsByViews(DateTime fromDate, DateTime toDate, int limit)
        {
            var result = messagesReportBuilder.GetTopAuthorsByViews(fromDate, toDate, limit);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}
