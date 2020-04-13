using Brokers.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brokers.DAL.Interfaces
{
    public interface IMessagesReportBuilder
    {
        IEnumerable<AuthorTotals> GetTopAuthorsByViews(DateTime fromDate, DateTime toDate, int limit);//TODO AuthorTotals?
    }
}
