using System;

namespace Brokers.DAL.Model
{
    public class Message
    {
        public DateTime PublicationDate { get; set; }
        public int AuthorId { get; set; }
        public int HostId { get; set; }
        public int ViewCount { get; set; }
        public int ReactionCount { get; set; }
        public long PublicatonId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
