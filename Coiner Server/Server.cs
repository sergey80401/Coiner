using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using MySql.Data.MySqlClient;

namespace Coiner_Server
{
    class Server
    {
        private TcpListener ServerSocket;
        private readonly MySqlConnection DbConnection;
        private static int QuantityOfUsers = 0;
        public static int QuantityOfIncomes = 0;
        public static int QuantityOfExpenses = 0;
        public static int QuantityOfHistoryNotes = 0;

        public Server(string IP, int Port, string DbConnection)
        {
            try
            {
                ServerSocket = new TcpListener(IPAddress.Parse(IP), Port);
                this.DbConnection = new MySqlConnection(DbConnection);
                ServerSocket.Start();
                
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                QuantityOfUsers = Convert.ToInt32(DbReadNote("SELECT COUNT(*) FROM users"));
                QuantityOfIncomes = Convert.ToInt32(DbReadNote("SELECT COUNT(*) FROM incomes"));
                QuantityOfExpenses = Convert.ToInt32(DbReadNote("SELECT COUNT(*) FROM expenses"));
                QuantityOfHistoryNotes = Convert.ToInt32(DbReadNote("SELECT COUNT(*) FROM history"));
            }
        }

        public NetworkStream AccpeptConnection()
        {
            return ServerSocket.AcceptTcpClient().GetStream();
        }

        public void SendMessage(NetworkStream stream, string response)
        {
            byte[] data = Encoding.UTF8.GetBytes(response);
            stream.Write(data, 0, data.Length);
        }

        public void SendMessage<T>(NetworkStream stream, T response)
        {
            SendMessage(stream, response.ToString());
        }

        public string Read(NetworkStream stream)
        {
            byte[] data = new byte[256];
            int bytes = stream.Read(data, 0, data.Length);

            return Encoding.UTF8.GetString(data, 0, bytes);
        }

        public string DbReadNote(string SqlRequest)
        {
            DbConnection.Open();
            MySqlCommand command = new MySqlCommand(SqlRequest, DbConnection);
            string request = command.ExecuteScalar().ToString();
            DbConnection.Close();

            return request;
        }

        public void DbWriteNote(string SqlRequest)
        {
            DbConnection.Open();
            MySqlCommand command = new MySqlCommand(SqlRequest, DbConnection);
            command.ExecuteNonQuery();
            DbConnection.Close();
        }

        public string ClientSingIn(string ClientRequest)
        {
            string username = ClientRequest.Substring(0, ClientRequest.IndexOf(';'));
            string password = ClientRequest.Substring(ClientRequest.IndexOf(';') + 1);

            for (int id = 1; id <= QuantityOfUsers; id++)
            {
                if (username == DbReadNote($"SELECT username FROM users WHERE id = {id}") &&
                    password == DbReadNote($"SELECT password FROM users WHERE id = {id}"))
                {
                    Console.WriteLine($"User username {username} is logged in\t{DateTime.Now}");
                    return $"{id};{DbReadNote($"SELECT cash FROM users WHERE id = {id}")}";
                }
            }

            return "Invalid query";
        }

        public string ClientSingUp(string ClientRequest)
        {
            string username = ClientRequest.Substring(0, ClientRequest.IndexOf(';'));
            string password = ClientRequest.Substring(ClientRequest.IndexOf(';') + 1);

            for(int id = 1; id <= QuantityOfUsers; id++)
            {
                if(username == DbReadNote($"SELECT username FROM users WHERE id = {id}"))
                    return "Invalid query";
            }

            DbWriteNote($"INSERT INTO users (id, username, password, cash) VALUES ({++QuantityOfUsers}, '{username}', '{password}', 0)");

            Console.WriteLine($"User {username} has registered\t{DateTime.Now}");
            return $"{QuantityOfUsers}";
        }

