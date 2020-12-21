using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Coiner
{
    public partial class Coiner : Form
    {
        Client client = new Client("127.0.0.1", 2222);

        public Coiner()
        {
            InitializeComponent();
        }

        private void SingIn_Button_Click(object sender, EventArgs e)
        {
            Info.Visible = false;

            string info;
            if(EntryDataCkeck(out info, Username_TextBox.Text, Password_TextBox.Text))
            {
                Username_Panel.BackColor = Color.FromArgb(255, 128, 128);
                Password_Panel.BackColor = Color.FromArgb(255, 128, 128);
                Password_TextBox.UseSystemPasswordChar = false;
                Username_TextBox.Text = "Username";
                Password_TextBox.Text = "Password";
                Info.Text = info;
                Info.Visible = true;
            }
            else
            {
                client.ActiveUser = client.SingIn(Username_TextBox.Text, Password_TextBox.Text);
                
                if(client.ActiveUser == null)
                {
                    Username_Panel.BackColor = Color.FromArgb(255, 128, 128);
                    Password_Panel.BackColor = Color.FromArgb(255, 128, 128);

                    Password_TextBox.UseSystemPasswordChar = false;
                    Info.Text = "Info:\nUser not registered";
                    Username_TextBox.Text = "Username";
                    Password_TextBox.Text = "Password";
                    Info.Visible = true;
                }
                else
                {
                    Author_Animation.Stop();
                    Author.Visible = false;
                    AnotherProjects.Visible = false;

                    Username_Panel.BackColor = Color.FromArgb(121, 252, 103);
                    Password_Panel.BackColor = Color.FromArgb(121, 252, 103);

                    client.ActiveUser.Incomes = client.GetIncomes();
                    client.ActiveUser.Expenses = client.GetExpenses();
                    client.ActiveUser.Histories = client.GetHistories();

                    double speed = 1;
                    while (User_Panel.Location.X <= Coiner.ActiveForm.Width)
                    {
                        speed *= 1.5;
                        User_Panel.Location = new Point(User_Panel.Location.X + (int)speed, User_Panel.Location.Y);
                        Thread.Sleep(1);
                    }

                    speed = 1;
                    while (Main_Panel.Location.Y <= 0)
                    {
                        speed *= 1.1;
                        Main_Panel.Location = new Point(Main_Panel.Location.X, Main_Panel.Location.Y + (int)speed);
                        Thread.Sleep(1);
                    }
                    Main_Panel.Location = new Point(0, 0);
                    User_Label.Text = Username_TextBox.Text;
                    Cash_Label.Text = "Cash: " + client.ActiveUser.Cash;

                    foreach (var income in client.ActiveUser.Incomes)
                    {
                        ListViewItem item = new ListViewItem(income.Name);
                        item.SubItems.Add(income.Cash.ToString());
                        item.SubItems.Add(income.Period.ToString());

                        Incomes_ListView.Items.AddRange(new ListViewItem[] { item });
                    }

                    foreach (var expense in client.ActiveUser.Expenses)
                    {
                        ListViewItem item = new ListViewItem(expense.Name);
                        item.SubItems.Add(expense.Cash.ToString());
                        item.SubItems.Add(expense.Period.ToString());

                        Expenses_ListView.Items.AddRange(new ListViewItem[] { item });
                    }

                    foreach (var history in client.ActiveUser.Histories)
                    {
                        ListViewItem item = new ListViewItem(history.Message);

                        if (history.Cash > 0)
                            item.SubItems.Add("+ " + history.Cash.ToString());
                        else
                            item.SubItems.Add("- " + Math.Abs(history.Cash).ToString());

                        History_ListView.Items.AddRange(new ListViewItem[] { item });
                    }

                    //Incomes_Panel
                    speed = 1;
                    while (Incomes_Panel.Location.X <= 4)
                    {
                        speed *= 1.5;
                        Incomes_Panel.Location = new Point(Incomes_Panel.Location.X + (int)speed, Incomes_Panel.Location.Y);
                        Expenses_Panel.Location = new Point(Expenses_Panel.Location.X + (int)speed, Expenses_Panel.Location.Y);
                        Thread.Sleep(1);
                    }
                    Incomes_Panel.Location = new Point(4, Incomes_Panel.Location.Y);
                    Expenses_Panel.Location = new Point(4, Expenses_Panel.Location.Y);

                    //History_Panel
                    speed = 1;
                    while(History_Panel.Location.X >= 470)
                    {
                        speed *= 1.5;
                        History_Panel.Location = new Point(History_Panel.Location.X - (int)speed, History_Panel.Location.Y);
                        Thread.Sleep(1);
                    }
                    History_Panel.Location = new Point(470, History_Panel.Location.Y);

                    rope1.Visible = true;
                    rope2.Visible = true;
                    rope3.Visible = true;
                    rope4.Visible = true;
                    rope5.Visible = true;
                    rope6.Visible = true;
                }
            }
        }

        private void Coiner_Load(object sender, EventArgs e)
        {
            rope1.Visible = false;
            rope2.Visible = false;
            rope3.Visible = false;
            rope4.Visible = false;
            rope5.Visible = false;
            rope6.Visible = false;

            Main_Panel.Location = new Point(Main_Panel.Location.X, -90);
            AnotherProjects.Visible = false;
            GoBack_PictureBox.Visible = false;
            Click_Animation.Start();
            User_Panel.Location = new Point(-586, 180);
            SingUp_Button.Visible = false;
            Info.Visible = false;
            Password_PictureBox2.Visible = false;
            Password_Panel2.Visible = false;
            Password_TextBox2.Visible = false;
            Transaction_Panel.Visible = false;
            Incomes_Panel.Location = new Point(-512, Incomes_Panel.Location.Y);
            Incomes_Edit_Panel.Visible = false;
            Expenses_Edit_Panel.Visible = false;
            History_Panel.Location = new Point(1000, History_Panel.Location.Y);
            Expenses_Panel.Location = new Point(-512, Expenses_Panel.Location.Y);
        }

        public static bool EntryDataCkeck(out string Info, string username, string password, string passwaord2 = null)
        {
            bool invalidEntryData = false;
            Info = "Info:";
            
            if (username == "" || password == "" ||
                username == "Username" || password == "Password"
                )
            {
                Info += "\nEnter the data";
                invalidEntryData = true;
            }

            if(passwaord2 != null && password != passwaord2)
            {
                Info += "\nPasswords do not match";
                invalidEntryData = true;
            }
            
            if (username.Contains(';') || password.Contains(';'))
            {
                Info += "\nUsername or password cannot contain ';'";
                invalidEntryData = true;
            }

            if (username.Length > 20)
            {
                Info += "\nUsername cannot be longer than 20 characters";
                invalidEntryData = true;
            }

            if (password.Length > 30)
            {
                Info += "\nPassword cannot be longer than 30 characters";
                invalidEntryData = true;
            }

            return invalidEntryData;
        }

        private void Username_TextBox_MouseClick(object sender, MouseEventArgs e)
        {
            if(Username_TextBox.Text == "Username")
                Username_TextBox.Text = "";

            Username_Panel.BackColor = Color.FromArgb(255, 255, 255);
        }

        private void Password_TextBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (Password_TextBox.Text == "Password")
            {
                Password_TextBox.UseSystemPasswordChar = true;
                Password_TextBox.Text = "";
            }

            Password_Panel.BackColor = Color.FromArgb(255, 255, 255);
        }

        private void Exit_PictureBox_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Exit_PictureBox_MouseEnter(object sender, EventArgs e)
        {
            Exit_PictureBox.BackColor = Color.FromArgb(70, 70, 70);
        }

        private void Exit_PictureBox_MouseLeave(object sender, EventArgs e)
        {
            Exit_PictureBox.BackColor = Color.FromArgb(120, 120, 120);
        }
        
        private bool isDragging = false;
        private Point lastCursor;
        private Point lastForm;
        private void Comntrol_Panel_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
            this.Opacity = 0.5;
            lastCursor = Cursor.Position;
            lastForm = this.Location;
        }

        private void Control_Panel_MouseUp(object sender, MouseEventArgs e)
        {
            this.Opacity = 1;
            isDragging = false;
        }

        private void Control_Panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                this.Location = Point.Add(lastForm, new Size(Point.Subtract(Cursor.Position, new Size(lastCursor))));
            }
        }

        private void Minimize_PictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Minimize_PictureBox_MouseEnter(object sender, EventArgs e)
        {
            Minimize_PictureBox.BackColor = Color.FromArgb(70, 70, 70);
        }

        private void Minimize_PictureBox_MouseLeave(object sender, EventArgs e)
        {
            Minimize_PictureBox.BackColor = Color.FromArgb(120, 120, 120);
        }

        private void View_PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            View_PictureBox.BackColor = Color.FromArgb(70, 70, 70);

            Password_TextBox.UseSystemPasswordChar = false;
            Password_TextBox2.UseSystemPasswordChar = false;
        }

        private void View_PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            View_PictureBox.BackColor = Color.FromArgb(50, 50, 50);

            if (Password_TextBox.Text != "Password")
            {
                Password_TextBox.UseSystemPasswordChar = true;
                Password_TextBox2.UseSystemPasswordChar = true;
            }
            
        }
        private void SingUp_LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SingIn_Button.Visible = false;
            GoBack_PictureBox.Visible = true;
            Password_TextBox.UseSystemPasswordChar = false;
            Password_TextBox2.UseSystemPasswordChar = false;
            Username_TextBox.Text = "Username";
            Password_TextBox.Text = "Password";
            Password_TextBox2.Text = "Password";

            Username_Panel.BackColor = Color.FromArgb(255, 255, 255);
            Password_Panel.BackColor = Color.FromArgb(255, 255, 255);
            Password_Panel2.BackColor = Color.FromArgb(255, 255, 255);

            SingUp_Button.Visible = true;
            Info.Visible = false;
            Password_PictureBox2.Visible = true;
            Password_Panel2.Visible = true;
            Password_TextBox2.Visible = true;
            SingUp_LinkLabel.Visible = false;
        }

        private void Password_TextBox2_Click(object sender, EventArgs e)
        {
            if (Password_TextBox2.Text == "Password")
            {
                Password_TextBox2.UseSystemPasswordChar = true;
                Password_TextBox2.Text = "";
            }

            Password_Panel2.BackColor = Color.FromArgb(255, 255, 255);
        }

        private void SingUp_Button_Click(object sender, EventArgs e)
        {
            Info.Visible = false;

            string info;
            if (EntryDataCkeck(out info, Username_TextBox.Text, Password_TextBox.Text, Password_TextBox2.Text))
            {
                Password_TextBox.UseSystemPasswordChar = false;
                Password_TextBox2.UseSystemPasswordChar = false;
                Username_TextBox.Text = "Username";
                Password_TextBox.Text = "Password";
                Password_TextBox2.Text = "Password";

                Username_Panel.BackColor = Color.FromArgb(255, 128, 128);
                Password_Panel.BackColor = Color.FromArgb(255, 128, 128);
                Password_Panel2.BackColor = Color.FromArgb(255, 128, 128);

                Info.Text = info;
                Info.Visible = true;
            }
            else
            {
                client.ActiveUser = client.SingUp(Username_TextBox.Text, Password_TextBox.Text);

                if (client.ActiveUser == null)
                {
                    Username_Panel.BackColor = Color.FromArgb(255, 128, 128);
                    Password_Panel.BackColor = Color.FromArgb(255, 128, 128);
                    Password_Panel2.BackColor = Color.FromArgb(255, 128, 128);

                    Password_TextBox.UseSystemPasswordChar = false;
                    Password_TextBox2.UseSystemPasswordChar = false;
                    Info.Text = $"Info:\nUsername \"{Username_TextBox.Text}\"\nis already taken";
                    Username_TextBox.Text = "Username";
                    Password_TextBox.Text = "Password";
                    Password_TextBox2.Text = "Password";
                    Info.Visible = true;
                }
                else
                {
                    Author_Animation.Stop();
                    Author.Visible = false;
                    AnotherProjects.Visible = false;

                    User_Panel.Controls.Remove(Info);
                    Info.Location = new Point(718, 83);
                    this.Controls.Add(Info);
                    Username_Panel.BackColor = Color.FromArgb(121, 252, 103);
                    Password_Panel.BackColor = Color.FromArgb(121, 252, 103);
                    Password_Panel2.BackColor = Color.FromArgb(121, 252, 103);

                    client.ActiveUser.Incomes = client.GetIncomes();
                    client.ActiveUser.Expenses = client.GetExpenses();

                    double speed = 1;
                    while(User_Panel.Location.X <= Coiner.ActiveForm.Width)
                    {
                        speed *= 1.5;
                        User_Panel.Location = new Point(User_Panel.Location.X + (int)speed, User_Panel.Location.Y);
                        Thread.Sleep(1);
                    }

                    speed = 1;
                    while (Main_Panel.Location.Y <= 0)
                    {
                        speed *= 1.1;
                        Main_Panel.Location = new Point(Main_Panel.Location.X, Main_Panel.Location.Y + (int)speed);
                        Thread.Sleep(1);
                    }
                    Main_Panel.Location = new Point(0, 0);
                    User_Label.Text = Username_TextBox.Text;
                    Cash_Label.Text += client.ActiveUser.Cash;

                    foreach (var income in client.ActiveUser.Incomes)
                    {
                        ListViewItem item = new ListViewItem(income.Name);
                        item.SubItems.Add(income.Cash.ToString());
                        item.SubItems.Add(income.Period.ToString());

                        Incomes_ListView.Items.AddRange(new ListViewItem[] { item });
                    }

                    foreach (var expense in client.ActiveUser.Expenses)
                    {
                        ListViewItem item = new ListViewItem(expense.Name);
                        item.SubItems.Add(expense.Cash.ToString());
                        item.SubItems.Add(expense.Period.ToString());

                        Expenses_ListView.Items.AddRange(new ListViewItem[] { item });
                    }

                    foreach (var history in client.ActiveUser.Histories)
                    {
                        ListViewItem item = new ListViewItem(history.Message);

                        if (history.Cash > 0)
                            item.SubItems.Add("+" + history.Cash.ToString());
                        else
                            item.SubItems.Add("-" + history.Cash.ToString());

                        History_ListView.Items.AddRange(new ListViewItem[] { item });
                    }

                    //Incomes_Panel
                    speed = 1;
                    while (Incomes_Panel.Location.X <= 4)
                    {
                        speed *= 1.5;
                        Incomes_Panel.Location = new Point(Incomes_Panel.Location.X + (int)speed, Incomes_Panel.Location.Y);
                        Expenses_Panel.Location = new Point(Expenses_Panel.Location.X + (int)speed, Expenses_Panel.Location.Y);
                        Thread.Sleep(1);
                    }
                    Incomes_Panel.Location = new Point(4, Incomes_Panel.Location.Y);
                    Expenses_Panel.Location = new Point(4, Expenses_Panel.Location.Y);

                    //History_Panel
                    speed = 1;
                    while (History_Panel.Location.X >= 470)
                    {
                        speed *= 1.5;
                        History_Panel.Location = new Point(History_Panel.Location.X - (int)speed, History_Panel.Location.Y);
                        Thread.Sleep(1);
                    }
                    History_Panel.Location = new Point(470, History_Panel.Location.Y);

                    rope1.Visible = true;
                    rope2.Visible = true;
                    rope3.Visible = true;
                    rope4.Visible = true;
                    rope5.Visible = true;
                    rope6.Visible = true;
                }
            }
        }

        int CounterOfSteps = 0;
        bool DirectionIsUp = true;
        private void Click_Animation_Tick(object sender, EventArgs e)
        {
            if (CounterOfSteps < 15 && DirectionIsUp)
            {
                Click_PictureBox.Location = new Point(Click_PictureBox.Location.X, Click_PictureBox.Location.Y - 1);
                CounterOfSteps++;

                if (CounterOfSteps == 15)
                    DirectionIsUp = false;
            }

            if (CounterOfSteps > 0 && !DirectionIsUp)
            {
                Click_PictureBox.Location = new Point(Click_PictureBox.Location.X, Click_PictureBox.Location.Y + 1);
                CounterOfSteps--;

                if (CounterOfSteps == 0)
                    DirectionIsUp = true;
            }
        }

        private void Get_Rich_Button_Click(object sender, EventArgs e)
        {
            Click_Animation.Stop();
            Get_Rich_Button.Visible = false;
            Click_PictureBox.Visible = false;
            AnotherProjects.Visible = true;

            Username_Panel.BackColor = Color.White;
            Password_Panel.BackColor = Color.White;
            Password_Panel2.BackColor = Color.White;

            double speed = 1;
            while (User_Panel.Location.X <= Coiner.ActiveForm.Width / 2 - User_Panel.Width / 2)
            {
                speed *= 1.5;
                User_Panel.Location = new Point(User_Panel.Location.X + (int)speed, 180);
                Thread.Sleep(1);
            }
            User_Panel.Location = new Point(Coiner.ActiveForm.Width / 2 - User_Panel.Width / 2, 180);

            Author.Visible = true;
        }

        private void GoBack_PictureBox_Click(object sender, EventArgs e)
        {
            SingIn_Button.Visible = true;
            GoBack_PictureBox.Visible = false;
            SingUp_Button.Visible = false;
            Password_Panel2.Visible = false;
            Password_PictureBox2.Visible = false;
            Password_TextBox2.Visible = false;
            SingUp_LinkLabel.Visible = true;
        }

        private void GoBack_PictureBox_MouseEnter(object sender, EventArgs e)
        {
            GoBack_PictureBox.BackColor = Color.FromArgb(70, 70, 70);
        }

        private void GoBack_PictureBox_MouseLeave(object sender, EventArgs e)
        {
            GoBack_PictureBox.BackColor = Color.FromArgb(50, 50, 50);
        }

        private void View_PictureBox_MouseEnter(object sender, EventArgs e)
        {
            View_PictureBox.BackColor = Color.FromArgb(60, 60, 60);
        }

        private void View_PictureBox_MouseLeave(object sender, EventArgs e)
        {
            View_PictureBox.BackColor = Color.FromArgb(50, 50, 50);
        }

        private void Plus_PictureBox_MouseEnter(object sender, EventArgs e)
        {
            Plus_PictureBox.BackColor = Color.FromArgb(50, 50, 50);
        }

        private void Plus_PictureBox_MouseLeave(object sender, EventArgs e)
        {
            Plus_PictureBox.BackColor = Color.FromArgb(30, 30, 30);
        }

        private void Minus_PictureBox_MouseEnter(object sender, EventArgs e)
        {
            Minus_PictureBox.BackColor = Color.FromArgb(50, 50, 50);
        }

        private void Minus_PictureBox_MouseLeave(object sender, EventArgs e)
        {
            Minus_PictureBox.BackColor = Color.FromArgb(30, 30, 30);
        }

        private void Transaction_Cash_TextBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (Transaction_Cash_TextBox.Text == "Cash")
                Transaction_Cash_TextBox.Text = "";
        }

        private void Incomes_Edit_Message_TextBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (Incomes_Message_Edit_TextBox.Text == "Name")
                Incomes_Message_Edit_TextBox.Text = "";
        }

        private void Incomes_Cash_Edit_TextBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (Incomes_Cash_Edit_TextBox.Text == "Cash")
                Incomes_Cash_Edit_TextBox.Text = "";
        }

        private void Incomes_Period_Edit_TextBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (Incomes_Period_Edit_TextBox.Text == "Period in days")
                Incomes_Period_Edit_TextBox.Text = "";
        }

        private void Transaction_Message_TextBox_Click(object sender, EventArgs e)
        {
            if (Transaction_Message_TextBox.Text == "Message")
                Transaction_Message_TextBox.Text = "";
        }

        private void Add_Income_Button_MouseClick(object sender, MouseEventArgs e)
        {
            Info.Visible = false;
            Expenses_Info.Visible = false;
            Transaction_Panel.Visible = false;
            Expenses_Edit_Panel.Visible = false;
            IsAdd = true;
            Incomes_Edit_Panel.Visible = true;
            Message_Edit_Panel.BackColor = Color.FromArgb(121, 252, 103);
            Cash_Edit_Panel.BackColor = Color.FromArgb(121, 252, 103);
            Period_Edit_Panel.BackColor = Color.FromArgb(121, 252, 103);
        }

        private void Plus_PictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            Incomes_Info.Visible = false;
            Expenses_Info.Visible = false;
            Incomes_Edit_Panel.Visible = false;
            Expenses_Edit_Panel.Visible = false;
            Transaction_Panel.Visible = true;
            IsIncome = true;
            Transaction_Cash_Panel.BackColor = Color.FromArgb(121, 252, 103);
            Transaction_Message_Panel.BackColor = Color.FromArgb(121, 252, 103);
        }

        bool IsAdd = true;
        private void Accept_Incomes_Button_MouseClick(object sender, MouseEventArgs e)
        {
            Incomes_Info.Visible = false;
            Incomes_Info.Text = "Info:";
            List<Income> incomes = client.GetIncomes();
            bool isReiteration = false;

            if(IsAdd)
                foreach (var income in incomes)
                {
                    if (Incomes_Message_Edit_TextBox.Text == income.Name)
                    {
                        isReiteration = true;
                        break;
                    }
                }

            if (Incomes_Message_Edit_TextBox.Text != "Name" && Incomes_Message_Edit_TextBox.Text != "" &&
                Incomes_Cash_Edit_TextBox.Text != "Cash" && Incomes_Cash_Edit_TextBox.Text != "" &&
                Incomes_Period_Edit_TextBox.Text != "Period in days" && Incomes_Period_Edit_TextBox.Text != "" &&
                !isReiteration && incomes != null &&
                ContainOnlyNums(Incomes_Cash_Edit_TextBox.Text) && ContainOnlyNums(Incomes_Period_Edit_TextBox.Text)
                )
            {
                bool isContains = false;
                Income checkForContains = new Income(Convert.ToInt32(Incomes_Cash_Edit_TextBox.Text), Incomes_Message_Edit_TextBox.Text, Convert.ToInt32(Incomes_Period_Edit_TextBox.Text));

                foreach (var i in incomes)
                    if (checkForContains.Cash == i.Cash)
                        if (checkForContains.Name == i.Name)
                            if (checkForContains.Period == i.Period)
                                isContains = true;

                if (IsAdd)
                {
                    Incomes_Edit_Panel.Visible = false;
                    client.AddIncome(Incomes_Message_Edit_TextBox.Text,
                        Convert.ToInt32(Incomes_Cash_Edit_TextBox.Text),
                        Convert.ToInt32(Incomes_Period_Edit_TextBox.Text),
                        DateTime.Now
                        );

                    ListViewItem item = new ListViewItem(Incomes_Message_Edit_TextBox.Text);
                    item.SubItems.Add(Incomes_Cash_Edit_TextBox.Text);
                    item.SubItems.Add(Incomes_Period_Edit_TextBox.Text);

                    Incomes_ListView.Items.AddRange(new ListViewItem[] { item });
                }
                else if (isContains)
                {
                    Incomes_ListView.Items.Clear();

                    client.RemoveIncome(Incomes_Message_Edit_TextBox.Text,
                        Convert.ToInt32(Incomes_Cash_Edit_TextBox.Text),
                        Convert.ToInt32(Incomes_Period_Edit_TextBox.Text)
                        );

                    client.ActiveUser.Incomes = client.GetIncomes();

                    foreach (var income in client.ActiveUser.Incomes)
                    {
                        ListViewItem item = new ListViewItem(income.Name);
                        item.SubItems.Add(income.Cash.ToString());
                        item.SubItems.Add(income.Period.ToString());

                        Incomes_ListView.Items.AddRange(new ListViewItem[] { item });
                    }
                }
                else
                {
                    Incomes_Info.Text += "\nYou don't have this income";
                    Incomes_Info.Visible = true;
                }
            }
            else
            {
                if (isReiteration)
                {
                    Incomes_Info.Text += "\nThe name must be unique";
                    Incomes_Info.Visible = true;
                }

                if(!(Incomes_Message_Edit_TextBox.Text != "Name" && Incomes_Message_Edit_TextBox.Text != "" &&
                Incomes_Cash_Edit_TextBox.Text != "Cash" && Incomes_Cash_Edit_TextBox.Text != "" &&
                Incomes_Period_Edit_TextBox.Text != "Period in days" && Incomes_Period_Edit_TextBox.Text != ""
                ))
                    Incomes_Info.Text += "\nEnter the data";

                if(incomes != null && !IsAdd)
                    Incomes_Info.Text += "\nYou have no income";

                if(!ContainOnlyNums(Incomes_Cash_Edit_TextBox.Text))
                    Incomes_Info.Text += "\nCash field must contain only numbers";

                if (!ContainOnlyNums(Incomes_Period_Edit_TextBox.Text))
                    Incomes_Info.Text += "\nPeriod field must contain only numbers";

                Incomes_Info.Visible = true;
            }

            Incomes_Message_Edit_TextBox.Text = "Name";
            Incomes_Cash_Edit_TextBox.Text = "Cash";
            Incomes_Period_Edit_TextBox.Text = "Period in days";
        }

        private void X_Incomes_Button_MouseClick(object sender, MouseEventArgs e)
        {
            Incomes_Info.Visible = false;
            Incomes_Edit_Panel.Visible = false;
            Incomes_Message_Edit_TextBox.Text = "Name";
            Incomes_Cash_Edit_TextBox.Text = "Cash";
            Incomes_Period_Edit_TextBox.Text = "Period in days";
        }

        private void X_Button_MouseClick(object sender, MouseEventArgs e)
        {
            Transaction_Panel.Visible = false;
            Transaction_Info.Visible = false;
            Transaction_Cash_TextBox.Text = "Cash";
            Transaction_Message_TextBox.Text = "Message";
        }

        bool IsIncome = true;
        private void Accept_Button_MouseClick(object sender, MouseEventArgs e)
        {
            Transaction_Info.Visible = false;
            Transaction_Info.Text = "Info:";
            if (Transaction_Cash_TextBox.Text != "Cash" && Transaction_Cash_TextBox.Text != "" &&
                Transaction_Message_TextBox.Text != "Message" && Transaction_Message_TextBox.Text != "" &&
                ContainOnlyNums(Transaction_Cash_TextBox.Text)
                )
            {
                Transaction_Panel.Visible = false;
                if (IsIncome)
                {
                    Cash_Label.Text = $"Cash: {client.Save(Convert.ToInt32(Transaction_Cash_TextBox.Text), Transaction_Message_TextBox.Text)}";

                    ListViewItem item = new ListViewItem(Transaction_Message_TextBox.Text);
                    item.SubItems.Add("+" + Transaction_Cash_TextBox.Text);

                    History_ListView.Items.AddRange(new ListViewItem[] { item });
                }
                else
                {
                    Cash_Label.Text = $"Cash: {client.Spend(Convert.ToInt32(Transaction_Cash_TextBox.Text), Transaction_Message_TextBox.Text)}";

                    ListViewItem item = new ListViewItem(Transaction_Message_TextBox.Text);
                    item.SubItems.Add("-" + Transaction_Cash_TextBox.Text);

                    History_ListView.Items.AddRange(new ListViewItem[] { item });
                }
                    
            }
            else
            {
                Transaction_Info.Visible = true;
                Transaction_Info.Text += "\nEnter the data";

                if(!ContainOnlyNums(Transaction_Cash_TextBox.Text))
                    Transaction_Info.Text += "\nСash field must contain only numbers";
            }

            Transaction_Cash_TextBox.Text = "Cash";
            Transaction_Message_TextBox.Text = "Message";
        }

        private void Minus_PictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            Incomes_Info.Visible = false;
            Expenses_Info.Visible = false;
            Incomes_Edit_Panel.Visible = false;
            Expenses_Edit_Panel.Visible = false;
            Transaction_Panel.Visible = true;
            IsIncome = false;
            Transaction_Cash_Panel.BackColor = Color.FromArgb(255, 128, 128);
            Transaction_Message_Panel.BackColor = Color.FromArgb(255, 128, 128);
        }

        private void Coiner_MouseClick(object sender, MouseEventArgs e)
        {
            Transaction_Panel.Visible = false;
            Incomes_Edit_Panel.Visible = false;
            Transaction_Info.Visible = false;
            Incomes_Info.Visible = false;
            Info.Visible = false;
            Transaction_Info.Visible = false;
            Transaction_Cash_TextBox.Text = "Cash";
            Transaction_Message_TextBox.Text = "Message";
            Incomes_Message_Edit_TextBox.Text = "Name";
            Incomes_Cash_Edit_TextBox.Text = "Cash";
            Incomes_Period_Edit_TextBox.Text = "Period in days";
            Expenses_Edit_Panel.Visible = false;
            Expenses_Name_TextBox.Text = "Name";
            Expenses_Cash_TextBox.Text = "Cash";
            Expenses_Period_TextBox.Text = "Period in days";
        }

        private void Remove_Income_Button_MouseClick(object sender, MouseEventArgs e)
        {
            Info.Visible = false;
            Expenses_Info.Visible = false;
            IsAdd = false;
            Transaction_Panel.Visible = false;
            Expenses_Edit_Panel.Visible = false;
            Incomes_Edit_Panel.Visible = true;
            Message_Edit_Panel.BackColor = Color.FromArgb(255, 128, 128);
            Cash_Edit_Panel.BackColor = Color.FromArgb(255, 128, 128);
            Period_Edit_Panel.BackColor = Color.FromArgb(255, 128, 128);
        }

        private void Add_Expenses_Button_MouseClick(object sender, MouseEventArgs e)
        {
            Info.Visible = false;
            Incomes_Info.Visible = false;
            Transaction_Panel.Visible = false;
            Incomes_Edit_Panel.Visible = false;
            IsAddExp = true;
            Expenses_Edit_Panel.Visible = true;
            Expenses_Panel_Name.BackColor = Color.FromArgb(121, 252, 103);
            Expenses_Panel_Cash.BackColor = Color.FromArgb(121, 252, 103);
            Expenses_Panel_Period.BackColor = Color.FromArgb(121, 252, 103);
        }

        private void Expenses_Cancel_Button_MouseClick(object sender, MouseEventArgs e)
        {
            Expenses_Info.Visible = false;
            Expenses_Edit_Panel.Visible = false;
            Expenses_Name_TextBox.Text = "Name";
            Expenses_Cash_TextBox.Text = "Cash";
            Expenses_Period_TextBox.Text = "Period in days";
        }

        private void Expenses_Name_TextBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (Expenses_Name_TextBox.Text == "Name")
                Expenses_Name_TextBox.Text = "";
        }

        private void Expenses_Cash_TextBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (Expenses_Cash_TextBox.Text == "Cash")
                Expenses_Cash_TextBox.Text = "";
        }

        private void Expenses_Period_TextBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (Expenses_Period_TextBox.Text == "Period in days")
                Expenses_Period_TextBox.Text = "";
        }

        private void Remove_Expenses_Button_MouseClick(object sender, MouseEventArgs e)
        {
            Info.Visible = false;
            Incomes_Info.Visible = false;
            Transaction_Panel.Visible = false;
            Incomes_Edit_Panel.Visible = false;
            IsAddExp = false;
            Expenses_Edit_Panel.Visible = true;
            Expenses_Panel_Name.BackColor = Color.FromArgb(255, 128, 128);
            Expenses_Panel_Cash.BackColor = Color.FromArgb(255, 128, 128);
            Expenses_Panel_Period.BackColor = Color.FromArgb(255, 128, 128);
        }

        bool IsAddExp = true;

        private void Expenses_Accept_Button_MouseClick(object sender, MouseEventArgs e)
        {
            Expenses_Info.Visible = false;
            Expenses_Info.Text = "Info:";
            List<Expense> expenses = client.GetExpenses();
            bool isReiteration = false;

            if (IsAddExp)
                foreach (var expense in expenses)
                {
                    if (Expenses_Name_TextBox.Text == expense.Name)
                    {
                        isReiteration = true;
                        break;
                    }
                }

            if (Expenses_Name_TextBox.Text != "Name" && Expenses_Name_TextBox.Text != "" &&
                Expenses_Cash_TextBox.Text != "Cash" && Expenses_Cash_TextBox.Text != "" &&
                Expenses_Period_TextBox.Text != "Period in days" && Expenses_Period_TextBox.Text != "" &&
                !isReiteration && expenses != null &&
                ContainOnlyNums(Expenses_Cash_TextBox.Text) && ContainOnlyNums(Expenses_Period_TextBox.Text)
                )
            {
                bool isContains = false;
                Expense checkForContains = new Expense(Convert.ToInt32(Expenses_Cash_TextBox.Text), Expenses_Name_TextBox.Text, Convert.ToInt32(Expenses_Period_TextBox.Text));

                foreach (var i in expenses)
                    if (checkForContains.Cash == i.Cash)
                        if (checkForContains.Name == i.Name)
                            if (checkForContains.Period == i.Period)
                                isContains = true;

                if (IsAddExp)
                {
                    Expenses_Edit_Panel.Visible = false;
                    client.AddExpense(Expenses_Name_TextBox.Text,
                        Convert.ToInt32(Expenses_Cash_TextBox.Text),
                        Convert.ToInt32(Expenses_Period_TextBox.Text),
                        DateTime.Now
                        );

                    ListViewItem item = new ListViewItem(Expenses_Name_TextBox.Text);
                    item.SubItems.Add(Expenses_Cash_TextBox.Text);
                    item.SubItems.Add(Expenses_Period_TextBox.Text);

                    Expenses_ListView.Items.AddRange(new ListViewItem[] { item });
                }
                else if (isContains)
                {
                    Expenses_ListView.Items.Clear();

                    client.RemoveExpense(Expenses_Name_TextBox.Text,
                        Convert.ToInt32(Expenses_Cash_TextBox.Text),
                        Convert.ToInt32(Expenses_Period_TextBox.Text)
                        );

                    client.ActiveUser.Expenses = client.GetExpenses();

                    foreach (var expense in client.ActiveUser.Expenses)
                    {
                        ListViewItem item = new ListViewItem(expense.Name);
                        item.SubItems.Add(expense.Cash.ToString());
                        item.SubItems.Add(expense.Period.ToString());

                        Expenses_ListView.Items.AddRange(new ListViewItem[] { item });
                    }
                }
                else
                {
                    Expenses_Info.Text += "\nYou don't have this expense";
                    Expenses_Info.Visible = true;
                }
            }
            else
            {
                if (isReiteration)
                {
                    Expenses_Info.Text += "\nThe name must be unique";
                }

                if (!(Expenses_Name_TextBox.Text != "Name" && Expenses_Name_TextBox.Text != "" &&
                Expenses_Cash_TextBox.Text != "Cash" && Expenses_Cash_TextBox.Text != "" &&
                Expenses_Period_TextBox.Text != "Period in days" && Expenses_Period_TextBox.Text != ""
                ))
                    Expenses_Info.Text += "\nEnter the data";

                if (expenses != null && !IsAddExp)
                    Expenses_Info.Text += "\nYou have no income";

                if(!ContainOnlyNums(Expenses_Cash_TextBox.Text))
                    Expenses_Info.Text += "\nCash field must contain only numbers";

                if (!ContainOnlyNums(Expenses_Period_TextBox.Text))
                    Expenses_Info.Text += "\nPeriod field must contain only numbers";

                Expenses_Info.Visible = true;
            }

            Expenses_Name_TextBox.Text = "Name";
            Expenses_Cash_TextBox.Text = "Cash";
            Expenses_Period_TextBox.Text = "Period in days";
        }

        private void Clear_Button_MouseClick(object sender, MouseEventArgs e)
        {
            History_ListView.Items.Clear();
            client.ClearHistory();
        }

        private void Add_Income_Button_MouseEnter(object sender, EventArgs e)
        {
            Add_Income_Button.ForeColor = Color.Black;
        }

        private void Add_Income_Button_MouseLeave(object sender, EventArgs e)
        {
            Add_Income_Button.ForeColor = Color.FromArgb(64, 64, 64);
        }

        private void Remove_Income_Button_MouseEnter(object sender, EventArgs e)
        {
            Remove_Income_Button.ForeColor = Color.Black;
        }

        private void Remove_Income_Button_MouseLeave(object sender, EventArgs e)
        {
            Remove_Income_Button.ForeColor = Color.FromArgb(64, 64, 64);
        }

        private void Add_Expenses_Button_MouseEnter(object sender, EventArgs e)
        {
            Add_Expenses_Button.ForeColor = Color.Black;
        }

        private void Add_Expenses_Button_MouseLeave(object sender, EventArgs e)
        {
            Add_Expenses_Button.ForeColor = Color.FromArgb(64, 64, 64);
        }

        private void Remove_Expenses_Button_MouseEnter(object sender, EventArgs e)
        {
            Remove_Expenses_Button.ForeColor = Color.Black;
        }

        private void Remove_Expenses_Button_MouseLeave(object sender, EventArgs e)
        {
            Remove_Expenses_Button.ForeColor = Color.FromArgb(64, 64, 64);
        }

        private void Clear_Button_MouseEnter(object sender, EventArgs e)
        {
            Clear_Button.ForeColor = Color.Black;
        }

        private void Clear_Button_MouseLeave(object sender, EventArgs e)
        {
            Clear_Button.ForeColor = Color.FromArgb(64, 64, 64);
        }

        private void Accept_Button_MouseEnter(object sender, EventArgs e)
        {
            Accept_Button.ForeColor = Color.Black;
        }

        private void Accept_Button_MouseLeave(object sender, EventArgs e)
        {
            Accept_Button.ForeColor = Color.FromArgb(64, 64, 64);
        }

        private void Accept_Incomes_Button_MouseEnter(object sender, EventArgs e)
        {
            Accept_Incomes_Button.ForeColor = Color.Black;
        }

        private void Accept_Incomes_Button_MouseLeave(object sender, EventArgs e)
        {
            Accept_Incomes_Button.ForeColor = Color.FromArgb(64, 64, 64);
        }

        private void Expenses_Accept_Button_MouseEnter(object sender, EventArgs e)
        {
            Expenses_Accept_Button.ForeColor = Color.Black;
        }

        private void Expenses_Accept_Button_MouseLeave(object sender, EventArgs e)
        {
            Expenses_Accept_Button.ForeColor = Color.FromArgb(64, 64, 64);
        }

        private void X_Button_MouseEnter(object sender, EventArgs e)
        {
            X_Button.ForeColor = Color.Black;
        }

        private void X_Button_MouseLeave(object sender, EventArgs e)
        {
            X_Button.ForeColor = Color.FromArgb(64, 64, 64);
        }

        private void X_Incomes_Button_MouseEnter(object sender, EventArgs e)
        {
            X_Incomes_Button.ForeColor = Color.Black;
        }

        private void X_Incomes_Button_MouseLeave(object sender, EventArgs e)
        {
            X_Incomes_Button.ForeColor = Color.FromArgb(64, 64, 64);
        }

        private void Expenses_Cancel_Button_MouseEnter(object sender, EventArgs e)
        {
            Expenses_Cancel_Button.ForeColor = Color.Black;
        }

        private void Expenses_Cancel_Button_MouseLeave(object sender, EventArgs e)
        {
            Expenses_Cancel_Button.ForeColor = Color.FromArgb(64, 64, 64);
        }

        private void Get_Rich_Button_MouseEnter(object sender, EventArgs e)
        {
            Get_Rich_Button.ForeColor = Color.Black;
        }

        private void Get_Rich_Button_MouseLeave(object sender, EventArgs e)
        {
            Get_Rich_Button.ForeColor = Color.FromArgb(64, 64, 64);
        }

        private void LogOut_MouseClick(object sender, MouseEventArgs e)
        {
            Coiner_Load(sender, e);
            Get_Rich_Button_Click(sender, e);
            Username_TextBox.Text = "Username";
            Password_TextBox.Text = "Password";
            Password_TextBox.UseSystemPasswordChar = false;
            Password_TextBox2.Text = "Password";
            Password_TextBox2.UseSystemPasswordChar = false;
        }

        private void LogOut_MouseEnter(object sender, EventArgs e)
        {
            double speed = 1;
            while(LogOut.Location.Y <= 20)
            {
                LogOut.Location = new Point(LogOut.Location.X, LogOut.Location.Y + (int)speed);
                speed *= 1.1;
            }
            LogOut.Location = new Point(LogOut.Location.X, 20);
        }

        private void LogOut_MouseLeave(object sender, EventArgs e)
        {
            double speed = 1;
            while (LogOut.Location.Y >= 16)
            {
                LogOut.Location = new Point(LogOut.Location.X, LogOut.Location.Y - (int)speed);
                speed *= 1.1;
            }
            LogOut.Location = new Point(LogOut.Location.X, 16);
        }

        int R = 0;
        bool isR = true;

        int G = 0;
        bool isG = false;

        int B = 255;
        bool isB = false;
        private void Author_Animation_Tick(object sender, EventArgs e)
        {
            if (isR)
            {
                R += 5;
                B -= 5;

                if (R == 255)
                {
                    isR = false;
                    isG = true;
                }
            }

            if (isG)
            {
                G += 5;
                R -= 5;

                if (G == 255)
                {
                    isG = false;
                    isB = true;
                }
            }

            if (isB)
            {
                B += 5;
                G -= 5;

                if (B == 255)
                {
                    isB = false;
                    isR = true;
                }
            }

            Author.ForeColor = Color.FromArgb(R, G, B);
        }

        private void AnotherProjects_MouseEnter(object sender, EventArgs e)
        {
            Author_Animation.Start();
        }

        private void AnotherProjects_MouseLeave(object sender, EventArgs e)
        {
            Author_Animation.Stop();
            R = 0;
            isR = true;
            G = 0;
            isG = false;
            B = 255;
            isB = false;
            Author.ForeColor = Color.Gray;
        }

        private void Author_MouseEnter(object sender, EventArgs e)
        {
            Author_Animation.Start();
        }

        private void Author_MouseLeave(object sender, EventArgs e)
        {
            Author_Animation.Stop();
            R = 0;
            isR = true;
            G = 0;
            isG = false;
            B = 255;
            isB = false;
            Author.ForeColor = Color.Gray;
        }

        private void Author_MouseClick(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/sergey80401");
        }

        private void AnotherProjects_MouseClick(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/sergey80401");
        }

        public static bool ContainOnlyNums(string someString)
        {

            foreach(var character in someString)
            {
                if ((int)character < 48 || (int)character > 57)
                    return false;
            }

            return true;
        }
    }
}
