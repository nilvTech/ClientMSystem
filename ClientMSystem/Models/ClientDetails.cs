using System.ComponentModel.DataAnnotations;

namespace ClientMSystem.Models
{
    public class ClientDetail
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [MaxLength(100)]
        public string ClientName { get; set; }
        [Required]
        public string IssuedDate { get; set; }
        [Required]
        [MaxLength(100)]
        public string DomainName { get; set; }
        [Required]
        [MaxLength(50)]
        public string Technology { get; set; }
        [Required]
        [MaxLength(100)]
        public string Assigned { get; set; }

        [Required]
        [MaxLength(200)]
        public string ProjectTitle { get; set; }  

        [MaxLength(500)]
        public string Description { get; set; }  
        [Required]
        public DateTime StartDate { get; set; }  

        public DateTime? EndDate { get; set; }  

        [Required]
        public decimal Budget { get; set; }     

        [MaxLength(50)]
        public string Status { get; set; }      

        [Required]
        [MaxLength(100)]
        public string ProjectManager { get; set; }

        [Required]
        [MaxLength(200)]
        public string TeamMembers { get; set; } 

        [MaxLength(200)]
        public string ClientFeedback { get; set; }

    }
}
