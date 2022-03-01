using Common;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace XMS
{
    public class ClientProxy : ChannelFactory<IMASLogging>, IDisposable
    {
        IMASLogging factory;

        public static int nacinLogovanja;

        public static List<string> zaostali = new List<string>();

        public ClientProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public ClientProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
           
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;         
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            
            this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);





            factory = this.CreateChannel();
        }



        public void Log(string message)
        {
            string[] parametri = message.Split(',');

            string time = parametri[0];
            string user = parametri[1];
            string file = parametri[2];
            string deniedAccessType = parametri[3];

            if (nacinLogovanja == 1)
            {
                Audit.NewDataStored(time, user, file, deniedAccessType);
            }
            else
            {
                StreamWriter sw = new StreamWriter("logs.txt", true);              
                sw.WriteLine($"Time: {time}, User: {user}, File: {file}, DeniedAccessType: {deniedAccessType}");
                sw.Close();
            }


            string path = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "stanjeMASa.txt");
            StreamReader sr = new StreamReader(path);
            string stanje = sr.ReadLine();
            sr.Close();

            if (stanje == "Podignut")
            {
                try
                {

                    factory.Log(message);

                    foreach(string s in zaostali)
                    {
                        factory.Log(s);
                    }

                    zaostali.Clear();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    
                }

            } else
            {
                zaostali.Add(message);
            }
        }



    }
}
