using System;
using System.IO;
using System.Globalization;

class MainClass {
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
  public static void Main (string[] args) {
    string[,] podaci_matrica = new string[1000,6];
    Ucitavanje_podataka(ref podaci_matrica);
  }
}