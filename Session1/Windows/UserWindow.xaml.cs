using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
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
    /// Interaction logic for UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {

        Users CurrentUser { get; set; }

        LoginHistory PreviousLogin;


        List<UserInfo> UserInfoList;

        public UserWindow(string UserEmail)
        {
            InitializeComponent();

            Session1Entities entities = new Session1Entities();

            CurrentUser = entities.Users
                .Where(u => u.Email == UserEmail)
                .Single();
            
            
            PreviousLogin = entities.LoginHistory
                .Where(lh => lh.userId == CurrentUser.ID)
                .OrderByDescending(lh => lh.Id)
                .First();

            if(PreviousLogin.LogoutTime is null && PreviousLogin.NoLogoutReasonShort is null)
            {
                NoLogoutDetectedWindow window = new NoLogoutDetectedWindow(PreviousLogin,entities);
                window.ShowDialog();
            }


            WelcomeMessageTextBox.Text = $"Hi {CurrentUser.FirstName}, Welcome to AMONIC Airlines.";
            CrashesTextBox.Text = $"Number of crashes {entities.LoginHistory.Where(lh => lh.userId == CurrentUser.ID && lh.LogoutTime == null).Count()}";


            int? SecondsSpentOnSystem = entities.LoginHistory
                .Where(lh => lh.userId == CurrentUser.ID && lh.LogoutTime != null)
                .Select(lh => DbFunctions.DiffSeconds(lh.LoginTime, lh.LogoutTime))
                .Sum();


            TimeSpentTextBox.Text = $"Time spent on system: { TimeSpan.FromSeconds(SecondsSpentOnSystem ?? 0)} ";

            UserInfoList = entities.LoginHistory.Where(lh => lh.userId == CurrentUser.ID).AsEnumerable().Select(lh => new UserInfo
            {
                Date = lh.LoginTime.Date,
                LoginTime = lh.LoginTime.TimeOfDay,
                LogoutTime = lh.LogoutTime == null ? "*" : lh.LoginTime.ToString("T"),
                FailedLogoutReason = lh.NoLogoutReasonShort + "," + lh.NoLogoutReasonLong,
                TimeSpent = lh.LogoutTime == null ? new TimeSpan(0) : (lh.LogoutTime - lh.LoginTime)


            }).ToList();

            UserInfoDatagrid.ItemsSource = UserInfoList;

            //Add login time
            DateTime LoginTime = DatabaseHelper.GetServerTime();
            entities.LoginHistory.Add(new LoginHistory
            {

                userId = CurrentUser.ID,
                LoginTime = LoginTime

            });
            entities.SaveChanges();

            




        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            LoginWindow window = new LoginWindow();
            window.Show();
            this.Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Session1Entities entities = new Session1Entities();

            LoginHistory CurrentLogin = entities.LoginHistory.Where(lh => lh.userId == CurrentUser.ID).OrderByDescending(lh => lh.Id).First();
            CurrentLogin.LogoutTime = DatabaseHelper.GetServerTime();
            entities.SaveChanges();

            base.OnClosing(e);

        }


        public class UserInfo
        {
            public DateTime Date { get; set; }

            public TimeSpan LoginTime { get; set; }

            public string LogoutTime { get; set; }

            public TimeSpan? TimeSpent { get; set; }

            public string FailedLogoutReason { get; set; }

        }

    }
}
