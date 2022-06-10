using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for NoLogoutDetectedWindow.xaml
    /// </summary>
    public partial class NoLogoutDetectedWindow : Window
    {

        LoginHistory PreviousLogin;
        Session1Entities Entities;


        public NoLogoutDetectedWindow(LoginHistory previousLogin,Session1Entities entities)
        {
            InitializeComponent();
            PreviousLogin = previousLogin;
            Entities = entities;

            TitleTextBox.Text = $"No Logout detected for your last login on {PreviousLogin.LoginTime.ToShortDateString()} at {PreviousLogin.LoginTime.ToShortTimeString()}";

        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            PreviousLogin.NoLogoutReasonLong = ReasonTextBox.Text;
            PreviousLogin.NoLogoutReasonShort = SoftwareCrashRadioButton.IsChecked == true
                ? SoftwareCrashRadioButton.Content.ToString()
                : SystemCrashRadioButton.Content.ToString();

            Entities.SaveChanges();
           
            this.Close();

        }


        protected override void OnClosing(CancelEventArgs e)
        {
            PreviousLogin.NoLogoutReasonLong = ReasonTextBox.Text;
            PreviousLogin.NoLogoutReasonShort = SoftwareCrashRadioButton.IsChecked == true
                ? SoftwareCrashRadioButton.Content.ToString()
                : SystemCrashRadioButton.Content.ToString();

            Entities.SaveChanges();
            base.OnClosing(e);
        }
    }
}
