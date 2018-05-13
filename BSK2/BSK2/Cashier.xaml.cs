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

namespace BSK2
{
    /// <summary>
    /// Interaction logic for cashier.xaml
    /// </summary>
    public partial class cashier : Window
    {
        LINQtoSQLDataContext dc = new LINQtoSQLDataContext(Properties.Settings.Default.bskaConnectionString);
        public Uzytkownik userLoggedIn;
        public String rola;
        MainWindow loginWindow;
       
        public cashier()
        {
            InitializeComponent();

        }

        public cashier(MainWindow loginWindow)
        {
            InitializeComponent();
            this.loginWindow = loginWindow;
            userLoggedIn = loginWindow.userLoggedIn;
           var rolaUzytkownika = 
                    (from r in dc.Rolas
                    where r.ID == userLoggedIn.ID_roli
                     select r);
            rola = rolaUzytkownika.First().Nazwa;

            refreshAuth();
            refreshWyp();

        }

        public void refreshWyp()
        {
            var wypList =
               (from f in dc.Fakturas
                join w in dc.Wypozyczenies on f.ID equals w.ID_Faktury
                join u in dc.Uzytkowniks on f.ID_user equals u.ID
                where u.ID == userLoggedIn.ID
                select w).ToList();
            wypData.ItemsSource = wypList;
        }

       
        public void dodajWypozyczenie()
        {
            var client = (from c in dc.Uzytkowniks
                          where c.Imie == userRentCashier.Text
                              && c.Nazwisko == userRentCashierSurname.Text
                          select c);
           
            if (dataRentCashier.SelectedDate == null || dataReturnCashier.SelectedDate == null)
            {
                MessageBox.Show(" Wybierz daty ! ");
            }
            else if(PriceCashier.Text =="" ||  karaBox.Text =="" || statusWyp.Text == ""
                     || idPlay.Text == "" || discountCashier.Text == "" ||  userRentCashier.Text == "" 
                     || userRentCashierSurname.Text == "" || idPlay.Text == "0" || PriceCashier.Text == "0")
            {
                MessageBox.Show(" Nie uzupelniles wszytskich pol ! ");
            }
            else if (client.Count() != 1 )
            {
                MessageBox.Show(" Nie ma takiego uzytkownika ");
            }
            else
            {

                DateTime date1 = (DateTime)dataRentCashier.SelectedDate;
                DateTime date2 = (DateTime)dataReturnCashier.SelectedDate;

                int hajs = int.Parse(PriceCashier.Text);
                int kara = int.Parse(karaBox.Text);
                String status = statusWyp.Text;

                int idGry = int.Parse(idPlay.Text);
                int rabat = int.Parse(discountCashier.Text);

                Faktura f = new Faktura
                {
                    Data_wystawienia = date1,
                    ID_user = userLoggedIn.ID,
                    Classification = 2,
                };
                dc.Fakturas.InsertOnSubmit(f);
                dc.SubmitChanges();

                Wypozyczenie wyp = new Wypozyczenie
                {
                    Data_wypozyczenia = (DateTime)date1,
                    Data_zwrotu = (DateTime)date2,
                    Cena = hajs,
                    Kara = kara,
                    Status_Wypo = status,
                    ID_Faktury = f.ID,
                    Classification = 2,
                };
                dc.Wypozyczenies.InsertOnSubmit(wyp);
                dc.SubmitChanges();

                Wypo_user wu = new Wypo_user
                {
                    ID_user = client.First().ID,
                    ID_wypo = wyp.ID,
                    Classification = 2,
                };
                dc.Wypo_users.InsertOnSubmit(wu);
                dc.SubmitChanges();

                Wypo_gra wyp_gra = new Wypo_gra
                {
                    ID_gry = idGry,
                    ID_wypo = wyp.ID,
                    Classification = 2,
                    Rabat = rabat
                };
                dc.Wypo_gras.InsertOnSubmit(wyp_gra);
                dc.SubmitChanges();
                MessageBox.Show(" Dodano wypożyczenie i fakture ! ");
            }

           
        }

        public void dodajUprawnienia()
        {
            var user = (from c in dc.Uzytkowniks
                          where c.Imie == nameCashier1.Text
                              && c.Nazwisko == surnameCashier1.Text
                          select c);
            var usID = user.First().ID;

            if (nameCashier1.Text == "" || surnameCashier1.Text == "" || statusWyp1.Text == "")
            {
                MessageBox.Show(" Nie uzupelniles wszytskich pol ! ");
            }
            else if (user.Count() != 1)
            {
                MessageBox.Show(" Nie ma takiego uzytkownika. ");
            }
            else
            {
                var rolaDoZmiany =
                   (from p in dc.Rolas
                    where p.Nazwa == statusWyp1.Text
                    select p);
                var idRoliDoZmiany = rolaDoZmiany.First().ID;


                (from p in dc.Uzytkowniks
                 where p.ID == usID
                 select p).ToList().ForEach(x => x.ID_roli = idRoliDoZmiany);
                dc.SubmitChanges();
                MessageBox.Show("Rola została zmieniona!");
            }


        }

        public void wpiszStale()
        {
            var idFac = dc.Fakturas.OrderByDescending(p => p.ID).First().ID;
            idFac = idFac + 1;
            idFacture.Text = idFac.ToString();

            idCashier.Text = (userLoggedIn.ID).ToString();

            var idRen = dc.Wypozyczenies.OrderByDescending(p => p.ID).First().ID;
            idRen = idRen + 1;
            idRentCashier.Text = idRen.ToString();
        }

       

        

