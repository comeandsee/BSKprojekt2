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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;

namespace BSK2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LINQtoSQLDataContext dc = new LINQtoSQLDataContext(Properties.Settings.Default.bskaConnectionString);
        public Uzytkownik userLoggedIn;
        public MainWindow()
        {
            InitializeComponent();
          
        }

        public void refreshAuth()
        {
            if(userLoggedIn!=null)
            {
                if (userLoggedIn.ID_roli == 1) //klient
                {
                    klient k = new klient(this);
                    k.Owner = this;
                    k.Show();
                    this.Hide();
                }
                if (userLoggedIn.ID_roli != 1) //kasjerGlowny 2 , kasjer - 3  admin -4
                {
                    cashier k = new cashier(this);
                    k.Owner = this;
                    k.Show();
                    this.Hide();
                }        
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String userName = login.Text;
           // String userName = "Kamil";
            if (login.Text == "")
            {
                MessageBox.Show(" Nie wpisales loginu. ");
            }
            else if (password.Password == "")
            {
                MessageBox.Show(" Nie wpisales hasla. ");
            }
            else
            {
                var getAccount =
                    (from c in dc.Uzytkowniks
                     where c.User_login == userName 
                            && c.Haslo== password.Password
                     select c);

                if(getAccount.Count()!=1)
                {
                    MessageBox.Show(" Konta nie znaleziono ! ");
                }
                else 
                {
                    userLoggedIn = getAccount.First();
                    refreshAuth();

                }
            }

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
          
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //dc.SubmitChanges();

        }
    }
}
