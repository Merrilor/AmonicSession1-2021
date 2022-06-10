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
    /// Interaction logic for AddNewUserWindow.xaml
    /// </summary>
    public partial class AddNewUserWindow : Window
    {
        public AddNewUserWindow(List<string> officeList)
        {
            InitializeComponent();

            officeList.RemoveAt(0);

            OfficeComboBox.ItemsSource = officeList;
        }

        private void CancelButtonClick_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LetterTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            char character = (char)KeyInterop.VirtualKeyFromKey(e.Key);

            if (!char.IsLetter(character) && e.Key != Key.Back)
                e.Handled = true;

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            if (EmailTextBox.Text == string.Empty
                || FirstNameTextBox.Text == string.Empty
                || LastNameTextBox.Text == string.Empty
                || PasswordTextBox.Text == string.Empty)
            {
                MessageBox.Show("Все данные должны быть введены");
                return;
            }

            Session1Entities entities = new Session1Entities();

            if(entities.Users.Where(u=>u.Email == EmailTextBox.Text).Count() > 0)
            {
                MessageBox.Show("Пользователь с таким логином уже существует","Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
                
            }

            string MD5HashString = String.Join("", MD5.Create().ComputeHash(Encoding.Unicode.GetBytes(PasswordTextBox.Text)).Select(hashbyte => hashbyte.ToString("x2")));

            

            Users NewUser = new Users
            {
                ID = entities.Users.Max(u => u.ID) + 1,
                FirstName = FirstNameTextBox.Text,
                LastName = LastNameTextBox.Text,
                OfficeID = OfficeComboBox.SelectedIndex + 1,
                Email = EmailTextBox.Text,
                Birthdate = BirthdatePicker.SelectedDate,
                RoleID = 2,
                Active = true,
                Password = MD5HashString


            };

            entities.Users.Add(NewUser);
            entities.SaveChanges();

            MessageBox.Show("Пользователь успешно добавен");
            this.Close();


        }
    }
}
