using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace sait.Models
{
    public class User : IdentityUser
    {
        public override bool PhoneNumberConfirmed { get => true; set { } }
        public override bool EmailConfirmed { get => true; set { } }
        public override bool TwoFactorEnabled { get => false; set { } }
        public override string PhoneNumber { get => ""; set { } }
    }
    











    
        
}