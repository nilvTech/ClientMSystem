using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ClientMSystem.Models
{
    public class TimeSheet
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Date { get; set; }
        [Required]
        [MaxLength(500)]
        public string Module { get; set; }
        [Required]
        [MaxLength(1000)]
        public string ExpectedTaskToCompleted  { get; set; }
        [Required]
        [MaxLength(500)]
        public string ExpectedHours { get; set; }
        [Required]
        [MaxLength(500)]
        public string CompletedTasks { get; set; }
        [Required]
        [MaxLength(500)]
        public string UnPlannedTask { get; set; }
        [Required]
        [MaxLength(100)]
        public string ActualHours { get; set; }
        [Required]
        [MaxLength(1000)]
        public string CommentsForAnyDealy { get; set; }
        [Required]
        [MaxLength(1000)]
        public string QuestionsActionsToBeAsked { get; set; }
    }
}
