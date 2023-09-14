using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PwServer.Models
{
    public class RegistrationModel
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }

    }
}
