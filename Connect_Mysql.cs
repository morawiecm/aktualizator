using System;
using System.Windows.Forms;
using System.IO;
using MySql.Data.MySqlClient;


namespace PocztaSpecjalnaStart
{

    public class Connect_Mysql
    {
        public MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        private string connectionString;
        string wersja;

        public Connect_Mysql()
        {


            Initialize1();

        }//koniec public Connect_Mysql

        private void Initialize1()
        {
            FileStream plik = new FileStream(@"C:\Program\Konfiguracja\utill_start.txt", FileMode.Open);
            StreamReader odczyt = new StreamReader(plik);
            string wiersz = odczyt.ReadLine();

            string[] dziel = wiersz.Split(new string[] { " - " }, StringSplitOptions.None);
            wersja = dziel[0];

            odczyt.Close();
            plik.Close();



            server = wersja;
            database = "policja";
            uid = "policja";
            password = "policja";
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
                   database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";charset=utf8";

            connection = new MySqlConnection(connectionString);
        }//koniec Initialize

        private void Initialize()
        {
            server = "127.0.0.1";
            database = "policja";
            uid = "root";
            password = "";
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
                   database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";charset=utf8";

            connection = new MySqlConnection(connectionString);
        }//koniec Initialize

        public bool OpenConnection(Label labelek)
        {


            try
            {

                connection.Open();

                return true;
            }
            catch (MySqlException ex)
            {

                switch (ex.Number)
                {
                    case 1042:
                        labelek.Text = "Brak połączenia z bazą";
                        break;

                    case 0:

                        labelek.Text = "Błędny login lub hasło root";
                        break;

                }
                return false;
            }
        }//koniec OpenConnection

        public void CloseConnection()
        {
            try
            {
                connection.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }//koniec CloseConnection


    }

}
