// Banjo's Backpack – Modern Edition · FGSeRaser (https://github.com/FGSeRaser)
// Dunkles Design ("Honigwabe") für alle WinForms-Fenster des Editors.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BanjoKazooieLevelEditor.Modern
{
  internal static class Farben
  {
    public static readonly Color Hintergrund = Color.FromArgb(26, 28, 34);
    public static readonly Color Flaeche = Color.FromArgb(34, 37, 45);
    public static readonly Color FlaecheHell = Color.FromArgb(41, 45, 54);
    public static readonly Color Eingabe = Color.FromArgb(43, 47, 56);
    public static readonly Color Rand = Color.FromArgb(58, 63, 75);
    public static readonly Color Hover = Color.FromArgb(52, 57, 70);
    public static readonly Color Text = Color.FromArgb(233, 231, 226);
    public static readonly Color TextGedimmt = Color.FromArgb(150, 156, 168);
    public static readonly Color Akzent = Color.FromArgb(244, 166, 42);        // Honig
    public static readonly Color AkzentGedrueckt = Color.FromArgb(201, 127, 18);
    public static readonly Color AkzentText = Color.FromArgb(30, 22, 8);
    public static readonly Color Auswahl = Color.FromArgb(92, 68, 28);
    public static readonly Color Kazooie = Color.FromArgb(224, 83, 61);        // Rot für Warnungen
    public static readonly Color Gruen = Color.FromArgb(94, 196, 120);

    // Helles Gegenstück für die Schnellleiste, wenn das Original-Design aktiv ist
    public static readonly Color HellLeiste = Color.FromArgb(250, 246, 238);
    public static readonly Color HellRand = Color.FromArgb(222, 214, 200);
  }

  internal static class Theme
  {
    public static bool Aktiv
    {
      get { return Einstellungen.DunklesDesign; }
    }

    // Farben, die WinForms standardmäßig vergibt bzw. die das Original verwendet hat
    private static bool IstStandardHintergrund(Color c)
    {
      if (c.IsEmpty || c.IsSystemColor)
        return true;
      int argb = c.ToArgb();
      return argb == Color.White.ToArgb()
          || argb == Color.FromArgb(229, 229, 229).ToArgb()
          || argb == Color.FromArgb(240, 240, 240).ToArgb()
          || argb == SystemColors.Control.ToArgb()
          || argb == SystemColors.Window.ToArgb()
          || argb == SystemColors.ButtonFace.ToArgb();
    }

    private static bool IstStandardText(Color c)
    {
      if (c.IsEmpty || c.IsSystemColor)
        return true;
      int argb = c.ToArgb();
      return argb == Color.Black.ToArgb() || argb == SystemColors.ControlText.ToArgb() || argb == SystemColors.WindowText.ToArgb();
    }

    private static bool IstFremdeZeichenflaeche(Control c)
    {
      string n = c.GetType().Name;
      return n.Contains("GLControl") || n == "ColorTrackBar" || n == "PictureBox";
    }

    /// <summary>Wendet das dunkle Design auf ein Control und alle Kinder an.</summary>
    public static void Anwenden(Control c)
    {
      if (!Aktiv || c == null || IstFremdeZeichenflaeche(c))
        return;

      try
      {
        AnwendenEinzeln(c);
      }
      catch
      {
        // Ein einzelnes Control darf nie den Editor stoppen
      }

      foreach (Control kind in c.Controls)
        Anwenden(kind);
    }

    private static void AnwendenEinzeln(Control c)
    {
      if (c is Form)
      {
        Form f = (Form)c;
        f.BackColor = Farben.Hintergrund;
        f.ForeColor = Farben.Text;
        DunkleTitelleiste(f);
        return;
      }

      if (c is ToolStrip)
      {
        ToolStrip ts = (ToolStrip)c;
        ts.RenderMode = ToolStripRenderMode.ManagerRenderMode;
        ts.BackColor = c is MenuStrip ? Farben.Hintergrund : Farben.Flaeche;
        ts.ForeColor = Farben.Text;
        foreach (ToolStripItem item in ts.Items)
          ItemAnwenden(item);
        return;
      }

      if (c is Button)
      {
        Button b = (Button)c;
        b.FlatStyle = FlatStyle.Flat;
        b.FlatAppearance.BorderColor = Farben.Rand;
        b.FlatAppearance.MouseOverBackColor = Farben.Hover;
        b.FlatAppearance.MouseDownBackColor = Farben.Auswahl;
        if (IstStandardHintergrund(b.BackColor) || b.BackColor == Color.Transparent)
          b.BackColor = Farben.FlaecheHell;
        if (IstStandardText(b.ForeColor))
          b.ForeColor = Farben.Text;
        b.Cursor = Cursors.Hand;
        return;
      }

      if (c is TextBoxBase)
      {
        TextBoxBase t = (TextBoxBase)c;
        if (IstStandardHintergrund(t.BackColor))
          t.BackColor = Farben.Eingabe;
        if (IstStandardText(t.ForeColor))
          t.ForeColor = Farben.Text;
        if (t.BorderStyle == BorderStyle.Fixed3D)
          t.BorderStyle = BorderStyle.FixedSingle;
        return;
      }

      if (c is ComboBox)
      {
        ComboBox cb = (ComboBox)c;
        cb.FlatStyle = FlatStyle.Flat;
        cb.BackColor = Farben.Eingabe;
        cb.ForeColor = Farben.Text;
        return;
      }

      if (c is ListBox || c is NumericUpDown || c is ListView || c is TreeView)
      {
        if (IstStandardHintergrund(c.BackColor))
          c.BackColor = Farben.Eingabe;
        if (IstStandardText(c.ForeColor))
          c.ForeColor = Farben.Text;
        if (c is ListBox)
          ((ListBox)c).BorderStyle = BorderStyle.FixedSingle;
        return;
      }

      if (c is DataGridView)
      {
        TabelleAnwenden((DataGridView)c);
        return;
      }

      if (c is LinkLabel)
      {
        LinkLabel l = (LinkLabel)c;
        l.LinkColor = Farben.Akzent;
        l.ActiveLinkColor = Color.White;
        l.VisitedLinkColor = Farben.AkzentGedrueckt;
        if (IstStandardHintergrund(l.BackColor))
          l.BackColor = Color.Transparent;
        return;
      }

      if (c is GroupBox)
      {
        if (IstStandardText(c.ForeColor))
          c.ForeColor = Farben.Akzent;
        if (IstStandardHintergrund(c.BackColor))
          c.BackColor = Farben.Flaeche;
        // Kinder sollen wieder normale Textfarbe haben
        foreach (Control kind in c.Controls)
          if (kind.ForeColor.ToArgb() == Farben.Akzent.ToArgb())
            kind.ForeColor = Farben.Text;
        return;
      }

      if (c is TrackBar)
      {
        if (c.Parent != null)
          c.BackColor = c.Parent.BackColor;
        return;
      }

      if (c is TabControl)
      {
        foreach (TabPage p in ((TabControl)c).TabPages)
        {
          p.BackColor = Farben.Flaeche;
          p.ForeColor = Farben.Text;
        }
        return;
      }

      // Label, CheckBox, RadioButton, Panels …
      if (IstStandardHintergrund(c.BackColor))
        c.BackColor = (c is Panel || c is TableLayoutPanel || c is FlowLayoutPanel || c is UserControl) ? Farben.Flaeche : Color.Transparent;
      if (IstStandardText(c.ForeColor))
        c.ForeColor = Farben.Text;

      if (c is CheckBox && ((CheckBox)c).Appearance == Appearance.Button)
      {
        CheckBox k = (CheckBox)c;
        k.FlatStyle = FlatStyle.Flat;
        k.FlatAppearance.BorderColor = Farben.Rand;
        k.FlatAppearance.CheckedBackColor = Farben.Auswahl;
        k.FlatAppearance.MouseOverBackColor = Farben.Hover;
      }
    }

    public static void ItemAnwenden(ToolStripItem item)
    {
      if (!Aktiv)
        return;
      if (item is ToolStripTextBox)
      {
        ToolStripTextBox t = (ToolStripTextBox)item;
        t.BackColor = Farben.Eingabe;
        t.ForeColor = Farben.Text;
        t.BorderStyle = BorderStyle.FixedSingle;
      }
      else if (item is ToolStripComboBox)
      {
        ToolStripComboBox cb = (ToolStripComboBox)item;
        cb.FlatStyle = FlatStyle.Flat;
        cb.BackColor = Farben.Eingabe;
        cb.ForeColor = Farben.Text;
      }
      else
      {
        item.ForeColor = Farben.Text;
      }

      ToolStripDropDownItem dd = item as ToolStripDropDownItem;
      if (dd != null)
        foreach (ToolStripItem kind in dd.DropDownItems)
          ItemAnwenden(kind);
    }

    private static void TabelleAnwenden(DataGridView g)
    {
      g.EnableHeadersVisualStyles = false;
      g.BackgroundColor = Farben.Flaeche;
      g.GridColor = Farben.Rand;
      g.BorderStyle = BorderStyle.None;

      Zellstil(g.DefaultCellStyle, Farben.Eingabe);
      Zellstil(g.RowsDefaultCellStyle, Farben.Eingabe);
      Zellstil(g.AlternatingRowsDefaultCellStyle, Color.FromArgb(48, 52, 62));

      g.ColumnHeadersDefaultCellStyle.BackColor = Farben.Flaeche;
      g.ColumnHeadersDefaultCellStyle.ForeColor = Farben.Akzent;
      g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Farben.Flaeche;
      g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
      g.RowHeadersDefaultCellStyle.BackColor = Farben.Flaeche;
      g.RowHeadersDefaultCellStyle.ForeColor = Farben.Text;
      g.RowHeadersDefaultCellStyle.SelectionBackColor = Farben.Auswahl;
    }

    private static void Zellstil(DataGridViewCellStyle s, Color hintergrund)
    {
      if (s == null)
        return;
      if (IstStandardHintergrund(s.BackColor))
        s.BackColor = hintergrund;
      if (IstStandardText(s.ForeColor))
        s.ForeColor = Farben.Text;
      s.SelectionBackColor = Farben.Auswahl;
      s.SelectionForeColor = Color.White;
    }

    // ---------- Windows-Titelleiste dunkel (Windows 10 2004+ / 11) ----------
    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    public static void DunkleTitelleiste(Form f)
    {
      EventHandler setzen = (s, e) =>
      {
        try
        {
          int an = 1;
          if (DwmSetWindowAttribute(f.Handle, 20, ref an, 4) != 0)
            DwmSetWindowAttribute(f.Handle, 19, ref an, 4);
        }
        catch
        {
        }
      };
      if (f.IsHandleCreated)
        setzen(f, EventArgs.Empty);
      else
        f.HandleCreated += setzen;
    }
  }

  /// <summary>Zeichnet Menüs, Werkzeugleisten und Kontextmenüs im dunklen Design.</summary>
  internal class DunklerRenderer : ToolStripProfessionalRenderer
  {
    public DunklerRenderer() : base(new DunkleFarbtabelle())
    {
      this.RoundedEdges = false;
    }

    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
      if (e.Item.Tag as string == "akzent")
        e.TextColor = Farben.AkzentText;
      else
        e.TextColor = e.Item.Enabled ? Farben.Text : Farben.TextGedimmt;
      base.OnRenderItemText(e);
    }

    protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
    {
      e.ArrowColor = Farben.TextGedimmt;
      base.OnRenderArrow(e);
    }

    protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
    {
      // Hervorgehobener Knopf (z. B. "Testen")
      if (e.Item.Tag as string == "akzent")
      {
        Rectangle r = new Rectangle(1, 2, e.Item.Width - 3, e.Item.Height - 5);
        Color farbe = e.Item.Pressed ? Farben.AkzentGedrueckt : (e.Item.Selected ? Color.FromArgb(255, 186, 80) : Farben.Akzent);
        if (!e.Item.Enabled)
          farbe = Color.FromArgb(90, 80, 60);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using (GraphicsPath p = Grafik.Abgerundet(r, 6))
        using (SolidBrush b = new SolidBrush(farbe))
          e.Graphics.FillPath(b, p);
        return;
      }
      ToolStripButton btn = e.Item as ToolStripButton;
      if (btn != null && btn.Checked)
      {
        using (SolidBrush b = new SolidBrush(Farben.Auswahl))
          e.Graphics.FillRectangle(b, new Rectangle(Point.Empty, e.Item.Size));
        using (Pen p = new Pen(Farben.Akzent))
          e.Graphics.DrawRectangle(p, 0, 0, e.Item.Width - 1, e.Item.Height - 1);
        return;
      }
      base.OnRenderButtonBackground(e);
    }
  }

  internal class DunkleFarbtabelle : ProfessionalColorTable
  {
    public override Color MenuStripGradientBegin { get { return Farben.Hintergrund; } }
    public override Color MenuStripGradientEnd { get { return Farben.Hintergrund; } }
    public override Color ToolStripGradientBegin { get { return Farben.Flaeche; } }
    public override Color ToolStripGradientMiddle { get { return Farben.Flaeche; } }
    public override Color ToolStripGradientEnd { get { return Farben.Flaeche; } }
    public override Color ToolStripBorder { get { return Farben.Rand; } }
    public override Color ToolStripDropDownBackground { get { return Farben.Flaeche; } }
    public override Color ToolStripContentPanelGradientBegin { get { return Farben.Hintergrund; } }
    public override Color ToolStripContentPanelGradientEnd { get { return Farben.Hintergrund; } }
    public override Color StatusStripGradientBegin { get { return Farben.Flaeche; } }
    public override Color StatusStripGradientEnd { get { return Farben.Flaeche; } }
    public override Color MenuBorder { get { return Farben.Rand; } }
    public override Color MenuItemBorder { get { return Farben.Akzent; } }
    public override Color MenuItemSelected { get { return Farben.Hover; } }
    public override Color MenuItemSelectedGradientBegin { get { return Farben.Hover; } }
    public override Color MenuItemSelectedGradientEnd { get { return Farben.Hover; } }
    public override Color MenuItemPressedGradientBegin { get { return Farben.FlaecheHell; } }
    public override Color MenuItemPressedGradientMiddle { get { return Farben.FlaecheHell; } }
    public override Color MenuItemPressedGradientEnd { get { return Farben.FlaecheHell; } }
    public override Color ImageMarginGradientBegin { get { return Farben.Flaeche; } }
    public override Color ImageMarginGradientMiddle { get { return Farben.Flaeche; } }
    public override Color ImageMarginGradientEnd { get { return Farben.Flaeche; } }
    public override Color SeparatorDark { get { return Farben.Rand; } }
    public override Color SeparatorLight { get { return Farben.Rand; } }
    public override Color ButtonSelectedHighlight { get { return Farben.Hover; } }
    public override Color ButtonSelectedGradientBegin { get { return Farben.Hover; } }
    public override Color ButtonSelectedGradientMiddle { get { return Farben.Hover; } }
    public override Color ButtonSelectedGradientEnd { get { return Farben.Hover; } }
    public override Color ButtonSelectedBorder { get { return Farben.Rand; } }
    public override Color ButtonPressedGradientBegin { get { return Farben.Auswahl; } }
    public override Color ButtonPressedGradientMiddle { get { return Farben.Auswahl; } }
    public override Color ButtonPressedGradientEnd { get { return Farben.Auswahl; } }
    public override Color ButtonCheckedGradientBegin { get { return Farben.Auswahl; } }
    public override Color ButtonCheckedGradientMiddle { get { return Farben.Auswahl; } }
    public override Color ButtonCheckedGradientEnd { get { return Farben.Auswahl; } }
    public override Color CheckBackground { get { return Farben.Auswahl; } }
    public override Color CheckSelectedBackground { get { return Farben.Akzent; } }
    public override Color CheckPressedBackground { get { return Farben.AkzentGedrueckt; } }
    public override Color GripDark { get { return Farben.Rand; } }
    public override Color GripLight { get { return Farben.Flaeche; } }
    public override Color OverflowButtonGradientBegin { get { return Farben.Flaeche; } }
    public override Color OverflowButtonGradientMiddle { get { return Farben.Flaeche; } }
    public override Color OverflowButtonGradientEnd { get { return Farben.Flaeche; } }
  }

  internal static class Grafik
  {
    public static GraphicsPath Abgerundet(Rectangle r, int radius)
    {
      GraphicsPath p = new GraphicsPath();
      int d = radius * 2;
      if (r.Width <= d || r.Height <= d)
      {
        p.AddRectangle(r);
        return p;
      }
      p.AddArc(r.X, r.Y, d, d, 180, 90);
      p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
      p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
      p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
      p.CloseFigure();
      return p;
    }
  }

  /// <summary>Symbole aus der Windows-Symbolschrift (Segoe Fluent Icons / Segoe MDL2 Assets).</summary>
  internal static class Symbole
  {
    public const char RomOeffnen = '';
    public const char Level = '';
    public const char Speichern = '';
    public const char SpeichernUnter = '';
    public const char Abspielen = '';
    public const char Rueckgaengig = '';
    public const char Wiederholen = '';
    public const char Assistent = '';
    public const char Hilfe = '';
    public const char Einstellungen = '';
    public const char Objekt = '';
    public const char Modell = '';
    public const char Musik = '';
    public const char Text = '';
    public const char Haken = '';
    public const char Ordner = '';
    public const char Github = '';

    private static string schrift;

    private static string Schrift
    {
      get
      {
        if (schrift != null)
          return schrift;
        schrift = "";
        foreach (string name in new[] { "Segoe Fluent Icons", "Segoe MDL2 Assets" })
        {
          try
          {
            using (Font f = new Font(name, 10f))
              if (f.Name == name)
              {
                schrift = name;
                break;
              }
          }
          catch
          {
          }
        }
        return schrift;
      }
    }

    public static bool Verfuegbar
    {
      get { return Schrift != ""; }
    }

    public static Bitmap Bild(char zeichen, int groesse, Color farbe)
    {
      Bitmap bmp = new Bitmap(groesse, groesse);
      if (!Verfuegbar)
        return bmp;
      using (Graphics g = Graphics.FromImage(bmp))
      using (Font f = new Font(Schrift, groesse * 0.62f, GraphicsUnit.Pixel))
      {
        // Graustufen-Glättung, damit auf transparentem Grund keine farbigen Ränder entstehen
        g.TextRenderingHint = TextRenderingHint.AntiAlias;
        using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
        using (SolidBrush b = new SolidBrush(farbe))
          g.DrawString(zeichen.ToString(), f, b, new RectangleF(0, 0, groesse, groesse), sf);
      }
      return bmp;
    }
  }
}
