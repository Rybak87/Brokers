using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using Brokers.DAL.Generator;
using Brokers.DAL.Interfaces;
using Brokers.DAL.Model;
using MessagesAPI.Controllers;
using MessagesAPI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;

namespace TestMessagesAPI
{
    [TestClass]
    public class UnitTest1
    {
        static readonly string nameIndex = "messagestest";
        static ElasticClient esClient = new ElasticClient(new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex(nameIndex));
        static readonly IGenerator<Message> generator = new RandomPublicationGenerator();
        static readonly AuthorController controller = new AuthorController(nameIndex);

        [TestMethod]
        public void TestMethod1()
        {
            esClient.DeleteIndex(nameIndex);
            int totalViewCount = 0;
            int totalReactionCount = 0;
            int totalCount = 0;
            int authorId = 1;
            var fromDate = new DateTime(2020, 1, 1);
            var toDate = new DateTime(2020, 1, 14);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();


            for (int i = 0; i < 3; i++)
            {
                var newMessage = generator.GenerateNew();
                newMessage.AuthorId = authorId;

                totalViewCount += newMessage.ViewCount;
                totalReactionCount += newMessage.ReactionCount;
                totalCount++;

                ResendMessageToES(newMessage);
            }
            Thread.Sleep(1000);

            var response = controller.GetTopAuthorsByViews(fromDate, toDate, 3);
            var content = response.Content as ObjectContent<IEnumerable<AuthorTotalViewResult>>;
            var result = (content.Value as IEnumerable<AuthorTotalViewResult>)?.FirstOrDefault();
            Assert.AreEqual(result.TotalCount, totalCount);
            Assert.AreEqual(result.TotalViewCount, totalViewCount);
            Assert.AreEqual(result.TotalReactionCount, totalReactionCount);
        }

        private static void ResendMessageToES(Message message)
        {
            _ = esClient.Index(message, i => i.Id(message.PublicatonId));
        }
    }
}
