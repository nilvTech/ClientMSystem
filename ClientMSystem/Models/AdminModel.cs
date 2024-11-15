using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace ClientMSystem.Models
{
    public class AdminModel
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]

        [DataType(DataType.Password)]
        public string Password { get; set; }


    }
}
