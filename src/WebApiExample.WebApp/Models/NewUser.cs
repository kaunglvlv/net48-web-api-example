using System.ComponentModel.DataAnnotations;

namespace WebApiExample.WebApp.Models
{
    public class NewUser
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int Age { get; set; }
    }
}