using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SharedEA.Core.WebApi.JWT
{
    public class SendContentModel
    {
        public SendContentModel()
        {
        }

        public SendContentModel(uint groupId, string title, string content, string label)
        {
            GroupId = groupId;
            Title = title;
            Content = content;
            Label = label;
        }

        public uint GroupId { get; set; } = 1;//默认是第一组

        //512
        public string Title { get; set; }
        //mult
        public string Content { get; set; }
        //100
        public string Label { get; set; }

        public IEnumerable<ApiFileModel> FileDatas { get; set; }
    }
}
