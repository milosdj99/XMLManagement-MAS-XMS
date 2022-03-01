﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    class CustomPrincipal : IPrincipal
    {
        WindowsIdentity identity = null;
        public CustomPrincipal(WindowsIdentity windowsIdentity)
        {
            identity = windowsIdentity;
        }

        public IIdentity Identity
        {
            get { return identity; }
        }

        public bool IsInRole(string permission)
        {
                string name = Formatter.ParseName(identity.Name);

                
                string[] permissions;

                if (RolesConfig.GetPermissions(name, out permissions))
                {
                    if (permissions.Contains(permission))
                    {
                        return true;
                    }
                }
            
            return false;
        }
    }
}
