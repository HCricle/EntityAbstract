using SharedEA.Server.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SharedEA.Core.DataService.Services
{
    /// <summary>
    /// 这个是
    ///     1,根据某些数据制作一个xml(html)文件
    /// 
    /// 规则
    /// <content [attributeName]="[attributeValue]">
    ///     <contents>
    ///         .....
    ///     </contents>
    /// </content>
    /// </summary>
    public class XmlDataService : IService
    {
        public XmlDataService()
        {
        }
        public XmlDocBuilder CreateXmlDoc()
        {
            var xmlDoc = new XmlDocument();
            var content = xmlDoc.CreateElement("content");
            content.SetAttribute("class", "content-main");
            xmlDoc.AppendChild(content);
            return new XmlDocBuilder(xmlDoc, content);
        }
        public class XmlDocBuilder
        {
            public XmlDocBuilder()
            {
            }

            public XmlDocBuilder(XmlDocument xmlDoc, XmlElement rootElement)
            {
                XmlDoc = xmlDoc;
                RootElement = rootElement;
            }
            private bool isBuild;

            public bool IsBuild => isBuild;

            public XmlDocument XmlDoc { get; }
            public XmlElement RootElement { get;  }
            public async Task<XmlDocBuilder> AndXmlDocAsync(Func<XmlDocBuilder,Task<XmlDocBuilder>> funcs)
            {
                return await funcs(this);
            }
            public XmlDocBuilder AndXmlDoc(Func<XmlDocBuilder, XmlDocBuilder> funcs)
            {
                return funcs(this);
            }
            public async Task BuildAsync()
            {
                if (!IsBuild)
                {
                    isBuild = true;
                    await Task.CompletedTask;
                }
            }
        }
    }
}
