using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedEA.Core.Data.Models
{
    public class FormFileModel
    {
        public FormFileModel(IFormFile file)
        {
            File = file;
            var ex = file.FileName.Split('.');
            ExtensionName = ex.Count() == 1 ? string.Empty : ex.Last().Replace('.', '\0').ToLower();
            CanDownload = true;
            OriginalName = file.FileName;
        }

        public IFormFile File { get;  }
        public string ExtensionName { get;  }
        public string OriginalName { get;  }
        public bool CanDownload { get; set; }
    }
}
