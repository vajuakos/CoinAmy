using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoinAmy
{
    internal class UserDataModel
    {
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }

        public UserDataModel(string username, string email, string password)
        {
            this.username = username;
            this.email = email;
            this.password = password;
        }
    }
}
