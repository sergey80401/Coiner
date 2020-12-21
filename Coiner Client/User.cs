using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coiner
{
    class User
    {
        public int Id;
        public string UserName;
        public string Password;
        public int Cash;
        public List<Income> Incomes = new List<Income>();
        public List<Expense> Expenses = new List<Expense>();
        public List<History> Histories = new List<History>();

        public User(int Id, string UserName, string Password, int Cash)
        {
            this.Id = Id;

            if(UserName != null)
                this.UserName = UserName;

            if (UserName != null)
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

        public void AddIncome(Income someIncome)
        {
            this.Incomes.Add(someIncome);
        }
    }
}
