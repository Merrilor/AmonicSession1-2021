using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Session1.Windows
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private int failCounter = 0;

        public enum Roles
        {
            Administrator = 1,
            User = 2
        }

        public LoginWindow()
        {
            InitializeComponent();
        }
     
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            if(EmailTextBox.Text == String.Empty || UserPasswordBox.Password == String.Empty)
            {
                MessageBox.Show("Все поля должны быть заполнены", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            string MD5HashString = String.Join("", MD5.Create().ComputeHash(Encoding.Unicode.GetBytes(UserPasswordBox.Password)).Select(hashbyte => hashbyte.ToString("x2"))); 

            Session1Entities entities = new Session1Entities();

            Users User;

            try
            {
                 User = entities.Users.Where(u => u.Email == EmailTextBox.Text && u.Password == MD5HashString).Single();
            }
            catch
            {

                MessageBox.Show("Неверные логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                failCounter++;

                if (failCounter > 3)
                {
                    Task.Run(() =>
                    {
                        LoginButton.Dispatcher.Invoke(async () =>
                        {
                            LoginButton.IsHitTestVisible = false;
                                                        
                            int SecondsLeft = 10;
                            do
                            {
                                LoginButton.Content = SecondsLeft.ToString();
                                SecondsLeft--;

                                await Task.Delay(1000);
                            } while (SecondsLeft > 0);

                            LoginButton.Content = "Login";
                            LoginButton.IsHitTestVisible = true;
                            failCounter = 0;
                            return;

                        });



                    });
                }


                return;
            }
            

            if(User.Active == false)
            {
                MessageBox.Show("Пользователь отключен администратором", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

           
            Window NewWindow;

            switch (User.RoleID)
            {
                case (int)Roles.Administrator:
                    NewWindow = new AdministratorWindow();                    
                    break;
                case (int)Roles.User:
                    NewWindow = new UserWindow(EmailTextBox.Text);
                    break;
                default:
                    MessageBox.Show("Такой роли не существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;

            }

            NewWindow.Show();
            this.Close();
           
        }

              
    }
}
