using Common.Utils.Const;
using Microsoft.EntityFrameworkCore;
using RequestTicketService.Domain.Models;
using Shared.Infrastructure.Context;

namespace RequestTicketService.Infrastructure.Data.Contexts;

public partial class RequestTicketServiceContext : AppDbContext
{
    public RequestTicketServiceContext(DbContextOptions<RequestTicketServiceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<RequestTicket> RequestTickets { get; set; }

    public virtual DbSet<RequestTicketChat> RequestTicketChats { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            DotNetEnv.Env.Load(); 

            var connectionString = Environment.GetEnvironmentVariable(ConstEnv.RequestTicketServiceDB);

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Missing CONNECTION_STRING environment variable");

            optionsBuilder.UseNpgsql(connectionString);
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RequestTicket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("request_tickets_pkey");

            entity.ToTable("request_tickets");

            entity.HasIndex(e => e.CounselorId, "idx_request_tickets_counselor_id");

            entity.HasIndex(e => e.PriorityId, "idx_request_tickets_priority_id");

            entity.HasIndex(e => e.StatusId, "idx_request_tickets_status_id");

            entity.HasIndex(e => e.StudentId, "idx_request_tickets_student_id");

            entity.Property(e => e.TicketId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("ticket_id");
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .HasColumnName("category");
            entity.Property(e => e.ClosedAt).HasColumnName("closed_at");
            entity.Property(e => e.CounselorId).HasColumnName("counselor_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(256)
                .HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.PriorityId)
                .HasDefaultValue((short)2)
                .HasColumnName("priority_id");
            entity.Property(e => e.ResolvedAt).HasColumnName("resolved_at");
            entity.Property(e => e.StatusId)
                .HasDefaultValue((short)1)
                .HasColumnName("status_id");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(256)
                .HasColumnName("updated_by");
        });

        modelBuilder.Entity<RequestTicketChat>(entity =>
        {
            entity.HasKey(e => e.ChatId).HasName("request_ticket_chat_pkey");

            entity.ToTable("request_ticket_chat");

            entity.HasIndex(e => e.CreatedAt, "idx_request_ticket_chat_created_at");

            entity.HasIndex(e => e.TicketId, "idx_request_ticket_chat_ticket_id");

            entity.Property(e => e.ChatId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("chat_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(256)
                .HasColumnName("created_by");
            entity.Property(e => e.FileUrl)
                .HasMaxLength(500)
                .HasColumnName("file_url");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsInternal)
                .HasDefaultValue(false)
                .HasColumnName("is_internal");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.MessageTypeId)
                .HasDefaultValue((short)1)
                .HasColumnName("message_type_id");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(256)
                .HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Ticket).WithMany(p => p.RequestTicketChats)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("request_ticket_chat_ticket_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
