using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasicSecurity.Models
{
    public class User
    {
        public string Name {
            get; set;
        }

        public User(string userName)
        {
            Name = userName;
        }

    }
}