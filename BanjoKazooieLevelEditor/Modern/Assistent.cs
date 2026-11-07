// Banjo's Backpack – Modern Edition · FGSeRaser (https://github.com/FGSeRaser)
// Einsteiger-Assistent: führt in 5 Schritten von der ROM bis zum Testen im Emulator.
// Das Fenster bleibt offen, während man im Editor arbeitet, und hakt Schritte automatisch ab.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BanjoKazooieLevelEditor.Modern
{
  internal class Assistent : Form
  {
    public static string SteuerungsText
    {
      get
      {
        if (Sp.Deutsch)
          return
        "3D-ANSICHT (zuerst einmal hineinklicken)\r\n"
      + "  W / S          vor / zurück\r\n"
      + "  A / D          seitwärts\r\n"
      + "  E / Q          hoch / runter\r\n"
      + "  Linke Maustaste halten + ziehen   umsehen\r\n"
      + "  Regler „Kamera-Tempo“ oben rechts   Flug-Tempo\r\n\r\n"
      + "OBJEKTE\r\n"
      + "  Modus „Erstellen“ + Rechtsklick   neues Objekt setzen\r\n"
      + "  Modus „Auswählen“ + Klick          Objekt auswählen\r\n"
      + "  Umschalt + Klick       mehrere auswählen\r\n"
      + "  Alt+M / Alt+R / Alt+S  bewegen / drehen / skalieren\r\n"
      + "  Alt+D                  duplizieren\r\n"
      + "  Alt+Entf               löschen\r\n"
      + "  Esc                    Auswahl aufheben\r\n"
      + "  Strg+B                 Rechteck-Auswahl\r\n\r\n"
      + "ALLGEMEIN\r\n"
      + "  Strg+O  Level öffnen     Strg+S  in ROM speichern\r\n"
      + "  Strg+Umschalt+S  als neue ROM speichern\r\n"
      + "  Strg+Z / Strg+Y  rückgängig / wiederholen\r\n"
      + "  F5  im Emulator testen     F1  dieser Assistent";
        return
        "3D VIEW (click into it once first)\r\n"
      + "  W / S          forward / back\r\n"
      + "  A / D          strafe\r\n"
      + "  E / Q          up / down\r\n"
      + "  Hold left mouse button + drag   look around\r\n"
      + "  “Camera Speed” slider top right   flight speed\r\n\r\n"
      + "OBJECTS\r\n"
      + "  Mode “Create” + right-click   place new object\r\n"
      + "  Mode “Select” + click         select object\r\n"
      + "  Shift + click          select multiple\r\n"
      + "  Alt+M / Alt+R / Alt+S  move / rotate / scale\r\n"
      + "  Alt+D                  duplicate\r\n"
      + "  Alt+Del                delete\r\n"
      + "  Esc                    deselect\r\n"
      + "  Ctrl+B                 rectangle select\r\n\r\n"
      + "GENERAL\r\n"
      + "  Ctrl+O  open level     Ctrl+S  save to ROM\r\n"
      + "  Ctrl+Shift+S  save as new ROM\r\n"
      + "  Ctrl+Z / Ctrl+Y  undo / redo\r\n"
      + "  F5  test in emulator     F1  this assistant";
      }
    }

    private class Schritt
    {
      public string Titel;
      public string Text;
      public Func<bool> Fertig;
      public Func<bool> Moeglich;
      public Panel Karte;
      public Label Nummer;
      public Label TitelLabel;
      public List<Button> Knoepfe = new List<Button>();
    }

    private readonly Form1 editor;
    private readonly List<Schritt> schritte = new List<Schritt>();
    private readonly Timer aktualisieren;
    private readonly bool dunkel = Theme.Aktiv;
    private bool bearbeitetGeklickt;

    private Color Hg { get { return dunkel ? Farben.Hintergrund : Color.FromArgb(250, 248, 244); } }
    private Color Karte { get { return dunkel ? Farben.Flaeche : Color.White; } }
    private Color Txt { get { return dunkel ? Farben.Text : Color.FromArgb(30, 30, 30); } }
    private Color Gedimmt { get { return dunkel ? Farben.TextGedimmt : Color.DimGray; } }
    private Color RandFarbe { get { return dunkel ? Farben.Rand : Farben.HellRand; } }

    public Assistent(Form1 editor)
    {
      this.editor = editor;
      this.Text = Sp.T("Einsteiger-Assistent", "Beginner assistant");
      this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
      this.ShowInTaskbar = false;
      this.ClientSize = new Size(410, 640);
      this.MinimumSize = new Size(360, 420);
      this.Font = new Font("Segoe UI", 9.5f);
      this.BackColor = Hg;
      this.ForeColor = Txt;
      this.AutoScaleMode = AutoScaleMode.None;
      this.KeyPreview = true;
      this.KeyDown += (s, e) =>
      {
        if (e.KeyCode == Keys.Escape)
          this.Close();
      };
      if (dunkel)
        Theme.DunkleTitelleiste(this);

      this.SchritteAnlegen();
      this.Aufbauen();

      this.aktualisieren = new Timer { Interval = 300 };
      this.aktualisieren.Tick += (s, e) => this.Aktualisieren();
      this.aktualisieren.Start();
      this.FormClosed += (s, e) => this.aktualisieren.Stop();
      this.Aktualisieren();
    }

    private void SchritteAnlegen()
    {
      Schritt rom = new Schritt
      {
        Titel = Sp.T("ROM öffnen", "Open ROM"),
        Text = Sp.T("Wähle deine Banjo-Kazooie-ROM: US-Version 1.0 (NTSC) als .z64. ", "Choose your Banjo-Kazooie ROM: US version 1.0 (NTSC) as .z64. ")
             + Sp.T("Beim ersten Öffnen wird sie erweitert – lass dabei eine saubere Sicherungskopie anlegen (Ja klicken).", "It gets extended the first time you open it – let it create a clean backup (click Yes)."),
        Fertig = () => editor.RomGeladen,
        Moeglich = () => true,
      };
      rom.Knoepfe.Add(KnopfErstellen(Sp.T("ROM öffnen…", "Open ROM…"), true, (s, e) => editor.Aktion_RomOeffnen()));
      schritte.Add(rom);

      Schritt level = new Schritt
      {
        Titel = Sp.T("Level wählen", "Choose level"),
        Text = Sp.T("Such dir ein Level aus, zum Beispiel Mumbo's Mountain. Es erscheint danach in der 3D-Ansicht.", "Pick a level, for example Mumbo's Mountain. It then appears in the 3D view."),
        Fertig = () => editor.LevelGeladen,
        Moeglich = () => editor.RomGeladen,
      };
      level.Knoepfe.Add(KnopfErstellen(Sp.T("Level wählen…", "Choose level…"), true, (s, e) => editor.Aktion_LevelWaehlen()));
      schritte.Add(level);

      Schritt bauen = new Schritt
      {
        Titel = Sp.T("Level umbauen", "Remodel the level"),
        Text = Sp.T("Klick einmal in die 3D-Ansicht. Fliegen mit W/A/S/D, hoch/runter mit E/Q, umsehen mit gedrückter linker Maustaste.\n", "Click into the 3D view once. Fly with W/A/S/D, up/down with E/Q, look around by holding the left mouse button.\n")
             + Sp.T("• Neues Objekt: Modus „Erstellen“, dann Rechtsklick an die Stelle.\n", "• New object: mode “Create”, then right-click the spot.\n")
             + Sp.T("• Ändern: Modus „Auswählen“, Objekt anklicken, dann Bewegen/Drehen/Löschen.", "• Change: mode “Select”, click an object, then move/rotate/delete."),
        Fertig = () => bearbeitetGeklickt,
        Moeglich = () => editor.LevelGeladen,
      };
      bauen.Knoepfe.Add(KnopfErstellen(Sp.T("Erstellen", "Create"), false, (s, e) => { editor.Modus = 1; bearbeitetGeklickt = true; editor.Focus(); }));
      bauen.Knoepfe.Add(KnopfErstellen(Sp.T("Auswählen", "Select"), false, (s, e) => { editor.Modus = 0; bearbeitetGeklickt = true; editor.Focus(); }));
      bauen.Knoepfe.Add(KnopfErstellen(Sp.T("Steuerung", "Controls"), false, (s, e) =>
        MessageBox.Show(this, SteuerungsText, Sp.T("Steuerung & Tastenkürzel", "Controls & shortcuts"), MessageBoxButtons.OK, MessageBoxIcon.Information)));
      schritte.Add(bauen);

      Schritt speichern = new Schritt
      {
        Titel = Sp.T("Speichern", "Save"),
        Text = Sp.T("„Neue ROM…“ speichert alles in eine neue Datei – deine bisherige ROM bleibt sicher. ", "“New ROM…” saves everything into a new file – your previous ROM stays safe. ")
             + Sp.T("„In ROM speichern“ überschreibt die gerade geöffnete ROM.", "“Save to ROM” overwrites the currently opened ROM."),
        Fertig = () => editor.GespeicherteRom != "",
        Moeglich = () => editor.LevelGeladen,
      };
      speichern.Knoepfe.Add(KnopfErstellen(Sp.T("Neue ROM…", "New ROM…"), true, (s, e) => editor.Aktion_AlsNeueRomSpeichern()));
      speichern.Knoepfe.Add(KnopfErstellen(Sp.T("In ROM speichern", "Save to ROM"), false, (s, e) => editor.Aktion_InRomSpeichern()));
      schritte.Add(speichern);

      Schritt testen = new Schritt
      {
        Titel = Sp.T("Im Emulator testen", "Test in emulator"),
        Text = Sp.T("Startet deine gespeicherte ROM direkt im Emulator (z. B. Project64 oder ares). ", "Starts your saved ROM directly in the emulator (e.g. Project64 or ares). ")
             + Sp.T("Beim ersten Mal wählst du die .exe deines Emulators aus. Danach reicht F5.", "The first time you pick your emulator's .exe. After that, F5 is enough."),
        Fertig = () => false,
        Moeglich = () => editor.RomGeladen,
      };
      testen.Knoepfe.Add(KnopfErstellen(Sp.T("▶  Testen", "▶  Test"), true, (s, e) => editor.Aktion_Testen()));
      testen.Knoepfe.Add(KnopfErstellen(Sp.T("Emulator wählen", "Choose emulator"), false, (s, e) => editor.EmulatorWaehlen()));
      schritte.Add(testen);
    }

    private Button KnopfErstellen(string text, bool hauptknopf, EventHandler klick)
    {
      Button b = new Button
      {
        Text = text,
        AutoSize = true,
        AutoSizeMode = AutoSizeMode.GrowAndShrink,
        Padding = new Padding(8, 2, 8, 2),
        Margin = new Padding(0, 0, 6, 0),
        FlatStyle = FlatStyle.Flat,
        Cursor = Cursors.Hand,
        Tag = hauptknopf ? "haupt" : null,
      };
      b.FlatAppearance.BorderColor = hauptknopf ? Farben.Akzent : RandFarbe;
      b.FlatAppearance.MouseOverBackColor = hauptknopf ? Color.FromArgb(255, 186, 80) : (dunkel ? Farben.Hover : Color.FromArgb(240, 236, 228));
      b.BackColor = hauptknopf ? Farben.Akzent : (dunkel ? Farben.FlaecheHell : Color.White);
      b.ForeColor = hauptknopf ? Farben.AkzentText : Txt;
      if (hauptknopf)
        b.Font = new Font("Segoe UI Semibold", 9.5f);
      b.Click += klick;
      return b;
    }

    private void Aufbauen()
    {
      Panel kopf = new Panel { Dock = DockStyle.Top, Height = 74, BackColor = Hg, Padding = new Padding(18, 14, 18, 0) };
      Label titel = new Label
      {
        Text = Sp.T("Dein eigenes Banjo in 5 Schritten", "Your own Banjo in 5 steps"),
        Font = new Font("Segoe UI Semibold", 13f),
        ForeColor = Farben.Akzent,
        AutoSize = true,
        Location = new Point(16, 12),
      };
      Label unter = new Label
      {
        Text = Sp.T("Die Haken setzen sich von selbst, sobald ein Schritt erledigt ist.", "Steps tick themselves off as soon as they are done."),
        ForeColor = Gedimmt,
        AutoSize = true,
        Location = new Point(18, 42),
      };
      kopf.Controls.Add(titel);
      kopf.Controls.Add(unter);

      FlowLayoutPanel liste = new FlowLayoutPanel
      {
        Dock = DockStyle.Fill,
        FlowDirection = FlowDirection.TopDown,
        WrapContents = false,
        AutoScroll = true,
        BackColor = Hg,
        Padding = new Padding(12, 4, 12, 8),
      };

      int nr = 1;
      foreach (Schritt s in schritte)
      {
        liste.Controls.Add(KarteErstellen(s, nr++));
      }
      EventHandler breiteAnpassen = (o, e) =>
      {
        int breite = liste.ClientSize.Width - liste.Padding.Horizontal - 6 - (liste.VerticalScroll.Visible ? 0 : SystemInformation.VerticalScrollBarWidth);
        foreach (Control k in liste.Controls)
          k.Width = Math.Max(200, breite);
      };
      liste.SizeChanged += breiteAnpassen;
      this.Load += breiteAnpassen;

      Panel fuss = new Panel { Dock = DockStyle.Bottom, Height = 46, BackColor = Hg };
      CheckBox beimStart = new CheckBox
      {
        Text = Sp.T("Beim Start anzeigen", "Show on startup"),
        Checked = Einstellungen.AssistentBeimStart,
        AutoSize = true,
        ForeColor = Gedimmt,
        Location = new Point(16, 14),
      };
      beimStart.CheckedChanged += (o, e) => Einstellungen.AssistentBeimStart = beimStart.Checked;
      LinkLabel fg = new LinkLabel
      {
        Text = "Modern Edition · FGSeRaser",
        AutoSize = true,
        LinkColor = Farben.Akzent,
        ActiveLinkColor = Farben.AkzentGedrueckt,
        VisitedLinkColor = Farben.Akzent,
        Anchor = AnchorStyles.Top | AnchorStyles.Right,
      };
      fg.LinkClicked += (o, e) => Links.Oeffnen(Links.GithubProfil);
      fuss.Controls.Add(beimStart);
      fuss.Controls.Add(fg);
      fuss.Layout += (o, e) => fg.Location = new Point(fuss.ClientSize.Width - fg.Width - 16, 16);

      this.Controls.Add(liste);
      this.Controls.Add(fuss);
      this.Controls.Add(kopf);
    }

    private Panel KarteErstellen(Schritt s, int nummer)
    {
      Panel karte = new Panel
      {
        Width = 370,
        Height = 120,
        BackColor = Karte,
        Padding = new Padding(54, 12, 14, 12),
        Margin = new Padding(0, 0, 0, 10),
      };
      karte.Paint += (o, e) =>
      {
        Color rand = s.Fertig() ? Farben.Gruen : (s.Moeglich() ? Farben.Akzent : RandFarbe);
        using (Pen p = new Pen(rand, 1))
          e.Graphics.DrawRectangle(p, 0, 0, karte.Width - 1, karte.Height - 1);
      };

      Label kreis = new Label
      {
        Text = nummer.ToString(),
        Size = new Size(28, 28),
        Location = new Point(14, 12),
        TextAlign = ContentAlignment.MiddleCenter,
        Font = new Font("Segoe UI Semibold", 10f),
      };
      kreis.Paint += (o, e) =>
      {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.Clear(Karte);
        bool fertig = s.Fertig();
        bool moeglich = s.Moeglich();
        Color fuell = fertig ? Farben.Gruen : (moeglich ? Farben.Akzent : RandFarbe);
        using (SolidBrush b = new SolidBrush(fuell))
          e.Graphics.FillEllipse(b, 1, 1, 25, 25);
        string zeichen = fertig ? "✓" : nummer.ToString();
        TextRenderer.DrawText(e.Graphics, zeichen, kreis.Font, new Rectangle(0, 0, 28, 28),
          fertig || moeglich ? Farben.AkzentText : Txt, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
      };

      FlowLayoutPanel inhalt = new FlowLayoutPanel
      {
        FlowDirection = FlowDirection.TopDown,
        WrapContents = false,
        AutoSize = true,
        AutoSizeMode = AutoSizeMode.GrowAndShrink,
        Location = new Point(54, 12),
        BackColor = Karte,
        Margin = new Padding(0),
      };
      inhalt.SizeChanged += (o, e) => karte.Height = inhalt.Bottom + 12;

      Label t = new Label
      {
        Text = s.Titel,
        Font = new Font("Segoe UI Semibold", 11f),
        AutoSize = true,
        ForeColor = Txt,
        Margin = new Padding(0, 2, 0, 4),
      };
      Label beschreibung = new Label
      {
        Text = s.Text,
        AutoSize = true,
        MaximumSize = new Size(290, 0),
        ForeColor = Gedimmt,
        Margin = new Padding(0, 0, 0, 8),
      };
      FlowLayoutPanel knoepfe = new FlowLayoutPanel
      {
        AutoSize = true,
        AutoSizeMode = AutoSizeMode.GrowAndShrink,
        WrapContents = true,
        BackColor = Karte,
        Margin = new Padding(0),
      };
      foreach (Button b in s.Knoepfe)
        knoepfe.Controls.Add(b);

      inhalt.Controls.Add(t);
      inhalt.Controls.Add(beschreibung);
      inhalt.Controls.Add(knoepfe);
      karte.Controls.Add(inhalt);
      karte.Controls.Add(kreis);

      // Beschreibung mit der Kartenbreite umbrechen lassen
      karte.SizeChanged += (o, e) =>
      {
        int w = Math.Max(140, karte.Width - karte.Padding.Horizontal);
        beschreibung.MaximumSize = new Size(w, 0);
        knoepfe.MaximumSize = new Size(w, 0);
        karte.Height = inhalt.Bottom + 12;
        karte.Invalidate();
      };

      s.Karte = karte;
      s.Nummer = kreis;
      s.TitelLabel = t;
      return karte;
    }

    private void Aktualisieren()
    {
      if (this.IsDisposed || this.editor.IsDisposed)
      {
        this.aktualisieren.Stop();
        return;
      }
      foreach (Schritt s in schritte)
      {
        bool moeglich = s.Moeglich();
        foreach (Button b in s.Knoepfe)
          if (b.Enabled != moeglich)
            b.Enabled = moeglich;
        Color titelFarbe = moeglich || s.Fertig() ? Txt : Gedimmt;
        if (s.TitelLabel.ForeColor != titelFarbe)
          s.TitelLabel.ForeColor = titelFarbe;
        s.Nummer.Invalidate();
        s.Karte.Invalidate();
      }
    }
  }
}
