using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SharedEA.Core.WebApi.JWT
{
    public class ApiFileModel
    {
        public ApiFileModel(string name, Stream fileStream)
        {
            Name = name;
            FileStream = fileStream;
        }
        public ApiFileModel(string name, string path)
        {
            Name = name;
            Path = path;
        }
        public string Name { get;  }
        public string Path { get;  }
        public Stream FileStream { get;  }
    }
}
