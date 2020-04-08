using Nest;
using System;

namespace Brokers.DAL.Model
{
    [ElasticsearchType(Name = "Message")]
    public class Message
    {
        [Text(Name = "PublicationDate")]
        public DateTime PublicationDate { get; set; }

        [Text(Name = "AuthorId")]
        public int AuthorId { get; set; }

        [Text(Name = "HostId")]
        public int HostId { get; set; }

        [Text(Name = "ViewCount")]
        public int ViewCount { get; set; }

        [Text(Name = "ReactionCount")]
        public int ReactionCount { get; set; }

        [Text(Name = "PublicatonId")]
        public long PublicatonId { get; set; }

        [Text(Name = "Title")]
        public string Title { get; set; }

        [Text(Name = "Content")]
        public string Content { get; set; }
    }
}
