using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Coiner_Server
{
    class Program
    {
        public static void UpdateDataBase(object serv)
        {
            Server server = (Server)serv;
            DateTime dateTime = DateTime.Now;
            while (true)
            {
                if ((DateTime.Now - dateTime).TotalDays >= 1)
                {
                    server.CheckIncomes();
                    server.CheckExpenses();
                    dateTime = DateTime.Now;
                }
            }
        }
        

        static void Main(string[] args)
        {
            Server server = new Server(
                "127.0.0.1",
                2222,
                "server=localhost;user=root;database=coiner_db;password=serg.hook80401"
                );

            Thread thr = new Thread(new ParameterizedThreadStart(UpdateDataBase));
            thr.Start(server);

            while (true)
            {
                try
                {
                    NetworkStream stream = server.AccpeptConnection();
                    string someQuery = server.Read(stream);
                    string someResponse = "Invalid query";

                    switch (someQuery.Substring(0, someQuery.IndexOf(';')))
                    {
                        case "!singin":
                            someResponse = server.ClientSingIn(someQuery.Substring(someQuery.IndexOf(';') + 1));
                            break;

                        case "!singup":
                            someResponse = server.ClientSingUp(someQuery.Substring(someQuery.IndexOf(';') + 1));
                            break;

                        case "!save":
                            someResponse = server.UpdateCash(someQuery.Substring(someQuery.IndexOf(';') + 1), true);
                            break;

                        case "!spend":
                            someResponse = server.UpdateCash(someQuery.Substring(someQuery.IndexOf(';') + 1), false);
                            break;

                        case "!getincomes":
                            someResponse = server.GiveIncomes(someQuery.Substring(someQuery.IndexOf(';') + 1));
                            break;

                        case "!getexpenses":
                            someResponse = server.GiveExpenses(someQuery.Substring(someQuery.IndexOf(';') + 1));
                            break;

                        case "!addincome":
                            someResponse = server.AddIncome(someQuery.Substring(someQuery.IndexOf(';') + 1));
                            break;

                        case "!addexpense":
                            someResponse = server.AddExpense(someQuery.Substring(someQuery.IndexOf(';') + 1));
                            break;

                        case "!removeincome":
                            someResponse = server.RemoveIncome(someQuery.Substring(someQuery.IndexOf(';') + 1));
                            break;

                        case "!removeexpense":
                            someResponse = server.RemoveExpense(someQuery.Substring(someQuery.IndexOf(';') + 1));
                            break;

                        case "!gethistory":
                            someResponse = server.GiveHistory(someQuery.Substring(someQuery.IndexOf(';') + 1));
                            break;

                        case "!clearhistory":
                            someResponse = server.ClearHistory(someQuery.Substring(someQuery.IndexOf(';') + 1));
                            break;
                    }
                    server.SendMessage(stream, someResponse);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

        }
    }
}
