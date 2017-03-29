using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace THT.Service
{
    public class AccountMembershipService
    {
        public bool ValidateUser(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");
            return System.Web.Security.Membership.Provider.ValidateUser(userName, password);
        }
    }
}