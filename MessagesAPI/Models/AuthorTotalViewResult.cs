using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessagesAPI.Models
{
    public class AuthorTotalViewResult
    {
        public int AuthorId { get; set; }
        public int TotalViewCount { get; set; }
        public int TotalReactionCount { get; set; }
        public int TotalCount { get; set; }
    }
}