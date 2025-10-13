using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesterLab.Domain.Models;
using Environment = TesterLab.Domain.Models.Environment;

namespace TesterLab.Engine.Models
{

    public class TestExecution
    {
        [Key]
        public int Id { get; set; }

        public int TestCaseId { get; set; }

        public int EnvironmentId { get; set; }

        public int? DataSetId { get; set; } // Optional

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } // passed, failed, skipped, in_progress

        public string ExecutionLogs { get; set; }

        [MaxLength(100)]
        public string ExecutedBy { get; set; }

        // Navigation properties
        [ForeignKey("TestCaseId")]
        public virtual TestCase? TestCase { get; set; }

        [ForeignKey("EnvironmentId")]
        public virtual Environment? Environment { get; set; }

        [ForeignKey("DataSetId")]
        public virtual DataSet? DataSet { get; set; }

        public virtual ICollection<Screenshot>? Screenshots { get; set; }
    }

}
