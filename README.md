# Banjo's Backpack – Modern Edition

Level-Editor für **Banjo-Kazooie (N64)** – mit neuem dunklem Design, Oberfläche auf **Deutsch und Englisch** und Einsteiger-Assistent.

🇬🇧 *English version below.*

**Modern Edition von [FGSeRaser](https://github.com/FGSeRaser)**
Original-Editor von **Skill, Adam und Cooliscool** – [RareExports/Banjo-s-Backpack](https://github.com/RareExports/Banjo-s-Backpack) (GPL-3.0)

---

## Was ist neu?

- **Dunkles „Honigwaben“-Design** für alle Fenster (Hauptfenster, Modell-Import, MIDI-Werkzeug, Spieleinstellungen …), inklusive dunkler Windows-Titelleiste
- **Deutsch / English** – umschaltbar unter **Ansicht → Sprache / Language** (Standard: Sprache von Windows)
- **Schnellleiste** mit großen Knöpfen: ROM öffnen · Level wählen · Speichern · Neue ROM · **▶ Testen** · Rückgängig/Wiederholen
- **Modus-Knöpfe** (Auswählen / Erstellen / Kameras / Pfade) statt versteckter Auswahlliste
- **Einsteiger-Assistent** (F1): führt in 5 Schritten von der ROM bis zum Test im Emulator und hakt erledigte Schritte automatisch ab
- **Im Emulator testen** (F5): startet die gespeicherte ROM direkt in Project64, ares, simple64 o. Ä.
- **Fenster ist jetzt vergrößerbar** und startet größer
- **Startprüfung**: fehlende Hilfsdateien werden verständlich gemeldet, statt dass das Programm still abstürzt
- **Fehlerprotokoll**: Abstürze landen in `fehler.log` im Programmordner
- Projekt auf **.NET Framework 4.8** (x86) aktualisiert

Alles lässt sich unter **Ansicht** ein- und ausschalten (Sprache, dunkles Design, Schnellleiste, Assistent beim Start).

---

## Bauen

1. **Visual Studio 2022** (Community reicht) mit der Workload „.NET-Desktopentwicklung“ installieren
2. `BB.sln` öffnen
3. Oben **Release** wählen → **Erstellen → Projektmappe erstellen**
4. Die fertige `BanjoKazooieLevelEditor.exe` liegt in `bin\Release\` (zusammen mit den DLLs aus `lib`)

> Fehlt das .NET Framework 4.8 Developer Pack, bietet Visual Studio die Installation direkt an.

## Hilfsdateien

Die nötigen Ordner `resources`, `out`, `Objects` und `text` (aus dem Original-Release 1.0.5) liegen im Ordner `Laufzeit` und werden **beim Bauen automatisch neben die .exe kopiert**. Fehlt trotzdem etwas, zeigt die Modern Edition beim Start genau an, welche Datei fehlt.

## Benutzung in Kürze

1. **ROM öffnen** – Banjo-Kazooie **US-Version 1.0 (NTSC)** als `.z64`. Beim ersten Mal wird sie erweitert; lass eine Sicherungskopie anlegen.
2. **Level wählen** – z. B. Mumbo's Mountain.
3. **Umbauen** – Modus „Erstellen“ + Rechtsklick setzt Objekte, Modus „Auswählen“ zum Verschieben/Drehen/Löschen.
   Kamera: W/S vor/zurück, A/D seitwärts, E/Q hoch/runter, linke Maustaste halten = umsehen.
4. **Neue ROM…** – speichert in eine neue Datei, das Original bleibt sicher.
5. **▶ Testen** – startet die ROM im Emulator (beim ersten Mal die Emulator-.exe wählen).

Eigene Level-Modelle baust du in Blender und importierst sie als OBJ über **Werkzeuge → OBJ importieren**.

## Teilen

Gib dein Projekt als **Patch** weiter (z. B. `.bps` mit Floating IPS oder beat), **niemals die ROM selbst** – das Spiel gehört Rare/Microsoft.

## Lizenz

GPL-3.0, wie das Original (siehe `LICENSE`). Die Original-Credits bleiben im Programm unter **Hilfe → Über** vollständig erhalten.

---

## 🇬🇧 English

Level editor for **Banjo-Kazooie (N64)** – Modern Edition by [FGSeRaser](https://github.com/FGSeRaser), based on the original by Skill, Adam and Cooliscool.

**New:** dark theme for every window · German/English UI (**View → Sprache / Language**, defaults to your Windows language) · quick bar (Open ROM, Choose level, Save, New ROM, **▶ Test**) · mode buttons · beginner assistant (F1) · one-click emulator testing (F5) · resizable window · friendly error messages and `fehler.log` · .NET Framework 4.8.

**Build:** open `BB.sln` in Visual Studio 2022 → Release → Build. Helper files from the `Laufzeit` folder are copied next to the .exe automatically.

**Use:** open a **Banjo-Kazooie US 1.0 (NTSC) .z64** ROM → choose a level → edit (mode “Create” + right-click places objects) → **New ROM…** → **▶ Test**.

**Share** your hack as a patch (`.bps`), never the ROM itself. License: GPL-3.0.

---

<sub>Original-README: Banjo Kazooie level editor – Displays maps/objects perfectly for the game. Map exports use RGBAOBJ. Map imports use GEOBJ.</sub>
