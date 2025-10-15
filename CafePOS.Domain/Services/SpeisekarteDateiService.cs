using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using CafePOS.Domain.Models;

namespace CafePOS.Domain.Services
{
    /// <summary>
    /// Speichert und lädt die Speisekarte als JSON-Datei
    /// unter %LOCALAPPDATA%\CafePOS\Config\speisekarte.json
    /// (unabhängig von BelegDateiService).
    /// </summary>
    public static class SpeisekarteDateiService
    {
        private static string Root =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CafePOS");

        private static string ConfigDir
        {
            get
            {
                var dir = Path.Combine(Root, "Config");
                Directory.CreateDirectory(dir);
                return dir;
            }
        }

        private static string DateiPfad => Path.Combine(ConfigDir, "speisekarte.json");

        private static readonly JsonSerializerOptions _opt = new()
        {
            WriteIndented = true
        };

        /// <summary>Speichert die aktuelle Artikelliste und gibt den vollständigen Dateipfad zurück.</summary>
        public static string Speichern(IEnumerable<Artikel> artikel)
        {
            File.WriteAllText(DateiPfad, JsonSerializer.Serialize(artikel, _opt));
            return DateiPfad; // für UI-Hinweis
        }

        /// <summary>Lädt die gespeicherte Liste; bei Fehlern/fehlender Datei leere Liste.</summary>
        public static List<Artikel> Laden()
        {
            if (!File.Exists(DateiPfad)) return new();
            try
            {
                var json = File.ReadAllText(DateiPfad);
                return JsonSerializer.Deserialize<List<Artikel>>(json) ?? new();
            }
            catch
            {
                return new();
            }
        }

        /// <summary>Gibt den Ordner der Speisekarten-Datei zurück (für „Ordner öffnen“ o.ä.).</summary>
        public static string OrdnerPfad() => ConfigDir;
    }
}