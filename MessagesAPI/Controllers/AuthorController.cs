using Brokers.DAL.Model;
using MessagesAPI.Models;
using Nest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MessagesAPI.Controllers
{
    public class AuthorController : ApiController
    {
        readonly string nameIndex/* = "messages"*/;
        readonly ElasticClient esClient;

        public AuthorController(string nameIndex = "messages")
        {
            this.nameIndex = nameIndex;
            esClient = new ElasticClient(new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex(nameIndex));
        }
        //Template
        //http://localhost:54413/api/GetTopAuthorsByViews?fromDate=01.01.2020&toDate=14.01.2020&limit=1
        [HttpGet]
        public HttpResponseMessage GetTopAuthorsByViews(DateTime fromDate, DateTime toDate, int limit)
        {
            toDate = toDate.AddDays(1).AddTicks(-1);
            var searchResponse = esClient.Search<Message>(s => s
                .Size(0)
                .Query(q => q
                    .DateRange(date => date
                        .Field(f => f.PublicationDate)
                        .GreaterThanOrEquals(fromDate)
                        .LessThanOrEquals(toDate)
                    )
                )
                .Aggregations(a => a
                    .Terms("AuthorId", t => t
                        .Field(f => f.AuthorId)
                        .Aggregations(a2 => a2
                            .Sum("TotalViewCount", sm => sm
                                .Field(f => f.ViewCount)
                            )
                            .Sum("TotalReactionCount", sm => sm
                                .Field(f => f.ReactionCount)
                            )
                        )
                        .OrderDescending("TotalViewCount")
                        .Size(limit)
                    )
                )
            );

            if (searchResponse.Aggregations.Count == 0)
                return Request.CreateResponse("Nothing was found for the specified parameters");

            var bucketAggregate = searchResponse.Aggregations.Values.First() as BucketAggregate;
            var keyedBuckets = bucketAggregate.Items.Select(i => i as KeyedBucket<object>);

            var result = keyedBuckets.Select(b => new AuthorTotalViewResult
            {
                AuthorId = Convert.ToInt32(b.Key),
                TotalViewCount = Convert.ToInt32((b.Aggregations["TotalViewCount"] as ValueAggregate).Value),
                TotalReactionCount = Convert.ToInt32((b.Aggregations["TotalReactionCount"] as ValueAggregate).Value),
                TotalCount = Convert.ToInt32(b.DocCount)
            });
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}
