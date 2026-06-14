using Microsoft.EntityFrameworkCore;
using LampEcommerce.Domain.Entities;

namespace LampEcommerce.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets for all entities
    public DbSet<User> Users { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<CampaignProduct> CampaignProducts { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<InvoiceAuditLog> InvoiceAuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();

            // One-to-Many: User -> Addresses
            entity.HasMany(e => e.Addresses)
                  .WithOne(e => e.User)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: User -> Orders
            entity.HasMany(e => e.Orders)
                  .WithOne(e => e.User)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Address configuration
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Province).IsRequired().HasMaxLength(50);
            entity.Property(e => e.City).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Street).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PostalCode).IsRequired().HasMaxLength(20);
            entity.Property(e => e.ReceiverName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ReceiverPhone).IsRequired().HasMaxLength(20);

            entity.HasIndex(e => e.UserId);
        });

        // Supplier configuration
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);

            // One-to-Many: Supplier -> Campaigns
            entity.HasMany(e => e.Campaigns)
                  .WithOne(e => e.Supplier)
                  .HasForeignKey(e => e.SupplierId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Campaign configuration
        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.AllowedProvinces).HasMaxLength(500);
            entity.Property(e => e.AllowedCities).HasMaxLength(500);

            // One-to-Many: Campaign -> CampaignProducts
            entity.HasMany(e => e.CampaignProducts)
                  .WithOne(e => e.Campaign)
                  .HasForeignKey(e => e.CampaignId)
                  .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Campaign -> Orders
            entity.HasMany(e => e.Orders)
                  .WithOne(e => e.Campaign)
                  .HasForeignKey(e => e.CampaignId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.BasePrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Description).HasMaxLength(2000);

            // One-to-Many: Product -> CampaignProducts
            entity.HasMany(e => e.CampaignProducts)
                  .WithOne(e => e.Product)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Product -> OrderItems
            entity.HasMany(e => e.OrderItems)
                  .WithOne(e => e.Product)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // CampaignProduct configuration (Many-to-Many between Campaign and Product)
        modelBuilder.Entity<CampaignProduct>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PurchasePrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.SellingPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Discount).HasColumnType("decimal(18,2)");

            entity.HasIndex(e => e.CampaignId);
            entity.HasIndex(e => e.ProductId);
        });

        // Order configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ShippingCost).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ShippingMethod).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PaymentMethod).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20);

            // One-to-Many: Order -> OrderItems
            entity.HasMany(e => e.OrderItems)
                  .WithOne(e => e.Order)
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Order -> InvoiceAuditLogs
            entity.HasMany(e => e.InvoiceAuditLogs)
                  .WithOne(e => e.Order)
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CampaignId);
            entity.HasIndex(e => e.Status);
        });

        // OrderItem configuration
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");

            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.ProductId);
        });

        // InvoiceAuditLog configuration
        modelBuilder.Entity<InvoiceAuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PreviousInvoiceData).IsRequired();
            entity.Property(e => e.NewInvoiceData).IsRequired();
            entity.Property(e => e.ChangedBy).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Reason).IsRequired().HasMaxLength(500);

            entity.HasIndex(e => e.OrderId);
        });
    }
}
