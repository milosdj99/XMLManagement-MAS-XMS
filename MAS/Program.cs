using Common;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace MAS
{
    public class Program
    {
        static void Main(string[] args)
        {

            string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            
            NetTcpBinding binding = new NetTcpBinding();

            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            string address = "net.tcp://localhost:7777/MASLogging";



            ServiceHost host = new ServiceHost(typeof(MASLogging));
            host.AddServiceEndpoint(typeof(IMASLogging), binding, address);

            host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
         

            ///If CA doesn't have a CRL associated, WCF blocks every client because it cannot be validated
            host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            ///Set appropriate service's certificate on the host. Use CertManager class to obtain the certificate based on the "srvCertCN"
            host.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);

            host.Open();

            string path = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "stanjeMASa.txt");

            using (StreamWriter sw = new StreamWriter(path, false))
            {

                sw.WriteLine("Podignut");

                sw.Close();
            }

            

            Console.WriteLine("Servis je pokrenut.");

            Console.WriteLine("Pritisnite bilo koje dugme za kraj rada.");

            Console.ReadKey();

            using (StreamWriter sw = new StreamWriter(path, false))
            {

                sw.WriteLine("Spusten");

                sw.Close();
            }


        }
    }
}
