namespace DOANTOTNGHIEP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Plagiarism
    {
        [Key]
        public long Ma { get; set; }

        public double? Percents { get; set; }

        public long? Mafile { get; set; }

        public long? Comparisonfile { get; set; }

        public virtual TTBaiTapTL TTBaiTapTL { get; set; }

        public virtual TTBaiTapTL TTBaiTapTL1 { get; set; }
    }
}
