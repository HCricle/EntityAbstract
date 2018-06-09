using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EntityAbstract.Api.Model.Models
{
    public class SendContentModel
    {
        public uint GroupId { get; set; } = 1;//默认是第一组

        //512
        public string Title { get; set; }
        //mult
        public string Content { get; set; }
        //100
        public string Label { get; set; }

        public IEnumerable<(string,Stream)> FileDatas { get; set; }
    }
}
