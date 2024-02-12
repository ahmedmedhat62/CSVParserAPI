using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace InfoTecs_API.Models
{
    public class value
    {
        [Ignore]
        [Key]
        public Guid value_id { get; set; }
        
        public int Id { get; set; }

        public string DateAndTime { get; set; }

        [Ignore] // This property will be ignored during CSV reading
        public DateTime DateAndTime1 { get; set; }
        
        //public DateTime DateAndTime { get; set; }
        public int IntegerTimeValue { get; set; }

        public double FloatingPointIndicator { get; set; }
        [Ignore]
        [ForeignKey("file_id")]
        public Guid File_Id { get; set; }

    }
}
