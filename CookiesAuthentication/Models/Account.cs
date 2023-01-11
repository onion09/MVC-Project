using System.ComponentModel.DataAnnotations;

namespace CookiesAuthentication.Models
{
    public class Account
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
