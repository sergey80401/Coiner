using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coiner_Server
{
    class Income
    {
        public int Cash;
        public string Message;
        public string Period;

        public Income(int Cash, string Message, string Period)
        {
            this.Cash = Cash;
            this.Message = Message;
            this.Period = Period;
        }
    }
}
