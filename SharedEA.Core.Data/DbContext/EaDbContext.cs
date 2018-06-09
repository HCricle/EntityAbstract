using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedEA.Core.DbModel.DbModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedEA.Core.DbModel.DbContext
{

    public class EaDbContext : IdentityDbContext<EaUser>
    {
        public EaDbContext(DbContextOptions options)
            : base(options)
        {
        }
        public EaDbContext()
        {
            
        }
        #region DbSets
        /// <summary>
        /// 内容组,所有的存在都是通过这个访问数据的
        /// </summary>
        public DbSet<ContentGroup> Groups { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public DbSet<Content> Contents { get; set; }
        /// <summary>
        /// 评论
        /// </summary>
        public DbSet<Comment> Comments { get; set; }

        /// <summary>
        /// 喜欢
        /// </summary>
        public DbSet<Like> Likes { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public DbSet<Msg> Msgs { get; set; }
        /// <summary>
        /// 消息详细
        /// </summary>
        public DbSet<MsgDetail> MsgDetails { get; set; }
        /// <summary>
        /// 内容文件
        /// </summary>
        public DbSet<ContentFile> ContentFiles { get; set; }
        /// <summary>
        /// 每天登陆
        /// </summary>
        public DbSet<PreLoad> PreLoads { get; set; }
        /// <summary>
        /// 商店
        /// </summary>
        public DbSet<Shop> Shops { get; set; }
        /// <summary>
        /// 商店物品
        /// </summary>
        public DbSet<ShopItem> ShopItems { get; set; }
        /// <summary>
        /// 命令
        /// </summary>
        public DbSet<ACmd> Commands { get; set; }
        /// <summary>
        /// 购买记录
        /// </summary>
        public DbSet<Buy> Buys { get; set; }
        /// <summary>
        /// 朋友
        /// </summary>
        public DbSet<Friend> Friends { get; set; }
        #endregion
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            //39.108.223.229
#if DEBUG
            optionsBuilder.UseMySql("server=localhost;user=root;pwd=355343;database=eadb;");
#else
            //optionsBuilder.UseMySql("server=39.108.223.229;user=pea;pwd=1969027465;database=eadb;");
            optionsBuilder.UseMySql("server=39.108.223.229;user=root;pwd=355343;database=eadb;");
#endif
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            var dbComment = builder.Entity<Comment>();
            var dbContent = builder.Entity<Content>();
            var dbGroup = builder.Entity<ContentGroup>();
            var dbLike = builder.Entity<Like>();
            var dbMsg = builder.Entity<Msg>();
            var dbMsgDetails = builder.Entity<MsgDetail>();
            var dbContentFiles = builder.Entity<ContentFile>();
            var dbPreLoads = builder.Entity<PreLoad>();
            base.OnModelCreating(builder);
        }
    }
}
