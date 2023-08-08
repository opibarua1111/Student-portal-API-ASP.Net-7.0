using System.ComponentModel.DataAnnotations;

namespace Student_portal.API.Requests
{
    public class LoginStudentRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
