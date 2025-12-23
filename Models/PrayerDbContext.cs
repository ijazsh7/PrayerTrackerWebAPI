using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PrayerTrackerWebAPI.Models;

public partial class PrayerDbContext : DbContext
{
    public PrayerDbContext()
    {
    }

    public PrayerDbContext(DbContextOptions<PrayerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdminLog> AdminLogs { get; set; }

    public virtual DbSet<Prayer> Prayers { get; set; }

    public virtual DbSet<PrayerGuidance> PrayerGuidances { get; set; }

    public virtual DbSet<PrayerRecord> PrayerRecords { get; set; }

    public virtual DbSet<User> Users { get; set; }    

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Name=DefaultConnection"); // Reads from appsettings.json
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdminLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__AdminLog__5E5499A8D21362FE");

            entity.Property(e => e.LogId).HasColumnName("LogID");
            entity.Property(e => e.Action).HasMaxLength(255);
            entity.Property(e => e.AdminId).HasColumnName("AdminID");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Admin).WithMany(p => p.AdminLogs)
                .HasForeignKey(d => d.AdminId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AdminLogs__Admin__31EC6D26");
        });

        modelBuilder.Entity<Prayer>(entity =>
        {
            entity.HasKey(e => e.PrayerId).HasName("PK__Prayers__72701224125A8A6E");

            entity.Property(e => e.PrayerId).HasColumnName("PrayerID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<PrayerGuidance>(entity =>
        {
            entity.HasKey(e => e.GuidanceId).HasName("PK__PrayerGu__FBED6C864E91624E");

            entity.ToTable("PrayerGuidance");

            entity.Property(e => e.GuidanceId).HasColumnName("GuidanceID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.VideoUrl)
                .HasMaxLength(500)
                .HasColumnName("VideoURL");
        });

        modelBuilder.Entity<PrayerRecord>(entity =>
        {
            entity.HasKey(e => e.PrayerRecordId).HasName("PK__PrayerRe__6E62241E79F52DA3");

            entity.Property(e => e.PrayerRecordId).HasColumnName("PrayerRecordID");
            entity.Property(e => e.PrayerId).HasColumnName("PrayerID");
            entity.Property(e => e.RecordedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Prayer).WithMany(p => p.PrayerRecords)
                .HasForeignKey(d => d.PrayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PrayerRec__Praye__32E0915F");

            entity.HasOne(d => d.User).WithMany(p => p.PrayerRecords)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PrayerRec__UserI__33D4B598");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC0F5A594D");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534FD90AD31").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
