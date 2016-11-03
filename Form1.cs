using System;
using System.Windows.Forms;
using System.IO;
using MySql.Data.MySqlClient;
using System.Net;
using ICSharpCode.SharpZipLib.Zip;

namespace PocztaSpecjalnaStart
{
    //plik update.zip umieszczasz na ftpie 192.168.0.240 l:marcin h:mk1234
    //format pliku zip - masz wzór
    public partial class Form1 : Form
    {
        public Connect_Mysql conn;
        int wersja, wersja_baza;
        string serverek;
        MySqlDataReader reader = null;

        public Form1()
        {
            InitializeComponent();
            button1.Enabled = false;
            pobierz();//pobierz wersje bazy z mysql
            sprawdz();//sprawdz z plikiem lokalnym n akompie
            if (wersja != wersja_baza) update_program();
            timer1.Start();


        }

        private void update_program()
        {

            FileStream plik = new FileStream(@"C:\Program\Konfiguracja\utill_aktualizacja.txt", FileMode.Open);
            StreamReader odczyt = new StreamReader(plik);
            string wiersz = odczyt.ReadLine();

            string[] dziel = wiersz.Split(new string[] { " - " }, StringSplitOptions.None);
            serverek = dziel[0];

            odczyt.Close();
            plik.Close();



            string root = @"C:\Program\Update";
                // If directory does not exist, don't even try 
                if (!Directory.Exists(root))
                {
                    Directory.CreateDirectory(root);
                }

            string inputfilepath = @"C:\Program\update.zip";
            string ftphost = serverek;
            string ftpfilepath = "/program/update/update.zip";

            string ftpfullpath = "ftp://" + ftphost + ftpfilepath;

            using (WebClient request = new WebClient())
            {
                request.Credentials = new NetworkCredential("marcin", "mk1234");
                byte[] fileData = request.DownloadData(ftpfullpath);

                using (FileStream file = File.Create(inputfilepath))
                {
                    file.Write(fileData, 0, fileData.Length);
                    file.Close();
                }
              //  MessageBox.Show("Download Complete");
            }

            

            root = @"c:\Program\Bin";
            clearFolder(root);

            root = @"c:\Program\Sources";
            clearFolder(root);

            root = @"c:\Program\Update";
            clearFolder(root);

            //    ZipFile.ExtractToDirectory(inputfilepath, @"C:\Program\");



            var zipFileName = @"C:\Program\update.zip";
            var targetDir = @"c:\Program";
            FastZip fastZip = new FastZip();
            string fileFilter = null;

            // Will always overwrite if target filenames already exist
            fastZip.ExtractZip(zipFileName, targetDir, fileFilter);

            string source = @"c:\Program\Update\Bin";
            string target = @"c:\Program\Bin";

            DirectoryInfo diSource = new DirectoryInfo(source);
            DirectoryInfo ditarget = new DirectoryInfo(target);
            CopyFilesRecursively(diSource, ditarget);

            source = @"c:\Program\Konfiguracja";
            target = @"c:\Program\Bin";

            diSource = new DirectoryInfo(source);
            ditarget = new DirectoryInfo(target);
            CopyFilesRecursively(diSource, ditarget);

            source = @"c:\Program\Update\Sources";
             target = @"c:\Program\Sources";

            diSource = new DirectoryInfo(source);
            ditarget = new DirectoryInfo(target);
            CopyFilesRecursively(diSource, ditarget);
            uaktualnij_wersje();
        }

        private void uaktualnij_wersje()
        {
            

            if (File.Exists(@"C:\Program\Konfiguracja\konfiguracja.txt"))
                File.Delete(@"C:\Program\Konfiguracja\konfiguracja.txt");

            StreamWriter plik = new StreamWriter(@"C:\Program\Konfiguracja\konfiguracja.txt");
            plik.Write(wersja_baza.ToString() + " - wersja programu");
            plik.Close();
        }

        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            foreach (FileInfo file in source.GetFiles())
                file.CopyTo(Path.Combine(target.FullName, file.Name));
        }

        private void clearFolder(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                clearFolder(di.FullName);
                di.Delete();
            }
        }


        private void pobierz()
        {
            FileStream plik = new FileStream(@"C:\Program\Konfiguracja\konfiguracja.txt", FileMode.Open);
            StreamReader odczyt = new StreamReader(plik);
            string wiersz = odczyt.ReadLine();

            string[] dziel = wiersz.Split(new string[] { " - " }, StringSplitOptions.None);
            wersja = Int16.Parse(dziel[0]);

            odczyt.Close();
            plik.Close();
        }

        private void sprawdz()
        {
            string sel = "SELECT Wersja FROM wersja_programu";
            conn = new Connect_Mysql();
            conn.OpenConnection(label2);
            MySqlCommand cmd = new MySqlCommand(sel, conn.connection);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                wersja_baza = reader.GetInt16(0);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (wersja != wersja_baza) progressBar1.Increment(+1);
            else progressBar1.Increment(+50);
            if (progressBar1.Value == 100) button1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            conn.CloseConnection();
            System.Diagnostics.Process.Start(@"c:\Program\Bin\SIPS.exe");
            ActiveForm.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }
    }
}
