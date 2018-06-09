using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EntityAbstract.Uwp.Helpers
{
    /// <summary>
    /// 文本解析
    /// </summary>
    public static class ActionTextFormat
    {
        //开始转换
        public static async Task<string> ActionFormat(string str,string code)
        {
            //var res = CSharpScript.Create("string form(){var value=@\"" + str.Trim()+"\";" + code+"}")
            //    .ContinueWith("form()");
            //var r=await res.RunAsync();
            var res = CSharpScript.Create("string r(int x,int y){return (x+y).ToString();}r(123,456)");
            var r=await res.RunAsync();
            return r.ReturnValue as string;
        }
        public static string GetBrackCode()
        {
            return $@"var begin = value.IndexOf({"\"@[\""});
            var end = value.LastIndexOf({"\"]\""})+1;
            if (begin!=-1&&end!=-1)
            {"{"}
                var ps = value.Substring(begin + 2, end - begin - 3);
                var pss = ps.Split({"\",\""});
                var stext = value.Substring(0, begin) + value.Substring(end, value.Length - end);
                return string.Format(stext, pss);{"}"}
            return value;";
        }
    }
}
