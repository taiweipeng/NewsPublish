using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using NewsPublish.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsPublish.Service
{
    public class Db_Help : DbContext
    {
        //[Obsolete]
        //public static readonly LoggerFactory LoggerFactory = new LoggerFactory(new[] { new DebugLoggerProvider((_, _) => true) });
        public Db_Help() { }
        public static readonly LoggerFactory LoggerFactory =
              new LoggerFactory(new[] { new DebugLoggerProvider() });
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string ConnectionString = @"Data Source=邰伟鹏的THINKPA\SQLEXPRESS;Initial Catalog=coreTest;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False";
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLoggerFactory(LoggerFactory);
            optionsBuilder.UseSqlServer(ConnectionString, b => b.UseRowNumberForPaging());
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        public virtual DbSet<Model.Entity.Banner> Banner{get;set;}
        public virtual DbSet<Model.Entity.NewsClassify> NewsClassify { get; set; }
        public virtual DbSet<Model.Entity.News> News { get; set; }
        public virtual DbSet<Model.Entity.NewsComment> NewsComment { get; set; }
    }
}
