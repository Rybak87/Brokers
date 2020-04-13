using Brokers.DAL.Interfaces;
using Brokers.DAL.Model;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brokers.DAL.Elastic
{
    public class ElasticCalculation: IMessagesReportBuilder
    {
        readonly ElasticClient esClient;

        public ElasticCalculation()
        {

        }
        public ElasticCalculation(string nameIndex = "messages")
        {
            esClient = new ElasticClient(new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex(nameIndex));
        }

        public IEnumerable<AuthorTotals> GetTopAuthorsByViews(DateTime fromDate, DateTime toDate, int limit)
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
                return Enumerable.Empty<AuthorTotals>();

            var bucketAggregate = searchResponse.Aggregations.Values.First() as BucketAggregate;
            var keyedBuckets = bucketAggregate.Items.Select(i => i as KeyedBucket<object>);

            var result = keyedBuckets.Select(b => new AuthorTotals
            {
                AuthorId = Convert.ToInt32(b.Key),
                TotalViewCount = Convert.ToInt32((b.Aggregations["TotalViewCount"] as ValueAggregate).Value),
                TotalReactionCount = Convert.ToInt32((b.Aggregations["TotalReactionCount"] as ValueAggregate).Value),
                TotalCount = Convert.ToInt32(b.DocCount)
            });
            return result;
        }
    }
}
