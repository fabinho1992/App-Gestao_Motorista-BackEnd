using Microsoft.AspNetCore.Identity;
using RotaCerta.Domain.Models;

namespace RotaCerta.Infraestructure.Context.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public Guid MotoristaId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string ResetToken { get; set; } = string.Empty;
        public DateTimeOffset ResetTokenExpiration { get; set; }
    }
}
