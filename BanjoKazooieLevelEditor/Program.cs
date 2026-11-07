// Banjo's Backpack – Modern Edition
// Original: Skill, Adam, Cooliscool (GPL-3.0)
// Modern Edition: FGSeRaser – https://github.com/FGSeRaser

using System;
using System.Windows.Forms;
using BanjoKazooieLevelEditor.Modern;

namespace BanjoKazooieLevelEditor
{
  internal static class Program
  {
    [STAThread]
    private static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
      Application.ThreadException += (s, e) => Fehler.Zeigen(e.Exception);

      // Fehlende Ordner/Programme freundlich melden statt still abzustürzen
      if (!Startpruefung.Ausfuehren())
        return;

      // Design + deutsche Texte für jedes Fenster, das geöffnet wird
      ModernUI.Starten();

      try
      {
        Application.Run((Form) new Form1());
      }
      catch (Exception ex)
      {
        Fehler.Zeigen(ex);
      }
    }
  }
}
