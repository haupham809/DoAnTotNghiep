namespace DOANTOTNGHIEP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("document")]
    public partial class document
    {
        [Key]
        public long Ma { get; set; }

        public string Ten { get; set; }

        public string Vitriluu { get; set; }

        [StringLength(10)]
        public string Noidung { get; set; }

        [StringLength(20)]
        public string Nguoisohuu { get; set; }

        public virtual TaiKhoan TaiKhoan { get; set; }
    }
}