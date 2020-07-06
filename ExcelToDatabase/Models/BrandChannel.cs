using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelToDatabase.Models
{
    public class BrandChannel
    {
        [Key]
        public long Id { get; set; }
        public string Brand { get; set; }
        public long YY_Year { get; set; }
        public long MM_Month { get; set; }
        public string DocNo { get; set; }
        public string Channel { get; set; }
        public string TimeBandStart { get; set; }
        public string TimeBandEnd { get; set; }
        public double Amount { get; set; } = 0;
        public DateTime ActivityDate { get; set; }
        public string MWUID { get; set; }
        public bool IsMonitored { get; set; }
        public bool IsDisputed { get; set; }
        public string ImpactBase { get; set; }

    }
}
