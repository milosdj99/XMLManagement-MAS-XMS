using Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MAS
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class MASLogging : IMASLogging
    {


        internal static Dictionary<string, Pristup> pristupi = new Dictionary<string, Pristup>();

        static string path = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName, "Database1.mdf");
        public static string con_string = @"Data Source=(LocalDB)\MSSQLLocalDB; " +
                $@"AttachDbFilename={path};
                Connect Timeout=30;
                Integrated Security=true;
                Trusted_Connection=True";
                


        public SqlConnection Con = new SqlConnection(con_string);


        public void Log(string message)
        {

            StreamReader sr = new StreamReader("s_n.txt");

            string sn = sr.ReadLine();

            sr.Close();

            string[] snArray = sn.Split('.');

            int s = Int32.Parse(snArray[0]);
            int n = Int32.Parse(snArray[1]);


            TimeSpan ts = new TimeSpan(0, 0, 0, s);

            if (Con.State != System.Data.ConnectionState.Open)
            {
                try
                {
                    Con.Open();
                    Console.WriteLine("Uspesno konektovanje sa bazom podataka.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Bezuspesno konektovanje sa bazom podataka.");
                    Console.WriteLine(e.Message);
                }
            }

            

            string[] parametri = message.Split(',');

            string time = parametri[0];
            string user = parametri[1];
            string file = parametri[2];
            string deniedAccessType = parametri[3];
            string criticalLevel = "LOW_LEVEL";

            DateTime timeDT = Convert.ToDateTime(time);

            if (pristupi.ContainsKey(file))
            {
                pristupi[file].BrojPristupa++;
                pristupi[file].VremenaPristupa.Add(timeDT);

            } else
            {
                Pristup pristup = new Pristup(file, 1);
                pristup.VremenaPristupa.Add(timeDT);
                pristupi.Add(file, pristup);
            }


            Pristup p = pristupi[file];

            


            if(p.BrojPristupa >= n)
            {
                if(p.VremenaPristupa[p.BrojPristupa-1].Subtract(p.VremenaPristupa[p.BrojPristupa-1 - (n-1)]) <= ts)
                {
                    criticalLevel = "MEDIUM_LEVEL";

                } 
            }

            if (p.BrojPristupa >= n+1)
            {
                if (p.VremenaPristupa[p.BrojPristupa-1].Subtract(p.VremenaPristupa[p.BrojPristupa-1 - (n-1) - 1]) <= ts)
                {
                    criticalLevel = "CRITICAL_LEVEL";
                }
            }



            string q = "insert into SecurityData " +
                        $"values ('{time}', '{user}', '{file}', '{deniedAccessType}', '{criticalLevel}')";

            SqlCommand com = new SqlCommand(q, Con);
            com.ExecuteNonQuery();

            Con.Close();
        }
    }
}
