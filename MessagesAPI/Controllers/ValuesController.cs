using Brokers.DAL.Model;
using Nest;
using System;
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
        public void GetTopAuthorsByViews(DateTime fromDate, DateTime toDate, int limit)
        {
            var x = esClient.Search<Message>(s => s
                //.Query(q => q
                //    .DateRange(date => date
                //        .GreaterThanOrEquals(fromDate)
                //        .LessThanOrEquals(toDate)
                //    )
                //)
                .Aggregations(a => a
                    .DateRange("Dates", d => d
                        .Field(f => f.PublicationDate)
                        .Ranges(r => r.From(fromDate).To(toDate))
                        .Aggregations(a2 => a2
                            .Terms("AuthorId", t => t
                                .Field(f => f.AuthorId)
                                .Aggregations(a3 => a3
                                    .Sum("view_sum", sm => sm
                                        .Field(p => p.ViewCount)
                                    )
                                    .Sum("reaction_sum", sm => sm
                                        .Field(p => p.ReactionCount)
                                    )
                                )
                                .Order(TermsOrder.CountDescending)
                            )
                        )
                    )
                )
            );
        }
    }
}
