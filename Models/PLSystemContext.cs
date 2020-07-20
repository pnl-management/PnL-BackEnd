using System;
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
        public virtual DbSet<BrandParticipantsDetail> BrandParticipantsDetail { get; set; }
        public virtual DbSet<Evidence> Evidence { get; set; }
        public virtual DbSet<Participant> Participant { get; set; }
        public virtual DbSet<Receipt> Receipt { get; set; }
        public virtual DbSet<RecepitTransactionDetail> RecepitTransactionDetail { get; set; }
        public virtual DbSet<Store> Store { get; set; }
        public virtual DbSet<StoreParticipantsDetail> StoreParticipantsDetail { get; set; }
        public virtual DbSet<Transaction> Transaction { get; set; }
        public virtual DbSet<TransactionCategory> TransactionCategory { get; set; }
        public virtual DbSet<TransactionJourney> TransactionJourney { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<AccountingPeriod>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.BrandId).HasColumnName("brandId");

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
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedTime)
                    .HasColumnName("createdTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<BrandParticipantsDetail>(entity =>
            {
                entity.HasKey(e => new { e.BrandId, e.ParticipantsId });

                entity.Property(e => e.BrandId).HasColumnName("brandId");

                entity.Property(e => e.ParticipantsId).HasColumnName("participantsId");

                entity.Property(e => e.CreatedTime)
                    .HasColumnName("createdTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(500);

                entity.Property(e => e.LastModified)
                    .HasColumnName("lastModified")
                    .HasColumnType("datetime");

                entity.Property(e => e.Role).HasColumnName("role");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.BrandParticipantsDetail)
                    .HasForeignKey(d => d.BrandId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BrandParticipantsDetail_Brand");

                entity.HasOne(d => d.Participants)
                    .WithMany(p => p.BrandParticipantsDetail)
                    .HasForeignKey(d => d.ParticipantsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BrandParticipantsDetail_Participant");
            });

            modelBuilder.Entity<Evidence>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.ReceiptId).HasColumnName("receiptId");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasColumnName("url")
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.HasOne(d => d.Receipt)
                    .WithMany(p => p.Evidence)
                    .HasForeignKey(d => d.ReceiptId)
                    .HasConstraintName("FK_Evidence_Receipt");
            });

            modelBuilder.Entity<Participant>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedTime)
                    .HasColumnName("createdTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Fullname)
                    .HasColumnName("fullname")
                    .HasMaxLength(20);

                entity.Property(e => e.LastModified)
                    .HasColumnName("lastModified")
                    .HasColumnType("datetime");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Receipt>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.BrandId).HasColumnName("brandId");

                entity.Property(e => e.CategoryId).HasColumnName("categoryId");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedTime)
                    .HasColumnName("createdTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.LastModified)
                    .HasColumnName("lastModified")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastModifiedBy).HasColumnName("lastModifiedBy");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.StoreId).HasColumnName("storeId");

                entity.Property(e => e.Value)
                    .HasColumnName("value")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Receipt)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_Receipt_Brand");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Receipt)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Receipt_Transaction Category");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ReceiptCreatedByNavigation)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_Receipt_Participant");

                entity.HasOne(d => d.LastModifiedByNavigation)
                    .WithMany(p => p.ReceiptLastModifiedByNavigation)
                    .HasForeignKey(d => d.LastModifiedBy)
                    .HasConstraintName("FK_Receipt_Participant_2");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.Receipt)
                    .HasForeignKey(d => d.StoreId)
                    .HasConstraintName("FK_Receipt_Store");
            });

            modelBuilder.Entity<RecepitTransactionDetail>(entity =>
            {
                entity.HasKey(e => new { e.ReceiptId, e.TransactionId })
                    .HasName("PK_Receipt-Transaction");

                entity.Property(e => e.ReceiptId).HasColumnName("receiptId");

                entity.Property(e => e.TransactionId).HasColumnName("transactionId");

                entity.Property(e => e.CreatedById).HasColumnName("createdById");

                entity.Property(e => e.CreatedTime)
                    .HasColumnName("createdTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastModified)
                    .HasColumnName("lastModified")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastModifiedById).HasColumnName("lastModifiedById");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.CreatedBy)
                    .WithMany(p => p.RecepitTransactionDetailCreatedBy)
                    .HasForeignKey(d => d.CreatedById)
                    .HasConstraintName("FK_RecepitTransactionDetail_Participant_1");

                entity.HasOne(d => d.LastModifiedBy)
                    .WithMany(p => p.RecepitTransactionDetailLastModifiedBy)
                    .HasForeignKey(d => d.LastModifiedById)
                    .HasConstraintName("FK_RecepitTransactionDetail_Participant_2");

                entity.HasOne(d => d.Receipt)
                    .WithMany(p => p.RecepitTransactionDetail)
                    .HasForeignKey(d => d.ReceiptId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecepitTransactionDetail_Receipt");

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.RecepitTransactionDetail)
                    .HasForeignKey(d => d.TransactionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecepitTransactionDetail_Transaction");
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.BrandId).HasColumnName("brandId");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Store)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_Store_Brand");
            });

            modelBuilder.Entity<StoreParticipantsDetail>(entity =>
            {
                entity.HasKey(e => new { e.StoreId, e.ParticipantId })
                    .HasName("PK_Store-Participants");

                entity.Property(e => e.StoreId).HasColumnName("storeId");

                entity.Property(e => e.ParticipantId).HasColumnName("participantId");

                entity.Property(e => e.CreatedTime)
                    .HasColumnName("createdTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastModified)
                    .HasColumnName("lastModified")
                    .HasColumnType("datetime");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.Participant)
                    .WithMany(p => p.StoreParticipantsDetail)
                    .HasForeignKey(d => d.ParticipantId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreParticipantsDetail_Participant");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.StoreParticipantsDetail)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreParticipantsDetail_Store");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.BrandId).HasColumnName("brandId");

                entity.Property(e => e.CategoryId).HasColumnName("categoryId");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedTime)
                    .HasColumnName("createdTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.PeriodId).HasColumnName("periodId");

                entity.Property(e => e.StoreId).HasColumnName("storeId");

                entity.Property(e => e.Value)
                    .HasColumnName("value")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Transaction)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_Transaction_Brand");

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

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.Transaction)
                    .HasForeignKey(d => d.StoreId)
                    .HasConstraintName("FK_Transaction_Store");
            });

            modelBuilder.Entity<TransactionCategory>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.BrandId).HasColumnName("brandId");

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

            modelBuilder.Entity<TransactionJourney>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreatedTime)
                    .HasColumnName("createdTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.FeedBack).HasColumnName("feedBack");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TransactionId).HasColumnName("transactionId");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.TransactionJourney)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK_TransactionJourney_Participant");

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.TransactionJourney)
                    .HasForeignKey(d => d.TransactionId)
                    .HasConstraintName("FK_TransactionJorney_Transaction");
            });
        }
    }
}
