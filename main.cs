using System;
using System.IO;
using System.Globalization;

class MainClass {
  static bool validan = true;
  static void Ucitavanje_podataka (ref string[,] matrica)
  {
    if (File.Exists("ulazni_podaci.csv"))
    {
      StreamReader podaci = new StreamReader("ulazni_podaci.csv");
      int brojac=0;
      string s = podaci.ReadLine();
      while (!podaci.EndOfStream)      
      {
        s = podaci.ReadLine();
        string[] elementi = s.Split(";");
        if (elementi.Length!=6) 
        {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.Error.WriteLine($"Greska! Nije unet dobar broj podataka o filmu sa id {(brojac)}");
          validan = false;
          break;
        }
        for (int i=0; i<6; i++)
        {
          matrica[brojac,i] = elementi[i];
        }
        brojac++;
      }
      podaci.Close();
    }
    else 
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.Error.WriteLine("Greska! Ne postoji datoteka ulazni_podaci.csv");
    }
  }

  ////////////////////////////////////////////////////////
  //A metoda
  struct Podaci_o_filmovima_rezisera
  {
    public string reziser;
    public string[] filmovi;
    public double[] zarade;
    public double ukupna_zarada;
  }

  //Metode za unos datuma
  static void Izbacivanje_razmaka_datum (ref string datum) //izbacivanje viska razmaka iz datuma
  {
    string datum1 = "";
    foreach (char c in datum)
      if (' '!=c) datum1+=c;
    datum = datum1;
  }
  
  static void Izbacivanje_tacke_datum (ref string datum) //izbacivanje tacki na kraju godine
  {
    string datum1 = "";
    foreach (char c in datum)
      if ('.'!=c) datum1+=c;
    datum = datum1;
  }

  static string[] dozvoljeni_formati = {"d/MM/yyyy","dd/MM/yyyy","dd/M/yyyy","d/M/yyyy","d.MM.yyyy","dd.MM.yyyy","dd.M.yyyy","d.M.yyyy","d.MM.yyyy.","dd.MM.yyyy.","dd.M.yyyy.","d.M.yyyy."};

  static DateTime Unos_datuma(string pocetni_krajnji)
  {
    string unet_datum = Console.ReadLine();
    Izbacivanje_razmaka_datum(ref unet_datum);
    DateTime konvertovan_datum;
    while (!DateTime.TryParseExact(unet_datum, dozvoljeni_formati, null, DateTimeStyles.None, out konvertovan_datum))
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.Error.WriteLine("Neispravan unos datuma. Poku??ajte ponovo.");
      Console.ForegroundColor = ConsoleColor.White;
      Console.Write($"Unesite {pocetni_krajnji} datum: ");
      unet_datum = Console.ReadLine();
      Izbacivanje_razmaka_datum(ref unet_datum);
    }
    return konvertovan_datum;
  }

  static DateTime[] Unos_perioda ()
  {
    DateTime[] datumi = new DateTime[2];
    Console.WriteLine("Unesite period za koji se obra??uju podaci. Datumi se unose u formatu: dan.mesec.godina ili dan/mesec/godina.");
    Console.WriteLine("Filmovi iz datoteke su izdavani u periodu od 14.1.2000. do 11.12.2020. godine.");
    Console.Write("Unesite po??etni datum: ");
    string pocetni_krajnji = "po??etni";
    datumi[0] = Unos_datuma(pocetni_krajnji);
    Console.Write("Unesite krajnji datum: ");
    pocetni_krajnji = "krajnji";
    datumi[1] = Unos_datuma(pocetni_krajnji);
    return datumi;
  }

  //Metoda za proveru da li datum ulazi u zadati period
  static bool Ulazi_u_period (string datum_iz_matrice, DateTime[] niz_period)
  {
    Izbacivanje_razmaka_datum(ref datum_iz_matrice);
    DateTime konvertovan_datum;
    DateTime.TryParseExact(datum_iz_matrice, dozvoljeni_formati, null, DateTimeStyles.None, out konvertovan_datum);
    if (konvertovan_datum>=niz_period[0] && konvertovan_datum<=niz_period[1]) return true;
    return false;
  }

   static double IzbacivanjeDolara (string s)
  {
    string S = "";
    for(int i=0; i<s.Length; i++)
    {
      if (i != 0)
      {
        S += s[i];
      }
    }
    double zarada_jednog_filma = Convert.ToDouble(S);
    return zarada_jednog_filma;
  }

  //Metoda vraca indeks vec unetog rezisera, koji se trazi
  static int Indeks_trazenog_rezisera (string reziser,Podaci_o_filmovima_rezisera[] strukture)
  {
    for (int i=0; i<strukture.Length; i++)
      if (reziser == strukture[i].reziser) return i;
    return -1;
  }

  //Glavna metoda obrade: izdvajaju se reziseri, njihovi filmovi i zarade filmova iz zadatog perioda
  static Podaci_o_filmovima_rezisera[] Izdvajanje_filmova_sa_reziserima_u_zadatom_periodu (string[,] podaci_matrica, DateTime[] niz_period)
  {
    Podaci_o_filmovima_rezisera[] reziser_filmovi = new Podaci_o_filmovima_rezisera[1000]; //niz struktura - jedna struktura: ime rezisera, njegovi filmovi, zarade filmova i ukupna zarada
    int brojac_struktura = 0; //brojac razlicitih rezisera(brojac-1 dalje u programu)
    int[] brojaci_filmova = new int[1000]; //brojac filmova za jednog rezisera
    int indeks_rezisera = 0;
    string film, datum;
    double zarada;
    
    for (int i=0; i<brojaci_filmova.Length; i++)
      brojaci_filmova[i] = 0;
    
    for (int i=0; i<podaci_matrica.GetLength(0); i++)
    {
      datum = podaci_matrica[i,3];
      if (!Ulazi_u_period(datum,niz_period)) continue;
      zarada = IzbacivanjeDolara(podaci_matrica[i,5]);
      film = podaci_matrica[i,1];
      indeks_rezisera = Indeks_trazenog_rezisera(podaci_matrica[i,4],reziser_filmovi);
      if (indeks_rezisera != -1)
      {
        Array.Resize(ref reziser_filmovi[indeks_rezisera].filmovi, brojaci_filmova[indeks_rezisera]+1); //prosirivanje niza filmova za jednog rezisera
        Array.Resize(ref reziser_filmovi[indeks_rezisera].zarade, brojaci_filmova[indeks_rezisera]+1); //prosirivanje niza zarada filmova za jednog rezisera
        reziser_filmovi[indeks_rezisera].filmovi[brojaci_filmova[indeks_rezisera]] = film;
        reziser_filmovi[indeks_rezisera].zarade[brojaci_filmova[indeks_rezisera]] = zarada;
        brojaci_filmova[indeks_rezisera]++; //postavlja se indeks za upis sledeceg filma za datog rezisera
      }
      else
      {
        reziser_filmovi[brojac_struktura].reziser = podaci_matrica[i,4];
        Array.Resize(ref reziser_filmovi[brojac_struktura].filmovi, 1);
        reziser_filmovi[brojac_struktura].filmovi[0] = film;
        brojaci_filmova[brojac_struktura] = 1; //postavlja se indeks za upis sledeceg filma za datog rezisera
        Array.Resize(ref reziser_filmovi[brojac_struktura].zarade, 1);
        reziser_filmovi[brojac_struktura].zarade[0] = zarada;
        reziser_filmovi[brojac_struktura].ukupna_zarada = 0;
        brojac_struktura++;
      }
    }
    Array.Resize(ref reziser_filmovi,brojac_struktura);
    return reziser_filmovi;
  }

  //Metoda koja sortira filmove u okviru struktura i racuna ukupnu zaradu za tri najprofitabilnija filma
  static void Sortiranje_uk_zarada(ref Podaci_o_filmovima_rezisera[] niz)
  {
    double max_zarada, zarada_zamena;
    string film_zamena;
    int indeks = 0;
    for (int i=0; i<niz.Length; i++)
    {
      for (int j=0; j<niz[i].zarade.Length; j++)
      {
        max_zarada = niz[i].zarade[j];
        indeks = j;
        for (int k=j+1; k<niz[i].zarade.Length; k++)
        {
          if (niz[i].zarade[k] > max_zarada)
          {
            max_zarada = niz[i].zarade[k];
            indeks = k;
          }
        }
        zarada_zamena = niz[i].zarade[indeks];
        film_zamena = niz[i].filmovi[indeks];
        niz[i].zarade[indeks] = niz[i].zarade[j];
        niz[i].filmovi[indeks] = niz[i].filmovi[j];
        niz[i].zarade[j] = zarada_zamena;
        niz[i].filmovi[j] = film_zamena;
      }
    }
    for (int i=0; i<niz.Length; i++)
    {
      if (niz[i].filmovi.Length > 3)
      {
        Array.Resize(ref niz[i].filmovi,3);
        Array.Resize(ref niz[i].zarade,3);
      }
    }
    for (int i=0; i<niz.Length; i++)
    {
      for (int j=0; j<niz[i].zarade.Length; j++)
        niz[i].ukupna_zarada += niz[i].zarade[j];
    }
  }

  //Metoda koja na kraju sortira sve, tj. ceo niz struktura, prema ukupnoj zaradi
  static void Konacno_sortiranje(ref Podaci_o_filmovima_rezisera[] niz)
  {
    double max_zarada;
    Podaci_o_filmovima_rezisera zamena;
    int indeks;
    for (int i=0; i<niz.Length; i++)
    {
      max_zarada = niz[i].ukupna_zarada;
      indeks = i;
      for (int j=i+1; j<niz.Length; j++)
      {
        if (niz[j].ukupna_zarada > max_zarada)
        {
          max_zarada = niz[j].ukupna_zarada;
          indeks = j;
        }
      }
      zamena = niz[indeks];
      niz[indeks] = niz[i];
      niz[i] = zamena;
    }
  }

  //Metoda koja daje poruku da je datoteka prazna, u slucaju da jeste
  static bool Poruka_prazna_datoteka(string ime_datoteke)
  {
    StreamReader ulaz = new StreamReader(ime_datoteke);
    int brojac_redova = 0;
    while (!ulaz.EndOfStream)
    {
      ulaz.ReadLine();
      brojac_redova++;
    }
    ulaz.Close();
    if (brojac_redova == 1) 
    {
      return true;
    }
    else return false;
  }

  //Ispis u izlaznu datoteku
   static void Ispis_niza_struktura(Podaci_o_filmovima_rezisera[] niz)
  {
    Console.Write("Unesite ime izlazne datoteke: ");
    string izlaz_ime = Console.ReadLine();
    StreamWriter ispis = new StreamWriter(izlaz_ime);
    ispis.WriteLine("Director;Movie Titles;Revenue");
    for (int i=0; i<niz.Length; i++)
    {
      ispis.Write(niz[i].reziser+";");
      for (int j=0; j<niz[i].filmovi.Length; j++)
      {
        ispis.Write(niz[i].filmovi[j]);
        if (j != niz[i].filmovi.Length-1) ispis.Write("|");
      }
      ispis.Write(";");
      ispis.Write("{0:C2}",niz[i].ukupna_zarada);
      ispis.WriteLine();
    }
    ispis.Close();
    if(Poruka_prazna_datoteka(izlaz_ime)) Console.WriteLine("Nema filmova u zadatom periodu.");

  }

  ////////////////////////////////////////////////////////
  //B metoda
  struct ZaradaPoZanru 
  {
    public string zanr;
    public double zarada;
  }
  static bool PostojiCrticaUPeriodu(string period)
  {
    foreach (char c in period)
      if (c == '-') return true;
    return false;
  }
  static void NajmanjePopularanZanr(string[,] matrica)
  {
    for (int i=0; i<4; i++)
        Console.WriteLine();
    Console.WriteLine("Filmovi iz datoteke su izdavani u periodu od 2000. do 2020. godine.");
    Console.Write("Unesite period (u formatu: godina-godina): ");
    string period = Console.ReadLine();
    while (!PostojiCrticaUPeriodu(period))
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.Error.WriteLine("Neispravan unos. Poku??ajte ponovo.");
      Console.ForegroundColor = ConsoleColor.White;
      Console.Write("Unesite period (u formatu: godina-godina): ");
      period = Console.ReadLine();
    }
    string[] godine_perioda = period.Split("-"); //unos perioda

    Izbacivanje_razmaka_datum (ref godine_perioda[0]); //izbacivanje razmaka
    Izbacivanje_razmaka_datum (ref godine_perioda[1]);

    Izbacivanje_tacke_datum (ref godine_perioda[0]); //izbacivanje ta??ke iz datuma
    Izbacivanje_tacke_datum (ref godine_perioda[1]);

    int prva_godina; //pretvaranje perioda u int
    int poslednja_godina;
    while (!int.TryParse(godine_perioda[0], out prva_godina) || !int.TryParse(godine_perioda[1], out poslednja_godina) || prva_godina <= 0 || poslednja_godina <= 0 || prva_godina > poslednja_godina)
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.Error.WriteLine("Neispravan unos. Poku??ajte ponovo.");
      Console.ForegroundColor = ConsoleColor.White;
      Console.Write("Unesite period (u formatu: godina-godina): ");
      period = Console.ReadLine();
      while (!PostojiCrticaUPeriodu(period))
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine("Neispravan unos. Poku??ajte ponovo.");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("Unesite period (u formatu: godina-godina): ");
        period = Console.ReadLine();
      }
      godine_perioda = period.Split("-");
      Izbacivanje_razmaka_datum(ref godine_perioda[0]);
      Izbacivanje_razmaka_datum(ref godine_perioda[1]);
    }

    Console.Write("Unesite ime izlazne datoteke: ");
    string izlaz_ime = Console.ReadLine();
    StreamWriter ispis = new StreamWriter(izlaz_ime);
    
    ispis.WriteLine("Year;Movie Genre;Genre Revenue");

    bool provera=false;

    for (int i=prva_godina; i<=poslednja_godina; i++) //ceo period
    {
      ZaradaPoZanru[] zarada_po_zanru = new ZaradaPoZanru[matrica.GetLength(0)];
      int brojac_zarada_po_zanru = 0;

      for (int j=0; j<matrica.GetLength(0); j++) //sve godine iz datoteke
      {
        string[] datum = matrica[j,3].Split(".");
        int godina;
        int.TryParse(datum[2], out godina);
        if (godina == i) 
        {
          string[] zanrovi_jednog_filma = matrica[j,2].Split("|");
          for (int l=0; l<zanrovi_jednog_filma.Length; l++) //svi zanrovi jednog filma
          { 
            double zarada_jednog_filma = IzbacivanjeDolara(matrica[j,5]);

            for (int k=0; k<brojac_zarada_po_zanru; k++) //zanrovi koji su vec definisani u toj godini
            {
              if (zarada_po_zanru[k].zanr == zanrovi_jednog_filma[l])
              {
                zarada_po_zanru[k].zarada += zarada_jednog_filma;
                provera = true;
              }
            }
            if (!provera) //ako ne postoji taj zanr, pravi se novi
            {
              zarada_po_zanru[brojac_zarada_po_zanru].zanr = zanrovi_jednog_filma[l];
              zarada_po_zanru[brojac_zarada_po_zanru].zarada = zarada_jednog_filma;
              brojac_zarada_po_zanru++;
            }
            provera=false;
          }
        }
      }
      double min = zarada_po_zanru[0].zarada;
      for (int b=1; b<brojac_zarada_po_zanru; b++)
      {
        if (min > zarada_po_zanru[b].zarada) //minimalna zarada po zanru
        {
          min = zarada_po_zanru[b].zarada; 
        }
      }
      if (min != 0)
      {
        ispis.Write(i+";");
        int brojac_minimalnih = 0; //ukupan broj minimalnih zarada po godini
        int brojac_minimalnih_2 = 0;

        for (int b=0; b<brojac_zarada_po_zanru; b++) //ukupan broj minimalnih zarada
        {
          if (min == zarada_po_zanru[b].zarada) brojac_minimalnih++;
        }

        for (int b=0; b<brojac_zarada_po_zanru; b++) //ispis minimalnih zarada 
        {
          if (min == zarada_po_zanru[b].zarada)
          {
            if(brojac_minimalnih_2 == brojac_minimalnih-1)
            {
              ispis.Write(zarada_po_zanru[b].zanr);
              brojac_minimalnih_2++;
            }
            else
            {
              ispis.Write(zarada_po_zanru[b].zanr+"|");
              brojac_minimalnih_2++;
            }
          }
        }
        ispis.Write(";{0:C2}",min);
        ispis.WriteLine();
      }
      min = 0;
    }
    ispis.Close();
    Poruka_prazna_datoteka(izlaz_ime);
    if(Poruka_prazna_datoteka(izlaz_ime)) Console.WriteLine("Nema filmova u zadatom periodu.");
  }
  
  ////////////////////////////////////////////////////////
  //C metoda
  struct Zanrovi_rezisera
  {
    public string reziser;
    public string[] zanrovi;
    public int[] br_filmovi;
  }
  static void Veliko_slovo_zanr(ref string zanr) 
  {
    string s = Convert.ToString(zanr[0]).ToUpper();
    string s1 = s;

    for (int i=1; i<zanr.Length; i++)
    {
      s = Convert.ToString(zanr[i]).ToLower();
      s1 += s;
    }
    zanr = s1;
    if (zanr == "Imax") zanr = zanr.ToUpper();
    else if (zanr == "Sci-fi") zanr = "Sci-Fi";
    else if (zanr == "Film-noir") zanr = "Film-Noir";
  }
  static string[] Unos_zanrova() //unos zanrova sa konzole
  {
    for (int i=0; i<4; i++)
        Console.WriteLine();
    Console.WriteLine("Postoje??i ??anrovi: ");
    Console.WriteLine();
    string[] zanrovi = {"Comedy","Drama","Romance","Thriller","Adventure","Western","Fantasy","Mystery","Animation","Musical","Sci-Fi","Action","Documentary","Horror","War","IMAX","Children","Crime","Film-Noir"};
    for (int i=0; i<zanrovi.Length; i++)
    {
      Console.Write("{0,-12}", zanrovi[i]);
      if ((i + 1) % 4 == 0) Console.WriteLine();
    }
    Console.WriteLine();
    Console.WriteLine();
    Console.Write("Unesite ??anrove po izboru (odvojene zapetama): ");
    string[] zanr = Console.ReadLine().Split(",");
    for (int i=0; i<zanr.Length; i++)
    {
      //Primena metode za izbacivanje razmaka
      string cuvar_unosa = zanr[i];
      Izbacivanje_razmaka_datum(ref zanr[i]);
      Veliko_slovo_zanr(ref zanr[i]);
      if(!Ne_postoji_uneti_zanr(zanrovi, zanr[i]))
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine($"Uneti ??anr {cuvar_unosa} ne postoji.");
        Console.ForegroundColor = ConsoleColor.White;
      }
    }
    return zanr;
  }

  //Metoda za uneti zanr koji ne postoji
  static bool Ne_postoji_uneti_zanr(string[] zanrovi, string jedan_zanr)
  {
    for(int i=0; i<zanrovi.Length; i++)
    {
      if(jedan_zanr==zanrovi[i])return true;
    }
    return false;
  }

  //Metoda koja broji filmove po zanru
  static void Postoji_zanr_rezisera(Zanrovi_rezisera reziser_zanrovi ,string[] niz_ulazni_zanrovi) 
  {
    for(int i=0;i<niz_ulazni_zanrovi.Length;i++)
    {
      for (int j=0; j<reziser_zanrovi.zanrovi.Length;j++)
      {
        if(niz_ulazni_zanrovi[i]==reziser_zanrovi.zanrovi[j])
        {
          reziser_zanrovi.br_filmovi[j]++;
        }
      }
    }
  }
   
   //Metoda koja vraca indeks rezisera ako je unet
  static int Unet_reziser(Zanrovi_rezisera[] reziser_zanrovi, string reziser,int brojac_struktura)
  {
    for(int i=0;i<brojac_struktura;i++)
    {
      if(reziser==reziser_zanrovi[i].reziser)return i;
    }
    return -1;
  }

  //Metoda koja poredi ulazni zanr sa zanrom niza
  static bool Poredjenje_sa_konzolom(string zanr_niz,string[] niz_ulazni_zanrovi)
  {
      for (int j=0; j<niz_ulazni_zanrovi.Length;j++)
      {
        if(zanr_niz==niz_ulazni_zanrovi[j])return true;
      }
      return false;
  }
  
  //Glavna metoda
  static Zanrovi_rezisera[] Zanrovi_po_reziserima (string[,] podaci_matrica, string[] zanr_niz) 
  {
    Zanrovi_rezisera[] reziser_zanrovi = new Zanrovi_rezisera[1000];
    int brojac_struktura = 0;
    string[] niz_ulazni_zanrovi;
    
    for (int i=0; i<podaci_matrica.GetLength(0); i++)
    {
      niz_ulazni_zanrovi = podaci_matrica[i,2].Split("|");
      int indeks=Unet_reziser(reziser_zanrovi,podaci_matrica[i,4],brojac_struktura);
      if(indeks!=-1)
      {
        for(int k=0; k<zanr_niz.Length; k++)
        {
          if(Poredjenje_sa_konzolom(zanr_niz[k],niz_ulazni_zanrovi))
          {
             Postoji_zanr_rezisera(reziser_zanrovi[indeks],niz_ulazni_zanrovi); 
          }
        }
      }
      else
      {
        int brojac=0;
        bool provera=false;
        reziser_zanrovi[brojac_struktura].br_filmovi= new int [zanr_niz.Length];
        reziser_zanrovi[brojac_struktura].zanrovi= new string [zanr_niz.Length];
        for(int k=0; k<zanr_niz.Length; k++)
        {
          if(Poredjenje_sa_konzolom(zanr_niz[k],niz_ulazni_zanrovi))
          {
            reziser_zanrovi[brojac_struktura].reziser = podaci_matrica[i,4];
            reziser_zanrovi[brojac_struktura].zanrovi[brojac] = zanr_niz[k];
            reziser_zanrovi[brojac_struktura].br_filmovi[brojac] = 1;
            brojac++;
            provera=true;
          }
        }
        Array.Resize(ref reziser_zanrovi[brojac_struktura].br_filmovi,brojac);
        Array.Resize(ref reziser_zanrovi[brojac_struktura].zanrovi,brojac);
        if (provera)brojac_struktura++;
      }
    }
    Array.Resize(ref reziser_zanrovi,brojac_struktura);
    return reziser_zanrovi;
  }

  //Metoda za leksikografsko sortiranje
  public static void BubbleSort(ref string[] a) 
  {
    bool promena = true;
    while(promena)
    {
      promena = false;
      for(int i=0;i < a.Length-1; i++)
      {
        if(a[i].CompareTo(a[i+1])==1)
        {
          a[i] = Zamena(ref  a[i+1], a[i]);
          promena = true;
        }
      }
    }
  }
  public static string Zamena(ref string a, string b)
  {
    string j = a;
    a = b;
    return j; 
  }

  static void Ispis_zanrova_po_reziseru(Zanrovi_rezisera[] niz) //ispis C)
  {
    Console.Write("Unesite ime izlazne datoteke: ");
    string izlaz_ime = Console.ReadLine();
    StreamWriter ispis = new StreamWriter(izlaz_ime);

    ispis.WriteLine("Director;Movie Genre|Movie Genre Count");

    for (int i=0; i<niz.Length; i++)
    {
      BubbleSort(ref niz[i].zanrovi);
      ispis.Write(niz[i].reziser+";");
      for (int j=0; j<niz[i].zanrovi.Length; j++)
      {
        if (j!=niz[i].zanrovi.Length-1) ispis.Write(niz[i].zanrovi[j]+"|"+niz[i].br_filmovi[j]+";");
        else ispis.Write(niz[i].zanrovi[j]+"|"+niz[i].br_filmovi[j]);
      }
      ispis.WriteLine();
    }
    ispis.Close();
    if(Poruka_prazna_datoteka(izlaz_ime)) Console.WriteLine("Nema filmova sa zadatim ??anrom.");
  }
  
  ////////////////////////////////////////////////////////
  static void Validnost_zarade (string[,] matrica) //validnost zarade u matrici
  {
    for (int i=0; i<matrica.GetLength(0); i++)
    {
      string s = matrica[i,5];
      if (s[0]!='$')
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine("Zarada filma id - "+(i+1)+" je pogresno uneta.");
        break;
      }
      for (int j=1; j<s.Length; j++)
      {
        int a;
        string b = Convert.ToString (s[j]);
        if (!int.TryParse (b, out a) && s[j]!='.')
        {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.Error.WriteLine("Zarada filma id - "+(i+1)+" je pogresno uneta.");
          break;
        }
      }
    }
  }
   //Validnost datuma u ulaznoj datoteci
  static void Validan_datum_u_matrici(string[,] matrica)
  {
    string unet_datum;
    for (int i = 0; i<matrica.GetLength(0); i++)
    {
      unet_datum = matrica[i,3];
      Izbacivanje_razmaka_datum(ref unet_datum);
      DateTime konvertovan_datum;
      if (!DateTime.TryParseExact(unet_datum, dozvoljeni_formati, null, DateTimeStyles.None, out konvertovan_datum)) {validan = false; break;}
    }
  }

  ////////////////////////////////////////////////////////
  //Biranje metoda - unos sa konzole
  static int red_pocetak = 15;
  static int kolona_pocetak = 25;
  static int x = kolona_pocetak + 3;
	static int y = red_pocetak + 2;
  
  static void Crtanje_tabele()
  {
    //linije za iscrtavanje - delovi tabele
    char hor = '\u2500', ver = '\u2502', g_levi = '\u250C', g_desni = '\u2510',d_levi = '\u2514', d_desni = '\u2518', gornji_sredina = '\u252C', donji_sredina = '\u2534';
    Console.BackgroundColor = ConsoleColor.Blue;
		Console.ForegroundColor = ConsoleColor.Black;
    Console.SetCursorPosition(kolona_pocetak, red_pocetak);
    for (int i=0; i<5; i++)
    {
      Console.SetCursorPosition(kolona_pocetak, red_pocetak++);
      if (i == 0) Console.Write(g_levi);
      else if (i == 4) Console.Write(d_levi);
      else Console.Write(ver);
      if (i == 0 || i == 4)
      {
        for (int j=0; j<3; j++)
        {
          for (int k=0; k<5; k++)
            Console.Write(hor);
          if (j != 2 && i == 0) Console.Write(gornji_sredina);
          else if (j != 2 && i == 4) Console.Write(donji_sredina);
        }
      }
      else if (i == 1 || i == 3)
      {
        for (int j=0; j<3; j++)
        {
          for (int k=0; k<5; k++)
          {
            Console.Write(" ");
          }
          if (j < 2) Console.Write(ver);
        }
      }
      else
      {
        Console.ForegroundColor = ConsoleColor.White;
				Console.Write("  A  ");
        Console.ForegroundColor = ConsoleColor.Black;
        Console.Write(ver);
        Console.ForegroundColor = ConsoleColor.White;
				Console.Write("  B  ");
        Console.ForegroundColor = ConsoleColor.Black;
        Console.Write(ver);
        Console.ForegroundColor = ConsoleColor.White;
				Console.Write("  C  ");
        Console.ForegroundColor = ConsoleColor.Black;
      }
      if (i == 0) Console.Write(g_desni);
      else if (i == 4) Console.Write(d_desni);
      else Console.Write(ver);
      if (i < 4) Console.WriteLine();
    }
    Console.ResetColor();
    Console.SetCursorPosition(x,y);
  }

  static void Pomeranje_desno()
  {
    if (x < kolona_pocetak + 15) x += 6;
  }
  static void Pomeranje_levo()
  {
    if (x > kolona_pocetak + 3) x -= 6;
  }

  static int Biranje_metode()
  {
    ConsoleKeyInfo cki;
    do
    {
      cki = Console.ReadKey(true);
			if (cki.Key == ConsoleKey.LeftArrow) Pomeranje_levo();
			else if (cki.Key == ConsoleKey.RightArrow) Pomeranje_desno();
			Console.SetCursorPosition(x,y);
    } while (cki.Key != ConsoleKey.Enter && cki.Key != ConsoleKey.Escape);
      return x;
  }
  public static void Main (string[] args) {
    string[,] podaci_matrica = new string[1000,6];
    Ucitavanje_podataka(ref podaci_matrica);
    Validnost_zarade(podaci_matrica);
    Validan_datum_u_matrici(podaci_matrica);
    if (!validan) return;
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("\u2022 Dobrodo??li u D2M program obrade tabelarnih podataka!");
    Console.WriteLine();
    ponovo: Console.WriteLine("\u2022 Izaberite na koji na??in ??elite da obradite podatke o filmovima (iz datoteke ulazni_podaci.csv).");
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("Napomena:");
    Console.ForegroundColor = ConsoleColor.DarkGreen;
    Console.WriteLine("\u2022 Metoda A izdvaja iz zadatog perioda do tri filma sa najve??om zaradom, po re??iserima, i sortira ih prema ukupnoj zaradi.");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("\u2022 Metoda B odre??uje najmanje popularan ??anr u godinama zadatog perioda.");
    Console.ForegroundColor = ConsoleColor.Magenta;
    Console.WriteLine("\u2022 Metoda C izdvaja broj filmova re??isera prema zadatim ??anrovima.");
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("   Izaberite metodu pomeranjem strelica na tastaturi i pritiskom na Enter: ");
    Crtanje_tabele();
    int metoda = Biranje_metode();
    if (metoda == kolona_pocetak + 3)
    {
      //Izvrsavanje A metode
      for (int i=0; i<4; i++)
        Console.WriteLine();
      DateTime[] niz_period = Unos_perioda();
      Podaci_o_filmovima_rezisera[] niz_struktura = Izdvajanje_filmova_sa_reziserima_u_zadatom_periodu(podaci_matrica,niz_period);
      Sortiranje_uk_zarada(ref niz_struktura);
      Konacno_sortiranje(ref niz_struktura);
      Ispis_niza_struktura(niz_struktura);
    }
    else if (metoda == kolona_pocetak + 9)
    {
      //Izvrsavanje B metode
      NajmanjePopularanZanr(podaci_matrica);
    }
    else 
    {
      //Izvrsavanje C metode
      //Ucitavanje_podataka(ref podaci_matrica);
      string[] zanr_niz = Unos_zanrova();
      Zanrovi_rezisera[] niz_provera = Zanrovi_po_reziserima(podaci_matrica,zanr_niz);
      Ispis_zanrova_po_reziseru(niz_provera);
    }
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("\u2022 Da li ??elite da zavr??ite sa izvr??avanjem programa? (da/ne) ");
    string odgovor = Console.ReadLine();
    while (odgovor != "da" && odgovor != "ne")
    {
      Console.ForegroundColor = ConsoleColor.Red;
      Console.Error.WriteLine("Neispravan unos odgovora. Poku??ajte ponovo.");
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.Write("Da li ??elite da zavr??ite sa izvr??avanjem programa? (da/ne) ");
      odgovor = Console.ReadLine();
    }
    if (odgovor == "ne")
    {
      Console.Clear();
      red_pocetak = 11;
      kolona_pocetak = 25;
      x = kolona_pocetak + 3;
      y = red_pocetak + 2;
      goto ponovo;
    }
    else 
    {
      Console.WriteLine();
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.WriteLine("Hvala Vam na kori????enju D2M programa za obradu tabelarnih podataka! \u263a");
    }
  }
}