using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class CustomAuthorizationManager : ServiceAuthorizationManager
    {
        static string path = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "filename.txt");

        static string srvCertCN = "MAS";

        static X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);

        static EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:7777/MASLogging"),
                                  new X509CertificateEndpointIdentity(srvCert));


        

        protected override bool CheckAccessCore(OperationContext operationContext)
        {

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            ClientProxy pr = new ClientProxy(binding, address);


            CustomPrincipal principal = operationContext.ServiceSecurityContext.
                 AuthorizationContext.Properties["Principal"] as CustomPrincipal;

            if (principal.IsInRole("Read"))
            {
                return true;

            } else
            {
                SecurityException ex = new SecurityException();
                ex.Message = $"{DateTime.Now.ToString()}: Nemate privilegiju citanja fajla!";


                StreamReader sr = new StreamReader(path);
                pr.Log(String.Format("{0},{1},{2},{3}", DateTime.Now.ToString(), Formatter.ParseName(principal.Identity.Name), sr.ReadLine(), "Read"));
                sr.Close();
                throw new FaultException<SecurityException>(ex);
                return false;
            }
        }
    }
}
