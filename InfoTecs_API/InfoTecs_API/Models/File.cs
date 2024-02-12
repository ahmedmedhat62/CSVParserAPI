using System.ComponentModel.DataAnnotations;

namespace InfoTecs_API.Models
{
    public class CSV_File
    {
        public string file_name { get; set; }

        [Key]
        public Guid file_id { get; set; }
    }
}
