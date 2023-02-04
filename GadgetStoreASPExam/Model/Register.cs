using System.ComponentModel.DataAnnotations;

namespace GadgetStoreASPExam.Model
{
    public class Register
    {
        [Required(ErrorMessage = "User name is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "User name is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "User email is required")]
        public string Email { get; set; }
    }
}
