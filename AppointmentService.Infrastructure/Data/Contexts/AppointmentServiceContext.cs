﻿using System;
using System.Collections.Generic;
using AppointmentService.Domain.WriteModels;
using AppointmentService.Infrastructure;
using Common.Utils.Const;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Context;

namespace AppointmentService.Infrastructure.Data.Contexts;

public partial class AppointmentServiceContext : AppDbContext
{
    public AppointmentServiceContext(DbContextOptions<AppointmentServiceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdmissionDocument> AdmissionDocuments { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<CounselorScheduleDetail> CounselorScheduleDetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<TimeSlot> TimeSlots { get; set; }

    public virtual DbSet<Weekday> Weekdays { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            DotNetEnv.Env.Load(); 

            var connectionString = Environment.GetEnvironmentVariable(ConstEnv.AppointmentServiceDB);

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Missing CONNECTION_STRING environment variable");

            optionsBuilder.UseNpgsql(connectionString);
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdmissionDocument>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("admission_documents_pkey");

            entity.ToTable("admission_documents");

            entity.Property(e => e.DocumentId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("document_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(256)
                .HasColumnName("created_by");
            entity.Property(e => e.FilePath)
                .HasMaxLength(500)
                .HasColumnName("file_path");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(256)
                .HasColumnName("updated_by");
            entity.Property(e => e.UploaderId).HasColumnName("uploader_id");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("appointments_pkey");

            entity.ToTable("appointments");
            
            entity.HasIndex(e => e.AppointmentDate, "idx_appointments_date");

            entity.HasIndex(e => e.StatusId, "idx_appointments_status_id");

            entity.HasIndex(e => e.StudentId, "idx_appointments_student_id");

            entity.Property(e => e.AppointmentId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("appointment_id");
            entity.Property(e => e.AppointmentDate).HasColumnName("appointment_date");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(256)
                .HasColumnName("created_by");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
            entity.Property(e => e.StatusId)
                .HasDefaultValue((short)1)
                .HasColumnName("status_id");
            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(256)
                .HasColumnName("updated_by");

            entity.HasOne(d => d.Schedule).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.ScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_schedule");
        });

        modelBuilder.Entity<CounselorScheduleDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("counselor_schedule_details_pkey");

            entity.ToTable("counselor_schedule_details");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CounselorId).HasColumnName("counselor_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(256)
                .HasColumnName("created_by");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.SlotId).HasColumnName("slot_id");
            entity.Property(e => e.Status)
                .HasDefaultValue((short)1)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(256)
                .HasColumnName("updated_by");
            entity.Property(e => e.WeekdayId).HasColumnName("weekday_id");

            entity.HasOne(d => d.Slot).WithMany(p => p.CounselorScheduleDetails)
                .HasForeignKey(d => d.SlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_slot");

            entity.HasOne(d => d.Weekday).WithMany(p => p.CounselorScheduleDetails)
                .HasForeignKey(d => d.WeekdayId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_weekday");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("payments_pkey");

            entity.ToTable("payments");

            entity.HasIndex(e => e.AppointmentId, "idx_payments_appointment_id");

            entity.HasIndex(e => e.TransactionId, "idx_payments_transaction_id");

            entity.HasIndex(e => e.TransactionId, "payments_transaction_id_key").IsUnique();

            entity.Property(e => e.PaymentId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(256)
                .HasColumnName("created_by");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");
            entity.Property(e => e.StatusId)
                .HasDefaultValue((short)1)
                .HasColumnName("status_id");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(255)
                .HasColumnName("transaction_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(256)
                .HasColumnName("updated_by");

            entity.HasOne(d => d.Appointment).WithMany(p => p.Payments)
                .HasForeignKey(d => d.AppointmentId)
                .HasConstraintName("payments_appointment_id_fkey");
        });

        modelBuilder.Entity<TimeSlot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("time_slots_pkey");

            entity.ToTable("time_slots");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('time_slots_slot_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
        });

        modelBuilder.Entity<Weekday>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("weekdays_pkey");

            entity.ToTable("weekdays");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.DayName)
                .HasMaxLength(50)
                .HasColumnName("day_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
