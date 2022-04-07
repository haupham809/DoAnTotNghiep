using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace HOCONLINE.Models
{
    public partial class DB : DbContext
    {
        public DB()
            : base("name=DB")
        {
        }

        public virtual DbSet<BaiTap> BaiTaps { get; set; }
        public virtual DbSet<BaiTapTL> BaiTapTLs { get; set; }
        public virtual DbSet<BaiTapTN> BaiTapTNs { get; set; }
        public virtual DbSet<CauHoi> CauHois { get; set; }
        public virtual DbSet<DapAn> DapAns { get; set; }
        public virtual DbSet<FileBTTL> FileBTTLs { get; set; }
        public virtual DbSet<FileTB> FileTBs { get; set; }
        public virtual DbSet<Loimoi> Loimois { get; set; }
        public virtual DbSet<LopHoc> LopHocs { get; set; }
        public virtual DbSet<Mess> Messes { get; set; }
        public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }
        public virtual DbSet<ThanhVienLop> ThanhVienLops { get; set; }
        public virtual DbSet<ThongBao> ThongBaos { get; set; }
        public virtual DbSet<TTBaiTapTL> TTBaiTapTLs { get; set; }
        public virtual DbSet<TTBaiTapTN> TTBaiTapTNs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BaiTap>()
                .HasMany(e => e.BaiTapTLs)
                .WithRequired(e => e.BaiTap)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BaiTap>()
                .HasMany(e => e.BaiTapTNs)
                .WithRequired(e => e.BaiTap)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BaiTap>()
                .HasMany(e => e.FileBTTLs)
                .WithOptional(e => e.BaiTap)
                .HasForeignKey(e => e.MaBt);

            modelBuilder.Entity<DapAn>()
                .HasMany(e => e.TTBaiTapTNs)
                .WithOptional(e => e.DapAn)
                .HasForeignKey(e => e.MaDapAnluaChon);

            modelBuilder.Entity<LopHoc>()
                .HasMany(e => e.Loimois)
                .WithRequired(e => e.LopHoc)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LopHoc>()
                .HasMany(e => e.ThanhVienLops)
                .WithRequired(e => e.LopHoc)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TaiKhoan>()
                .HasMany(e => e.BaiTapTLs)
                .WithOptional(e => e.TaiKhoan)
                .HasForeignKey(e => e.NguoiNop);

            modelBuilder.Entity<TaiKhoan>()
                .HasMany(e => e.BaiTapTNs)
                .WithOptional(e => e.TaiKhoan)
                .HasForeignKey(e => e.NguoiNop);

            modelBuilder.Entity<TaiKhoan>()
                .HasMany(e => e.Loimois)
                .WithRequired(e => e.TaiKhoan)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TaiKhoan>()
                .HasMany(e => e.LopHocs)
                .WithOptional(e => e.TaiKhoan)
                .HasForeignKey(e => e.NguoiTao);

            modelBuilder.Entity<TaiKhoan>()
                .HasMany(e => e.Messes)
                .WithOptional(e => e.TaiKhoan)
                .HasForeignKey(e => e.NguoiGui);

            modelBuilder.Entity<TaiKhoan>()
                .HasMany(e => e.Messes1)
                .WithOptional(e => e.TaiKhoan1)
                .HasForeignKey(e => e.NguoiNhan);

            modelBuilder.Entity<TaiKhoan>()
                .HasMany(e => e.ThanhVienLops)
                .WithRequired(e => e.TaiKhoan)
                .HasForeignKey(e => e.Mathanhvien)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TaiKhoan>()
                .HasMany(e => e.ThongBaos)
                .WithOptional(e => e.TaiKhoan)
                .HasForeignKey(e => e.NguoiDang);

            modelBuilder.Entity<TaiKhoan>()
                .HasMany(e => e.TTBaiTapTLs)
                .WithOptional(e => e.TaiKhoan)
                .HasForeignKey(e => e.NguoiNop);

            modelBuilder.Entity<TaiKhoan>()
                .HasMany(e => e.TTBaiTapTNs)
                .WithOptional(e => e.TaiKhoan)
                .HasForeignKey(e => e.NguoiNop);

            modelBuilder.Entity<ThongBao>()
                .HasMany(e => e.FileTBs)
                .WithOptional(e => e.ThongBao)
                .HasForeignKey(e => e.maTB);
        }
    }
}
