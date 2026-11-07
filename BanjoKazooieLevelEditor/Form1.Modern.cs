// Banjo's Backpack – Modern Edition · FGSeRaser (https://github.com/FGSeRaser)
// Erweiterungen des Hauptfensters: Schnellleiste, Modus-Knöpfe, Ansicht-Menü,
// Einsteiger-Assistent, Emulator-Start. Die Original-Logik in Form1.cs bleibt unangetastet.

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using BanjoKazooieLevelEditor.Modern;

namespace BanjoKazooieLevelEditor
{
  public partial class Form1
  {
    private const int LeistenHoehe = 46;

    private ToolStrip schnellleiste;
    private ToolStripButton sl_rom;
    private ToolStripButton sl_level;
    private ToolStripButton sl_speichern;
    private ToolStripButton sl_neueRom;
    private ToolStripButton sl_testen;
    private ToolStripButton sl_undo;
    private ToolStripButton sl_redo;
    private ToolStripButton[] sl_modi;
    private ToolStripButton sl_assistent;
    private ToolStripLabel sl_status;
    private Assistent assistent;
    private readonly DateTime sitzungsStart = DateTime.Now;
    private string zuletztGespeicherteRom = "";

    // ------------------------------------------------------------------
    // Zustand für Schnellleiste und Assistent
    // ------------------------------------------------------------------
    internal bool RomGeladen
    {
      get { return !string.IsNullOrEmpty(this.inRom) && this.rom != null; }
    }

    internal bool LevelGeladen
    {
      get { return this.save_tsmi.Enabled; }
    }

    internal string RomPfad
    {
      get { return this.inRom; }
    }

    /// <summary>Die zuletzt in dieser Sitzung gespeicherte ROM (oder "").</summary>
    internal string GespeicherteRom
    {
      get
      {
        string kandidat = "";
        DateTime zeit = this.sitzungsStart;
        foreach (string pfad in new[] { this.zuletztGespeicherteRom, this.inRom })
        {
          try
          {
            if (!string.IsNullOrEmpty(pfad) && File.Exists(pfad))
            {
              DateTime t = File.GetLastWriteTime(pfad);
              if (t > zeit)
              {
                zeit = t;
                kandidat = pfad;
              }
            }
          }
          catch
          {
          }
        }
        return kandidat;
      }
    }

    internal int Modus
    {
      get { return this.mode_cb.SelectedIndex; }
      set
      {
        if (value >= 0 && value < this.mode_cb.Items.Count)
          this.mode_cb.SelectedIndex = value;
      }
    }

    // ------------------------------------------------------------------
    // Aktionen (lösen die vorhandenen Menüpunkte aus)
    // ------------------------------------------------------------------
    internal void Aktion_RomOeffnen()
    {
      this.openRomToolStripMenuItem.PerformClick();
    }

    internal void Aktion_LevelWaehlen()
    {
      if (!this.RomGeladen)
      {
        Hinweis(Sp.T("Bitte öffne zuerst eine ROM.", "Please open a ROM first."));
        return;
      }
      this.openSetupFileToolStripMenuItem.PerformClick();
    }

    internal void Aktion_InRomSpeichern()
    {
      if (!this.LevelGeladen)
      {
        Hinweis(Sp.T("Es ist noch kein Level geöffnet – es gibt nichts zu speichern.", "No level is open yet – there is nothing to save."));
        return;
      }
      this.save_tsmi.PerformClick();
    }

    internal void Aktion_AlsNeueRomSpeichern()
    {
      if (!this.LevelGeladen)
      {
        Hinweis(Sp.T("Es ist noch kein Level geöffnet – es gibt nichts zu speichern.", "No level is open yet – there is nothing to save."));
        return;
      }
      DateTime vorher = DateTime.Now.AddSeconds(-2);
      this.saveAsRomToolStripMenuItem.PerformClick();
      try
      {
        string ziel = this.saveAsRomFileDialog.FileName;
        if (!string.IsNullOrEmpty(ziel) && File.Exists(ziel) && File.GetLastWriteTime(ziel) >= vorher)
        {
          this.zuletztGespeicherteRom = ziel;
          Einstellungen.LetzteGespeicherteRom = ziel;
        }
      }
      catch
      {
      }
    }

