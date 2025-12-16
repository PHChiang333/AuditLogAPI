using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace AuditLogAPI.Model
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<ApiLog> ApiLogs => Set<ApiLog>();

        // Keyless：只拿結果、不對應真實資料表
        public DbSet<Dto.JsonResultRow> JsonResultRows => Set<Dto.JsonResultRow>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Dto.JsonResultRow>().HasNoKey();  //EF Core 關鍵設定



        }


    }
}
