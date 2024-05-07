using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using backend_ProjectManagement.Models;

namespace backend_ProjectManagement.Data;

public partial class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<FileUpload> FileUploads { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectWithFile> ProjectWithFiles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-GJFDH1K\\SQLEXPRESS;Database=ProjectManagement;Trusted_Connection=False;TrustServerCertificate=True;User ID=Jasdakorn;Password=1150");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasIndex(e => e.Id, "IX_Activities");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ActivityHeaderId).HasColumnName("ActivityHeaderID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Detail).HasMaxLength(50);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.ActivityHeader).WithMany(p => p.InverseActivityHeader)
                .HasForeignKey(d => d.ActivityHeaderId)
                .HasConstraintName("FK_Activities_Activities");

            entity.HasOne(d => d.Project).WithMany(p => p.Activities)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK_Activities_Project");
        });

        modelBuilder.Entity<FileUpload>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("FileUpload");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.FileName).HasMaxLength(50);
            entity.Property(e => e.FilePath).HasMaxLength(50);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("Project");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Detail).HasMaxLength(50);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<ProjectWithFile>(entity =>
        {
            entity.ToTable("ProjectWithFile");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.FileId).HasColumnName("FileID");
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

            entity.HasOne(d => d.File).WithMany(p => p.InverseFile)
                .HasForeignKey(d => d.FileId)
                .HasConstraintName("FK_FID");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Pin)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.ProjectOwenerNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.ProjectOwener)
                .HasConstraintName("FK_User_Project");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
