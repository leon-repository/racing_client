using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;

namespace BLL
{
    public class SystemContext : DbContext
    {
        static SystemContext()
        {
            Database.SetInitializer<SystemContext>(null);
        }
        public SystemContext()
            : base("name=PortalContext")
        {
            // 禁用延迟加载
            this.Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            // 禁用默认表名复数形式
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            // 禁用一对多级联删除
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            // 禁用多对多级联删除
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
        }

        public DbSet<LoginModel> Logins { get; set; }

        public DbSet<QunTb> QunTbs { get; set; }

        public DbSet<Contact> Contacts { get; set; }

        public DbSet<OriginMsg> OriginMsgs { get; set; }

        public DbSet<NowMsg> NowMsgs { get; set; }
    }
}
