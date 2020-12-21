using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Coiner
{
    class Client
    {
        TcpClient ClientSocket;
        IPEndPoint Connection;
        public User ActiveUser;
        public Client(string IP, int Port)
        {
            this.Connection = new IPEndPoint(IPAddress.Parse(IP), Port);
        }

        public void SendMessage(string message)
        {
            ClientSocket = new TcpClient();
            ClientSocket.Connect(Connection);

            NetworkStream stream = ClientSocket.GetStream();

            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);

            stream.Close();
            ClientSocket.Close();
        }

        public string Ask(string request)
        {
            ClientSocket = new TcpClient();
            ClientSocket.Connect(Connection);

            NetworkStream stream = ClientSocket.GetStream();

            byte[] data = Encoding.UTF8.GetBytes(request);
            
            stream.Write(data, 0, data.Length);

            data = new byte[256];
            int bytes = stream.Read(data, 0, data.Length);

            stream.Close();
            ClientSocket.Close();

            return Encoding.UTF8.GetString(data, 0, bytes);
        }

        public void ClearHistory()
        {
            string request = Ask($"!clearhistory;{ActiveUser.Id}");
        }

        public User SingIn(string username, string password)
        {
            // QUERY "!singin*username*password"
            // RESPONSE new User

            string someResponse = Ask($"!singin;{username};{password}");

            if(someResponse != "Invalid query")
            {
                int id = Convert.ToInt32(someResponse.Substring(0, someResponse.IndexOf(';')));
                int cash = Convert.ToInt32(someResponse.Substring(someResponse.IndexOf(';') + 1));

                return new User(id, username, password, cash);
            }

            return null;
        }

        public User SingUp(string username, string password)
        {
            // QUERY "!singup*username*password"
            // RESPONSE new User

            string id = Ask($"!singup;{username};{password}");

            if(id != "Invalid query")
                return new User(Convert.ToInt32(id), username, password, 0);

            return null;
        }

        public string Save(int Cash, string Message)
        {
            ActiveUser.Save(Cash);
            string query = Ask($"!save;{ActiveUser.Id};{ActiveUser.Cash};{Message}");

            return ActiveUser.Cash.ToString();
        }

        public string Spend(int Cash, string Message)
        {
            ActiveUser.Spend(Cash);
            string query = Ask($"!spend;{ActiveUser.Id};{ActiveUser.Cash};{Message}");

            return ActiveUser.Cash.ToString();
        }

        public List<Income> GetIncomes()
        {
            List<Income> incomes = new List<Income>();

            string query = Ask($"!getincomes;{ActiveUser.Id}");

            int quantity = Convert.ToInt32(query.Substring(0, query.IndexOf(';')));
            query = query.Substring(query.IndexOf(';') + 1);

            for (int i = 0; i < quantity; i++)
            {
                int Cash = Convert.ToInt32(query.Substring(0, query.IndexOf(';')));
                query = query.Substring(query.IndexOf(';') + 1);
                string Message = query.Substring(0, query.IndexOf(';'));
                query = query.Substring(query.IndexOf(';') + 1);
                int Period = Convert.ToInt32(query.Substring(0, query.IndexOf(';')));
                query = query.Substring(query.IndexOf(';') + 1);

                incomes.Add(new Income(Cash, Message, Period));
            }

            return incomes;
        }

        public List<History> GetHistories()
        {
            List<History> histories = new List<History>();

            string query = Ask($"!gethistory;{ActiveUser.Id}");

            int quantity = Convert.ToInt32(query.Substring(0, query.IndexOf(';')));
            query = query.Substring(query.IndexOf(';') + 1);

            for (int i = 0; i < quantity; i++)
            {
                int Cash = Convert.ToInt32(query.Substring(0, query.IndexOf(';')));
                query = query.Substring(query.IndexOf(';') + 1);

                string Message = query.Substring(0, query.IndexOf(';'));
                query = query.Substring(query.IndexOf(';') + 1);

                histories.Add(new History(Message, Cash));
            }

            return histories;
        }

        public void AddIncome(string Message, int Cash, int Period, DateTime Date)
        {
            string date = Date.Year.ToString() + "-" + Date.Month.ToString() + "-" + Date.Day.ToString();
            string incomesQuery = Ask($"!addincome;{ActiveUser.Id};{Cash};{Message};{date};{Period}");
        }

        public void AddExpense(string Message, int Cash, int Period, DateTime Date)
        {
            string date = Date.Year.ToString() + "-" + Date.Month.ToString() + "-" + Date.Day.ToString();
            string expensesQuery = Ask($"!addexpense;{ActiveUser.Id};{Cash};{Message};{date};{Period}");
        }

        public void RemoveIncome(string Message, int Cash, int Period)
        {
            string query = Ask($"!removeincome;{ActiveUser.Id};{Message};{Cash};{Period}");
        }
        public void RemoveExpense(string Message, int Cash, int Period)
        {
            string query = Ask($"!removeexpense;{ActiveUser.Id};{Message};{Cash};{Period}");
        }

        public List<Expense> GetExpenses()
        {
            List<Expense> expenses = new List<Expense>();

            string query = Ask($"!getexpenses;{ActiveUser.Id}");

            int quantity = Convert.ToInt32(query.Substring(0, query.IndexOf(';')));
            query = query.Substring(query.IndexOf(';') + 1);

            for (int i = 0; i < quantity; i++)
            {
                int Cash = Convert.ToInt32(query.Substring(0, query.IndexOf(';')));
                query = query.Substring(query.IndexOf(';') + 1);
                string Message = query.Substring(0, query.IndexOf(';'));
                query = query.Substring(query.IndexOf(';') + 1);
                int Period = Convert.ToInt32(query.Substring(0, query.IndexOf(';')));
                query = query.Substring(query.IndexOf(';') + 1);

                expenses.Add(new Expense(Cash, Message, Period));
            }

            return expenses;
        }
    }
}
