using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for ChangeRoleWindow.xaml
    /// </summary>
    public partial class ChangeRoleWindow : Window
    {
        User SelectedUser;

        enum Role
        {
            Administrator = 1,
            User = 2
        }

        public ChangeRoleWindow(User selectedUser, List<string> officeList)
        {
            InitializeComponent();
            SelectedUser = selectedUser;

            EmailTextBox.Text = SelectedUser.EmailAddress;
            FirstNameTextBox.Text = SelectedUser.FirstName;
            LastNameTextBox.Text = SelectedUser.LastName;

            OfficeComboBox.Text = SelectedUser.Office;

            if(SelectedUser.UserRole == "User")
            {
                UserRoleButton.IsChecked = true;
            }
            else if(SelectedUser.UserRole == "Administrator")
            {
                AdministratorRoleButton.IsChecked = true;
            }
           


        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Session1Entities entities = new Session1Entities();

            Users UserToChange = entities.Users.Where(u => u.Email == SelectedUser.EmailAddress).Single();

            if(UserRoleButton.IsChecked == true)
            {
                UserToChange.RoleID = (int)Role.User;

            }else if(AdministratorRoleButton.IsChecked == true){

                UserToChange.RoleID = (int)Role.Administrator;
            }

            entities.SaveChanges();

            MessageBox.Show("Роль изменена");

            this.Close();

        }

        private void CancelButtonClick_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
