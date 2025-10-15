using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using CafePOS.Domain.Models;

namespace CafePOS.Domain.Services
{
    public static class BelegDateiService
    {
        // JSON-Optionen: schön formatiert speichern
        private static readonly JsonSerializerOptions _opt = new() { WriteIndented = true };

        public static class AppPaths
        {
            // Basisordner unter %LOCALAPPDATA%\CafePOS
            public static string Root =>
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CafePOS");

            // Unterordner für gespeicherte Belege
            public static string Belege => Ensure(Path.Combine(Root, "Belege"));

            // Unterordner für Konfigurationsdateien
            public static string Config => Ensure(Path.Combine(Root, "Config"));

            // Sicherstellen, dass der Ordner existiert (ansonsten anlegen)
            private static string Ensure(string p) { Directory.CreateDirectory(p); return p; }
        }

        /// <summary>Speichert einen Beleg als JSON-Datei und gibt den vollständigen Pfad zurück.</summary>
        public static string Speichern(Beleg beleg)
        {
            var file = Path.Combine(AppPaths.Belege,
                $"beleg_{DateTime.UtcNow:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}.json");
            File.WriteAllText(file, JsonSerializer.Serialize(beleg, _opt));
            return file;
        }

        /// <summary>Lädt alle gespeicherten Belege (neuste zuerst). Beschädigte Dateien werden übersprungen.</summary>
        public static List<Beleg> LadenAlle()
        {
            var list = new List<Beleg>();
            foreach (var path in Directory.GetFiles(AppPaths.Belege, "beleg_*.json"))
            {
                try
                {
                    var beleg = JsonSerializer.Deserialize<Beleg>(File.ReadAllText(path));
                    if (beleg != null) list.Add(beleg);
                }
                catch
                {
                    // Beschädigte/ungültige Dateien ignorieren
                }
            }
            return list.OrderByDescending(b => b.Datum).ToList();
        }

        /// <summary>Gibt den Ordnerpfad der Belege zurück (für „Ordner öffnen“).</summary>
        public static string OrdnerPfad() => AppPaths.Belege;

        /// <summary>Löscht die Datei, die den angegebenen Beleg enthält (Vergleich über BelegId).</summary>
        public static bool Loeschen(Beleg target)
        {
            try
            {
                foreach (var path in Directory.GetFiles(AppPaths.Belege, "beleg_*.json"))
                {
                    try
                    {
                        // Nur so viel lesen/deserialisieren, dass wir an die BelegId kommen
                        var json = File.ReadAllText(path);
                        var b = JsonSerializer.Deserialize<Beleg>(json);
                        if (b != null && b.BelegId == target.BelegId)
                        {
                            File.Delete(path);
                            return true;
                        }
                    }
                    catch
                    {
                        // Einzelne Fehler pro Datei ignorieren und mit der nächsten weitermachen
                    }
                }
            }
            catch
            {
                // Unerwarteten Fehler beim Durchsuchen/Löschen ignorieren => false zurückgeben
            }
            return false;
        }
    }
}