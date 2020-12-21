using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coiner_Server
{
    class User
    {
        public int Id;
        public string UserName;
        public string Password;
        public int Cash;

        public User(int Id, string UserName, string Password, int Cash)
        {
            this.Id = Id;
            this.UserName = UserName;
            this.Password = Password;
            this.Cash = Cash;
        }

        public void Save(int Money)
        {
            this.Cash += Money;
        }

        public void Spend(int Money)
        {
            if (this.Cash >= Money)
                this.Cash -= Money;
        }
    }
}
