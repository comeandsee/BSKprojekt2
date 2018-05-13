using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace BSK2
{
    /// <summary>
    /// Interaction logic for klient.xaml
    /// </summary>
    public partial class klient : Window
    {
        LINQtoSQLDataContext dc = new LINQtoSQLDataContext(Properties.Settings.Default.bskaConnectionString);
        public Uzytkownik userLoggedIn;
        MainWindow loginWindow;
        public String rola;
        public klient()
        {
            InitializeComponent();
        }

        public klient(MainWindow loginWindow)
        {
            InitializeComponent();
            this.loginWindow = loginWindow;
            userLoggedIn = loginWindow.userLoggedIn;
            var rolaUzytkownika =
                   (from r in dc.Rolas
                    where r.ID == userLoggedIn.ID_roli
                    select r);
            rola = rolaUzytkownika.First().Nazwa;

          
            if (rola.ToString() == "klient")
            {
                PanelButton.Visibility = Visibility.Hidden;
            }

                var rentalList =
                (from wu in dc.Wypo_users
                    join w in dc.Wypozyczenies on wu.ID_wypo equals w.ID
                    join u in dc.Uzytkowniks on wu.ID_user equals u.ID
                     where wu.ID_user == userLoggedIn.ID
                    select w).ToList();

            var user =
                    (from c in dc.Uzytkowniks
                     where c.ID == userLoggedIn.ID
                     select c);

            name.Text = userLoggedIn.Imie;
            surname.Text = userLoggedIn.Nazwisko;
            phoneNr.Text = userLoggedIn.Nr_telefonu;
            login.Text = userLoggedIn.Imie;
            password.Password = userLoggedIn.Haslo;
            password2.Password = userLoggedIn.Haslo;

            userData.ItemsSource = user;
            history.ItemsSource = rentalList;

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (from p in dc.Uzytkowniks
             where p.ID == userLoggedIn.ID
             select p).ToList().ForEach(x => x.Imie = name.Text);

            (from p in dc.Uzytkowniks
             where p.ID == userLoggedIn.ID
             select p).ToList().ForEach(x => x.Nazwisko = surname.Text);

            (from p in dc.Uzytkowniks
             where p.ID == userLoggedIn.ID
             select p).ToList().ForEach(x => x.Nr_telefonu = phoneNr.Text);

            (from p in dc.Uzytkowniks
             where p.ID == userLoggedIn.ID
             select p).ToList().ForEach(x => x.User_login = login.Text);

            if (password.Password == password2.Password)
            {
                (from p in dc.Uzytkowniks
                 where p.ID == userLoggedIn.ID
                 select p).ToList().ForEach(x => x.Haslo = password.Password);
                //  dc.SubmitChanges();

            }
            else
            {
                MessageBox.Show(" Haslo i powtorz haslo nie sa ze soba zgodne. Wpisz jeszcze raz. Pozostale dane zostaly zmienione ");
            }

            MessageBox.Show(" Dane zostały zmienione ! ");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            userLoggedIn = null;
            loginWindow.Show();
            this.Hide();
        }

        private void Worker_Panel(object sender, RoutedEventArgs e)
        {
            cashier k = new cashier(this.loginWindow);
            k.Owner = this.loginWindow;
            k.Show();
            this.Hide();
        }
    }
}
