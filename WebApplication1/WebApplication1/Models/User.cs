using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserApp.Models
{
    public class User : IdentityUser<int>
    {
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Image { get; set; }
    }
}
