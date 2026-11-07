// Banjo's Backpack – Modern Edition · FGSeRaser (https://github.com/FGSeRaser)
// Prüft vor dem Start, ob alle Hilfsdateien vorhanden sind, und erklärt verständlich, was fehlt.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BanjoKazooieLevelEditor.Modern
{
  internal static class Startpruefung
  {
    private static readonly string[] pflichtDateien =
    {
      "resources\\setups.xml",
      "resources\\objects.xml",
      "resources\\menu.xml",
      "resources\\animations.xml",
      "out\\rn64crc.exe",
      "out\\gzip.exe",
    };

    public static List<string> FehlendeDateien()
    {
      List<string> fehlt = new List<string>();
      foreach (string datei in pflichtDateien)
        if (!File.Exists(Path.Combine(Application.StartupPath, datei)))
          fehlt.Add(datei);
      return fehlt;
    }

    /// <summary>true = Editor darf starten.</summary>
    public static bool Ausfuehren()
    {
      // Der Editor arbeitet mit relativen Pfaden (.\resources\…) – darum immer im Programmordner starten
      try
      {
        Environment.CurrentDirectory = Application.StartupPath;
        Directory.CreateDirectory(Path.Combine(Application.StartupPath, "tmp"));
        Directory.CreateDirectory(Path.Combine(Application.StartupPath, "out"));
        string ini = Path.Combine(Application.StartupPath, "resources\\mw.ini");
        if (Directory.Exists(Path.GetDirectoryName(ini)) && !File.Exists(ini))
          File.WriteAllText(ini, "");
      }
      catch
      {
      }

      while (true)
      {
        List<string> fehlt = FehlendeDateien();
        if (fehlt.Count == 0)
          return true;

        ModernUI.RendererSetzen();
        using (Form f = Dialog(fehlt))
        {
          DialogResult r = f.ShowDialog();
          if (r == DialogResult.Retry)
            continue;
          return false;
        }
      }
    }

    private static Form Dialog(List<string> fehlt)
    {
      Form f = new Form
      {
        Text = Sp.T("Banjo's Backpack – Dateien fehlen", "Banjo's Backpack – files missing"),
        StartPosition = FormStartPosition.CenterScreen,
        FormBorderStyle = FormBorderStyle.FixedDialog,
        MaximizeBox = false,
        MinimizeBox = false,
        ClientSize = new Size(560, 380),
        Font = new Font("Segoe UI", 9.5f),
      };

      Label titel = new Label
      {
        Text = Sp.T("Es fehlen noch Hilfsdateien", "Some helper files are missing"),
        Font = new Font("Segoe UI Semibold", 15f),
        AutoSize = true,
        Location = new Point(20, 16),
      };

      Label info = new Label
      {
        Text = Sp.T(
          "Der Quellcode auf GitHub enthält nicht die Ordner \"resources\" und \"out\" aus dem originalen Banjo's-Backpack-Download "
          + "(Release 1.0.5). Kopiere diese beiden Ordner in den Programmordner (dort, wo die .exe liegt) und klicke dann auf \"Erneut prüfen\".",
          "The source code on GitHub does not include the \"resources\" and \"out\" folders from the original Banjo's Backpack download "
          + "(release 1.0.5). Copy both folders into the program folder (where the .exe is) and click \"Check again\"."),
        Location = new Point(20, 56),
        Size = new Size(520, 64),
      };

      TextBox liste = new TextBox
      {
        Multiline = true,
        ReadOnly = true,
        ScrollBars = ScrollBars.Vertical,
        Location = new Point(20, 124),
        Size = new Size(520, 150),
        Font = new Font("Consolas", 9.5f),
        Text = Sp.T("Programmordner:", "Program folder:") + Environment.NewLine + Application.StartupPath + Environment.NewLine + Environment.NewLine
             + Sp.T("Fehlt:", "Missing:") + Environment.NewLine + "  • " + string.Join(Environment.NewLine + "  • ", fehlt.ToArray()),
      };

      Button oeffnen = new Button { Text = Sp.T("Programmordner öffnen", "Open program folder"), Location = new Point(20, 326), Size = new Size(170, 34) };
      oeffnen.Click += (s, e) =>
      {
        try
        {
          Process.Start("explorer.exe", "\"" + Application.StartupPath + "\"");
        }
        catch
        {
        }
      };

      Button pruefen = new Button { Text = Sp.T("Erneut prüfen", "Check again"), Location = new Point(270, 326), Size = new Size(130, 34), DialogResult = DialogResult.Retry };
      Button beenden = new Button { Text = Sp.T("Beenden", "Exit"), Location = new Point(410, 326), Size = new Size(130, 34), DialogResult = DialogResult.Cancel };

      LinkLabel github = new LinkLabel
      {
        Text = Sp.T("Modern Edition von FGSeRaser · github.com/FGSeRaser", "Modern Edition by FGSeRaser · github.com/FGSeRaser"),
        AutoSize = true,
        Location = new Point(20, 290),
      };
      github.LinkClicked += (s, e) => Links.Oeffnen(Links.GithubProfil);

      f.Controls.AddRange(new Control[] { titel, info, liste, github, oeffnen, pruefen, beenden });
      f.AcceptButton = pruefen;
      f.CancelButton = beenden;

      Theme.Anwenden(f);
      if (Theme.Aktiv)
      {
        titel.ForeColor = Farben.Akzent;
        pruefen.BackColor = Farben.Akzent;
        pruefen.ForeColor = Farben.AkzentText;
      }
      return f;
    }
  }

  internal static class Links
  {
    public const string GithubProfil = "https://github.com/FGSeRaser";
    public const string Original = "https://github.com/RareExports/Banjo-s-Backpack";

    public static void Oeffnen(string url)
    {
      try
      {
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
      }
      catch
      {
      }
    }
  }

  internal static class Fehler
  {
    public static void Zeigen(Exception ex)
    {
      try
      {
        string log = Path.Combine(Application.StartupPath, "fehler.log");
        File.AppendAllText(log, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine + ex + Environment.NewLine + Environment.NewLine);
      }
      catch
      {
      }
      MessageBox.Show(
        Sp.T("Da ist etwas schiefgelaufen:", "Something went wrong:") + Environment.NewLine + Environment.NewLine + ex.Message
          + Environment.NewLine + Environment.NewLine + Sp.T("Details stehen in \"fehler.log\" im Programmordner.", "Details are in \"fehler.log\" in the program folder."),
        Sp.T("Banjo's Backpack – Fehler", "Banjo's Backpack – Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
  }
}
