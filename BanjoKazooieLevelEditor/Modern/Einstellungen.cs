// Banjo's Backpack – Modern Edition · FGSeRaser (https://github.com/FGSeRaser)
// Einfache Einstellungsdatei unter %AppData%\BanjosBackpackModern\einstellungen.ini

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BanjoKazooieLevelEditor.Modern
{
  internal static class Einstellungen
  {
    private static readonly Dictionary<string, string> werte = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    private static bool geladen;

    private static string Pfad
    {
      get
      {
        string ordner = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BanjosBackpackModern");
        return Path.Combine(ordner, "einstellungen.ini");
      }
    }

    private static void Laden()
    {
      if (geladen)
        return;
      geladen = true;
      try
      {
        if (!File.Exists(Pfad))
          return;
        foreach (string zeile in File.ReadAllLines(Pfad, Encoding.UTF8))
        {
          int i = zeile.IndexOf('=');
          if (i > 0)
            werte[zeile.Substring(0, i).Trim()] = zeile.Substring(i + 1).Trim();
        }
      }
      catch
      {
      }
    }

    private static void Speichern()
    {
      try
      {
        Directory.CreateDirectory(Path.GetDirectoryName(Pfad));
        List<string> zeilen = new List<string>();
        foreach (KeyValuePair<string, string> kv in werte)
          zeilen.Add(kv.Key + "=" + kv.Value);
        File.WriteAllLines(Pfad, zeilen.ToArray(), Encoding.UTF8);
      }
      catch
      {
      }
    }

    public static string Text(string schluessel, string standard)
    {
      Laden();
      string w;
      return werte.TryGetValue(schluessel, out w) ? w : standard;
    }

    public static void Text(string schluessel, string wert, bool speichern)
    {
      Laden();
      werte[schluessel] = wert ?? "";
      if (speichern)
        Speichern();
    }

    public static bool Schalter(string schluessel, bool standard)
    {
      string w = Text(schluessel, standard ? "1" : "0");
      return w == "1" || w.Equals("true", StringComparison.OrdinalIgnoreCase);
    }

    public static void SchalterSetzen(string schluessel, bool wert)
    {
      Text(schluessel, wert ? "1" : "0", true);
    }

    // ---- Komfort-Eigenschaften ----
    public static bool DunklesDesign
    {
      get { return Schalter("DunklesDesign", true); }
      set { SchalterSetzen("DunklesDesign", value); }
    }

    public static bool Deutsch
    {
      get { return Schalter("Deutsch", System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "de"); }
      set { SchalterSetzen("Deutsch", value); }
    }

    public static bool Schnellleiste
    {
      get { return Schalter("Schnellleiste", true); }
      set { SchalterSetzen("Schnellleiste", value); }
    }

    public static bool AssistentBeimStart
    {
      get { return Schalter("AssistentBeimStart", true); }
      set { SchalterSetzen("AssistentBeimStart", value); }
    }

    public static string Emulator
    {
      get { return Text("Emulator", ""); }
      set { Text("Emulator", value, true); }
    }

    public static string LetzteGespeicherteRom
    {
      get { return Text("LetzteGespeicherteRom", ""); }
      set { Text("LetzteGespeicherteRom", value, true); }
    }
  }
}

namespace BanjoKazooieLevelEditor.Modern
{
  /// <summary>Sprachauswahl für die Texte der Modern Edition (Deutsch / English).</summary>
  internal static class Sp
  {
    public static bool Deutsch
    {
      get { return Einstellungen.Deutsch; }
    }

    public static string T(string deutsch, string englisch)
    {
      return Deutsch ? deutsch : englisch;
    }
  }
}
