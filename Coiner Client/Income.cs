using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coiner
{
    class Income
    {
        public int Cash;
        public string Name;
        public int Period;

        public Income(int Cash, string Name, int Period)
        {
            this.Cash = Cash;
            this.Name = Name;
            this.Period = Period;
        }
    }
}
