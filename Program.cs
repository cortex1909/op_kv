/* Autor projekta "Police osiguranja": Hrvoje Đaković
   Predmet: Osnove Programiranja
   Ustanova: VSMTI
   Godina: 2019/20 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Data;
using System.Globalization;
using Newtonsoft.Json;
using ConsoleTables;


namespace Polica_Osiguranja
{
    class Program
    {
        //definiranje puteva za JSON datoteke
        static string Config_Path = @"Datoteke\config.json";
        static string Klijent_Path = @"Datoteke\klijent.json";
        static string PoliceOsiguranja_Path = @"Datoteke\policeosiguranja.json";
        //definiranje strukture pojedinih podataka
        public struct User
        {
            public string username { get; set; }
            public string password { get; set; }
        }

        public struct PoliceOsiguranja
        {
            public long OIB { get; set; }
            public int BrojPolice { get; set; }
            public string VrstaOsiguranja { get; set; }
            public string DatumPocetka { get; set; }
            public string DatumIsteka { get; set; }
            public long Vrijednost { get; set;}
        }

        public struct Klijent
        {
            public long OIB { get; set; }
            public string Ime { get; set; }
            public string Prezime { get; set; }
            public string Grad { get; set; }
        }

        public static void ObrisiKonzolu()
        {
            Console.Clear();
        }

        public static bool UserExists = false; //boolean da trenutno korisnik nije logiran u sistem
        static void Main() //dokle god je boolean false - korisnik nije logiran u sistem, te mu vrti ovu petlju koja ga salje u Login
        {
            while (UserExists == false)
            {
                Login();
            }
            Console.ReadKey();
        }
        public static bool Login() //funkcija za login korisnika u sistem
        {
            string sUsername;
            string sPassword;
            Console.WriteLine("Unesite korisnicko ime: ");
            sUsername = Console.ReadLine();
            Console.WriteLine("Unesite lozinku: ");
            sPassword = Console.ReadLine();

            List<User> lUser = DohvatiKorisnike();
            foreach (var User in lUser)
            {
                if (User.username == sUsername && User.password == sPassword)
                {
                    UserExists = true;
                    ObrisiKonzolu();
                    Console.WriteLine("\nPrijava uspjesna!");
                    Radnja();
                } 
            }
            if (!UserExists)
            {
                Console.WriteLine("\nPogresno korisnicko ime ili lozinka!\n");
            }
            return UserExists;
        }
        public static void DohvatiIzbornik() //izbornik
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n ***** GLAVNI IZBORNIK ***** \n");
            Console.ResetColor();
            Console.WriteLine("Odaberite jednu od opcija:");
            Console.WriteLine("1..... Pregled svih polica osiguranja");
            Console.WriteLine("2..... Pretraga klijenata");
            Console.WriteLine("3..... Dodavanje klijenata");
            Console.WriteLine("4..... Dodavanje police osiguranja");
            Console.WriteLine("5..... Odjava");
        }
        public static void Radnja() //nakon logiranja dobivamo Izbornik, te mozemo odabrati koju radnju zelimo dalje izvrsiti
                {
                    string check;
                    int nOdabir;
                    do
                    {
                        DohvatiIzbornik();
                        check = Console.ReadLine();
                        int.TryParse(check, out nOdabir);
                        switch (nOdabir)
                        {
                            case 1:
                                PregledSvihPolicaOsiguranja();
                                break;
                            case 2:
                                PretragaKlijenata();
                                break;
                            case 3:
                                DodavanjeKlijenata();
                                break;
                            case 4:
                                DodavanjePoliceOsiguranja();
                                break;
                            case 5:
                                System.Environment.Exit(0);
                                break;
                            default:
                                Console.WriteLine("\nUnijeli ste opciju koja ne postoji!");
                                break;
                        }
                    }
                    while (nOdabir != 5);
                }
        public static void Kraj() //funkcija koja ako korisnik pritisne ENTER vraca ga u glavni izbornik, a ako korisnik pritisne ESC onda se izlazi iz cijelog programa
        {
            Console.WriteLine("Pritisnite ENTER za povratak na glavni izbornik ili ESC za izlazak iz programa");
            ConsoleKeyInfo key;
            key = Console.ReadKey();
            if (key.Key == ConsoleKey.Escape)
            {
                Environment.Exit(0);
            }
            if (key.Key == ConsoleKey.Enter)
            {
                ObrisiKonzolu();
                Radnja();
            }
        }
        //naredne 3 funkcije -> dohvacanje podataka iz JSON datoteka
        public static List<User> DohvatiKorisnike()
        {
            List<User> lUser = new List<User>();
            StreamReader config = new StreamReader(Config_Path);
            string sJson = "";
            using (config)
            {
                sJson = config.ReadToEnd();
                lUser = JsonConvert.DeserializeObject<List<User>>(sJson);
            }

            return lUser;
        }
        public static List<PoliceOsiguranja> DohvatiPoliceOsiguranja()
        {
            List<PoliceOsiguranja> lPoliceOsiguranja = new List<PoliceOsiguranja>();
            StreamReader policeosiguranja = new StreamReader(PoliceOsiguranja_Path);
            string sJson = "";
            using (policeosiguranja)
            {
                sJson = policeosiguranja.ReadToEnd();
                lPoliceOsiguranja = JsonConvert.DeserializeObject<List<PoliceOsiguranja>>(sJson);
            }
            return JsonConvert.DeserializeObject<List<PoliceOsiguranja>>(File.ReadAllText(PoliceOsiguranja_Path));
        }
        public static List<Klijent> DohvatiKlijente()
        {
            List<Klijent> lKlijent = new List<Klijent>();
            StreamReader klijent = new StreamReader(Klijent_Path);
            string sJson = "";
            using (klijent)
            {
                sJson = klijent.ReadToEnd();
                lKlijent = JsonConvert.DeserializeObject<List<Klijent>>(sJson);
            }
            return JsonConvert.DeserializeObject<List<Klijent>>(File.ReadAllText(Klijent_Path));
        }
        public static void PregledSvihPolicaOsiguranja() //funkcija pomocu koje pristupamo Policama osiguranja, te prikazujemo sve police osiguranja trenutno spremljene u JSON datoteku
        {
            List<PoliceOsiguranja> lPoliceOsiguranja = DohvatiPoliceOsiguranja();
            List<Klijent> lKlijent = DohvatiKlijente();
            ObrisiKonzolu();
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("POPIS POLICA OSIGURANJA: ");
            Console.ResetColor();
            var table = new ConsoleTable("OIB", "Broj Police", "Vrsta Osiguranja", "Ime i prezime", "Datum pocetka", "Datum isteka", "Vrijednost");
            foreach (var PoliceOsiguranja in lPoliceOsiguranja)
            {
                Klijent client = lKlijent.Where(x => x.OIB == PoliceOsiguranja.OIB).FirstOrDefault();
                table.AddRow(PoliceOsiguranja.OIB, PoliceOsiguranja.BrojPolice, PoliceOsiguranja.VrstaOsiguranja, client.Ime + " " + client.Prezime, PoliceOsiguranja.DatumPocetka, PoliceOsiguranja.DatumIsteka, PoliceOsiguranja.Vrijednost);
            }
            table.Write();

            Kraj();
            //lPoliceOsiguranja.Sort((x, y) => x.Vrijednost.CompareTo(y.Vrijednost));
            //sortLista = sortLista.OrderBy(item => item.name).ToList();
        }
        public static void PretragaKlijenata() //funkcija pomocu koje pretrazujemo klijente pomocu OIBa ili punog/dijela prezimena
        {
            Console.WriteLine("------------------------------");
            Console.WriteLine("|       Pretrazivanje        |");
            Console.WriteLine("------------------------------");
            Console.WriteLine("|" + "Pretrazivanje po:" + "\t     |");
            Console.WriteLine("|" + "1 - OIBu klijenta          " + "|");
            Console.WriteLine("|" + "2 - Prezimenu klijenta" + "\t\t     |");
            Console.WriteLine("|" + "99 - Nazad" + "\t\t     |");
            Console.WriteLine("------------------------------");
            Console.WriteLine("Unesite broj:");
            string pIzbornik = Console.ReadLine();
            int nullIzbornik = 0;
            int.TryParse(pIzbornik, out nullIzbornik);
            switch (nullIzbornik)
            {
                case 1:
                    {
                        ObrisiKonzolu();
                        Console.WriteLine("Izabrali ste: Pretrazivanje po OIBu klijenta (1)");
                        Console.WriteLine("Upisite OIB:");
                        string pOIB = Console.ReadLine();
                        long nullOIB = 0;
                        long.TryParse(pOIB, out nullOIB);
                        List<PoliceOsiguranja> lPoliceOsiguranja = DohvatiPoliceOsiguranja();
                        List<Klijent> lKlijent = DohvatiKlijente();
                        int nPomocna = 0;
                        int RedniBr = 1;
                        foreach (var Klijent in lKlijent)
                        {
                            if (Klijent.OIB == nullOIB)
                            {
                            nPomocna = 1;
                            Console.WriteLine("Klijent po OIBu: " + Klijent.OIB + " - " + Klijent.Ime + " " + Klijent.Prezime + ", " + Klijent.Grad);
                            }
                        }
                        
                        Console.WriteLine("Kreirane police osiguranja:");
                        var tablica = new ConsoleTable("R.br. ", "Broj police", "Vrsta osiguranja", "Datum pocetka", "Datum isteka", "Vrijednost");
                        foreach (var PoliceOsiguranja in lPoliceOsiguranja)
                        {
                            if (PoliceOsiguranja.OIB == nullOIB)
                            {
                                nPomocna = 1;
                                tablica.AddRow(RedniBr++ + ".", PoliceOsiguranja.BrojPolice, PoliceOsiguranja.VrstaOsiguranja, PoliceOsiguranja.DatumPocetka, PoliceOsiguranja.DatumIsteka, PoliceOsiguranja.Vrijednost);
                            }
                        }
                        tablica.Write();
                        if (nPomocna == 0)
                        {
                        ObrisiKonzolu();
                        Console.WriteLine("\nNe postoji polica vezana za taj OIB\n");
                        PretragaKlijenata();
                        }
                        break;
                    }
                case 2:
                    {
                        ObrisiKonzolu();
                        List<PoliceOsiguranja> lPoliceOsiguranja = DohvatiPoliceOsiguranja();
                        List<Klijent> lKlijent = DohvatiKlijente();
                        Console.WriteLine("Izabrali ste: Pretrazivanje po prezimenu klijenta (2)");
                        Console.WriteLine("Upisite prezime:");
                        string pPrezime = Console.ReadLine();
                        int nPomocna = 0;
                        int RedniBr = 1;

                        foreach (var Klijent in lKlijent)
                        {
                            if (Klijent.Prezime.Contains(pPrezime))
                            {
                                nPomocna = 1;
                                Console.WriteLine("Klijent po OIBu: " + Klijent.OIB + " - " + Klijent.Ime + " " + Klijent.Prezime + ", " + Klijent.Grad);
                            }
                        }
                        IEnumerable<long> oibList = lKlijent.Where(lk => lk.Prezime.Contains(pPrezime)).Select(o => o.OIB);
                        Console.WriteLine("Kreirane police osiguranja:");
                        var tablica = new ConsoleTable("R.br. ", "Broj police", "Vrsta osiguranja", "Datum pocetka", "Datum isteka", "Vrijednost", "Ime", "Prezime", "OIB");
                        //lPoliceOsiguranja.Sort((x, y) => x.Vrijednost.CompareTo(y.Vrijednost));
                        foreach (PoliceOsiguranja policeOsiguranja in lPoliceOsiguranja)
                        {
                            
                            if (oibList.Contains(policeOsiguranja.OIB))
                            {
                                nPomocna = 1;
                                Klijent client = lKlijent.Where(x => x.OIB == policeOsiguranja.OIB).FirstOrDefault();
                                tablica.AddRow(RedniBr++ + ".", policeOsiguranja.BrojPolice, policeOsiguranja.VrstaOsiguranja, policeOsiguranja.DatumPocetka, policeOsiguranja.DatumIsteka, policeOsiguranja.Vrijednost, client.Ime, client.Prezime, client.OIB);
                            }
                        }
                        tablica.Write();
                        if (nPomocna == 0)
                        {
                            ObrisiKonzolu();
                            Console.WriteLine("\nNe postoji polica vezana za to prezime\n");
                            PretragaKlijenata();
                        }
                        break;
                    }
                case 99:
                    {
                        Console.WriteLine("Izabrali ste: Nazad");
                        ObrisiKonzolu();
                        Radnja();
                        break;
                    }
                default: ObrisiKonzolu(); Console.WriteLine("Krivi broj unesen!"); PretragaKlijenata(); break;
            }
        }

        public static void DodavanjeKlijenata() //funkcija za dodavanje novih klijenata u JSON datoteku
        {

            Console.Write("Unesite OIB klijenta: ");
            string pOIB = Console.ReadLine();
            long nullOIB = 0;
            long.TryParse(pOIB, out nullOIB);
            Console.Write("Unesite ime klijenta: ");
            string ime1 = Console.ReadLine();
            Console.Write("Unesite prezime klijenta: ");
            string prezime1 = Console.ReadLine();
            Console.Write("Unesite grad klijenta: ");
            string grad1 = Console.ReadLine();
            List<Klijent> lKlijent = DohvatiKlijente();
            Klijent dKlijent = new Klijent()
            {
                OIB = nullOIB,
                Ime = ime1,
                Prezime = prezime1,
                Grad = grad1
            };

            lKlijent.Add(dKlijent);

            var jsonString = JsonConvert.SerializeObject(lKlijent, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(Klijent_Path, jsonString);

            Kraj();

        }

        public static void DodavanjePoliceOsiguranja() //funkcija za dodavanje novih polica osiguranja u JSON datoteku
        {
            Random rnd = new Random();
            int brojpolice = rnd.Next();
            Console.Write("Unesite OIB klijenta: ");
            string pOIB = Console.ReadLine();
            long nullOIB = 0;
            long.TryParse(pOIB, out nullOIB);
            Console.Write("Unesite vrstu osiguranja (Stambeno/Zivotno/Vozilo): ");
            string VrstaOsiguranja2 = Console.ReadLine();
            Console.Write("Unesite Vrijednost police osiguranja: ");
            string Vrijednost2 = Console.ReadLine();
            long nullVrijednost2 = 0;
            long.TryParse(Vrijednost2, out nullVrijednost2);
            DateTime now = DateTime.Today;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n Datum od kojeg pocinje osiguravajuca polica: {0}\n", now.ToString("dd/MM/yyyy"));
            Console.ResetColor();
            string datump = now.ToString("dd/MM/yyyy");
            DateTime DatumPocetka = DateTime.Parse(datump);
            DateTime DatumKraja = DatumPocetka.AddYears(1);
            string datumi = DatumKraja.ToString("dd/MM/yyyy");
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n Datum kojeg istice osiguravajuca polica: {0}\n", datumi);
            Console.ResetColor();

            List<PoliceOsiguranja> lPoliceOsiguranja = DohvatiPoliceOsiguranja();
            PoliceOsiguranja dPoliceOsiguranja = new PoliceOsiguranja()
            {
            OIB = nullOIB,
            BrojPolice = brojpolice,
            VrstaOsiguranja = VrstaOsiguranja2,
            DatumPocetka = datump,
            DatumIsteka = datumi,
            Vrijednost = nullVrijednost2
            };

            lPoliceOsiguranja.Add(dPoliceOsiguranja);

            var jsonString = JsonConvert.SerializeObject(lPoliceOsiguranja, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(PoliceOsiguranja_Path, jsonString);

            Kraj();

        }
    }
}
 