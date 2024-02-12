using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfoTecs_API.Models
{
    public class Result
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("file_id")]
        public Guid file_id { get; set; }

        public int All_Time { get; set; }

        public DateTime Minimum_Date { get; set; }

        public int Average_Time { get; set; }

        public double Average_Indicator { get; set; }

        public double Median_Indicator { get; set; }

        public double Maximum_Indicator { get; set; }

        public double Minimum_Indicator { get; set; }

        public int Row_Numbers { get; set; }


    }
}
