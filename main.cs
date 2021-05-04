using System;
using System.IO;
using System.Globalization;

class MainClass {
  struct Zanrovi_rezisera
  {
    public string reziser;
    public string[] zanrovi;
    public int[] br_filmovi;
  }
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
        for (int i=0; i<6; i++)
        {
          matrica[brojac,i] = elementi[i];
        }
        brojac++;
      }
      podaci.Close();
    }
    else Console.Error.WriteLine("Greska! Ne postoji datoteka ulazni_podaci");
  }
  struct Podaci_o_filmovima_rezisera
  {
    public string reziser;
    public string[] filmovi;
    public double[] zarade;
    public double ukupna_zarada;
  }
   //////////////////////////////////////////////////////////
  //Metode za unos datuma
  static void Izbacivanje_razmaka_datum (ref string datum)
  {
    string datum1 = "";
    foreach (char c in datum)
      if (' '!=c) datum1+=c;
    datum = datum1;
  }
  ////////////////////////////////////////////////////////////////////////////
  static string[] dozvoljeni_formati = {"d/MM/yyyy","dd/MM/yyyy","dd/M/yyyy","d/M/yyyy","d.MM.yyyy","dd.MM.yyyy","dd.M.yyyy","d.M.yyyy","d.MM.yyyy.","dd.MM.yyyy.","dd.M.yyyy.","d.M.yyyy."};
  ///////////////////////////////////////////////////////////////////////////
  static DateTime Unos_datuma(string pocetni_krajnji)
  {
    string unet_datum = Console.ReadLine();
    Izbacivanje_razmaka_datum(ref unet_datum);
    DateTime konvertovan_datum;
    while (!DateTime.TryParseExact(unet_datum, dozvoljeni_formati, null, DateTimeStyles.None, out konvertovan_datum))
    {
      Console.Error.WriteLine("Neispravan unos datuma. Pokusajte ponovo.");
      Console.Write($"Unesite {pocetni_krajnji} datum: ");
      unet_datum = Console.ReadLine();
      Izbacivanje_razmaka_datum(ref unet_datum);
    }
    return konvertovan_datum;
  }
  /////////////////////////////////////////////////////////////////
  static DateTime[] Unos_perioda ()
  {
    DateTime[] datumi = new DateTime[2];
    Console.WriteLine("Unesite period za koji se obrađuju podaci.");
    Console.Write("Unesite početni datum: ");
    string pocetni_krajnji = "početni";
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
  ///////////////////////////////////////////////////////////
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
  //////////////////////////////////////////////////////////
  //Metoda vraca indeks vec unetog rezisera, koji se trazi
  static int Indeks_trazenog_rezisera (string reziser,Podaci_o_filmovima_rezisera[] strukture)
  {
    for (int i=0; i<strukture.Length; i++)
      if (reziser == strukture[i].reziser) return i;
    return -1;
  }
  /////////////////////////////////////////////////////////
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
        //brojaci_filmova[brojac_struktura]=0; unet je prvi film u novu strukturu;
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
  ///////////////////////////////////////////////////
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
  ///////////////////////////////////////////////////
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
  ////////////////////////////////////////////////////////////////////////////
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
  }
  struct ZaradaPoZanru 
  {
    public string zanr;
    public double zarada;
  }
  static void NajmanjePopularanZanr(string[,] matrica)
  {
    StreamWriter ispis = new StreamWriter("Najmanje popularan zanr.txt");

    Console.Write("Unesite period: ");
    string period = Console.ReadLine();
    string[] godine_perioda = period.Split(); //unos perioda

    int prva_godina;
    int.TryParse(godine_perioda[0], out prva_godina);
    int poslednja_godina;
    int.TryParse(godine_perioda[1], out poslednja_godina);

    bool provera=false;

    for (int i=prva_godina; i<=poslednja_godina; i++) //ceo period
    {
      ZaradaPoZanru[] zarada_po_zanru = new ZaradaPoZanru[matrica.GetLength(0)];
      int brojac_zarada_po_zanru=0;

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
                provera=true;
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
        int brojac_minimalnih=0;
        int brojac_minimalnih_2=0;
        for (int b=0; b<brojac_zarada_po_zanru; b++)
        {
          if (min==zarada_po_zanru[b].zarada) brojac_minimalnih++;
        }
        for (int b=0; b<brojac_zarada_po_zanru; b++)
        {
          if (min==zarada_po_zanru[b].zarada)
          {
            if(brojac_minimalnih_2==brojac_minimalnih-1)
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
        min=Math.Round(min,2);
        ispis.Write(";{0:C2}",min);
        ispis.WriteLine();
      }
      min=0;
    }
    ispis.Close();
  }
  public static void Main (string[] args) {
    string[,] podaci_matrica = new string[1000,6];
    Ucitavanje_podataka(ref podaci_matrica);
  }
}