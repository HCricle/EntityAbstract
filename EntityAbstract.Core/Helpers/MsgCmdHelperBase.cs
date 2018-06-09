using EntityAbstract.Core.Args;
using SharedEA.Core.DbModel.DbModels;
using SharedEA.Core.DbModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityAbstract.Core.Helpers
{
    public delegate TWithUi MsgCmdParseEvent<TWithUi>(MsgCmdParseArgs<TWithUi> args) where TWithUi:class;
    /// <summary>
    /// 对于消息命令解析
    /// </summary>
    /// <typeparam name="TWithUi"></typeparam>
    public abstract class MsgCmdHelperBase<TWithUi>
        where TWithUi:class
    {
        public static readonly int Key = 0x23409;
        public static readonly float Version = 1.0f;
        private static int groupIndex = 0;
        private Dictionary<int, IDictionary<int, MsgCmdParseEvent<TWithUi>>> detailProcess;
        /// <summary>
        /// 处理组
        /// </summary>
        private IReadOnlyDictionary<int, IDictionary<int, MsgCmdParseEvent<TWithUi>>> DetailProcess=>detailProcess;
        /// <summary>
        /// 有多少组
        /// </summary>
        public int GroupCount => DetailProcess.Count;
        /// <summary>
        /// 获取一个组与GetGroup一样
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IDictionary<int, MsgCmdParseEvent<TWithUi>> this[int id]=>GetGroup(id);
        public event Action<MsgDetail> ParseErr;
        public MsgCmdHelperBase()
        {
            detailProcess = new Dictionary<int, IDictionary<int, MsgCmdParseEvent<TWithUi>>>();
        }
        /// <summary>
        /// 创建一个消息处理过程组，返回过程组id
        /// </summary>
        /// <param name="procs">过程集合</param>
        /// <returns></returns>
        [STAThread]
        public int CreateGroup(params MsgCmdParseEvent<TWithUi>[] procs)
        {
            var typeIndex = 0;
            if (procs!=null)
            {
                var g = new Dictionary<int, MsgCmdParseEvent<TWithUi>>();
                foreach (var item in procs)
                {
                    g.Add(typeIndex++, item);
                }
                detailProcess.Add(groupIndex++, g);
            }
            return groupIndex - 1;
        }
        /// <summary>
        /// 移除一个处理组
        /// </summary>
        /// <param name="id">组id</param>
        public void RemoveGroup(int id)
        {
            EnsureHasGroup(id);
            detailProcess.Remove(id);
        }
        /// <summary>
        /// 开始解析，必须要在ui线程操作
        /// </summary>
        /// <param name="msgDetail">消息详细</param>
        /// <param name="uiGetter">调用时需要创建一个ui层</param>
        /// <returns>返回创建的ui集合</returns>
        public virtual TWithUi[] Parse(MsgDetail msgDetail)
        {
            TWithUi[] uis=null;
            if (msgDetail.SerCmds!=null)
            {
                uis = new TWithUi[msgDetail.SerCmds.Count];
                int i = 0;
                foreach (var item in msgDetail.SerCmds)
                {
                    if (EnsureHasType(item.Id, item.Type))
                    {
                        uis[i++] = Parse(item);
                    }
                    else
                    {
                        ParseErr?.Invoke(msgDetail);
                    }
                }
            }
            return uis;
        }
        /// <summary>
        /// 解析一个
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual TWithUi Parse(MsgCmd item)
        {
            return DetailProcess[item.Id][item.Type](new MsgCmdParseArgs<TWithUi>(this, item, GetUi()));
        }
        /// <summary>
        /// 获取ui容器
        /// </summary>
        /// <returns></returns>
        protected abstract TWithUi GetUi();
        /// <summary>
        /// 获取一个组的处理方法
        /// </summary>
        /// <param name="id">组id</param>
        /// <returns></returns>
        public IDictionary<int, MsgCmdParseEvent<TWithUi>> GetGroup(int id)
        {
            EnsureHasGroup(id);
            return detailProcess[id];
        }
        private bool EnsureHasGroup(int id)
        {
            return DetailProcess.Keys.Contains(id);
        }
        private bool EnsureHasType(int id,int type)
        {
            EnsureHasGroup(id);
            var g = DetailProcess[id];
            return g.Keys.Contains(type);
        }
    }
}
