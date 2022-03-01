using Common;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace XMS
{
    class Program
    {
        static void Main(string[] args)
        {
            bool uspesno;
            int izbor;

            do
            {
                Console.WriteLine("Unesite nacin logovanja:");
                Console.WriteLine("1. Windows event log");
                Console.WriteLine("2. TXT fajl");

                

                uspesno = Int32.TryParse(Console.ReadLine(), out izbor);

            } while ((izbor != 1 && izbor != 2) || uspesno == false);

            ClientProxy.nacinLogovanja = izbor;
            SecurityManager.ClientProxy.nacinLogovanja = izbor;

            NetTcpBinding binding = new NetTcpBinding();
            

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            ServiceHost host = new ServiceHost(typeof(XMLManagement));

            string address = host.BaseAddresses.First().ToString();

            host.AddServiceEndpoint(typeof(IXMLManagement), binding, address);

            host.Authorization.ServiceAuthorizationManager = new CustomAuthorizationManager();

            host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;
            List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
            policies.Add(new CustomAuthorizationPolicy());
            host.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();

            host.Open();

            

            Console.WriteLine("Servis je pokrenut.");

            Console.ReadLine();
        }
    }
}