    internal void Aktion_Testen()
    {
      string rom = this.GespeicherteRom;
      if (rom == "" && this.LevelGeladen)
      {
        DialogResult r = MessageBox.Show(
          Sp.T("Deine Änderungen sind noch nicht gespeichert.", "Your changes have not been saved yet.") + Environment.NewLine + Environment.NewLine
            + Sp.T("Ja = als neue ROM speichern und testen", "Yes = save as new ROM and test") + Environment.NewLine
            + Sp.T("Nein = die geöffnete ROM ohne Änderungen testen", "No = test the opened ROM without changes"),
          Sp.T("Vor dem Testen speichern?", "Save before testing?"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        if (r == DialogResult.Cancel)
          return;
        if (r == DialogResult.Yes)
        {
          this.Aktion_AlsNeueRomSpeichern();
          rom = this.GespeicherteRom;
          if (rom == "")
            return;
        }
      }
      if (rom == "")
        rom = File.Exists(Einstellungen.LetzteGespeicherteRom) ? Einstellungen.LetzteGespeicherteRom : this.inRom;
      if (string.IsNullOrEmpty(rom) || !File.Exists(rom))
      {
        Hinweis(Sp.T("Es gibt noch keine ROM zum Testen. Öffne eine ROM und speichere dein Level.", "There is no ROM to test yet. Open a ROM and save your level."));
        return;
      }

      string emu = Einstellungen.Emulator;
      if (string.IsNullOrEmpty(emu) || !File.Exists(emu))
      {
        emu = EmulatorWaehlen();
        if (emu == "")
          return;
      }

      try
      {
        Process.Start(new ProcessStartInfo(emu, "\"" + rom + "\"")
        {
          UseShellExecute = false,
          WorkingDirectory = Path.GetDirectoryName(emu)
        });
      }
      catch (Exception ex)
      {
        MessageBox.Show(Sp.T("Der Emulator konnte nicht gestartet werden:", "The emulator could not be started:") + Environment.NewLine + ex.Message,
          Sp.T("Testen", "Test"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
    }

    internal string EmulatorWaehlen()
    {
      using (OpenFileDialog d = new OpenFileDialog())
      {
        d.Title = Sp.T("Emulator auswählen (z. B. Project64.exe, ares.exe, simple64-gui.exe)", "Choose emulator (e.g. Project64.exe, ares.exe, simple64-gui.exe)");
        d.Filter = Sp.T("Programme (*.exe)|*.exe", "Programs (*.exe)|*.exe");
        if (d.ShowDialog(this) != DialogResult.OK)
          return "";
        Einstellungen.Emulator = d.FileName;
        return d.FileName;
      }
    }

    private void Hinweis(string text)
    {
      MessageBox.Show(this, text, "Banjo's Backpack", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    internal void AssistentZeigen()
    {
      if (this.assistent == null || this.assistent.IsDisposed)
      {
        this.assistent = new Assistent(this);
        Point p = this.PointToScreen(new Point(this.LevelViewer.Left + 16, this.LevelViewer.Top + 16));
        this.assistent.StartPosition = FormStartPosition.Manual;
        this.assistent.Location = p;
        this.assistent.Show(this);
      }
      else
      {
        this.assistent.Activate();
      }
    }

    // ------------------------------------------------------------------
    // Aufbau
    // ------------------------------------------------------------------
    private void ModernInit()
    {
      try
      {
        this.ModusTexteUebersetzen();
        if (Einstellungen.Schnellleiste)
          this.SchnellleisteEinbauen();
        this.AnsichtMenueEinbauen();
        this.HilfeMenueErweitern();
        this.TooltipsSetzen();

        this.MinimumSize = this.Size;
        this.StartPosition = FormStartPosition.CenterScreen;
        // Etwas größeres Startfenster (wenn der Bildschirm es hergibt) – die Anker verteilen den Platz
        Rectangle frei = Screen.PrimaryScreen.WorkingArea;
        int breite = Math.Min(Math.Max(this.Width, 1300), frei.Width);
        int hoehe = Math.Min(Math.Max(this.Height, 780), frei.Height);
        if (breite >= this.MinimumSize.Width && hoehe >= this.MinimumSize.Height)
          this.Size = new Size(breite, hoehe);

        ModernUI.FensterAnpassen(this);
        foreach (ToolStripItem item in this.cameraMenu.Items)
          Uebersetzung.ItemAnwenden(item);

        Application.Idle += this.ZustandAbgleichen;
        this.FormClosed += (s, e) => Application.Idle -= this.ZustandAbgleichen;
        this.Shown += (s, e) =>
        {
          if (Einstellungen.AssistentBeimStart)
            this.AssistentZeigen();
        };
      }
      catch (Exception ex)
      {
        // Die Modern-Oberfläche darf den Editor nie blockieren
        Debug.WriteLine(ex);
      }
    }

    private void ModusTexteUebersetzen()
    {
      if (!Uebersetzung.Aktiv)
        return;
      for (int i = 0; i < this.mode_cb.Items.Count; i++)
        this.mode_cb.Items[i] = Uebersetzung.Uebersetzen(this.mode_cb.Items[i].ToString());
    }

    private ToolStripButton Knopf(string text, char symbol, string tooltip, EventHandler klick)
    {
      bool dunkel = Theme.Aktiv;
      ToolStripButton b = new ToolStripButton(text)
      {
        DisplayStyle = Symbole.Verfuegbar ? ToolStripItemDisplayStyle.ImageAndText : ToolStripItemDisplayStyle.Text,
        TextImageRelation = TextImageRelation.ImageBeforeText,
        Image = Symbole.Bild(symbol, 20, dunkel ? Farben.Text : Color.FromArgb(40, 40, 40)),
        ToolTipText = tooltip,
        AutoToolTip = false,
        Padding = new Padding(6, 0, 6, 0),
        Margin = new Padding(1, 6, 1, 6),
        ImageScaling = ToolStripItemImageScaling.None,
      };
      b.Click += klick;
      return b;
    }

    private void SchnellleisteEinbauen()
    {
      bool dunkel = Theme.Aktiv;

      this.schnellleiste = new ToolStrip
      {
        Name = "schnellleiste",
        GripStyle = ToolStripGripStyle.Hidden,
        AutoSize = false,
        Dock = DockStyle.None,
        Height = LeistenHoehe,
        Font = new Font("Segoe UI", 9.5f),
        Padding = new Padding(8, 0, 8, 0),
        ImageScalingSize = new Size(20, 20),
        RenderMode = ToolStripRenderMode.ManagerRenderMode,
        CanOverflow = true,
        BackColor = dunkel ? Farben.Flaeche : Farben.HellLeiste,
      };

      this.sl_rom = Knopf(Sp.T("ROM öffnen", "Open ROM"), Symbole.RomOeffnen, Sp.T("Banjo-Kazooie-ROM (US 1.0, .z64) öffnen", "Open Banjo-Kazooie ROM (US 1.0, .z64)"), (s, e) => this.Aktion_RomOeffnen());
      this.sl_level = Knopf(Sp.T("Level wählen", "Choose level"), Symbole.Level, Sp.T("Ein Level aus der ROM zum Bearbeiten öffnen (Strg+O)", "Open a level from the ROM for editing (Ctrl+O)"), (s, e) => this.Aktion_LevelWaehlen());
      this.sl_speichern = Knopf(Sp.T("Speichern", "Save"), Symbole.Speichern, Sp.T("Änderungen in die geöffnete ROM schreiben (Strg+S)", "Write changes into the opened ROM (Ctrl+S)"), (s, e) => this.Aktion_InRomSpeichern());
      this.sl_neueRom = Knopf(Sp.T("Neue ROM…", "New ROM…"), Symbole.SpeichernUnter, Sp.T("Als neue ROM-Datei speichern – das Original bleibt unverändert (Strg+Umschalt+S)", "Save as a new ROM file – the original stays untouched (Ctrl+Shift+S)"), (s, e) => this.Aktion_AlsNeueRomSpeichern());
      this.sl_testen = Knopf(Sp.T("Testen", "Test"), Symbole.Abspielen, Sp.T("Gespeicherte ROM im Emulator starten (F5)", "Start the saved ROM in the emulator (F5)"), (s, e) => this.Aktion_Testen());
      this.sl_testen.Tag = "akzent";
      this.sl_testen.Font = new Font("Segoe UI Semibold", 9.5f);
      this.sl_testen.Image = Symbole.Bild(Symbole.Abspielen, 20, Farben.AkzentText);
      if (!dunkel)
      {
        this.sl_testen.BackColor = Farben.Akzent;
        this.sl_testen.ForeColor = Farben.AkzentText;
      }

      this.sl_undo = Knopf("", Symbole.Rueckgaengig, Sp.T("Rückgängig (Strg+Z)", "Undo (Ctrl+Z)"), (s, e) => this.undoToolStripMenuItem.PerformClick());
      this.sl_redo = Knopf("", Symbole.Wiederholen, Sp.T("Wiederholen (Strg+Y)", "Redo (Ctrl+Y)"), (s, e) => this.redoToolStripMenuItem.PerformClick());
      if (!Symbole.Verfuegbar)
      {
        this.sl_undo.Text = "↶";
        this.sl_redo.Text = "↷";
      }

      ToolStripLabel modusLabel = new ToolStripLabel(Sp.T("Modus:", "Mode:"))
      {
        ForeColor = dunkel ? Farben.TextGedimmt : Color.DimGray,
        Margin = new Padding(8, 0, 2, 0),
      };
      string[] modusNamen = { Sp.T("Auswählen", "Select"), Sp.T("Erstellen", "Create"), Sp.T("Kameras", "Cameras"), Sp.T("Pfade", "Paths") };
      string[] modusHilfe =
      {
        Sp.T("Objekte anklicken, verschieben, drehen, löschen", "Click, move, rotate and delete objects"),
        Sp.T("Rechtsklick in die 3D-Ansicht setzt ein neues Objekt", "Right-click in the 3D view places a new object"),
        Sp.T("Spiel- und Zwischensequenz-Kameras bearbeiten", "Edit gameplay and cutscene cameras"),
        Sp.T("Laufwege von Gegnern und Objekten bearbeiten", "Edit movement paths of enemies and objects"),
      };
      this.sl_modi = new ToolStripButton[4];
      for (int i = 0; i < 4; i++)
      {
        int index = i;
        this.sl_modi[i] = new ToolStripButton(modusNamen[i])
        {
          DisplayStyle = ToolStripItemDisplayStyle.Text,
          ToolTipText = modusHilfe[i],
          AutoToolTip = false,
          Padding = new Padding(4, 0, 4, 0),
          Margin = new Padding(1, 8, 1, 8),
        };
        this.sl_modi[i].Click += (s, e) => this.Modus = index;
      }

      this.sl_assistent = Knopf(Sp.T("Assistent", "Assistant"), Symbole.Assistent, Sp.T("Schritt-für-Schritt-Hilfe öffnen", "Open step-by-step help"), (s, e) => this.AssistentZeigen());
      this.sl_assistent.Alignment = ToolStripItemAlignment.Right;
      this.sl_status = new ToolStripLabel("")
      {
        Alignment = ToolStripItemAlignment.Right,
        ForeColor = dunkel ? Farben.TextGedimmt : Color.DimGray,
        Margin = new Padding(4, 0, 8, 0),
      };

      this.schnellleiste.Items.AddRange(new ToolStripItem[]
      {
        this.sl_rom, this.sl_level, new ToolStripSeparator(),
        this.sl_speichern, this.sl_neueRom, this.sl_testen, new ToolStripSeparator(),
        this.sl_undo, this.sl_redo, new ToolStripSeparator(),
        modusLabel, this.sl_modi[0], this.sl_modi[1], this.sl_modi[2], this.sl_modi[3],
        this.sl_assistent, this.sl_status,
      });

      // Platz schaffen: Fenster wächst nach unten, alles unterhalb der Menüleiste rutscht nach unten
      int menueUnten = this.menuStrip1.Bottom;
      this.ClientSize = new Size(this.ClientSize.Width, this.ClientSize.Height + LeistenHoehe);
      foreach (Control c in this.Controls)
      {
        if (c == this.menuStrip1 || c == this.toolStrip1 || c.Dock != DockStyle.None)
          continue;
        if (c.Top < menueUnten)
          continue;
        bool untenVerankert = (c.Anchor & AnchorStyles.Bottom) == AnchorStyles.Bottom;
        c.Top += LeistenHoehe;
        if (untenVerankert)
          c.Height -= LeistenHoehe;
      }

      this.schnellleiste.Location = new Point(0, menueUnten);
      this.schnellleiste.Width = this.ClientSize.Width;
      this.schnellleiste.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.Controls.Add(this.schnellleiste);
      this.schnellleiste.BringToFront();

    }

    private void AnsichtMenueEinbauen()
    {
      ToolStripMenuItem ansicht = new ToolStripMenuItem(Sp.T("Ansicht", "View"));

      ToolStripMenuItem dunkel = new ToolStripMenuItem(Sp.T("Dunkles Design", "Dark theme")) { Checked = Einstellungen.DunklesDesign, CheckOnClick = true };
      dunkel.CheckedChanged += (s, e) =>
      {
        Einstellungen.DunklesDesign = dunkel.Checked;
        NeustartHinweis();
      };

      // Sprache / Language
      ToolStripMenuItem sprache = new ToolStripMenuItem("Sprache / Language");
      ToolStripMenuItem de = new ToolStripMenuItem("Deutsch") { Checked = Einstellungen.Deutsch };
      ToolStripMenuItem en = new ToolStripMenuItem("English") { Checked = !Einstellungen.Deutsch };
      de.Click += (s, e) => this.SpracheWechseln(true);
      en.Click += (s, e) => this.SpracheWechseln(false);
      sprache.DropDownItems.AddRange(new ToolStripItem[] { de, en });

      ToolStripMenuItem leiste = new ToolStripMenuItem(Sp.T("Schnellleiste anzeigen", "Show quick bar")) { Checked = Einstellungen.Schnellleiste, CheckOnClick = true };
      leiste.CheckedChanged += (s, e) =>
      {
        Einstellungen.Schnellleiste = leiste.Checked;
        NeustartHinweis();
      };

      ToolStripMenuItem assistentStart = new ToolStripMenuItem(Sp.T("Assistent beim Start zeigen", "Show assistant on startup")) { Checked = Einstellungen.AssistentBeimStart, CheckOnClick = true };
      assistentStart.CheckedChanged += (s, e) => Einstellungen.AssistentBeimStart = assistentStart.Checked;

      ToolStripMenuItem emulator = new ToolStripMenuItem(Sp.T("Emulator festlegen…", "Set emulator…"));
      emulator.Click += (s, e) => this.EmulatorWaehlen();

      ToolStripMenuItem testen = new ToolStripMenuItem(Sp.T("Im Emulator testen", "Test in emulator")) { ShortcutKeys = Keys.F5 };
      testen.Click += (s, e) => this.Aktion_Testen();

      ansicht.DropDownItems.AddRange(new ToolStripItem[]
      {
        sprache, new ToolStripSeparator(), dunkel, leiste, assistentStart, new ToolStripSeparator(), testen, emulator
      });

      int hilfeIndex = this.menuStrip1.Items.IndexOf(this.helpToolStripMenuItem);
      this.menuStrip1.Items.Insert(hilfeIndex < 0 ? this.menuStrip1.Items.Count : hilfeIndex, ansicht);
    }

    private void NeustartHinweis()
    {
      if (MessageBox.Show(this,
            Sp.T("Die Änderung wird nach einem Neustart aktiv. Jetzt neu starten?\r\n(Nicht gespeicherte Änderungen gehen verloren.)",
                 "The change takes effect after a restart. Restart now?\r\n(Unsaved changes will be lost.)"),
            "Banjo's Backpack", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        Application.Restart();
    }

    private void SpracheWechseln(bool deutsch)
    {
      if (Einstellungen.Deutsch == deutsch)
        return;
      Einstellungen.Deutsch = deutsch;
      this.NeustartHinweis();
    }

    private void HilfeMenueErweitern()
    {
      ToolStripMenuItem assistentItem = new ToolStripMenuItem(Sp.T("Einsteiger-Assistent", "Beginner assistant")) { ShortcutKeys = Keys.F1 };
      assistentItem.Click += (s, e) => this.AssistentZeigen();

      ToolStripMenuItem steuerung = new ToolStripMenuItem(Sp.T("Steuerung & Tastenkürzel", "Controls & shortcuts"));
      steuerung.Click += (s, e) => MessageBox.Show(this, Assistent.SteuerungsText, Sp.T("Steuerung & Tastenkürzel", "Controls & shortcuts"),
        MessageBoxButtons.OK, MessageBoxIcon.Information);

      ToolStripMenuItem fg = new ToolStripMenuItem(Sp.T("Modern Edition von FGSeRaser (GitHub)", "Modern Edition by FGSeRaser (GitHub)"));
      fg.Click += (s, e) => Links.Oeffnen(Links.GithubProfil);

      ToolStripMenuItem original = new ToolStripMenuItem(Sp.T("Original-Projekt (Skill, Adam, Cooliscool)", "Original project (Skill, Adam, Cooliscool)"));
      original.Click += (s, e) => Links.Oeffnen(Links.Original);

      this.helpToolStripMenuItem.DropDownItems.Insert(0, assistentItem);
      this.helpToolStripMenuItem.DropDownItems.Insert(1, steuerung);
      this.helpToolStripMenuItem.DropDownItems.Insert(2, new ToolStripSeparator());
      this.helpToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
      this.helpToolStripMenuItem.DropDownItems.Add(fg);
      this.helpToolStripMenuItem.DropDownItems.Add(original);
    }

    private void TooltipsSetzen()
    {
      if (!Uebersetzung.Aktiv)
        return;
      foreach (ToolStripItem item in this.toolStrip1.Items)
      {
        string hinweis;
        if (item.Name != null && Uebersetzung.Hinweise.TryGetValue(item.Name, out hinweis))
        {
          item.ToolTipText = hinweis;
          item.AutoToolTip = false;
        }
      }
    }

    // ------------------------------------------------------------------
    // Laufend: Knöpfe aktivieren/deaktivieren, Titel und Status
    // ------------------------------------------------------------------
    private void ZustandAbgleichen(object sender, EventArgs e)
    {
      try
      {
        // Fenstertitel aufräumen ("Banjo's Backpack: ROM:C:\…" → Dateiname)
        string titel = Form1.TitelFuerRom(this.inRom);
        if (this.Text != titel)
          this.Text = titel;

        if (this.schnellleiste == null)
          return;

        bool rom = this.RomGeladen;
        bool level = this.LevelGeladen;
        Setzen(this.sl_level, rom);
        Setzen(this.sl_speichern, level);
        Setzen(this.sl_neueRom, level);
        Setzen(this.sl_testen, rom);
        Setzen(this.sl_undo, level);
        Setzen(this.sl_redo, level);
        for (int i = 0; i < this.sl_modi.Length; i++)
        {
          Setzen(this.sl_modi[i], level && this.mode_cb.Enabled);
          bool aktiv = this.mode_cb.SelectedIndex == i;
          if (this.sl_modi[i].Checked != aktiv)
            this.sl_modi[i].Checked = aktiv;
        }

        string status = !rom ? Sp.T("Keine ROM geladen", "No ROM loaded") : (level ? Sp.T("Level geladen · bereit", "Level loaded · ready") : Sp.T("ROM geladen · wähle ein Level", "ROM loaded · choose a level"));
        if (this.sl_status.Text != status)
          this.sl_status.Text = status;
      }
      catch
      {
      }
    }

    private static void Setzen(ToolStripItem item, bool an)
    {
      if (item.Enabled != an)
        item.Enabled = an;
    }

    internal static string TitelFuerRom(string romPfad)
    {
      string basis = "Banjo's Backpack – Modern Edition";
      if (string.IsNullOrEmpty(romPfad))
        return basis;
      try
      {
        return basis + " · " + Path.GetFileName(romPfad);
      }
      catch
      {
        return basis;
      }
    }

    /// <summary>Ersetzt das alte Lesen von mw.ini für den Fenstertitel (ohne die Datei offen zu lassen).</summary>
    internal static string TitelAusIni()
    {
      try
      {
        string ini = Path.Combine(Application.StartupPath, "resources\\mw.ini");
        if (File.Exists(ini))
        {
          foreach (string zeile in File.ReadAllLines(ini))
            if (zeile.StartsWith("ROM:"))
              return TitelFuerRom(zeile.Substring(4).Trim());
        }
      }
      catch
      {
      }
      return TitelFuerRom("");
    }
  }
}
