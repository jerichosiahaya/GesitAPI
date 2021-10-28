using System;
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

        public virtual DbSet<Checklist> Checklists { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<ProgoDocument> ProgoDocuments { get; set; }
        public virtual DbSet<ProgoProject> ProgoProjects { get; set; }
        public virtual DbSet<Rha> Rhas { get; set; }
        public virtual DbSet<SubRha> SubRhas { get; set; }
        public virtual DbSet<SubRhaevidence> SubRhaevidences { get; set; }
        public virtual DbSet<SubRhaimage> SubRhaimages { get; set; }
        public virtual DbSet<TindakLanjut> TindakLanjuts { get; set; }
        public virtual DbSet<TindakLanjutEvidence> TindakLanjutEvidences { get; set; }
        public virtual DbSet<User> Users { get; set; }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//                optionsBuilder.UseSqlServer("Server=35.219.8.90;Initial Catalog=GesitDb;Persist Security Info=True;User ID=sa;Password=bni46SQL;");
//            }
//        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Checklist>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Checklist");

                entity.Property(e => e.AipId)
                    .IsRequired()
                    .HasMaxLength(1)
                    .HasColumnName("aip_id");

                entity.Property(e => e.CapexOpex)
                    .HasColumnType("text")
                    .HasColumnName("capex_opex");

                entity.Property(e => e.CostBenefitAnalysis)
                    .HasColumnType("text")
                    .HasColumnName("cost_benefit_analysis");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.IzinRegulator)
                    .HasColumnType("text")
                    .HasColumnName("izin_regulator");

                entity.Property(e => e.KategoriProject)
                    .HasMaxLength(150)
                    .HasColumnName("kategori_project");

                entity.Property(e => e.NamaAip)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("nama_aip");

                entity.Property(e => e.NewEnhance)
                    .HasMaxLength(150)
                    .HasColumnName("new_enhance");

                entity.Property(e => e.PengadaanInhouse)
                    .HasMaxLength(150)
                    .HasColumnName("pengadaan_inhouse");

                entity.Property(e => e.ProjectCategory)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("project_category");

                entity.Property(e => e.ProjectId).HasColumnName("project_id");

                entity.Property(e => e.ProjectTitle)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("project_title");

                entity.Property(e => e.Requirement)
                    .HasColumnType("text")
                    .HasColumnName("requirement");

                entity.Property(e => e.Risk)
                    .HasColumnType("text")
                    .HasColumnName("risk");

                entity.Property(e => e.Severity)
                    .HasColumnType("text")
                    .HasColumnName("severity");

                entity.Property(e => e.SeveritySystem)
                    .HasColumnType("text")
                    .HasColumnName("severity_system");

                entity.Property(e => e.SystemImpact)
                    .HasColumnType("text")
                    .HasColumnName("system_impact");

                entity.Property(e => e.TargetImplementasi)
                    .HasMaxLength(150)
                    .HasColumnName("target_implementasi");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");
            });

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

                entity.Property(e => e.ProjectId)
                    .HasMaxLength(255)
                    .HasColumnName("project_id");

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

            modelBuilder.Entity<ProgoDocument>(entity =>
            {
                entity.ToTable("ProgoDocument");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AipId)
                    .HasMaxLength(255)
                    .HasColumnName("aip_id");

                entity.Property(e => e.JenisDokumen)
                    .HasMaxLength(255)
                    .HasColumnName("jenis_dokumen");

                entity.Property(e => e.NamaFile).HasColumnName("nama_file");

                entity.Property(e => e.Tahun).HasColumnName("tahun");

                entity.Property(e => e.TaskId)
                    .HasMaxLength(255)
                    .HasColumnName("task_id");

                entity.Property(e => e.UrlDownloadFile)
                    .HasColumnType("text")
                    .HasColumnName("url_download_file");

                entity.HasOne(d => d.Aip)
                    .WithMany(p => p.ProgoDocuments)
                    .HasForeignKey(d => d.AipId)
                    .HasConstraintName("FK__ProgoDocu__aip_i__2704CA5F");
            });

            modelBuilder.Entity<ProgoProject>(entity =>
            {
                entity.HasKey(e => e.AipId)
                    .HasName("PK__ProgoPro__2977E19143EF68E6");

                entity.ToTable("ProgoProject");

                entity.Property(e => e.AipId)
                    .HasMaxLength(255)
                    .HasColumnName("aip_id");

                entity.Property(e => e.AplikasiTerdampak)
                    .HasMaxLength(255)
                    .HasColumnName("aplikasi_terdampak");

                entity.Property(e => e.Divisi)
                    .HasMaxLength(255)
                    .HasColumnName("divisi");

                entity.Property(e => e.Durasi).HasColumnName("durasi");

                entity.Property(e => e.EksImplementasi)
                    .HasMaxLength(255)
                    .HasColumnName("eks_implementasi");

                entity.Property(e => e.EstimasiBiayaCapex).HasColumnName("estimasi_biaya_capex");

                entity.Property(e => e.EstimasiBiayaOpex).HasColumnName("estimasi_biaya_opex");

                entity.Property(e => e.JenisPengembangan)
                    .HasMaxLength(255)
                    .HasColumnName("jenis_pengembangan");

                entity.Property(e => e.Lob)
                    .HasMaxLength(255)
                    .HasColumnName("lob");

                entity.Property(e => e.LokasiDc)
                    .HasMaxLength(255)
                    .HasColumnName("lokasi_dc");

                entity.Property(e => e.LokasiDrc)
                    .HasMaxLength(255)
                    .HasColumnName("lokasi_drc");

                entity.Property(e => e.NamaAip)
                    .HasColumnType("text")
                    .HasColumnName("nama_aip");

                entity.Property(e => e.NamaLob)
                    .HasMaxLength(255)
                    .HasColumnName("nama_lob");

                entity.Property(e => e.NamaProject)
                    .HasMaxLength(255)
                    .HasColumnName("nama_project");

                entity.Property(e => e.NamaSquad)
                    .HasMaxLength(255)
                    .HasColumnName("nama_squad");

                entity.Property(e => e.Pengembang)
                    .HasMaxLength(255)
                    .HasColumnName("pengembang");

                entity.Property(e => e.PeriodeAip).HasColumnName("periode_aip");

                entity.Property(e => e.PpjtiPihakTerkait)
                    .HasMaxLength(255)
                    .HasColumnName("ppjti_pihak_terkait");

                entity.Property(e => e.ProjectBudget).HasColumnName("project_budget");

                entity.Property(e => e.ProjectCategory)
                    .HasMaxLength(255)
                    .HasColumnName("project_category");

                entity.Property(e => e.ProjectDemandValue).HasColumnName("project_demand_value");

                entity.Property(e => e.ProjectId)
                    .HasMaxLength(255)
                    .HasColumnName("project_id");

                entity.Property(e => e.Squad)
                    .HasMaxLength(255)
                    .HasColumnName("squad");

                entity.Property(e => e.StatusAip)
                    .HasMaxLength(255)
                    .HasColumnName("status_aip");

                entity.Property(e => e.StrategicImportance)
                    .HasMaxLength(255)
                    .HasColumnName("strategic_importance");

                entity.Property(e => e.TahunCreate).HasColumnName("tahun_create");
            });

            modelBuilder.Entity<Rha>(entity =>
            {
                entity.ToTable("RHA");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Assign)
                    .HasMaxLength(255)
                    .HasColumnName("assign");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(255)
                    .HasColumnName("created_by");

                entity.Property(e => e.DirSekor)
                    .HasMaxLength(255)
                    .HasColumnName("dir_sekor");

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
                    .HasMaxLength(255)
                    .HasColumnName("kondisi");

                entity.Property(e => e.Rekomendasi)
                    .HasMaxLength(255)
                    .HasColumnName("rekomendasi");

                entity.Property(e => e.StatusJt)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("status_jt");

                entity.Property(e => e.StatusTemuan)
                    .HasMaxLength(25)
                    .HasColumnName("status_temuan");

                entity.Property(e => e.SubKondisi)
                    .HasMaxLength(255)
                    .HasColumnName("sub_kondisi");

                entity.Property(e => e.TargetDate)
                    .HasColumnType("date")
                    .HasColumnName("target_date");

                entity.Property(e => e.Uic)
                    .HasMaxLength(255)
                    .HasColumnName("UIC");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");
            });

            modelBuilder.Entity<SubRha>(entity =>
            {
                entity.ToTable("SubRHA");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Assign)
                    .HasMaxLength(255)
                    .HasColumnName("assign");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.DivisiBaru)
                    .HasMaxLength(255)
                    .HasColumnName("divisi_baru");

                entity.Property(e => e.JatuhTempo).HasColumnName("jatuh_tempo");

                entity.Property(e => e.Lokasi)
                    .HasMaxLength(255)
                    .HasColumnName("lokasi");

                entity.Property(e => e.Masalah)
                    .HasColumnType("text")
                    .HasColumnName("masalah");

                entity.Property(e => e.NamaAudit)
                    .HasMaxLength(255)
                    .HasColumnName("nama_audit");

                entity.Property(e => e.Nomor).HasColumnName("nomor");

                entity.Property(e => e.OpenClose)
                    .HasMaxLength(255)
                    .HasColumnName("open_close");

                entity.Property(e => e.Pendapat)
                    .HasColumnType("text")
                    .HasColumnName("pendapat");

                entity.Property(e => e.RhaId).HasColumnName("rha_id");

                entity.Property(e => e.Status)
                    .HasMaxLength(255)
                    .HasColumnName("status");

                entity.Property(e => e.StatusJatuhTempo)
                    .HasMaxLength(255)
                    .HasColumnName("status_jatuh_tempo");

                entity.Property(e => e.TahunTemuan).HasColumnName("tahun_temuan");

                entity.Property(e => e.UicBaru)
                    .HasMaxLength(255)
                    .HasColumnName("uic_baru");

                entity.Property(e => e.UicLama)
                    .HasMaxLength(255)
                    .HasColumnName("uic_lama");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UsulClose)
                    .HasColumnType("text")
                    .HasColumnName("usul_close");
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

                entity.Property(e => e.Notes)
                    .HasColumnType("text")
                    .HasColumnName("notes");

                entity.Property(e => e.SubRhaId).HasColumnName("sub_rha_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.SubRha)
                    .WithMany(p => p.SubRhaevidences)
                    .HasForeignKey(d => d.SubRhaId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__SubRHAEvi__sub_r__719CDDE7");
            });

            modelBuilder.Entity<SubRhaimage>(entity =>
            {
                entity.ToTable("SubRHAImage");

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
                    .WithMany(p => p.SubRhaimages)
                    .HasForeignKey(d => d.SubRhaId)
                    .HasConstraintName("FK__SubRHAIma__sub_r__09746778");
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
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__TindakLan__sub_r__1EA48E88");
            });

            modelBuilder.Entity<TindakLanjutEvidence>(entity =>
            {
                entity.ToTable("TindakLanjutEvidence");

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

                entity.Property(e => e.TindaklanjutId).HasColumnName("tindaklanjut_id");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.Tindaklanjut)
                    .WithMany(p => p.TindakLanjutEvidences)
                    .HasForeignKey(d => d.TindaklanjutId)
                    .HasConstraintName("FK__TindakLan__tinda__69FBBC1F");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Npp)
                    .HasName("PK__User__DF90E5D6663EDFC5");

                entity.ToTable("User");

                entity.Property(e => e.Npp)
                    .HasMaxLength(255)
                    .HasColumnName("npp");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.Division)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("division");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("first_name");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.LastName)
                    .HasMaxLength(255)
                    .HasColumnName("last_name");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("password");

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("role");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
