using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEA.Core.DbModel.RepositoryModel
{
    /// <summary>
    /// 响应的模型
    /// </summary>
    public abstract class ArrayRepository<T>
    {
        public ArrayRepository()
        {
        }
        public ArrayRepository(bool ok)
        {
            Ok = ok;
        }
        public ArrayRepository(int total, int skip, int take, T[] datas, bool ok=true)
        {
            Total = total;
            Skip = skip;
            Take = take;
            Datas = datas;
            Ok = ok;
        }

        public int Total { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public T[] Datas { get; set; }
        public bool Ok { get; set; }
        public string Msg { get; set; }
    }
}
