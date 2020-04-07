using Brokers.DAL.Model;
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
    public class ValuesController : ApiController
    {
        static readonly string nameIndex = "messages";
        static ElasticClient esClient = new ElasticClient(new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex(nameIndex));

        //http://localhost:54413/api/GetTopAuthorsByViews?fromDate=01.01.2020&toDate=06.04.2020&limit=1
        [HttpGet]
        public IEnumerable GetTopAuthorsByViews(DateTime fromDate, DateTime toDate, int limit)
        {
            //var fromDate = new DateTime(2020, 1, 1);
            //var toDate = new DateTime(2020, 1, 9);
            //int limit = 3;

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
                                .Field(p => p.ViewCount)
                            )
                            .Sum("TotalReactionCount", sm => sm
                                .Field(p => p.ReactionCount)
                            )
                            .ValueCount("TotalCount", v => v
                                .Field(p => p.AuthorId)
                            )
                        )
                        .OrderDescending("TotalViewCount")
                        .Size(limit)
                    )
                )
            );

            var bucketAggregate = searchResponse.Aggregations.Values.First() as BucketAggregate;
            var keyedBuckets = bucketAggregate.Items.Select(i => i as KeyedBucket<object>);

            return keyedBuckets.Select(b => new
            {
                AuthorId = b.Key,
                TotalViewCount = Convert.ToInt32((b.Aggregations["TotalViewCount"] as ValueAggregate).Value),
                TotalReactionCount = Convert.ToInt32((b.Aggregations["TotalReactionCount"] as ValueAggregate).Value),
                TotalCount = Convert.ToInt32((b.Aggregations["TotalCount"] as ValueAggregate).Value)
            });
        }
    }
}
