using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolZhelnov
{
    public partial class Zapis
    {
        public override string ToString()
        {
            return datetimeZapis.ToString() + '\n' + '\t' + kodDoca;
        }
    }
    public partial class Doc
    {
        public override string ToString()
        {
            return FIODoc + "  -  " + Kabinet + " каб.";
        }
    }
    public partial class Pacient
    {
        public override string ToString()
        {
            return FIOPacient;
        }
    }
}
