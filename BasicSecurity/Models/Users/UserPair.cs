using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasicSecurity.Models
{
    public class UserPair
    {
        public User User1 { get; set; }
        public User User2 { get; set; }

        public UserPair(User user1, User user2)
        {
            User1 = user1;
            User2 = user2;
        }
    }
}