using Common;
using SecurityManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static string path = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "filename.txt");

        static void Main(string[] args)
        {

            string izborr;

            do
            {
                Console.WriteLine("Izaberite XMS:");
                Console.WriteLine("1.net.tcp://localhost:8888/IXMLManagement");
                Console.WriteLine("2.net.tcp://localhost:9999/IXMLManagement");

                izborr = Console.ReadLine();

            } while (izborr != "1" && izborr != "2");

            string address;

            if (izborr == "1")
            {
                address = "net.tcp://localhost:8888/IXMLManagement";
            } else
            {
               address = "net.tcp://localhost:9999/IXMLManagement";
            }

            NetTcpBinding binding = new NetTcpBinding();
            

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            Console.WriteLine("Korisnik koji je pokrenuo klijenta je : " + Formatter.ParseName(WindowsIdentity.GetCurrent().Name));

            EndpointAddress endpointAddress = new EndpointAddress(new Uri(address),
                EndpointIdentity.CreateUpnIdentity("XMS"));

            ClientProxy proxy = new ClientProxy(binding, endpointAddress);

            int izbor;

            string filename;

            string poruka = "";

            while (true)
            {
                do
                {

                    Console.WriteLine("Izaberite opciju:");
                    Console.WriteLine("1.Kreiranje fajla");
                    Console.WriteLine("2.Brisanje fajla");
                    Console.WriteLine("3.Iscitavanje fajla");
                    Console.WriteLine("4.Modifikacija fajla");
                    

                    if (!Int32.TryParse(Console.ReadLine(), out izbor))
                    {
                        Console.WriteLine("Niste uneli broj!");
                        continue;
                    }

                } while (izbor < 1 || izbor > 7);


                switch (izbor)
                {
                        case 1:
                            Console.WriteLine("Unesite ime fajla:");
                            filename = Console.ReadLine();
                            UpisiFilename(filename);
                            
                            proxy.Create(filename);
                            break;

                        case 2:
                            Console.WriteLine("Unesite ime fajla:");
                            filename = Console.ReadLine();
                            UpisiFilename(filename);
                            proxy.Delete(filename);
                            break;

                    case 3:
                            Console.WriteLine("Unesite ime fajla:");
                            filename = Console.ReadLine();
                            UpisiFilename(filename);
                            string s = proxy.Read(filename);
                            Ispis(s);
                            break;

                        case 4:

                            Console.WriteLine("Unesite ime fajla:");
                            filename = Console.ReadLine();
                            UpisiFilename(filename);

                            while (true)
                            {


                                do
                                {


                                    Console.WriteLine("1. Dodavanje profesora");
                                    Console.WriteLine("2. Dodavanje studenta");
                                    Console.WriteLine("3. Dodavanje predmeta");
                                    Console.WriteLine("4. Brisanje profesora");
                                    Console.WriteLine("5. Brisanje studenta");
                                    Console.WriteLine("6. Brisanje predmeta");
                                    Console.WriteLine("7. Povratak na glavni meni");



                                    if (Int32.TryParse(Console.ReadLine(), out izbor) == false)
                                    {
                                        Console.WriteLine("Niste uneli broj!");
                                        continue;
                                    }


                                } while (izbor < 1 || izbor > 7);

                                long provera;

                                switch (izbor)
                                {




                                    case 1:

                                        Console.WriteLine("Unesite jmbg:");
                                        string jmbg = Console.ReadLine();

                                        if (!long.TryParse(jmbg, out provera))
                                        {
                                            Console.WriteLine("Niste uneli broj!");
                                            break;
                                        }

                                        Console.WriteLine("Unesite ime:");
                                        string ime = Console.ReadLine();
                                        Console.WriteLine("Unesite prezime:");
                                        string prezime = Console.ReadLine();

                                        poruka = jmbg + "," + ime + "," + prezime;

                                        proxy.AddProfessor(filename, poruka);

                                        break;

                                    case 2:

                                        Console.WriteLine("Unesite jmbg:");
                                        jmbg = Console.ReadLine();

                                        if (!long.TryParse(jmbg, out provera))
                                        {
                                            Console.WriteLine("Niste uneli broj!");
                                            break;
                                        }
                                        Console.WriteLine("Unesite ime:");
                                        ime = Console.ReadLine();
                                        Console.WriteLine("Unesite prezime:");
                                        prezime = Console.ReadLine();
                                        Console.WriteLine("Unesite broj indeksa:");
                                        string index = Console.ReadLine();



                                        poruka = jmbg + "," + ime + "," + prezime + "," + index;

                                        proxy.AddStudent(filename, poruka);
                                        break;


                                    case 3:

                                        Console.WriteLine("Unesite id predmeta:");
                                        string id = Console.ReadLine();
                                        if (!long.TryParse(id, out provera))
                                        {
                                            Console.WriteLine("Niste uneli broj!");
                                            break;
                                        }

                                        Console.WriteLine("Unesite ime predmeta:");
                                        ime = Console.ReadLine();

                                        Console.WriteLine("Unesite broj esp bodova:");
                                        string espb = Console.ReadLine();

                                        if (!long.TryParse(espb, out provera))
                                        {
                                            Console.WriteLine("Niste uneli broj!");
                                            break;
                                        }

                                        poruka = id + "," + ime + "," + espb;

                                        proxy.AddSubject(filename, poruka);
                                        break;

                                    case 4:

                                        Console.WriteLine("Unesite jmbg profesora:");
                                        jmbg = Console.ReadLine();

                                        if (!long.TryParse(jmbg, out provera))
                                        {
                                            Console.WriteLine("Niste uneli broj!");
                                            break;
                                        }


                                        proxy.DeleteProfessor(filename, jmbg);
                                        break;

                                    case 5:

                                        Console.WriteLine("Unesite jmbg studenta:");
                                        jmbg = Console.ReadLine();

                                        if (!long.TryParse(jmbg, out provera))
                                        {
                                            Console.WriteLine("Niste uneli broj!");
                                            break;
                                        }


                                        proxy.DeleteStudent(filename, jmbg);
                                        break;

                                    case 6:

                                        Console.WriteLine("Unesite id predmeta:");
                                        id = Console.ReadLine();

                                        if (!long.TryParse(id, out provera))
                                        {
                                            Console.WriteLine("Niste uneli broj!");
                                            break;
                                        }


                                        proxy.DeleteSubject(filename, id);
                                        break;

                                    case 7:
                                        break;

                                }

                                if (izbor == 7)
                                {
                                    break;
                                }

                            }
                            break;


                    }


               
            }
        }


        private static void Ispis(string s)
        {
            if (s == "")
            {
                return;
            }

            string[] objekti = s.Split('|');

            string[] profesori = objekti[0].Split(',');

            Console.WriteLine("Profesori:");

            
            foreach(string prof in profesori)
            {
                if (prof != "")
                {
                    string[] atributi = prof.Split(' ');

                    Console.WriteLine($"Jmbg: {atributi[0]}, Ime: {atributi[1]}, Prezime: {atributi[2]}");
                }
            }

            string[] studenti = objekti[1].Split(',');

            Console.WriteLine("Studenti:");

            foreach (string st in studenti)
            {
                if (st != "")
                {
                    string[] atributi = st.Split(' ');

                    Console.WriteLine($"Jmbg: {atributi[0]}, Ime: {atributi[1]}, Prezime: {atributi[2]}, Indeks: {atributi[3]}");
                }
            }

            string[] predmeti = objekti[2].Split(',');

            Console.WriteLine("Predmeti:");

            foreach (string pr in predmeti)
            {
                if (pr != "")
                {
                    string[] atributi = pr.Split(' ');

                    Console.WriteLine($"Id: {atributi[0]}, Ime: {atributi[1]}, Espb: {atributi[2]}");
                }
            }



        }

        private static void UpisiFilename(string filename)
        {
            using (StreamWriter sw = new StreamWriter(path, false))
            {
                sw.WriteLine(filename);
                sw.Close();
            }
        }
    }
}
