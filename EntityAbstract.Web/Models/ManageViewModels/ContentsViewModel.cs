using SharedEA.Core.DbModel.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityAbstract.Web.Models.ManageViewModels
{
    public class ContentsViewModel
    {
        public IEnumerable<Content> Contents { get; set; }
        public int LocPage { get; set; }
        public int TotalCount { get; set; }
    }
}
