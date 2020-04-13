using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using Brokers.DAL.Elastic;
using Brokers.DAL.Generator;
using Brokers.DAL.Interfaces;
using Brokers.DAL.Model;
using MessagesAPI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;

namespace TestMessagesAPI
{
    [TestClass]
    public class UnitTest
    {
        static readonly string nameIndex = "messagestest";
        static ElasticClient esClient = new ElasticClient(new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex(nameIndex));
        static readonly IGenerator<Message> generator = new RandomPublicationGenerator();
        static readonly AuthorController controller = new AuthorController(new ElasticCalculation(nameIndex));


        public UnitTest()
        {
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();
            esClient.DeleteIndex(nameIndex);
        }

        [TestMethod]
        public void OneAuthor()
        {
            int totalViewCount = 0;
            int totalReactionCount = 0;
            int totalCount = 0;
            int authorId = 1;
            var fromDate = new DateTime(2020, 1, 1);
            var toDate = new DateTime(2020, 1, 14);



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
            var content = response.Content as ObjectContent<IEnumerable<AuthorTotals>>;
            var result = (content.Value as IEnumerable<AuthorTotals>)?.FirstOrDefault();



            Assert.AreEqual(result.TotalCount, totalCount);
            Assert.AreEqual(result.TotalViewCount, totalViewCount);
            Assert.AreEqual(result.TotalReactionCount, totalReactionCount);
        }

        [TestMethod]
        public void FiveAuthor()
        {
            var fromDate = new DateTime(2020, 1, 4);
            var toDate = new DateTime(2020, 1, 10);
            var limit = 3;
            var messages = new List<Message>();

            var authorPublicationCount = new[]
            {
                new {AuthorId = 3, PublicationCount = 4 },
                new {AuthorId = 77, PublicationCount = 8 },
                new {AuthorId = 15, PublicationCount = 3 },
                new {AuthorId = 8, PublicationCount = 2 },
                new {AuthorId = 27, PublicationCount = 7 },
            };


            foreach (var item in authorPublicationCount)
            {
                for (int j = 0; j < item.PublicationCount; j++)
                {
                    var newMessage = generator.GenerateNew();
                    newMessage.AuthorId = item.AuthorId;
                    ResendMessageToES(newMessage);
                    messages.Add(newMessage);
                }
            }
            Thread.Sleep(1000);

            var response = controller.GetTopAuthorsByViews(fromDate, toDate, limit);
            var content = response.Content as ObjectContent<IEnumerable<AuthorTotals>>;
            var result = (content.Value as IEnumerable<AuthorTotals>).ToArray();

            var sample = messages
                .Where(m => m.PublicationDate >= fromDate && m.PublicationDate <= toDate.AddDays(1).AddTicks(-1))
                .GroupBy(m => m.AuthorId)
                .Select(g => new AuthorTotals()
                {
                    AuthorId = g.Key,
                    TotalViewCount = g.Select(k => k).Sum(l => l.ViewCount),
                    TotalReactionCount = g.Select(k => k).Sum(l => l.ReactionCount),
                    TotalCount = g.Count(),
                })
                .OrderByDescending(a => a.TotalViewCount)
                .Take(limit);




            Assert.IsTrue(sample.SequenceEqual(result));
        }

        private static void ResendMessageToES(Message message)
        {
            _ = esClient.Index(message, i => i.Id(message.PublicatonId));
        }
    }
}
