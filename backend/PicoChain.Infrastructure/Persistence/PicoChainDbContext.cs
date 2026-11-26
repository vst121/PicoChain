using Microsoft.EntityFrameworkCore;
using PicoChain.Domain.Entities;
using PicoChain.Domain.Events;

namespace PicoChain.Infrastructure.Persistence;

public sealed class PicoChainDbContext(DbContextOptions<PicoChainDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Carrier> Carriers => Set<Carrier>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<ShipmentEvent> ShipmentEvents => Set<ShipmentEvent>();
    public DbSet<OperationalTask> OperationalTasks => Set<OperationalTask>();
    public DbSet<AgentTask> AgentTasks => Set<AgentTask>();
    public DbSet<ActionProposal> ActionProposals => Set<ActionProposal>();
    public DbSet<ApprovalRequest> ApprovalRequests => Set<ApprovalRequest>();
    public DbSet<AuditRecord> AuditRecords => Set<AuditRecord>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<HumanReviewRecord> HumanReviewRecords => Set<HumanReviewRecord>();
    public DbSet<EvaluationRecord> EvaluationRecords => Set<EvaluationRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.CustomerNumber).IsUnique();
            entity.Property(x => x.CustomerNumber).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Sku).IsUnique();
            entity.Property(x => x.Sku).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Carrier>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.CarrierCode).IsUnique();
            entity.Property(x => x.CarrierCode).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.OrderNumber).IsUnique();
            entity.Property(x => x.OrderNumber).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
            entity.HasMany(x => x.Items).WithOne().HasForeignKey(x => x.OrderId);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Quantity).IsRequired();
        });

        modelBuilder.Entity<Shipment>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.ShipmentNumber).IsUnique();
            entity.Property(x => x.ShipmentNumber).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
            entity.HasMany(x => x.Events).WithOne().HasForeignKey(x => x.ShipmentId);
        });

        modelBuilder.Entity<ShipmentEvent>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.EventType).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(1000);
        });

        modelBuilder.Entity<OperationalTask>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TaskType).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(40);
        });

        modelBuilder.Entity<AgentTask>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.AgentName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.TaskType).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(40);
            entity.HasIndex(x => x.CorrelationId);
            entity.HasIndex(x => x.TraceId);
        });

        modelBuilder.Entity<ActionProposal>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ActionType).HasMaxLength(100).IsRequired();
            entity.Property(x => x.ParametersJson).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(40);
            entity.HasIndex(x => x.AgentTaskId);
        });

        modelBuilder.Entity<ApprovalRequest>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.RequestedBy).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(40);
            entity.HasIndex(x => new { x.Status, x.CreatedAt });
        });

        modelBuilder.Entity<AuditRecord>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Action).HasMaxLength(100).IsRequired();
            entity.Property(x => x.EntityType).HasMaxLength(100).IsRequired();
            entity.HasIndex(x => new { x.EntityType, x.EntityId });
            entity.HasIndex(x => x.CorrelationId);
        });

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.EventType).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Payload).IsRequired();
            entity.HasIndex(x => new { x.PublishedAt, x.OccurredAt });
            entity.HasIndex(x => x.CorrelationId);
        });

        modelBuilder.Entity<HumanReviewRecord>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Reviewer).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Decision).HasConversion<string>().HasMaxLength(30);
            entity.HasIndex(x => x.EvaluationId);
        });

        modelBuilder.Entity<EvaluationRecord>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ScenarioId).HasMaxLength(200).IsRequired();
            entity.Property(x => x.EvaluatorType).HasMaxLength(100).IsRequired();
            entity.Property(x => x.ScoresJson).IsRequired();
            entity.Property(x => x.FailureReasonsJson).IsRequired();
            entity.HasIndex(x => new { x.AgentTaskId, x.CreatedAt });
        });
    }
}
