using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace sait.Models
{
    public class ProfileView
    {
        public IdentityUser UserName { get; set; }
        public string Email { get; set; }

        public bool Status {  get; set; }

        
        public IEnumerable<Request> Requests { get; set; }
    }
}
