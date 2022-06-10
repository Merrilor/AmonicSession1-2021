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
using System.Data.Entity;

namespace Session1.Windows
{
    /// <summary>
    /// Interaction logic for AdministratorWindow.xaml
    /// </summary>
    ///
    public partial class AdministratorWindow : Window
    {

        List<User> UserList = new List<User>();

        List<User> FilteredList = new  List<User>();

        List<string> OfficeList = new List<string>();

        Func<User, bool> CurrentFilter = x => true;

        public AdministratorWindow()
        {
            InitializeComponent();

            SelectUsers();

            Session1Entities entities = new Session1Entities();

            OfficeList.Add("All offices");
            OfficeList.AddRange(entities.Offices.Select(o => o.Title).ToList());
            OfficeComboBox.ItemsSource = OfficeList;
            OfficeComboBox.SelectedIndex = 0;

            FilteredList = UserList;

            UserDataGrid.ItemsSource = FilteredList;
        }

       

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            LoginWindow window = new LoginWindow();
            window.Show();
            this.Close();
        }

        private void OfficeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (Convert.ToString(e.AddedItems[0]) == "All offices")
            {
                CurrentFilter = x => true;
            }
            else
            {
                CurrentFilter = x => x.Office == Convert.ToString(e.AddedItems[0]);
            }


            UpdateFilterList();

        }

        private void UpdateFilterList()
        {
            FilteredList = UserList.Where(CurrentFilter).ToList();
            UserDataGrid.ItemsSource = FilteredList;
        }

        private void EnableDisableLoginButtonClick_Click(object sender, RoutedEventArgs e)
        {
            if (UserDataGrid.SelectedItem is null)
                return;

            User SelectedUser = (User)UserDataGrid.SelectedItem;
            FilteredList[UserDataGrid.SelectedIndex].Active = FilteredList[UserDataGrid.SelectedIndex].Active == true ? false : true;

            Session1Entities entities = new Session1Entities();

            Users UserToChange = entities.Users.Where(u => u.ID == SelectedUser.Id).Single();
            
            UserToChange.Active = UserToChange.Active == true ? false : true;

            entities.SaveChanges();

            UpdateFilterList();


        }

        private void SelectUsers()
        {
            Session1Entities entities = new Session1Entities();
            DateTime ServerTime = DatabaseHelper.GetServerTime();

            UserList = (from u in entities.Users
                        from r in entities.Roles
                        from o in entities.Offices
                        where u.RoleID == r.ID && u.OfficeID == o.ID
                        select new User
                        {
                            Id = u.ID,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Age = (int)DbFunctions.DiffYears(u.Birthdate, ServerTime),
                            EmailAddress = u.Email,
                            UserRole = r.Title,
                            Office = o.Title,
                            Active = (bool)u.Active


                        }).ToList();


            UserDataGrid.ItemsSource = UserList;
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewUserWindow window = new AddNewUserWindow(OfficeList);
            window.ShowDialog();

            SelectUsers();
            


        }

        private void ChangeRoleButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserDataGrid.SelectedItem is null)
                return;

            ChangeRoleWindow window = new ChangeRoleWindow((User)UserDataGrid.SelectedItem, OfficeList);
            window.ShowDialog();

            SelectUsers();

        }
    }

}


public class User
{
    public int Id { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    public int Age { get; set; }

    public string UserRole { get; set; }

    public string EmailAddress { get; set; }

    public string Office { get; set; }

    public bool Active { get; set; }
}
