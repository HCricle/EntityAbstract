using SharedEA.Core.DbModel.DbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Core.DbModel.RepositoryModel
{
    public class ContentFileRepositoryModel
    {
        public ContentFileRepositoryModel()
        {
        }

        public ContentFileRepositoryModel(bool ok)
        {
            Ok = ok;
        }

        public ContentFileRepositoryModel(string path, string contentType, ContentFile file, bool ok)
        {
            Path = path;
            ContentType = contentType;
            File = file;
            Ok = ok;
        }

        public string Path { get; set; }
        public string ContentType { get; set; }
        public ContentFile File { get; set; }
        public bool Ok { get; set; }
    }
}
