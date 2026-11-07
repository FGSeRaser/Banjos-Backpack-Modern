// Banjo's Backpack – Modern Edition · FGSeRaser (https://github.com/FGSeRaser)
// Deutsche Oberfläche. Übersetzt wird nur die Anzeige – der englische Originaltext
// bleibt pro Element gespeichert, weil der Editor Objektnamen aus Menütexten liest.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace BanjoKazooieLevelEditor.Modern
{
  internal static class Uebersetzung
  {
    private class Box
    {
      public string Text;
    }

    private static readonly ConditionalWeakTable<object, Box> originale = new ConditionalWeakTable<object, Box>();
    private static readonly ConditionalWeakTable<object, Box> verdrahtet = new ConditionalWeakTable<object, Box>();
    private static bool beschaeftigt;

    public static bool Aktiv
    {
      get { return Einstellungen.Deutsch; }
    }

    /// <summary>Liefert den englischen Originaltext (für Programmlogik).</summary>
    public static string Original(ToolStripItem item)
    {
      if (item == null)
        return "";
      Box b;
      return originale.TryGetValue(item, out b) ? b.Text : item.Text;
    }

    private static readonly Dictionary<string, string> woerter = new Dictionary<string, string>(StringComparer.Ordinal)
    {
      // Menüs
      { "File", "Datei" }, { "Edit", "Bearbeiten" }, { "Options", "Optionen" }, { "Tools", "Werkzeuge" },
      { "Help", "Hilfe" }, { "About", "Über" }, { "View", "Ansicht" },
      { "Open Rom", "ROM öffnen…" }, { "Open Setup File From Rom", "Level aus ROM öffnen…" },
      { "Load Setup File", "Setup-Datei laden…" }, { "Save Setup File", "Setup-Datei speichern…" },
      { "Save To Rom", "In ROM speichern" }, { "Save to Rom", "In ROM speichern" },
      { "Save As Rom", "Als neue ROM speichern…" }, { "Save As Rom (injected)", "Als ROM speichern (injiziert)…" },
      { "Undo", "Rückgängig" }, { "Redo", "Wiederholen" }, { "History", "Verlauf" },
      { "Have all moves", "Alle Moves freigeschaltet" }, { "Have no moves", "Ohne Moves starten" },
      { "Include camera triggers when saving", "Kamera-Trigger mitspeichern" },
      { "Remove Numbers from Note Doors", "Zahlen von Notentüren entfernen" },
      { "Hide Unknown Bytes", "Unbekannte Bytes ausblenden" },
      { "Model Importer", "Modell-Import" }, { "Import OBJ", "OBJ importieren" }, { "Model Viewer", "Modell-Ansicht" },
      { "Text Editor", "Text-Editor" }, { "Midi Tool", "MIDI-Werkzeug" }, { "SNS Editor", "Stop-'n'-Swop-Editor" },
      { "Sprite Manager", "Sprite-Verwaltung" }, { "Sprite Editor", "Sprite-Editor" },
      { "General Game Settings", "Allgemeine Spieleinstellungen" }, { "General Settings", "Allgemeine Einstellungen" },
      { "Injector", "Injektor" },

      // Hauptfenster
      { "Layers", "Ebenen" }, { "Camera Speed", "Kamera-Tempo" }, { "Object Draw Distance", "Objekt-Sichtweite" },
      { "Objects", "Objekte" }, { "Object Details", "Objekt-Details" }, { "Replace Model", "Modell ersetzen" },
      { "Replace Level Model", "Level-Modell ersetzen" }, { "Level Boundaries", "Level-Grenzen" },
      { "Level Entries", "Level-Eingänge" }, { "Structs", "Strukturen" }, { "Camera", "Kamera" },
      { "Selected Path", "Gewählter Pfad" }, { "Model Modifiers", "Modell-Anpassungen" },
      { "Mouse Settings", "Maus-Einstellungen" }, { "Vert Painting", "Vertex-Bemalung" }, { "Add Part", "Teil hinzufügen" },
      { "Select Mode", "Auswählen" }, { "Create Mode", "Erstellen" }, { "Camera Mode", "Kameras" }, { "Path Mode", "Pfade" },
      { "Rectangle Select", "Rechteck-Auswahl" }, { "Move", "Bewegen" }, { "Rotate", "Drehen" }, { "Scale", "Skalieren" },
      { "Duplicate", "Duplizieren" }, { "Delete", "Löschen" }, { "Deselect", "Abwählen" },
      { "LOCK X", "X sperren" }, { "LOCK Y", "Y sperren" }, { "LOCK Z", "Z sperren" },
      { "Erase All", "Alles löschen" }, { "Clear", "Leeren" },
      { "Add Node", "Knoten +" }, { "Del. Node", "Knoten −" }, { "Move Node", "Knoten bewegen" },
      { "Delete Nodes", "Knoten löschen" }, { "End Node", "Endknoten" }, { "New Path", "Neuer Pfad" },
      { "Deselect Path", "Pfad abwählen" }, { "Delete Path", "Pfad löschen" }, { "Add Path Controller", "Pfad-Controller +" },
      { "Link", "Verbinden" }, { "Node Properties", "Knoten-Eigenschaften" }, { "Controllers", "Controller" },
      { "Standard Nodes", "Standard-Knoten" }, { "Pause Time", "Pausenzeit" }, { "To Node", "Zu Knoten" }, { "Path ID", "Pfad-ID" },
      { "Assign OBJ", "OBJ zuweisen" }, { "Object ID", "Objekt-ID" }, { "Object Name", "Objektname" },
      { "Description", "Beschreibung" }, { "Location", "Position" }, { "Size", "Größe" }, { "Script", "Skript" },
      { "Speed", "Tempo" }, { "Yaw", "Gieren" }, { "Pitch", "Neigen" }, { "Roll", "Rollen" },
      { "Acceleration", "Beschleunigung" }, { "Rotation Speed", "Drehtempo" }, { "H Speed", "H-Tempo" }, { "V Speed", "V-Tempo" },
      { "Close Distance", "Nahdistanz" }, { "Far Distance", "Ferndistanz" }, { "Camera ID:", "Kamera-ID:" },
      { "Set to BB Camera", "Auf Editor-Kamera setzen" }, { "Create Cutscene Camera", "Zwischensequenz-Kamera erstellen" },
      { "Create Gameplay Camera", "Spiel-Kamera erstellen" }, { "Create Camera Trigger", "Kamera-Trigger erstellen" },
      { "Cameras", "Kameras" }, { "Model (A)", "Modell (A)" }, { "Model (B)", "Modell (B)" },
      { "Background A", "Hintergrund A" }, { "Background B", "Hintergrund B" },
      { "Level Boundary", "Level-Grenze" }, { "Level Boundary (alpha)", "Level-Grenze (Alpha)" },
      { "Update", "Übernehmen" }, { "Entry ID", "Eingangs-ID" }, { "In Use", "Belegt" },
      { "Warp To", "Warp nach" }, { "Warps To", "Warpt nach" }, { "Level Stats", "Level-Statistik" },
      { "Enemy Radius", "Gegner-Radius" }, { "Flag Radius", "Flag-Radius" }, { "Camera Trigger Radius", "Kamera-Trigger-Radius" },
      { "Warp Radius", "Warp-Radius" }, { "Unknown Object Radius", "Radius (unbek. Objekt)" },
      { "Unknown", "Unbekannt" }, { "Unknown:", "Unbekannt:" }, { "Y-Offset", "Y-Versatz" },
      { "Camera |  Select", "Kamera | Auswahl" }, { "Rotation", "Rotation" }, { "Timer", "Timer" },

      // Allgemein / Dialoge
      { "OK", "OK" }, { "Cancel", "Abbrechen" }, { "Close", "Schließen" }, { "Save", "Speichern" }, { "Back", "Zurück" },
      { "Reset", "Zurücksetzen" }, { "Search", "Suchen" }, { "Import", "Importieren" }, { "Settings", "Einstellungen" },
      { "General", "Allgemein" }, { "Type:", "Typ:" }, { "Map", "Karte" }, { "Item", "Gegenstand" }, { "All", "Alle" },
      { "Transform", "Transformieren" }, { "Collision", "Kollision" }, { "Collision Type:", "Kollisionstyp:" },
      { "Ground Type:", "Bodentyp:" }, { "Sound Type:", "Soundtyp:" }, { "Cull Mode:", "Cull-Modus:" },
      { "Environment Map:", "Umgebungs-Map:" }, { "Ground", "Boden" }, { "Triangle", "Dreieck" }, { "Vert", "Vertex" },
      { "Flip", "Spiegeln" }, { "Entry", "Eingang" }, { "Setup File", "Setup-Datei" }, { "File:", "Datei:" },
      { "File Loc", "Datei-Pos." },

      // Modell-Import / Modell-Ansicht
      { "Auto Collision (Recommended)", "Auto-Kollision (empfohlen)" }, { "Flip Textures Vertically", "Texturen vertikal spiegeln" },
      { "Always move to Model A", "Immer nach Modell A" }, { "Animate Vertex Colours", "Vertex-Farben animieren" },
      { "Apply To Texture", "Auf Textur anwenden" }, { "Apply as Base Coat", "Als Grundfarbe anwenden" },
      { "Assign to Level...", "Level zuweisen…" }, { "Export to Collada (.dae)", "Als Collada (.dae) exportieren" },
      { "Export to geObj", "Als geObj exportieren" }, { "Import Collada (.dae)", "Collada (.dae) importieren" },
      { "Load Replacement Model", "Ersatzmodell laden" }, { "Save As Level Model", "Als Level-Modell speichern" },
      { "Save As Object Model", "Als Objekt-Modell speichern" }, { "Draw Banjo", "Banjo anzeigen" },
      { "Draw Collision Map", "Kollision anzeigen" }, { "Draw Edges", "Kanten anzeigen" }, { "Draw Sound Map", "Sound-Map anzeigen" },
      { "Scroll Texture", "Textur scrollen" }, { "WARNING will update model file", "ACHTUNG: ändert die Modelldatei" },
      { "Pick Colour", "Farbe wählen" }, { "Particle Colour", "Partikelfarbe" }, { "Update Colour:", "Farbe übernehmen:" },

      // Musik / Szenen / Sprites / Text
      { "Skybox Scale", "Skybox-Größe" }, { "Skyboxes", "Skyboxen" }, { "Sky Box Reassignment", "Skybox-Zuweisung" },
      { "Music Assignment", "Musik-Zuweisung" }, { "Scene Assignment", "Szenen-Zuweisung" }, { "Scene Select", "Szene wählen" },
      { "Select Entry Point", "Eingangspunkt wählen" }, { "Export Midi", "MIDI exportieren" }, { "Loop", "Schleife" },
      { "Loop Point", "Schleifenpunkt" }, { "Generate Wave", "Welle erzeugen" }, { "Change Head", "Kopf ändern" },
      { "Replace Sprite", "Sprite ersetzen" }, { "Open Bin", "Bin öffnen" }, { "Frame Count:", "Frames:" },
      { "Type Your Edit Below", "Änderung hier eingeben" }, { "New:", "Neu:" },
      { "Decompressing files please wait...", "Dateien werden entpackt, bitte warten…" },
      { "Mumbo's Messages", "Mumbos Nachrichten" }, { "Cutscenes", "Zwischensequenzen" }, { "Ending", "Abspann" },
      { "Pre-Ending", "Vor-Abspann" }, { "Top", "Oben" }, { "Bottom", "Unten" }, { "Top Sections", "Obere Abschnitte" },
      { "Bottom Sections", "Untere Abschnitte" }, { "Play This Section First:", "Diesen Abschnitt zuerst:" },
      { "Use Repeaters", "Wiederholer nutzen" }, { "Mouse Left Click", "Linksklick" }, { "Mouse Right Click", "Rechtsklick" },
      { "Act %", "Akt %" }, { "Act Time %", "Akt-Zeit %" }, { "Bits Remaining ", "Bits übrig " }, { "bits left", "Bits übrig" },

      // Spieleinstellungen
      { "Level Name", "Level-Name" }, { "Level Names", "Level-Namen" }, { "Parent Level", "Übergeordnetes Level" },
      { "Start Level", "Start-Level" }, { "Unlockable Requirements", "Freischalt-Bedingungen" },
      { "Required Notes", "Benötigte Noten" }, { "Required Tokens", "Benötigte Token" }, { "Jiggies Per Level", "Jiggies pro Level" },
      { "Max Notes", "Max. Noten" }, { "Max Items", "Max. Items" }, { "Max Eggs:", "Max. Eier:" },
      { "Max Feathers", "Max. Federn" }, { "Max Gold Feathers", "Max. Goldfedern" },
      { "Max Eggs Cheato", "Max. Eier (Cheato)" }, { "Max Feathers Cheato", "Max. Federn (Cheato)" },
      { "Max Gold Feathers Cheato", "Max. Goldfedern (Cheato)" }, { "Empty Honeycomb", "Leere Honigwabe" },
      { "Notes Door", "Notentür" }, { "Level to hide Jiggies and Notes", "Level für Jiggies und Noten" },
      { "Level to hide Notes and Extra Honeycomb Pieces", "Level für Noten und Extra-Waben" },
      { "Honeycomb Special Level", "Waben-Speziallevel" }, { "Empty Honeycomb for Special Level", "Leere Wabe fürs Speziallevel" },
      { "Final Battle / Grunty Squares", "Endkampf / Grunty-Felder" },

      // Über-Dialog
      { "Testers", "Tester" }, { "Banjo's Backpack Development Team", "Banjo's-Backpack-Entwicklerteam" },
      { "Special Thanks to Tee-Hee for the 3D icons", "Besonderer Dank an Tee-Hee für die 3D-Icons" },
    };

    // Texte, die das Programm zur Laufzeit zusammensetzt ("OBJ ID: 1f4" …)
    private static readonly string[,] praefixe =
    {
      { "OBJ ID: ", "Objekt-ID: " }, { "File: 0x", "Datei: 0x" }, { "File Info: ", "Datei-Info: " },
      { "OpenGL Version: ", "OpenGL-Version: " }, { "New Scale:  0x", "Neue Größe: 0x" }, { "frame: ", "Frame: " },
    };

    public static string Uebersetzen(string englisch)
    {
      if (string.IsNullOrEmpty(englisch))
        return englisch;
      string de;
      if (woerter.TryGetValue(englisch, out de))
        return de;

      // Aufklapp-Knöpfe: "+ Objects" / "- Objects"
      if (englisch.Length > 2 && (englisch.StartsWith("+ ") || englisch.StartsWith("- ")))
      {
        string rest = englisch.Substring(2);
        if (woerter.TryGetValue(rest, out de))
          return (englisch[0] == '+' ? "▸ " : "▾ ") + de;
      }

      for (int i = 0; i < praefixe.GetLength(0); i++)
        if (englisch.StartsWith(praefixe[i, 0], StringComparison.Ordinal))
          return praefixe[i, 1] + englisch.Substring(praefixe[i, 0].Length);

      return englisch;
    }

    private static bool Uebersetzbar(Control c)
    {
      return c is Label || c is ButtonBase || c is GroupBox || c is TabPage || c is Form;
    }

    /// <summary>Übersetzt ein Control samt Kindern und hält es bei Textänderungen deutsch.</summary>
    public static void Anwenden(Control c)
    {
      if (!Aktiv || c == null)
        return;

      if (Uebersetzbar(c) && !(c is LinkLabel))
      {
        ControlSetzen(c);
        Box dummy;
        if (!verdrahtet.TryGetValue(c, out dummy))
        {
          verdrahtet.Add(c, new Box());
          c.TextChanged += (s, e) => ControlSetzen((Control)s);
        }
      }

      if (c is ToolStrip)
        foreach (ToolStripItem item in ((ToolStrip)c).Items)
          ItemAnwenden(item);

      if (c is DataGridView)
        foreach (DataGridViewColumn spalte in ((DataGridView)c).Columns)
          spalte.HeaderText = Uebersetzen(spalte.HeaderText);

      if (c.ContextMenuStrip != null)
        foreach (ToolStripItem item in c.ContextMenuStrip.Items)
          ItemAnwenden(item);

      foreach (Control kind in c.Controls)
        Anwenden(kind);
    }

    private static void ControlSetzen(Control c)
    {
      if (beschaeftigt || c is Form)
        return;
      string neu = Uebersetzen(c.Text);
      if (neu == c.Text)
        return;
      beschaeftigt = true;
      try
      {
        c.Text = neu;
      }
      finally
      {
        beschaeftigt = false;
      }
    }

    public static void ItemAnwenden(ToolStripItem item)
    {
      if (!Aktiv || item == null || item is ToolStripTextBox || item is ToolStripComboBox || item is ToolStripSeparator)
        return;

      Box dummy;
      if (!verdrahtet.TryGetValue(item, out dummy))
      {
        verdrahtet.Add(item, new Box());
        ItemSetzen(item);
        item.TextChanged += (s, e) => ItemSetzen((ToolStripItem)s);
        // Untermenüs, die erst später befüllt werden, beim Öffnen nachziehen
        ToolStripDropDownItem spaeter = item as ToolStripDropDownItem;
        if (spaeter != null)
          spaeter.DropDownOpening += (s, e) =>
          {
            foreach (ToolStripItem kind in ((ToolStripDropDownItem)s).DropDownItems)
            {
              ItemAnwenden(kind);
              Theme.ItemAnwenden(kind);
            }
          };
      }

      ToolStripDropDownItem dd = item as ToolStripDropDownItem;
      if (dd != null)
        foreach (ToolStripItem kind in dd.DropDownItems)
          ItemAnwenden(kind);
    }

    private static void ItemSetzen(ToolStripItem item)
    {
      if (beschaeftigt)
        return;
      string englisch = item.Text;
      string neu = Uebersetzen(englisch);
      if (neu == englisch)
        return;
      beschaeftigt = true;
      try
      {
        Box b;
        if (originale.TryGetValue(item, out b))
          b.Text = englisch;
        else
          originale.Add(item, new Box { Text = englisch });
        item.Text = neu;
      }
      finally
      {
        beschaeftigt = false;
      }
    }

    private static readonly Dictionary<string, string> meldungen = new Dictionary<string, string>(StringComparer.Ordinal)
    {
      { "The last used rom could not be found. Please open your rom again.", "Die zuletzt benutzte ROM wurde nicht gefunden (verschoben oder gelöscht). Bitte öffne deine ROM noch einmal." },
      { "Please open a Banjo Kazooie V1.0 NTSC rom", "Bitte öffne eine Banjo-Kazooie-ROM (US-Version 1.0, NTSC)." },
      { "Invalid Rom", "Falsche ROM" },
      { "Do you want to retry?", "Möchtest du es noch einmal versuchen?" },
      { "Unable to open rom", "ROM konnte nicht geöffnet werden" },
      { "Could not open rom ", "Die ROM konnte nicht geöffnet werden: " },
      { "Changes saved successfully", "Änderungen erfolgreich gespeichert." },
      { " saved successfully", " wurde erfolgreich gespeichert." },
      { "Error: Failed to Save", "Fehler: Speichern fehlgeschlagen." },
      { "An error occured, please try again. ", "Es ist ein Fehler aufgetreten, bitte versuch es erneut. " },
      { "Banjo's Backpack has detected that this is your first time using the program, please open a Banjo Kazooie Rom", "Willkommen bei Banjo's Backpack! Öffne bitte zuerst deine Banjo-Kazooie-ROM (US-Version 1.0)." },
      { "Setup File Saved", "Setup-Datei gespeichert." },
      { "Please open a setup file first", "Bitte öffne zuerst ein Level." },
      { "Rom Updated", "ROM aktualisiert." },
      { "All paths used BB can't calculate any more", "Alle Pfade sind belegt – es können keine weiteren angelegt werden." },
      { "Please move the start position to the desired location, it is currently set at 0,0,0", "Bitte verschiebe die Startposition an die gewünschte Stelle – sie steht aktuell auf 0,0,0." },
      { "Banjo's Backpack Failed to open the rom ", "Banjo's Backpack konnte die ROM nicht öffnen: " },
      { "Error: General Failure", "Fehler: Allgemeiner Fehler." },
      { "This rom has not been extended in order to use it with Banjo's Backpack the rom will need to be extended, would you like to extend the rom now?", "Diese ROM wurde noch nicht erweitert. Damit Banjo's Backpack damit arbeiten kann, muss sie erweitert werden. Jetzt erweitern?" },
      { "BB will open the extended rom automatically once completed", "Die erweiterte ROM wird danach automatisch geöffnet (Dateiname endet auf .ext.z64)." },
      { "Extend Rom?", "ROM erweitern?" },
      { "BB will alter this rom so that it can make some changes that would otherwise not be possible, would you like BB to make a copy of your rom so you have a clean rom to patch?", "Banjo's Backpack verändert diese ROM, um weitere Änderungen zu ermöglichen. Soll vorher eine saubere Sicherungskopie angelegt werden? (Empfohlen: Ja)" },
      { "Copy Rom?", "Sicherungskopie anlegen?" }
    };

    /// <summary>Übersetzt Meldungstexte (MessageBox) – bei englischer Oberfläche bleibt das Original.</summary>
    public static string T(string englisch)
    {
      string de;
      if (Aktiv && englisch != null && meldungen.TryGetValue(englisch, out de))
        return de;
      return englisch;
    }

    /// <summary>Kurze deutsche Zusatz-Tooltips für die wichtigsten Werkzeuge.</summary>
    public static readonly Dictionary<string, string> Hinweise = new Dictionary<string, string>
    {
      { "obj_move_btn", "Ausgewähltes Objekt mit der Maus verschieben (Alt+M)" },
      { "obj_rot_btn", "Ausgewähltes Objekt drehen (Alt+R)" },
      { "obj_scale_btn", "Ausgewähltes Objekt skalieren (Alt+S)" },
      { "obj_duplicate_btn", "Ausgewähltes Objekt kopieren (Alt+D)" },
      { "delete_btn", "Ausgewähltes Objekt löschen (Alt+Entf)" },
      { "deselect_btn", "Auswahl aufheben" },
      { "rectSelect_btn", "Mehrere Objekte per Rechteck auswählen (Strg+B)" },
      { "cam_moveToCurrent_btn", "Editor-Kamera zur Spielkamera springen lassen" },
      { "lockX_btn", "Bewegung auf der X-Achse sperren" },
      { "lockY_btn", "Bewegung auf der Y-Achse (Höhe) sperren" },
      { "lockZ_btn", "Bewegung auf der Z-Achse sperren" },
    };
  }
}
