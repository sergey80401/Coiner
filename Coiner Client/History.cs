using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coiner
{
    class History
    {
        public string Message;
        public int Cash;

        public History(string Message, int Cash)
        {
            this.Message = Message;
            this.Cash = Cash;
        }
    }
}
