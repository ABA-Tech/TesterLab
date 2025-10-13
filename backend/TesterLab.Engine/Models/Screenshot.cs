using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TesterLab.Domain.Models;

namespace TesterLab.Engine.Models
{
    public class Screenshot
    {
        [Key]
        public int Id { get; set; }

        public int TestExecutionId { get; set; }

        public int TestStepId { get; set; }

        [Required]
        public string ImageData { get; set; } // Path to file or Base64 string

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string Description { get; set; }

        // Navigation properties
        [ForeignKey("TestExecutionId")]
        public virtual TestExecution TestExecution { get; set; }

        [ForeignKey("TestStepId")]
        public virtual TestStep TestStep { get; set; }
    }

}
