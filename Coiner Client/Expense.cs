using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coiner
{
    class Expense
    {
        public int Cash;
        public string Name;
        public int Period;

        public Expense(int Cash, string Name, int Period)
        {
            this.Cash = Cash;
            this.Name = Name;
            this.Period = Period;
        }
    }
}
