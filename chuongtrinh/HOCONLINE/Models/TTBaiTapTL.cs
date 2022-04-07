namespace HOCONLINE.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TTBaiTapTL")]
    public partial class TTBaiTapTL
    {
        [Key]
        public long Ma { get; set; }

        [StringLength(100)]
        public string NoiLuu { get; set; }

        public DateTime? NgayNop { get; set; }

        public long? MaBaiNop { get; set; }

        [StringLength(20)]
        public string NguoiNop { get; set; }

        [StringLength(200)]
        public string Tenfile { get; set; }

        public virtual BaiTapTL BaiTapTL { get; set; }

        public virtual TaiKhoan TaiKhoan { get; set; }
    }
}