        public string UpdateCash(string ClientRequest, bool IsSave)
        {
            int id = Convert.ToInt32(ClientRequest.Substring(0, ClientRequest.IndexOf(';')));
            ClientRequest = ClientRequest.Substring(ClientRequest.IndexOf(';') + 1);
            int cash = Convert.ToInt32(ClientRequest.Substring(0, ClientRequest.IndexOf(';')));
            string message = ClientRequest.Substring(ClientRequest.IndexOf(';') + 1);

            int money = Convert.ToInt32(DbReadNote($"SELECT cash FROM users WHERE id = {id}"));
            string username = DbReadNote($"SELECT username FROM users WHERE id = {id}");

            DbWriteNote($"UPDATE users SET cash = {cash} WHERE id = {id}");

            if (IsSave)
            {
                DbWriteNote($"INSERT INTO history (id, user_id, cash, message) VALUES ({++QuantityOfHistoryNotes}, {id}, {cash - money}, '{message}')");
                Console.WriteLine($"User {username} a saved {cash - money} coins\t{DateTime.Now}");
            }
            else
            {
                DbWriteNote($"INSERT INTO history (id, user_id, cash, message) VALUES ({++QuantityOfHistoryNotes}, {id}, {cash - money}, '{message}')");
                Console.WriteLine($"User {username} a spent {cash - money} coins\t{DateTime.Now}");
            }

            return "Success";
        }
        public string GiveIncomes(string ClientRequest)
        {
            int id = Convert.ToInt32(ClientRequest);

            string response = DbReadNote($"SELECT COUNT(*) FROM incomes WHERE user_id = {id}") + ";";
            DbConnection.Open();
            MySqlCommand command = new MySqlCommand($"SELECT cash, message, period FROM incomes WHERE user_id = {id}", DbConnection);

            MySqlDataReader reader = command.ExecuteReader();

            while(reader.Read())
            {
                response += reader[0] + ";" + reader[1].ToString() + ";" + reader[2].ToString() + ";";
            }

            DbConnection.Close();
            return response;
        }

        public string GiveExpenses(string ClientRequest)
        {
            int id = Convert.ToInt32(ClientRequest);

            string response = DbReadNote($"SELECT COUNT(*) FROM expenses WHERE user_id = {id}") + ";";
            DbConnection.Open();
            MySqlCommand command = new MySqlCommand($"SELECT cash, message, period FROM expenses WHERE user_id = {id}", DbConnection);

            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                response += reader[0] + ";" + reader[1].ToString() + ";" + reader[2].ToString() + ";";
            }

            DbConnection.Close();
            return response;
        }

        public string GiveHistory(string ClientRequest)
        {
            int id = Convert.ToInt32(ClientRequest);

            string response = DbReadNote($"SELECT COUNT(*) FROM history WHERE user_id = {id}") + ";";
            DbConnection.Open();
            MySqlCommand command = new MySqlCommand($"SELECT cash, message FROM history WHERE user_id = {id}", DbConnection);

            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                response += reader[0] + ";" + reader[1].ToString() + ";";
            }

