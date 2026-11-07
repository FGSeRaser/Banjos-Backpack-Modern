// Banjo's Backpack – Modern Edition · FGSeRaser (https://github.com/FGSeRaser)
// Robuste Pfade für ROM-Dateien (OneDrive-"Dokumente", verschobene ROMs usw.)

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BanjoKazooieLevelEditor.Modern
{
  internal static class RomHilfe
  {
    /// <summary>Ordner "roms" neben der .exe – funktioniert immer.</summary>
    public static string RomOrdner
    {
      get
      {
        string ordner = Path.Combine(Application.StartupPath, "roms");
        try
        {
          Directory.CreateDirectory(ordner);
        }
        catch
        {
        }
        return ordner;
      }
    }

    /// <summary>
    /// Liefert einen beschreibbaren Pfad für die erweiterte ROM.
    /// Aus "Banjo-Kazooie.z64" wird "Banjo-Kazooie.ext.z64" (statt ".z64.ext.z64").
    /// Ist der Ordner der Original-ROM nicht beschreibbar/vorhanden, wird "roms" neben der .exe benutzt.
    /// </summary>
    public static string ZielFuerErweiterteRom(string original)
    {
      string name = Path.GetFileNameWithoutExtension(original) + ".ext.z64";
      string ordner = "";
      try
      {
        ordner = Path.GetDirectoryName(Path.GetFullPath(original));
      }
      catch
      {
      }

      if (!string.IsNullOrEmpty(ordner) && Beschreibbar(ordner))
        return Path.Combine(ordner, name);
      return Path.Combine(RomOrdner, name);
    }

    private static bool Beschreibbar(string ordner)
    {
      try
      {
        if (!Directory.Exists(ordner))
          return false;
        string test = Path.Combine(ordner, ".bb_schreibtest_" + Guid.NewGuid().ToString("N"));
        File.WriteAllText(test, "");
        File.Delete(test);
        return true;
      }
      catch
      {
        return false;
      }
    }

    /// <summary>Entfernt den gespeicherten ROM-Pfad aus resources\mw.ini.</summary>
    public static void IniRomEntfernen()
    {
      try
      {
        string ini = Path.Combine(Application.StartupPath, "resources\\mw.ini");
        if (!File.Exists(ini))
          return;
        string inhalt = File.ReadAllText(ini);
        inhalt = Regex.Replace(inhalt, "ROM:[^\\r\\n]*(\\r?\\n)?", "");
        File.WriteAllText(ini, inhalt);
      }
      catch
      {
      }
    }
  }
}
