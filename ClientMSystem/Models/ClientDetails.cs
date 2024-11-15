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

    }
}
