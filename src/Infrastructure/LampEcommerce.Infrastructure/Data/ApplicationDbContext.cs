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
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Address> Addresses { get; set; } = null!;
    public DbSet<Supplier> Suppliers { get; set; } = null!;
    public DbSet<Campaign> Campaigns { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<CampaignProduct> CampaignProducts { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<InvoiceAuditLog> InvoiceAuditLogs { get; set; } = null!;
    public DbSet<ShippingMethod> ShippingMethods { get; set; } = null!;
    public DbSet<Ticket> Tickets { get; set; } = null!;
    public DbSet<TicketMessage> TicketMessages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash);
            entity.Property(e => e.Role).IsRequired().HasMaxLength(20).HasDefaultValue(UserRoles.Customer);
            entity.Property(e => e.BaleChatId).HasMaxLength(50);
            entity.Property(e => e.TelegramChatId).HasMaxLength(50);

            entity.HasIndex(e => e.PhoneNumber).IsUnique();

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
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.Slug).IsUnique();
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

        // ShippingMethod configuration
        modelBuilder.Entity<ShippingMethod>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LogoUrl).HasMaxLength(500);
            entity.Property(e => e.ApiKey).HasMaxLength(200);
            entity.Property(e => e.ApiUrl).HasMaxLength(500);
            entity.Property(e => e.BaseCost).HasColumnType("decimal(18,2)");
        });

        // Ticket configuration
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Subject).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Priority).IsRequired().HasMaxLength(20).HasDefaultValue("Low");

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // TicketMessage configuration
        modelBuilder.Entity<TicketMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(2000);

            entity.HasOne(e => e.Ticket)
                  .WithMany(e => e.Messages)
                  .HasForeignKey(e => e.TicketId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.SenderUser)
                  .WithMany()
                  .HasForeignKey(e => e.SenderUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
