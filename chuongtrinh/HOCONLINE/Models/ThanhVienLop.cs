namespace HOCONLINE.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ThanhVienLop")]
    public partial class ThanhVienLop
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long MaLop { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Mathanhvien { get; set; }

        public DateTime? NgayThamGia { get; set; }

        public virtual LopHoc LopHoc { get; set; }

        public virtual TaiKhoan TaiKhoan { get; set; }
    }
}
