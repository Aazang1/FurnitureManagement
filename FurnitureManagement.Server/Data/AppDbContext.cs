using FurnitureManagement.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace FurnitureManagement.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Furniture> Furniture { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<Warehouse> Warehouse { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrder { get; set; }
        public DbSet<PurchaseDetail> PurchaseDetail { get; set; }
        public DbSet<SaleOrder> SaleOrder { get; set; }
        public DbSet<SaleDetail> SaleDetail { get; set; }
        
        // 报表视图
        public DbSet<InventorySummary> InventorySummary { get; set; }
        public DbSet<SalesDaily> SalesDaily { get; set; }
        public DbSet<UserOperations> UserOperations { get; set; }

        public DbSet<CapitalFlow> CapitalFlow { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 配置User表
            modelBuilder.Entity<User>().ToTable("user");
            modelBuilder.Entity<User>().HasKey(u => u.UserId);
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            // 配置Category表
            modelBuilder.Entity<Category>().ToTable("category");
            modelBuilder.Entity<Category>().HasKey(c => c.CategoryId);

            // 配置Furniture表
            modelBuilder.Entity<Furniture>().ToTable("furniture");
            modelBuilder.Entity<Furniture>().HasKey(f => f.FurnitureId);

            // 配置Supplier表
            modelBuilder.Entity<Supplier>().ToTable("supplier");
            modelBuilder.Entity<Supplier>().HasKey(s => s.SupplierId);

            // 配置Warehouse表
            modelBuilder.Entity<Warehouse>().ToTable("warehouse");
            modelBuilder.Entity<Warehouse>().HasKey(w => w.WarehouseId);

            // 配置Inventory表
            modelBuilder.Entity<Inventory>().ToTable("inventory");
            modelBuilder.Entity<Inventory>().HasKey(i => i.InventoryId);

            // 配置PurchaseOrder表
            modelBuilder.Entity<PurchaseOrder>().ToTable("purchase_order");
            modelBuilder.Entity<PurchaseOrder>().HasKey(po => po.PurchaseOrderId);

            // 配置PurchaseDetail表
            modelBuilder.Entity<PurchaseDetail>().ToTable("purchase_detail");
            modelBuilder.Entity<PurchaseDetail>().HasKey(pd => pd.PurchaseDetailId);

            // 配置PurchaseOrder和PurchaseDetail之间的一对多关系
            modelBuilder.Entity<PurchaseOrder>()
                .HasMany(po => po.PurchaseDetails)
                .WithOne(pd => pd.PurchaseOrder)
                .HasForeignKey(pd => pd.PurchaseOrderId);

            // 配置SaleOrder表
            modelBuilder.Entity<SaleOrder>().ToTable("sale_order");
            modelBuilder.Entity<SaleOrder>().HasKey(so => so.SaleId);
            modelBuilder.Entity<SaleOrder>().HasIndex(so => so.SaleDate).HasDatabaseName("idx_sale_date");

            // 配置SaleDetail表
            modelBuilder.Entity<SaleDetail>().ToTable("sale_detail");
            modelBuilder.Entity<SaleDetail>().HasKey(sd => sd.DetailId);
            
            // 配置销售订单和销售明细的一对多关系
            modelBuilder.Entity<SaleOrder>()
                .HasMany(so => so.SaleDetails)
                .WithOne(sd => sd.SaleOrder)
                .HasForeignKey(sd => sd.SaleId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // 配置报表视图映射
            modelBuilder.Entity<InventorySummary>().ToView("v_inventory_summary");
            modelBuilder.Entity<SalesDaily>().ToView("v_sales_daily");
            modelBuilder.Entity<UserOperations>().ToView("v_user_operations");

            // 配置CapitalFlow表
            modelBuilder.Entity<CapitalFlow>().ToTable("capital_flow");
            modelBuilder.Entity<CapitalFlow>().HasKey(cf => cf.FlowId);
            modelBuilder.Entity<CapitalFlow>().HasIndex(cf => cf.FlowDate).HasDatabaseName("idx_flow_date");
            
            // 配置属性与数据库字段的映射
            modelBuilder.Entity<CapitalFlow>()
                .Property(cf => cf.FlowId).HasColumnName("flow_id");
            modelBuilder.Entity<CapitalFlow>()
                .Property(cf => cf.FlowDate).HasColumnName("flow_date");
            modelBuilder.Entity<CapitalFlow>()
                .Property(cf => cf.FlowType).HasColumnName("flow_type");
            modelBuilder.Entity<CapitalFlow>()
                .Property(cf => cf.Amount).HasColumnName("amount");
            modelBuilder.Entity<CapitalFlow>()
                .Property(cf => cf.Description).HasColumnName("description");
            modelBuilder.Entity<CapitalFlow>()
                .Property(cf => cf.ReferenceType).HasColumnName("reference_type");
            modelBuilder.Entity<CapitalFlow>()
                .Property(cf => cf.ReferenceId).HasColumnName("reference_id");
            modelBuilder.Entity<CapitalFlow>()
                .Property(cf => cf.CreatedBy).HasColumnName("created_by");
            modelBuilder.Entity<CapitalFlow>()
                .Property(cf => cf.CreatedAt).HasColumnName("created_at");
        }
    }
}