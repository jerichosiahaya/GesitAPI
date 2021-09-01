﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace GesitAPI.Models
{
    public partial class GesitDbContext : DbContext
    {
        public GesitDbContext()
        {
        }

        public GesitDbContext(DbContextOptions<GesitDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Rha> Rhas { get; set; }
        public virtual DbSet<SubRha> SubRhas { get; set; }
        public virtual DbSet<SubRhaevidence> SubRhaevidences { get; set; }
        public virtual DbSet<TindakLanjut> TindakLanjuts { get; set; }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//                optionsBuilder.UseSqlServer("Data Source=35.219.8.90;Initial Catalog=GesitDb;Persist Security Info=True;User ID=sa;Password=bni46SQL");
//            }
//        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AssignedBy)
                    .HasMaxLength(150)
                    .HasColumnName("assigned_by");

                entity.Property(e => e.AssignedFor)
                    .HasMaxLength(150)
                    .HasColumnName("assigned_for");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.ProjectCategory)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("project_category");

                entity.Property(e => e.ProjectDocument)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("project_document");

                entity.Property(e => e.ProjectId).HasColumnName("project_id");

                entity.Property(e => e.ProjectTitle)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("project_title");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TargetDate)
                    .HasColumnType("date")
                    .HasColumnName("target_date");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");
            });

            modelBuilder.Entity<Rha>(entity =>
            {
                entity.ToTable("RHA");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Assign)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("assign");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(255)
                    .HasColumnName("created_by");

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("file_name");

                entity.Property(e => e.FilePath)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("file_path");

                entity.Property(e => e.FileSize).HasColumnName("file_size");

                entity.Property(e => e.FileType)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("file_type");

                entity.Property(e => e.Kondisi)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("kondisi");

                entity.Property(e => e.Rekomendasi)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("rekomendasi");

                entity.Property(e => e.SubKondisi)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("sub_kondisi");

                entity.Property(e => e.TargetDate)
                    .HasColumnType("datetime")
                    .HasColumnName("target_date");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");
            });

            modelBuilder.Entity<SubRha>(entity =>
            {
                entity.ToTable("SubRHA");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Assign)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("assign");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.DivisiBaru)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("divisi_baru");

                entity.Property(e => e.JatuhTempo)
                    .HasColumnType("date")
                    .HasColumnName("jatuh_tempo");

                entity.Property(e => e.Lokasi)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("lokasi");

                entity.Property(e => e.Masalah)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("masalah");

                entity.Property(e => e.NamaAudit)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("nama_audit");

                entity.Property(e => e.Nomor).HasColumnName("nomor");

                entity.Property(e => e.Pendapat)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("pendapat");

                entity.Property(e => e.RhaId).HasColumnName("rha_id");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("status");

                entity.Property(e => e.TahunTemuan).HasColumnName("tahun_temuan");

                entity.Property(e => e.UicBaru)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("uic_baru");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.Rha)
                    .WithMany(p => p.SubRhas)
                    .HasForeignKey(d => d.RhaId)
                    .HasConstraintName("FK__SubRHA__rha_id__18EBB532");
            });

            modelBuilder.Entity<SubRhaevidence>(entity =>
            {
                entity.ToTable("SubRHAEvidence");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("file_name");

                entity.Property(e => e.FilePath)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("file_path");

                entity.Property(e => e.FileSize).HasColumnName("file_size");

                entity.Property(e => e.FileType)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("file_type");

                entity.Property(e => e.SubRhaId).HasColumnName("sub_rha_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.SubRha)
                    .WithMany(p => p.SubRhaevidences)
                    .HasForeignKey(d => d.SubRhaId)
                    .HasConstraintName("FK__SubRHAEvi__sub_r__1BC821DD");
            });

            modelBuilder.Entity<TindakLanjut>(entity =>
            {
                entity.ToTable("TindakLanjut");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("file_name");

                entity.Property(e => e.FilePath)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("file_path");

                entity.Property(e => e.FileSize).HasColumnName("file_size");

                entity.Property(e => e.FileType)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("file_type");

                entity.Property(e => e.Notes)
                    .HasColumnType("text")
                    .HasColumnName("notes");

                entity.Property(e => e.SubRhaId).HasColumnName("sub_rha_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.SubRha)
                    .WithMany(p => p.TindakLanjuts)
                    .HasForeignKey(d => d.SubRhaId)
                    .HasConstraintName("FK__TindakLan__sub_r__1EA48E88");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
