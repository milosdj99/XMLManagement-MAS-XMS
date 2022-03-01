using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using System.IO;
using System.Collections;
using System.Reflection;

namespace SecurityManager
{
    public class RolesConfig
    {
        static string path = @"~\..\..\..\..\SecurityManager\AccessControlListFile.resx";
        public static bool GetPermissions(string name, out string[] permissions)
        {
            permissions = new string[10];
            string permissionString = string.Empty;

            permissionString = (string)AccessControlListFile.ResourceManager.GetObject(name);
            if (permissionString != null)
            {
                permissions = permissionString.Split(',');
                return true;
            }
            return false;

        }

        

    }
}
