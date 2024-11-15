using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ClientMSystem.Models
{
    public class SignUp
    {
        [Key]
        public int ID { get; set; }
        [Required (ErrorMessage ="Please Enter First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please Enter Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please Enter User Name")]
        [Remote(action: "UserNameIsExits", controller:"Account")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please Enter FEmail Id")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter Mobile No")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please Enter Conform Password")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage =("Password Not Matched"))]
        [Display(Name ="Conform Password")]
        public string ConformPassword { get; set; }
    }
}