        public void refreshAuth()
        {
            wpiszStale();

        }

        public void pokazDaneUzytkownika()
        {
            var user = (from c in dc.Uzytkowniks
                        where c.Imie == nameCashier.Text
                            && c.Nazwisko == surnameCashier.Text
                        select c);

            if (nameCashier.Text == "" || surnameCashier.Text == "")
            {
                MessageBox.Show(" Nie uzupelniles wszytskich pol ! ");
            }
            else if (user.Count() != 1)
            {
                MessageBox.Show(" Nie ma takiego uzytkownika. ");
            }
            else
            {
                var usID = user.First().ID;
                var rolaZalogowanegoUzytkownika =
                       (from r in dc.Rolas
                        where r.ID == userLoggedIn.ID_roli
                        select r);
               // var idrolaZalogowanegoUzytkownika = rolaZalogowanegoUzytkownika.First().ID;
                var nazwaRolaZalogowanegoUzytkownika = rolaZalogowanegoUzytkownika.First().Nazwa;
                var clearanceZalogowanegoUzytkownika = rolaZalogowanegoUzytkownika.First().Clearance;

                var poziomUfnosciZalogowanegoUzytkownika =
                   (from p in dc.PoziomUfnoscis
                    where p.ID == clearanceZalogowanegoUzytkownika
                    select p);
                var idPoziomuUfnosciZalogowanegoUzytkownika = poziomUfnosciZalogowanegoUzytkownika.First().ID;
                var nazwaPoziomUfnosciZalogowanegoUzytkownika = poziomUfnosciZalogowanegoUzytkownika.First().Nazwa;

                var rolaUzytkownikaSzukanego =
                        (from r in dc.Rolas
                         where r.ID == user.First().ID_roli
                         select r);
                var nazwarolaUzytkownikaSzukanego = rolaUzytkownikaSzukanego.First().Nazwa;


                var poziomUfnosciDanychUzytkownikaSzukanego =
                        (from r in dc.PoziomUfnoscis
                         where r.ID == user.First().Classification
                         select r);
                var idPoziomUfnosciDanychUzytkownikaSzukanego = poziomUfnosciDanychUzytkownikaSzukanego.First().ID;
                var nazwaPoziomUfnosciDanychUzytkownikaSzukanego = poziomUfnosciDanychUzytkownikaSzukanego.First().Nazwa;


                if (idPoziomuUfnosciZalogowanegoUzytkownika >= idPoziomUfnosciDanychUzytkownikaSzukanego) //jezeli mamy uprawnienia
                {

                    //wypisywanie stalych
                    nameCashier2.Text = user.First().Imie;
                    surnameCashier2.Text = user.First().Nazwisko;
                    phoneNoCashie2r.Text = user.First().Nr_telefonu;
                    loginCashier2.Text = user.First().User_login;

                    rolaCashier2.Text = nazwarolaUzytkownikaSzukanego;
                    trustCashier2.Text = nazwaPoziomUfnosciDanychUzytkownikaSzukanego;
                }
                else
                {
                    MessageBox.Show("Nie masz uprawnien do odczytu danych." +
                        " Musisz miec rolę, której poziom ufności jest równy lub większy od poziomu ufności danych użytkownika, którego szukasz." +
                        "Twoja rola , to: " + nazwaRolaZalogowanegoUzytkownika +
                        " wiec twoj poziom ufnosci, to: " + nazwaPoziomUfnosciZalogowanegoUzytkownika );
                }


            }
        }

            private void Button_Click(object sender, RoutedEventArgs e)
        {
            userLoggedIn = null;
            loginWindow.Show();
            this.Hide();
        }

        private void addGame_Click(object sender, RoutedEventArgs e)
        {

        }

        private void rentGam_button_Click(object sender, RoutedEventArgs e)
        {
            dodajWypozyczenie();
            refreshWyp();
            refreshAuth();
        }

        private void userRentCashier_Copy_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void idFacture_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
 
            if (rola.ToString() == "admin")
            {
                var idFac = dc.Fakturas.OrderByDescending(p => p.ID).First().ID;

                (from p in dc.Fakturas
                 where p.ID == idFac
                 select p).ToList().ForEach(x => x.ID_user = int.Parse(idCashier.Text));
                dc.SubmitChanges();
                MessageBox.Show("ID pracownika zmienione!");

            }
            if (rola.ToString() == "kasjer" || rola.ToString() == "kasjerGlowny")
            {
                MessageBox.Show("Nie masz uprawnien, aby zmienic ID poprosc wlasciciela o dostep admina.");
            }
        }

        private void buttonRegister1_Click(object sender, RoutedEventArgs e)
        {
            
            if (rola.ToString() == "admin")
            {
                dodajUprawnienia(); 
            }
            if (rola.ToString() == "kasjer" || rola.ToString() == "kasjerGlowny")
            {
                MessageBox.Show("Nie masz uprawnien, aby zmienic ID poprosc wlasciciela o dostep admina.");
            }

        }

        private void buttonRegister_Click(object sender, RoutedEventArgs e)
        {
            pokazDaneUzytkownika();
        }

        private void Moje_konto(object sender, RoutedEventArgs e)
        {
            klient k = new klient(this.loginWindow);
            k.Owner = this.loginWindow;
            k.Show();
            this.Hide();
        }
    }
    }
