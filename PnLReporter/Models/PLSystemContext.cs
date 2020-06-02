﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PnLReporter.Models
{
    public partial class PLSystemContext : DbContext
    {
        public PLSystemContext()
        {
        }

        public PLSystemContext(DbContextOptions<PLSystemContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AccountingPeriod> AccountingPeriod { get; set; }
        public virtual DbSet<Brand> Brand { get; set; }
        public virtual DbSet<Evidence> Evidence { get; set; }
        public virtual DbSet<Participant> Participant { get; set; }
        public virtual DbSet<Transaction> Transaction { get; set; }
        public virtual DbSet<TransactionCategory> TransactionCategory { get; set; }
        public virtual DbSet<TransactionJorney> TransactionJorney { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=SE130139\\SQLEXPRESS2014;Database=P&LSystem;uid=sa;password=19091999+");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<AccountingPeriod>(entity =>
            {
                entity.HasKey(e => e.PeriodId);

                entity.Property(e => e.PeriodId)
                    .HasColumnName("periodId")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.BrandId)
                    .HasColumnName("brandId")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CreateTime)
                    .HasColumnName("createTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Deadline)
                    .HasColumnName("deadline")
                    .HasColumnType("datetime");

                entity.Property(e => e.EndDate)
                    .HasColumnName("endDate")
                    .HasColumnType("date");

                entity.Property(e => e.LastModifed)
                    .HasColumnName("lastModifed")
                    .HasColumnType("datetime");

                entity.Property(e => e.StartDate)
                    .HasColumnName("startDate")
                    .HasColumnType("date");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(100);

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.AccountingPeriod)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_Accounting Period_Brand");
            });

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.Property(e => e.BrandId)
                    .HasColumnName("brandId")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedTime)
                    .HasColumnName("createdTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<Evidence>(entity =>
            {
                entity.HasKey(e => e.Url);

                entity.Property(e => e.Url)
                    .HasColumnName("url")
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TransactionId)
                    .HasColumnName("transactionId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.Evidence)
                    .HasForeignKey(d => d.TransactionId)
                    .HasConstraintName("FK_Evidence_Transaction");
            });

            modelBuilder.Entity<Participant>(entity =>
            {
                entity.HasKey(e => e.Username);

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.BrandId)
                    .HasColumnName("brandId")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedTime)
                    .HasColumnName("createdTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(500);

                entity.Property(e => e.Fullname)
                    .HasColumnName("fullname")
                    .HasMaxLength(20);

                entity.Property(e => e.LastModified)
                    .HasColumnName("lastModified")
                    .HasColumnType("datetime");

                entity.Property(e => e.Role).HasColumnName("role");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Participant)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_Participant_Brand");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(e => e.TransactionId)
                    .HasColumnName("transactionId")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.CategoryId)
                    .HasColumnName("categoryId")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("createdBy")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedTime)
                    .HasColumnName("createdTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.MasterTransactionId)
                    .HasColumnName("masterTransactionId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.PeriodId)
                    .HasColumnName("periodId")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.Value)
                    .HasColumnName("value")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Transaction)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Transaction_Transaction Category");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Transaction)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_Transaction_Participant");

                entity.HasOne(d => d.Period)
                    .WithMany(p => p.Transaction)
                    .HasForeignKey(d => d.PeriodId)
                    .HasConstraintName("FK_Transaction_AccountingPeriod");
            });

            modelBuilder.Entity<TransactionCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryId)
                    .HasName("PK_Transaction Category");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("categoryId")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.BrandId)
                    .HasColumnName("brandId")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedTime)
                    .HasColumnName("createdTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastModified)
                    .HasColumnName("lastModified")
                    .HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(1000);

                entity.Property(e => e.Required).HasColumnName("required");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.TransactionCategory)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_Transaction Category_Brand");
            });

            modelBuilder.Entity<TransactionJorney>(entity =>
            {
                entity.HasKey(e => e.JorneyId);

                entity.Property(e => e.JorneyId)
                    .HasColumnName("jorneyId")
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("createdBy")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedTime)
                    .HasColumnName("createdTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.FeedBack).HasColumnName("feedBack");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TransactionId)
                    .HasColumnName("transactionId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.TransactionJorney)
                    .HasForeignKey(d => d.TransactionId)
                    .HasConstraintName("FK_TransactionJorney_Transaction");
            });
        }
    }
}
