using System.ComponentModel.DataAnnotations;

namespace GadgetStoreASPExam.Model
{
    public class Login
    {
        [Required(ErrorMessage ="User name is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "User name is required")]
        public string Password { get; set; }
    }
}
