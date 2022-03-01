using Common;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ClientProxy : ChannelFactory<IXMLManagement>, IDisposable
    {
        IXMLManagement factory;

        public ClientProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
            factory = this.CreateChannel();
        }

        public ClientProxy(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {

            factory = this.CreateChannel();
        }



        public void Create(string filename)
        {
            string signCertCN = "client_sign";

            X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My,
                  StoreLocation.LocalMachine, signCertCN);

            byte[] signature = DigitalSignature.Create(filename, HashAlgorithm.SHA1, certificateSign);

            try
            {
                factory.Create(filename, signature);
            }
            catch(FaultException<SecurityException> ex)
            {
                Console.WriteLine(ex.Detail.Message);
            }
            catch (FaultException<MyException> ex)
            {
                Console.WriteLine(ex.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                Console.ReadLine();
            }
        }

        public void Delete(string filename)
        {
            try
            {
                string signCertCN = "client_sign";

                X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My,
                      StoreLocation.LocalMachine, signCertCN);

                byte[] signature = DigitalSignature.Create(filename, HashAlgorithm.SHA1, certificateSign);

                factory.Delete(filename, signature);
            }
            catch (FaultException<SecurityException> ex)
            {
                Console.WriteLine(ex.Detail.Message);
            }
            catch (FaultException<MyException> ex)
            {
                Console.WriteLine(ex.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                Console.ReadLine();
            }
        }

        public string Read(string filename)
        {
            string signCertCN = "client_sign";

            X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My,
                  StoreLocation.LocalMachine, signCertCN);

            byte[] signature = DigitalSignature.Create(filename, HashAlgorithm.SHA1, certificateSign);

            try
            {
                return factory.Read(filename, signature);
            }
            catch (FaultException<SecurityException> ex)
            {
                Console.WriteLine(ex.Detail.Message);
            }
            catch (FaultException<MyException> ex)
            {
                Console.WriteLine(ex.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                Console.ReadLine();

                
            }
            return "";
        }

        public void AddProfessor(string filename, string poruka)
        {
            string signCertCN = "client_sign";

            X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My,
                  StoreLocation.LocalMachine, signCertCN);

            byte[] signatureFilename = DigitalSignature.Create(filename, HashAlgorithm.SHA1, certificateSign);
            

            byte[] signaturePoruka = DigitalSignature.Create(poruka, HashAlgorithm.SHA1, certificateSign);

            try
            {
                factory.AddProfessor(filename, poruka, signatureFilename, signaturePoruka);
            }
            catch (FaultException<SecurityException> ex)
            {
                Console.WriteLine(ex.Detail.Message);
            }
            catch (FaultException<MyException> ex)
            {
                Console.WriteLine(ex.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                Console.ReadLine();
            }
        }

        public void AddStudent(string filename, string poruka)
        {
            try
            {
                string signCertCN = "client_sign";

                X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My,
                      StoreLocation.LocalMachine, signCertCN);

                byte[] signatureFilename = DigitalSignature.Create(filename, HashAlgorithm.SHA1, certificateSign);


                byte[] signaturePoruka = DigitalSignature.Create(poruka, HashAlgorithm.SHA1, certificateSign);

                factory.AddStudent(filename, poruka, signatureFilename, signaturePoruka);
            }
            catch (FaultException<SecurityException> ex)
            {
                Console.WriteLine(ex.Detail.Message);
            }
            catch (FaultException<MyException> ex)
            {
                Console.WriteLine(ex.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                Console.ReadLine();
            }
        }

        public void AddSubject(string filename, string poruka)
        {
            string signCertCN = "client_sign";

            X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My,
                  StoreLocation.LocalMachine, signCertCN);

            byte[] signatureFilename = DigitalSignature.Create(filename, HashAlgorithm.SHA1, certificateSign);


            byte[] signaturePoruka = DigitalSignature.Create(poruka, HashAlgorithm.SHA1, certificateSign);

            try
            {
                factory.AddSubject(filename, poruka, signatureFilename, signaturePoruka);
            }
            catch (FaultException<SecurityException> ex)
            {
                Console.WriteLine(ex.Detail.Message);
            }
            catch (FaultException<MyException> ex)
            {
                Console.WriteLine(ex.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                Console.ReadLine();
            }
        }

        public void DeleteProfessor(string filename, string poruka)
        {
            string signCertCN = "client_sign";

            X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My,
                  StoreLocation.LocalMachine, signCertCN);

            byte[] signatureFilename = DigitalSignature.Create(filename, HashAlgorithm.SHA1, certificateSign);


            byte[] signaturePoruka = DigitalSignature.Create(poruka, HashAlgorithm.SHA1, certificateSign);

            try
            {
                factory.DeleteProfessor(filename, poruka, signatureFilename, signaturePoruka);
            }
            catch (FaultException<SecurityException> ex)
            {
                Console.WriteLine(ex.Detail.Message);
            }
            catch (FaultException<MyException> ex)
            {
                Console.WriteLine(ex.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                Console.ReadLine();
            }
        }
        public void DeleteStudent(string filename, string poruka)
        {
            string signCertCN = "client_sign";

            X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My,
                  StoreLocation.LocalMachine, signCertCN);

            byte[] signatureFilename = DigitalSignature.Create(filename, HashAlgorithm.SHA1, certificateSign);


            byte[] signaturePoruka = DigitalSignature.Create(poruka, HashAlgorithm.SHA1, certificateSign);

            try
            {
                factory.DeleteStudent(filename, poruka, signatureFilename, signaturePoruka);
            }
            catch (FaultException<SecurityException> ex)
            {
                Console.WriteLine(ex.Detail.Message);
            }
            catch (FaultException<MyException> ex)
            {
                Console.WriteLine(ex.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                Console.ReadLine();
            }
        }

        public void DeleteSubject(string filename, string poruka)
        {
            string signCertCN = "client_sign";

            X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My,
                  StoreLocation.LocalMachine, signCertCN);

            byte[] signatureFilename = DigitalSignature.Create(filename, HashAlgorithm.SHA1, certificateSign);


            byte[] signaturePoruka = DigitalSignature.Create(poruka, HashAlgorithm.SHA1, certificateSign);

            try
            {
                factory.DeleteSubject(filename, poruka, signatureFilename, signaturePoruka);
            }
            catch (FaultException<SecurityException> ex)
            {
                Console.WriteLine(ex.Detail.Message);
            }
            catch (FaultException<MyException> ex)
            {
                Console.WriteLine(ex.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                Console.ReadLine();
            }
        }



    }
}
