using System;
using System.IO;
using System.Globalization;

class MainClass {
  static void Ucitavanje_podataka (ref string[,] matrica)
  {
    if (File.Exists("ulazni_podaci.csv"))
    {
      StreamReader podaci = new StreamReader("ulazni_podaci.csv");
      //string[,] matrica = new string[1000,6];
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
  public static void Main (string[] args) {
    string[,] podaci_matrica = new string[1000,6];
    Ucitavanje_podataka(ref podaci_matrica);
  }
}