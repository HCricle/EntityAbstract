using SharedEA.Core.DbModel.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityAbstract.Web.Models.HomeViewModels
{
    public class IndexViewModel
    {
        public IndexViewModel()
        {
            Imgs = new List<ContentFile>();
            Contents = new List<Content>();
        }

        public List<Content> Contents { get;  }
        public List<ContentFile> Imgs { get;  }
        public int ImgCount { get; set; }
    }
}
