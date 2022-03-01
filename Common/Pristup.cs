using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Pristup
    {
        string filename;
        int brojPristupa;
        List<DateTime> vremenaPristupa;

        public Pristup(string filename, int brojPristupa)
        {
            this.Filename = filename;
            this.BrojPristupa = brojPristupa;
            VremenaPristupa = new List<DateTime>();
            
        }

        public string Filename { get => filename; set => filename = value; }
        public int BrojPristupa { get => brojPristupa; set => brojPristupa = value; }
        public List<DateTime> VremenaPristupa { get => vremenaPristupa; set => vremenaPristupa = value; }
    }
}
