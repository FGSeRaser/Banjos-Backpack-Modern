// Banjo's Backpack – Modern Edition · FGSeRaser (https://github.com/FGSeRaser)
// Sorgt dafür, dass JEDES Fenster des Editors (auch Modell-Import, MIDI-Werkzeug usw.)
// automatisch das neue Design und die deutschen Texte bekommt.

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BanjoKazooieLevelEditor.Modern
{
  internal static class ModernUI
  {
    private static readonly HashSet<Form> bearbeitet = new HashSet<Form>();
    private static Timer waechter;

    public static void RendererSetzen()
    {
      if (Theme.Aktiv && !(ToolStripManager.Renderer is DunklerRenderer))
        ToolStripManager.Renderer = new DunklerRenderer();
    }

    public static void Starten()
    {
      RendererSetzen();
      waechter = new Timer { Interval = 120 };
      waechter.Tick += (s, e) => NeueFensterPruefen();
      waechter.Start();
    }

    private static void NeueFensterPruefen()
    {
      // Kopie, weil sich OpenForms während der Schleife ändern kann
      List<Form> offen = new List<Form>();
      foreach (Form f in Application.OpenForms)
        offen.Add(f);

      foreach (Form f in offen)
      {
        if (bearbeitet.Contains(f) || f is Assistent)
          continue;
        bearbeitet.Add(f);
        Form gemerkt = f;
        f.FormClosed += (s, e) => bearbeitet.Remove(gemerkt);
        FensterAnpassen(f);
      }
    }

    /// <summary>Kann auch direkt aufgerufen werden (z. B. vom Hauptfenster), damit es ohne Flackern passiert.</summary>
    public static void FensterAnpassen(Form f)
    {
      if (!bearbeitet.Contains(f))
      {
        bearbeitet.Add(f);
        f.FormClosed += (s, e) => bearbeitet.Remove(f);
      }

      f.SuspendLayout();
      try
      {
        Theme.Anwenden(f);
        Uebersetzung.Anwenden(f);
        NachgeladeneControls(f);
      }
      catch
      {
      }
      finally
      {
        f.ResumeLayout(true);
      }
    }

    // Controls, die der Editor später dynamisch hinzufügt, ebenfalls anpassen
    private static void NachgeladeneControls(Control c)
    {
      c.ControlAdded += (s, e) =>
      {
        Theme.Anwenden(e.Control);
        Uebersetzung.Anwenden(e.Control);
        NachgeladeneControls(e.Control);
      };
      foreach (Control kind in c.Controls)
        NachgeladeneControls(kind);
    }
  }
}
