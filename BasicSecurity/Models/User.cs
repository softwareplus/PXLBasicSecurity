using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasicSecurity.Models
{
    public class User
    {
        public string Name {get; set;}
        public long Id { get; set; }
        public string publicKey { get; set; }

        public User(string userName)
        {
            Name = userName;
        }

        //ff leeg anders moest ik een nieuw model schijven voor de user grid
        public User()
        {
        }

    }
}