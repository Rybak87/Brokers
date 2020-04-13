using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Brokers.DAL.Model
{
    public class AuthorTotals
    {
        public int AuthorId { get; set; }
        public int TotalViewCount { get; set; }
        public int TotalReactionCount { get; set; }
        public int TotalCount { get; set; }

        public override bool Equals(object obj)
        {

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var obj2 = obj as AuthorTotals;

            if (AuthorId == obj2.AuthorId &&
                TotalViewCount == obj2.TotalViewCount &&
                TotalReactionCount == obj2.TotalReactionCount &&
                TotalCount == obj2.TotalCount)

            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return TotalViewCount * 1000 + TotalReactionCount * 100 + AuthorId * 10 + TotalCount;
        }
    }
}