            DbConnection.Close();
            return response;
        }

        public string ClearHistory(string ClientRequest)
        {
            int id = Convert.ToInt32(ClientRequest);
            DbWriteNote($"DELETE FROM history WHERE user_id = {id}");

            QuantityOfHistoryNotes = Convert.ToInt32(DbReadNote("SELECT COUNT(*) FROM history"));
            Console.WriteLine($"Send the history to the user under id: {id}");
            return "Success";
        }

        public string AddIncome(string ClientRequest)
        {
            int user_id = Convert.ToInt32(ClientRequest.Substring(0, ClientRequest.IndexOf(';')));
            ClientRequest = ClientRequest.Substring(ClientRequest.IndexOf(';') + 1);
            int cash = Convert.ToInt32(ClientRequest.Substring(0, ClientRequest.IndexOf(';')));
            ClientRequest = ClientRequest.Substring(ClientRequest.IndexOf(';') + 1);
            string message = ClientRequest.Substring(0, ClientRequest.IndexOf(';'));
            ClientRequest = ClientRequest.Substring(ClientRequest.IndexOf(';') + 1);
            string date = ClientRequest.Substring(0, ClientRequest.IndexOf(';'));
            int period = Convert.ToInt32(ClientRequest.Substring(ClientRequest.IndexOf(';') + 1));

            DbWriteNote($"INSERT INTO incomes (id, user_id, cash, message, сreation_date, period) VALUES ({++QuantityOfIncomes}, {user_id}, {cash}, '{message}', '{date}', {period})");
            Console.WriteLine($"User under id: {user_id} added income with cash: {cash}, message: {message}, period: {period}");
            return "Success";
        }

        public string AddExpense(string ClientRequest)
        {
            int user_id = Convert.ToInt32(ClientRequest.Substring(0, ClientRequest.IndexOf(';')));
            ClientRequest = ClientRequest.Substring(ClientRequest.IndexOf(';') + 1);
            int cash = Convert.ToInt32(ClientRequest.Substring(0, ClientRequest.IndexOf(';')));
            ClientRequest = ClientRequest.Substring(ClientRequest.IndexOf(';') + 1);
            string message = ClientRequest.Substring(0, ClientRequest.IndexOf(';'));
            ClientRequest = ClientRequest.Substring(ClientRequest.IndexOf(';') + 1);
            string date = ClientRequest.Substring(0, ClientRequest.IndexOf(';'));
            int period = Convert.ToInt32(ClientRequest.Substring(ClientRequest.IndexOf(';') + 1));

            DbWriteNote($"INSERT INTO expenses (id, user_id, cash, message, date, period) VALUES ({++QuantityOfExpenses}, {user_id}, {cash}, '{message}', '{date}', {period})");
            Console.WriteLine($"User under id: {user_id} added expense with cash: {cash}, message: {message}, period: {period}");
            return "Success";
        }

        public void CheckIncomes()
        {
            for(int id = 1; id <= QuantityOfIncomes; id++ )
            {
                string request = DbReadNote($"SELECT сreation_date FROM incomes WHERE id = {id}");
                request = request.Substring(0, request.IndexOf(' ') + 1);

                int day = Convert.ToInt32(request.Substring(0, request.IndexOf('.')));

                request = request.Substring(request.IndexOf('.') + 1);
                int month = Convert.ToInt32(request.Substring(0, request.IndexOf('.')));

                int year = Convert.ToInt32(request.Substring(request.IndexOf('.') + 1));

                DateTime today = DateTime.Now;
                DateTime date = new DateTime(
                    year,
                    month,
                    day
                    );
                Console.WriteLine($"{today.Year}-{today.Month}-{today.Day}");
                if((today - date).TotalDays >= Convert.ToInt32(DbReadNote($"SELECT period FROM incomes WHERE id = {id}")))
                {
                    int user_id = Convert.ToInt32(DbReadNote($"SELECT user_id FROM incomes WHERE id = {id}"));
                    string message = DbReadNote($"SELECT message FROM incomes WHERE id = {id}");
                    int money = Convert.ToInt32(DbReadNote($"SELECT cash FROM incomes WHERE id = {id}"));
                    int cash = money + Convert.ToInt32(DbReadNote($"SELECT cash FROM users WHERE id = {id}"));
                    DbWriteNote($"UPDATE users SET cash = {cash} WHERE id = {user_id}");
                    DbWriteNote($"UPDATE incomes SET сreation_date = '{today.Year}-{today.Month}-{today.Day}' WHERE id = {id}");
                    DbWriteNote($"INSERT INTO history (id, user_id, cash, message) VALUES ({++QuantityOfHistoryNotes}, {user_id}, {money}, '{message}')");
                }
            }
            Console.WriteLine("The data in the database has been updated");
        }

        public void CheckExpenses()
        {
            for (int id = 1; id <= QuantityOfExpenses; id++)
            {
                string request = DbReadNote($"SELECT date FROM expenses WHERE id = {id}");
                request = request.Substring(0, request.IndexOf(' ') + 1);

                int day = Convert.ToInt32(request.Substring(0, request.IndexOf('.')));

                request = request.Substring(request.IndexOf('.') + 1);
                int month = Convert.ToInt32(request.Substring(0, request.IndexOf('.')));

                int year = Convert.ToInt32(request.Substring(request.IndexOf('.') + 1));

                DateTime today = DateTime.Now;
                DateTime date = new DateTime(
                    year,
                    month,
                    day
                    );
                Console.WriteLine($"{today.Year}-{today.Month}-{today.Day}");
                if ((today - date).TotalDays >= Convert.ToInt32(DbReadNote($"SELECT period FROM expenses WHERE id = {id}")))
                {
                    int user_id = Convert.ToInt32(DbReadNote($"SELECT user_id FROM expenses WHERE id = {id}"));
                    string message = DbReadNote($"SELECT message FROM expenses WHERE id = {id}");
                    int money = Convert.ToInt32(DbReadNote($"SELECT cash FROM expenses WHERE id = {id}"));
                    int cash = money - Convert.ToInt32(DbReadNote($"SELECT cash FROM users WHERE id = {id}"));
                    DbWriteNote($"UPDATE users SET cash = {cash} WHERE id = {user_id}");
                    DbWriteNote($"UPDATE expenses SET date = '{today.Year}-{today.Month}-{today.Day}' WHERE id = {id}");
                    DbWriteNote($"INSERT INTO history (id, user_id, cash, message) VALUES ({++QuantityOfHistoryNotes}, {user_id}, {-money}, '{message}')");
                }
            }
            Console.WriteLine("The data in the database has been updated");
        }

        public string RemoveIncome(string ClientRequest)
        {
            int user_id = Convert.ToInt32(ClientRequest.Substring(0, ClientRequest.IndexOf(';')));
            ClientRequest = ClientRequest.Substring(ClientRequest.IndexOf(';') + 1);
            string message = ClientRequest.Substring(0, ClientRequest.IndexOf(';'));
            ClientRequest = ClientRequest.Substring(ClientRequest.IndexOf(';') + 1);
            int cash = Convert.ToInt32(ClientRequest.Substring(0, ClientRequest.IndexOf(';')));
            int period = Convert.ToInt32(ClientRequest.Substring(ClientRequest.IndexOf(';') + 1));

            int id = Convert.ToInt32(DbReadNote($"SELECT id FROM incomes WHERE user_id = {user_id} AND message = '{message}' AND cash = {cash} AND period = {period}"));
            DbWriteNote($"DELETE FROM incomes WHERE user_id = {user_id} AND message = '{message}' AND cash = {cash} AND period = {period}");
            QuantityOfIncomes--;

            for (int i = id; i <= QuantityOfIncomes; i++)
            {
                DbWriteNote($"UPDATE incomes SET id = {i} WHERE id = {i + 1}");
            }
            Console.WriteLine($"User under id: {user_id} removed income with cash: {cash}, message: {message}, period: {period}");
            return "Success";
        }

        public string RemoveExpense(string ClientRequest)
        {
            int user_id = Convert.ToInt32(ClientRequest.Substring(0, ClientRequest.IndexOf(';')));
            ClientRequest = ClientRequest.Substring(ClientRequest.IndexOf(';') + 1);
            string message = ClientRequest.Substring(0, ClientRequest.IndexOf(';'));
            ClientRequest = ClientRequest.Substring(ClientRequest.IndexOf(';') + 1);
            int cash = Convert.ToInt32(ClientRequest.Substring(0, ClientRequest.IndexOf(';')));
            int period = Convert.ToInt32(ClientRequest.Substring(ClientRequest.IndexOf(';') + 1));

            int id = Convert.ToInt32(DbReadNote($"SELECT id FROM expenses WHERE user_id = {user_id} AND message = '{message}' AND cash = {cash} AND period = {period}"));
            DbWriteNote($"DELETE FROM expenses WHERE user_id = {user_id} AND message = '{message}' AND cash = {cash} AND period = {period}");
            QuantityOfExpenses--;

            for (int i = id; i <= QuantityOfExpenses; i++)
            {
                DbWriteNote($"UPDATE expenses SET id = {i} WHERE id = {i + 1}");
            }
            Console.WriteLine($"User under id: {user_id} removed income with cash: {cash}, message: {message}, period: {period}");
            return "Success";
        }
    }
}
