using Common;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;



namespace XMS
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class XMLManagement : IXMLManagement
    {
        static string srvCertCN = "MAS";

        static X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, srvCertCN);

        static EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:7777/MASLogging"),
                                  new X509CertificateEndpointIdentity(srvCert));


        
        
        string s = "{0},{1},{2},{3}";     //vreme korisnik fajl pristup

        public void Create(string filename, byte[] sign)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            ClientProxy pr = new ClientProxy(binding, address);


            string signCertCN = "client_sign";

            X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                  StoreLocation.LocalMachine, signCertCN);

            if (Thread.CurrentPrincipal.IsInRole("Create"))
            {
                if (DigitalSignature.Verify(filename, SecurityManager.HashAlgorithm.SHA1, sign, certificateSign))
                {

                    if (!File.Exists(filename))
                    {
                        IntegrityCheck.LoadCheckSums();

                        XDocument xmlFile = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
                        
                        XElement items = new XElement("Items");

                        XElement professors = new XElement("Professors");
                        XElement students = new XElement("Students");
                        XElement subjects = new XElement("Subjects");

                        items.Add(professors);
                        items.Add(students);
                        items.Add(subjects);

                        xmlFile.Add(items);

                        xmlFile.Save(filename);

                        IntegrityCheck.Checksums.Add(filename, IntegrityCheck.GenerateCheckSum(xmlFile.ToString()));
                        IntegrityCheck.StoreCheckSums();
                    }
                    else
                    {
                        MyException ex = new MyException();
                        ex.Message = "Vec postoji fajl sa tim nazivom!";
                        throw new FaultException<MyException>(ex);
                    }

                } else
                {
                    Console.WriteLine("Sign is invalid");
                }

            } else
            {
                

                string name = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);
                pr.Log(String.Format("{0},{1},{2},{3}", DateTime.Now.ToString(), name, filename, "Create"));
                SecurityException ex = new SecurityException();
                ex.Message = $"{DateTime.Now.ToString()}: Nemate privilegiju kreiranja fajla!";                
                throw new FaultException<SecurityException>(ex);
            }

           
        }

        public void Delete(string filename, byte[] sign)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            ClientProxy pr = new ClientProxy(binding, address);

            string signCertCN = "client_sign";

            X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                  StoreLocation.LocalMachine, signCertCN);

           

            if (Thread.CurrentPrincipal.IsInRole("Delete"))
            {
                if (DigitalSignature.Verify(filename, SecurityManager.HashAlgorithm.SHA1, sign, certificateSign))
                {
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);

                        IntegrityCheck.LoadCheckSums();
                        IntegrityCheck.Checksums.Remove(filename);
                        IntegrityCheck.StoreCheckSums();
                    }
                    else
                    {
                        MyException ex = new MyException();
                        ex.Message = "Ne postoji fajl sa tim nazivom!";
                        throw new FaultException<MyException>(ex);
                    }

                } else
                {
                    Console.WriteLine("Sign is invalid");
                }

            } else
            {
                string name = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);
                pr.Log(String.Format(s, DateTime.Now.ToString(), name, filename, "Delete"));
                SecurityException ex = new SecurityException();
                ex.Message = $"{DateTime.Now.ToString()}: Nemate privilegiju brisanja fajla!";
                throw new FaultException<SecurityException>(ex);
            }
        }




        public string Read(string filename, byte[] sign)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            ClientProxy pr = new ClientProxy(binding, address);

            string signCertCN = "client_sign";

            X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                  StoreLocation.LocalMachine, signCertCN);

            string s = "";

            if (DigitalSignature.Verify(filename, SecurityManager.HashAlgorithm.SHA1, sign, certificateSign))
            {

                if (!File.Exists(filename))
                {
                    MyException ex = new MyException();
                    ex.Message = $"Ne postoji fajl sa tim nazivom!";
                    throw new FaultException<MyException>(ex);
                }


                XmlDocument doc = new XmlDocument();
                doc.Load(filename);

                IntegrityCheck.LoadCheckSums();

                if (!IntegrityCheck.Check(filename))
                {
                    MyException ex = new MyException();
                    ex.Message = $"Narusen je integritet fajla {filename}!";
                    throw new FaultException<MyException>(ex);
                    
                }


                foreach (XmlNode n in doc.DocumentElement.SelectNodes("/Items/Professors/Professor"))
                {
                    s += n.SelectSingleNode("jmbg").InnerText + " ";
                    s += n.SelectSingleNode("name").InnerText + " ";
                    s += n.SelectSingleNode("surname").InnerText + " ";

                    s += ",";

                }

                s += "|";

               


                foreach (XmlNode n in doc.DocumentElement.SelectNodes("/Items/Students/Student"))
                {
                    s += n.SelectSingleNode("jmbg").InnerText + " ";
                    s += n.SelectSingleNode("name").InnerText + " ";
                    s += n.SelectSingleNode("surname").InnerText + " ";
                    s += n.SelectSingleNode("index").InnerText + " ";

                    s += ",";
                }

                s += "|";

                foreach (XmlNode n in doc.DocumentElement.SelectNodes("/Items/Subjects/Subject"))
                {
                    s += n.SelectSingleNode("id").InnerText + " ";
                    s += n.SelectSingleNode("name").InnerText + " ";
                    s += n.SelectSingleNode("espb").InnerText + " ";

                    s += ",";
                }
            }
            else
            {
                Console.WriteLine("Sign is invalid");
            }

            return s;

        }


        public void AddProfessor(string filename, string poruka, byte[] sign, byte[] sign2)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            ClientProxy pr = new ClientProxy(binding, address);

            string signCertCN = "client_sign";

            X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                  StoreLocation.LocalMachine, signCertCN);

            if (Thread.CurrentPrincipal.IsInRole("Modify"))
            {
                

                if ((DigitalSignature.Verify(filename, SecurityManager.HashAlgorithm.SHA1, sign, certificateSign)==true) && (DigitalSignature.Verify(poruka, SecurityManager.HashAlgorithm.SHA1, sign2, certificateSign)==true))
                {
                    IntegrityCheck.LoadCheckSums();

                    if (!IntegrityCheck.Check(filename))
                    {
                        MyException ex = new MyException();
                        ex.Message = $"Narusen je integritet fajla {filename}!";
                        throw new FaultException<MyException>(ex);

                    }

                    if (!File.Exists(filename))
                    {
                        MyException ex = new MyException();
                        ex.Message = $"Ne postoji fajl sa tim nazivom!";
                        throw new FaultException<MyException>(ex);
                    }

                    string[] atributi = poruka.Split(',');

                   

                    var prof = new XElement("Professor");

                    var jmbg = new XElement("jmbg", atributi[0]);
                    var name = new XElement("name", atributi[1]);
                    var surname = new XElement("surname", atributi[2]);

                    prof.Add(jmbg);
                    prof.Add(name);
                    prof.Add(surname);

                    XDocument doc = XDocument.Load(filename);

                    bool postoji = false;


                    foreach (XElement e in doc.Element("Items").Element("Professors").Elements())
                    {
                        if (e.Element("jmbg").Value == atributi[0])
                        {
                            postoji = true;
                        }
                    }

                    if (postoji)
                    {
                        MyException ex = new MyException();
                        ex.Message = "Vec postoji profesor sa tim jmbg-om!";
                        throw new FaultException<MyException>(ex);


                    }
                    else
                    {
                        doc.Element("Items").Element("Professors").Add(prof);
                    }

                    
                    doc.Save(filename);

                    IntegrityCheck.Checksums[filename] = IntegrityCheck.GenerateCheckSum(doc.ToString());

                    IntegrityCheck.StoreCheckSums();
                }
                else
                {
                    Console.WriteLine("Sign is invalid");
                }

            } else
            {
                string name = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);
                pr.Log(String.Format(s, DateTime.Now.ToString(), name, filename, "Modify"));
                SecurityException ex = new SecurityException();
                ex.Message = $"{DateTime.Now.ToString()}: Nemate privilegiju izmene fajla!";
                throw new FaultException<SecurityException>(ex);
            }
            
        }




        public void AddStudent(string filename, string poruka, byte[] sign, byte[] sign2)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            ClientProxy pr = new ClientProxy(binding, address);

            string signCertCN = "client_sign";

            X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                  StoreLocation.LocalMachine, signCertCN);


            if (Thread.CurrentPrincipal.IsInRole("Modify"))
            {
                if (DigitalSignature.Verify(filename, SecurityManager.HashAlgorithm.SHA1, sign, certificateSign) && DigitalSignature.Verify(poruka, SecurityManager.HashAlgorithm.SHA1, sign2, certificateSign))
                {
                    IntegrityCheck.LoadCheckSums();

                    if (!IntegrityCheck.Check(filename))
                    {
                        MyException ex = new MyException();
                        ex.Message = $"Narusen je integritet fajla {filename}!";
                        throw new FaultException<MyException>(ex);

                    }

                    if (!File.Exists(filename))
                    {
                        MyException ex = new MyException();
                        ex.Message = $"Ne postoji fajl sa tim nazivom!";
                        throw new FaultException<MyException>(ex);
                    }

                    string[] atributi = poruka.Split(',');

                    var st = new XElement("Student");

                    var jmbg = new XElement("jmbg", atributi[0]);
                    var name = new XElement("name", atributi[1]);
                    var surname = new XElement("surname", atributi[2]);
                    var index = new XElement("index", atributi[3]);

                    st.Add(jmbg);
                    st.Add(name);
                    st.Add(surname);
                    st.Add(index);

                    XDocument doc = XDocument.Load(filename);

                    bool postoji = false;


                    foreach (XElement e in doc.Element("Items").Element("Students").Elements())
                    {
                        if (e.Element("jmbg").Value == atributi[0])
                        {
                            postoji = true;
                        }
                    }

                    if (postoji)
                    {
                        MyException ex = new MyException();
                        ex.Message = "Vec postoji student sa tim jmbg-om!";
                        throw new FaultException<MyException>(ex);

                    }
                    else
                    {
                        doc.Element("Items").Element("Students").Add(st);
                    }

                    doc.Save(filename);

                    IntegrityCheck.Checksums[filename] = IntegrityCheck.GenerateCheckSum(doc.ToString());

                    IntegrityCheck.StoreCheckSums();

                } else
                {
                    Console.WriteLine("Sign is invalid");
                }
            } else
            {
                string name = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);
                pr.Log(String.Format(s, DateTime.Now.ToString(), name, filename, "Modfiy"));
                SecurityException ex = new SecurityException();
                ex.Message = $"{DateTime.Now.ToString()}: Nemate privilegiju izmene fajla!";
                throw new FaultException<SecurityException>(ex);
            }

            
        }



        public void AddSubject(string filename, string poruka, byte[] sign, byte[] sign2)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            ClientProxy pr = new ClientProxy(binding, address);

            string signCertCN = "client_sign";

            X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                  StoreLocation.LocalMachine, signCertCN);

            if (Thread.CurrentPrincipal.IsInRole("Modify"))
            {
                if (DigitalSignature.Verify(filename, SecurityManager.HashAlgorithm.SHA1, sign, certificateSign) && DigitalSignature.Verify(poruka, SecurityManager.HashAlgorithm.SHA1, sign2, certificateSign))
                {
                    IntegrityCheck.LoadCheckSums();

                    if (!IntegrityCheck.Check(filename))
                    {
                        MyException ex = new MyException();
                        ex.Message = $"Narusen je integritet fajla {filename}!";
                        throw new FaultException<MyException>(ex);

                    }

                    if (!File.Exists(filename))
                    {
                        MyException ex = new MyException();
                        ex.Message = $"Ne postoji fajl sa tim nazivom!";
                        throw new FaultException<MyException>(ex);
                    }

                    string[] atributi = poruka.Split(',');

                    var sub = new XElement("Subject");

                    var id = new XElement("id", atributi[0]);
                    var name = new XElement("name", atributi[1]);
                    var espb = new XElement("espb", atributi[2]);

                    sub.Add(id);
                    sub.Add(name);
                    sub.Add(espb);


                    XDocument doc = XDocument.Load(filename);

                    bool postoji = false;

                    foreach (XElement e in doc.Element("Items").Element("Subjects").Elements())
                    {
                        if (e.Element("id").Value == atributi[0])
                        {
                            postoji = true;
                        }
                    }

                    if (postoji)
                    {

                        MyException ex = new MyException();
                        ex.Message = "Vec postoji predmet sa tim id-em!";
                        throw new FaultException<MyException>(ex);

                    }
                    else
                    {
                        doc.Element("Items").Element("Subjects").Add(sub);
                    }

                    doc.Save(filename);

                    IntegrityCheck.Checksums[filename] = IntegrityCheck.GenerateCheckSum(doc.ToString());

                    IntegrityCheck.StoreCheckSums();
                }
                else
                {
                    Console.WriteLine("Sign is invalid");
                }
            }
            else
            {
                string name = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);
                pr.Log(String.Format(s, DateTime.Now.ToString(), name, filename, "Modify"));
                SecurityException ex = new SecurityException();
                ex.Message = $"{DateTime.Now.ToString()}: Nemate privilegiju izmene fajla!";
                throw new FaultException<SecurityException>(ex);
            }

        }  
        






        public void DeleteProfessor(string filename, string jmbg, byte[] sign, byte[] sign2)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            ClientProxy pr = new ClientProxy(binding, address);

            string signCertCN = "client_sign";

            X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                  StoreLocation.LocalMachine, signCertCN);

            if (Thread.CurrentPrincipal.IsInRole("Modify"))
            {
                if (DigitalSignature.Verify(filename, SecurityManager.HashAlgorithm.SHA1, sign, certificateSign) && DigitalSignature.Verify(jmbg, SecurityManager.HashAlgorithm.SHA1, sign2, certificateSign))
                {
                    IntegrityCheck.LoadCheckSums();

                    if (!IntegrityCheck.Check(filename))
                    {
                        MyException ex = new MyException();
                        ex.Message = $"Narusen je integritet fajla {filename}!";
                        throw new FaultException<MyException>(ex);

                    }

                    if (!File.Exists(filename))
                    {
                        MyException ex = new MyException();
                        ex.Message = $"Ne postoji fajl sa tim nazivom!";
                        throw new FaultException<MyException>(ex);
                    }

                    XDocument doc = XDocument.Load(filename);

                    bool postoji = false;

                    foreach (XElement n in doc.Element("Items").Element("Professors").Elements())
                    {


                        if (n.Element("jmbg").Value == jmbg)
                        {
                            postoji = true;
                            n.Remove();

                        }
                    }

                    doc.Save(filename);

                    IntegrityCheck.Checksums[filename] = IntegrityCheck.GenerateCheckSum(doc.ToString());
                    IntegrityCheck.StoreCheckSums();


                    if (postoji == false)
                    {

                        MyException ex = new MyException();
                        ex.Message = "Ne postoji profesor sa unetim jmbg-om!";
                        throw new FaultException<MyException>(ex);
                    }
                }
                else
                {
                    Console.WriteLine("Sign is invalid");
                }
            } else
            {
                string name = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);
                pr.Log(String.Format(s, DateTime.Now.ToString(), name, filename, "Modify"));
                SecurityException ex = new SecurityException();
                ex.Message = $"{DateTime.Now.ToString()}: Nemate privilegiju izmene fajla!";
                throw new FaultException<SecurityException>(ex);
            }

        }




        public void DeleteStudent(string filename, string jmbg, byte[] sign, byte[] sign2)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            ClientProxy pr = new ClientProxy(binding, address);

            string signCertCN = "client_sign";

            X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                  StoreLocation.LocalMachine, signCertCN);

            if (Thread.CurrentPrincipal.IsInRole("Modify"))
            {
                if (DigitalSignature.Verify(filename, SecurityManager.HashAlgorithm.SHA1, sign, certificateSign) && DigitalSignature.Verify(jmbg, SecurityManager.HashAlgorithm.SHA1, sign2, certificateSign))
                {
                    IntegrityCheck.LoadCheckSums();

                    if (!IntegrityCheck.Check(filename))
                    {
                        MyException ex = new MyException();
                        ex.Message = $"Narusen je integritet fajla {filename}!";
                        throw new FaultException<MyException>(ex);

                    }

                    if (!File.Exists(filename))
                    {
                        MyException ex = new MyException();
                        ex.Message = $"Ne postoji fajl sa tim nazivom!";
                        throw new FaultException<MyException>(ex);
                    }

                    XDocument doc = XDocument.Load(filename);

                    bool postoji = false;

                    foreach (XElement n in doc.Element("Items").Element("Students").Elements())
                    {


                        if (n.Element("jmbg").Value == jmbg)
                        {
                            postoji = true;
                            n.Remove();

                        }
                    }

                    doc.Save(filename);

                    IntegrityCheck.Checksums[filename] = IntegrityCheck.GenerateCheckSum(doc.ToString());
                    IntegrityCheck.StoreCheckSums();

                    if (postoji == false)
                    {

                        MyException ex = new MyException();
                        ex.Message = "Ne postoji student sa unetim jmbg-om!";
                        throw new FaultException<MyException>(ex);
                    }
                }
                else
                {
                    Console.WriteLine("Sign is invalid");
                }
            } else
            {
                string name = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);
                pr.Log(String.Format(s, DateTime.Now.ToString(), name, filename, "Modify"));
                SecurityException ex = new SecurityException();
                ex.Message = $"{DateTime.Now.ToString()}: Nemate privilegiju izmene fajla!";
                throw new FaultException<SecurityException>(ex);
            }

        }




        public void DeleteSubject(string filename, string id, byte[] sign, byte[] sign2)
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            ClientProxy pr = new ClientProxy(binding, address);

            string signCertCN = "client_sign";

            X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                  StoreLocation.LocalMachine, signCertCN);

            if (Thread.CurrentPrincipal.IsInRole("Modify"))
            {
                if (DigitalSignature.Verify(filename, SecurityManager.HashAlgorithm.SHA1, sign, certificateSign) && DigitalSignature.Verify(id, SecurityManager.HashAlgorithm.SHA1, sign2, certificateSign))
                {
                    IntegrityCheck.LoadCheckSums();

                    if (!IntegrityCheck.Check(filename))
                    {
                        MyException ex = new MyException();
                        ex.Message = $"Narusen je integritet fajla {filename}!";
                        throw new FaultException<MyException>(ex);

                    }

                    if (!File.Exists(filename))
                    {
                        MyException ex = new MyException();
                        ex.Message = $"Ne postoji fajl sa tim nazivom!";
                        throw new FaultException<MyException>(ex);
                    }

                    XDocument doc = XDocument.Load(filename);

                    bool postoji = false;

                    foreach (XElement n in doc.Element("Items").Element("Subjects").Elements())
                    {


                        if (n.Element("id").Value == id)
                        {
                            postoji = true;
                            n.Remove();

                        }
                    }

                    doc.Save(filename);

                    IntegrityCheck.Checksums[filename] = IntegrityCheck.GenerateCheckSum(doc.ToString());
                    IntegrityCheck.StoreCheckSums();

                    if (postoji == false)
                    {
                        MyException ex = new MyException();
                        ex.Message = "Ne postoji predmet sa unetim id-em!";
                        throw new FaultException<MyException>(ex);
                    }
                }
                else
                {
                    Console.WriteLine("Sign is invalid");
                }
            }
            else
            {
                string name = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);
                pr.Log(String.Format(s, DateTime.Now.ToString(), name, filename, "Modify"));
                SecurityException ex = new SecurityException();
                ex.Message = $"{DateTime.Now.ToString()}: Nemate privilegiju izmene fajla!";
                throw new FaultException<SecurityException>(ex);
            }
        }

        
    }
}
