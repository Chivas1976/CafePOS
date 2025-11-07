// CafePOS.Wpf/ViewModels/SpeisekarteViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using CafePOS.Domain.Models;
using CafePOS.Domain.Services;
using CafePOS.Wpf.Commands;

namespace CafePOS.Wpf.ViewModels
{
    public class SpeisekarteViewModel : ObservableObject
    {
        // =========================
        // HELFERMETHODE: Aktualisiert die Ausführbarkeit von Kommandos
        // =========================
        private static void RaiseCanExecute(ICommand? cmd)
        {
            switch (cmd)
            {
                case RelayCommand r: r.RaiseCanExecuteChanged(); break;
                case RelayCommand<Bestellposition> rpos: rpos.RaiseCanExecuteChanged(); break;
            }
        }

        // =========================
        // 1) FELDER UND EIGENSCHAFTEN (DATA PROPERTIES)
        // =========================

        private readonly Benutzer _benutzer;

        // Berechtigungseigenschaften basierend auf Benutzerrolle
        public bool IstChef => _benutzer.Rolle == Rolle.Chef;
        public bool IstSpeisekarteReadOnly => !IstChef;

        // Speisekarte (linke Seite der Oberfläche)
        public ObservableCollection<Artikel> ArtikelListe { get; } = new();

        private Artikel? _ausgewaehlterArtikel;
        public Artikel? AusgewaehlterArtikel
        {
            get => _ausgewaehlterArtikel;
            set
            {
                if (Set(ref _ausgewaehlterArtikel, value))
                {
                    // Aktualisiere Kommandos wenn Auswahl ändert
                    RaiseCanExecute(InWarenkorbCmd);
                    RaiseCanExecute(LoeschenCmd);
                }
            }
        }

        // CollectionView für Filterung und Sortierung der Artikel
        public ICollectionView ArtikelView { get; }

        private string _suchtext = string.Empty;
        public string Suchtext
        {
            get => _suchtext;
            set { if (Set(ref _suchtext, value)) ArtikelView.Refresh(); } // Filter neu anwenden
        }

        private bool _nurVerfuegbar;
        public bool NurVerfuegbar
        {
            get => _nurVerfuegbar;
            set { if (Set(ref _nurVerfuegbar, value)) ArtikelView.Refresh(); } // Filter neu anwenden
        }

        // Warenkorb (rechte Seite der Oberfläche)
        public ObservableCollection<Bestellposition> Warenkorb { get; } = new();

        private Bestellposition? _ausgewaehltePosition;
        public Bestellposition? AusgewaehltePosition
        {
            get => _ausgewaehltePosition;
            set
            {
                if (Set(ref _ausgewaehltePosition, value))
                {
                    // Aktualisiere Warenkorb-Kommandos
                    RaiseCanExecute(AusWarenkorbCmd);
                    RaiseCanExecute(MengePlusCmd);
                    RaiseCanExecute(MengeMinusCmd);
                    RaiseCanExecute(PositionLoeschenCmd);
                }
            }
        }

        // --- RECHNUNGSBERECHNUNG ---
        private decimal _zwischensumme;
        public decimal Zwischensumme
        {
            get => _zwischensumme;
            private set => Set(ref _zwischensumme, value);
        }

        private decimal _gesamtsumme;
        public decimal Gesamtsumme
        {
            get => _gesamtsumme;
            private set => Set(ref _gesamtsumme, value);
        }

        private bool _bitAndBiteCard;
        public bool BitAndBiteCard
        {
            get => _bitAndBiteCard;
            set { if (Set(ref _bitAndBiteCard, value)) RechneGesamtsumme(); } // Neuberechnung bei Änderung
        }

        private decimal _rabattBetrag;
        public decimal RabattBetrag
        {
            get => _rabattBetrag;
            private set => Set(ref _rabattBetrag, value);
        }

        private bool _mwstAktiv;
        public bool MwstAktiv
        {
            get => _mwstAktiv;
            set { if (Set(ref _mwstAktiv, value)) RechneGesamtsumme(); } // Neuberechnung bei Änderung
        }

        private decimal _mwstSatz = 19m;
        public decimal MwstSatz
        {
            get => _mwstSatz;
            set { if (Set(ref _mwstSatz, value)) RechneGesamtsumme(); } // Neuberechnung bei Änderung
        }

        private decimal _mwstBetrag;
        public decimal MwstBetrag
        {
            get => _mwstBetrag;
            private set => Set(ref _mwstBetrag, value);
        }

        // Gesamtanzahl der Artikel im Warenkorb
        public int ArtikelAnzahl => Warenkorb.Sum(p => p.Menge);

        public ObservableCollection<Beleg> Belege { get; } = new();

        // =========================
        // 2) KOMMANDOS (COMMANDS)
        // =========================
        public ICommand LadenCmd { get; }
        public ICommand SpeichernCmd { get; }
        public ICommand NeuCmd { get; }
        public ICommand LoeschenCmd { get; }

        public ICommand InWarenkorbCmd { get; }
        public ICommand AusWarenkorbCmd { get; }      // parametrisiert
        public ICommand MengePlusCmd { get; }         // parametrisiert
        public ICommand MengeMinusCmd { get; }        // parametrisiert
        public ICommand PositionLoeschenCmd { get; }  // parametrisiert

        public ICommand BelegSpeichernCmd { get; }
        public ICommand BelegeLadenCmd { get; }
        public ICommand BelegOrdnerOeffnenCmd { get; }

        public ICommand FilterZuruecksetzenCmd { get; }
        public ICommand AllesLoeschenCmd { get; }

        // =========================
        // 3) KONSTRUKTOR
        // =========================
        public SpeisekarteViewModel(Benutzer benutzer)
        {
            _benutzer = benutzer;

            // CollectionView für Filterung initialisieren
            ArtikelView = CollectionViewSource.GetDefaultView(ArtikelListe);
            ArtikelView.Filter = FilterArtikel;

            // Startdaten laden
            foreach (var a in SpeisekarteService.LadeStandard())
                ArtikelListe.Add(a);
            ArtikelView.Refresh();

            // ===== DATEI-KOMMANDOS =====
            LadenCmd = new RelayCommand(Laden);
            SpeichernCmd = new RelayCommand(Speichern, () => IstChef && ArtikelListe.Count > 0);
            NeuCmd = new RelayCommand(Neu, () => IstChef);
            LoeschenCmd = new RelayCommand(Loeschen, () => IstChef && AusgewaehlterArtikel != null);

            // ===== WARENKORB-KOMMANDOS =====
            InWarenkorbCmd = new RelayCommand(
                InWarenkorb,
                () => AusgewaehlterArtikel != null &&
                      Warenkorb.Where(p => p.ArtikelId == AusgewaehlterArtikel!.Id).Sum(p => p.Menge) < AusgewaehlterArtikel!.Menge);
            RaiseCanExecute(InWarenkorbCmd);

            AusWarenkorbCmd = new RelayCommand<Bestellposition>(AusWarenkorb, pos => pos != null);

            MengePlusCmd = new RelayCommand<Bestellposition>(
                MengePlus,
                pos =>
                {
                    if (pos == null) return false;
                    var art = FindeArtikel(pos.ArtikelId);
                    if (art == null) return false;
                    var mengeImWarenkorb = MengeImWarenkorb(pos.ArtikelId);
                    return mengeImWarenkorb < art.Menge; // Nur möglich wenn Lagerbestand ausreicht
                });

            MengeMinusCmd = new RelayCommand<Bestellposition>(MengeMinus, pos => pos != null && pos.Menge > 0);

            PositionLoeschenCmd = new RelayCommand<Bestellposition>(PositionLoeschen, pos => pos != null);

            // ===== BELEG-KOMMANDOS =====
            BelegSpeichernCmd = new RelayCommand(BelegSpeichern, () => Warenkorb.Count > 0);
            BelegeLadenCmd = new RelayCommand(BelegeLaden);
            BelegOrdnerOeffnenCmd = new RelayCommand(BelegOrdnerOeffnen);

            // ===== ZUSÄTZLICHE KOMMANDOS =====
            FilterZuruecksetzenCmd = new RelayCommand(() => { Suchtext = string.Empty; });
            AllesLoeschenCmd = new RelayCommand(
                () => { Warenkorb.Clear(); RechneGesamtsumme(); },
                () => Warenkorb.Count > 0);

            // ===== EVENT-HANDLER FÜR COLLECTION-ÄNDERUNGEN =====

            // Wenn ArtikelListe sich ändert
            ArtikelListe.CollectionChanged += (_, __) =>
            {
                RaiseCanExecute(SpeichernCmd);
            };

            // Wenn Warenkorb sich ändert
            Warenkorb.CollectionChanged += (s, e) =>
            {
                // PropertyChanged Events für neue Positionen überwachen
                if (e.NewItems != null)
                {
                    foreach (Bestellposition p in e.NewItems)
                    {
                        p.PropertyChanged += (_, __) =>
                        {
                            RechneGesamtsumme();
                            RaiseCanExecute(InWarenkorbCmd);
                            RaiseCanExecute(MengePlusCmd);
                            RaiseCanExecute(MengeMinusCmd);
                            OnPropertyChanged(nameof(ArtikelAnzahl));
                        };
                    }
                }

                // Kommando-Ausführbarkeit aktualisieren
                RaiseCanExecute(BelegSpeichernCmd);
                RaiseCanExecute(AusWarenkorbCmd);
                RaiseCanExecute(InWarenkorbCmd);
                RaiseCanExecute(MengePlusCmd);
                RaiseCanExecute(MengeMinusCmd);
                RaiseCanExecute(AllesLoeschenCmd);

                RechneGesamtsumme();
                OnPropertyChanged(nameof(ArtikelAnzahl));
            };

            RechneGesamtsumme();
        }

        // =========================
        // 4) HILFSFUNKTIONEN
        // =========================

        // Berechnet Gesamtmenge eines Artikels im Warenkorb
        private int MengeImWarenkorb(int artikelId)
            => Warenkorb.Where(p => p.ArtikelId == artikelId).Sum(p => p.Menge);

        // Findet Artikel by ID
        private Artikel? FindeArtikel(int artikelId)
            => ArtikelListe.FirstOrDefault(a => a.Id == artikelId);

        // =========================
        // 5) FILTER-FUNKTION
        // =========================
        private bool FilterArtikel(object obj)
        {
            if (obj is not Artikel a) return false;

            // Filter: Nur verfügbare Artikel
            if (NurVerfuegbar && a.Menge <= 0) return false;

            // Filter: Suchtext
            if (string.IsNullOrWhiteSpace(Suchtext)) return true;
            var suchtext = Suchtext.Trim();
            return a.Name.Contains(suchtext, StringComparison.OrdinalIgnoreCase)
                || a.Id.ToString().Contains(suchtext);
        }

        // =========================
        // 6) DATEIOPERATIONEN - SPEISEKARTE
        // =========================
        private void Laden()
        {
            var daten = SpeisekarteDateiService.Laden();

            if (daten.Count == 0)
            {
                daten = SpeisekarteService.LadeStandard();
                MessageBox.Show("Keine gespeicherte Speisekarte gefunden.\nStandardliste wurde geladen.",
                                "Speisekarte", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            ArtikelListe.Clear();
            foreach (var a in daten) ArtikelListe.Add(a);
            ArtikelView.Refresh();
        }

        private void Speichern()
        {
            var pfad = SpeisekarteDateiService.Speichern(ArtikelListe);
            MessageBox.Show($"Speisekarte gespeichert:\n{pfad}",
                            "Speisekarte", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // =========================
        // 7) CRUD-OPERATIONEN - SPEISEKARTE
        // =========================
        private void Neu()
        {
            var neueId = ArtikelListe.Count == 0 ? 1 : ArtikelListe.Max(a => a.Id) + 1;
            var neu = new Artikel { Id = neueId, Name = "Neuer Artikel", Preis = 0.01m, Menge = 0 };
            ArtikelListe.Add(neu);
            AusgewaehlterArtikel = neu;
        }

        private void Loeschen()
        {
            if (AusgewaehlterArtikel is null) return;
            ArtikelListe.Remove(AusgewaehlterArtikel);
            AusgewaehlterArtikel = null;
        }

        // =========================
        // 8) WARENKORB-OPERATIONEN (parametrisierte Kommandos)
        // =========================
        private void InWarenkorb()
        {
            if (AusgewaehlterArtikel is null) return;

            var art = AusgewaehlterArtikel;
            var bereitsImWarenkorb = MengeImWarenkorb(art.Id);

            // Lagerbestand prüfen
            if (bereitsImWarenkorb >= art.Menge)
            {
                MessageBox.Show(
                    $"Nicht genügend Lagerbestand für „{art.Name}“. Lager: {art.Menge}, bereits im Warenkorb: {bereitsImWarenkorb}.",
                    "Lager", MessageBoxButton.OK, MessageBoxImage.Information);
                RaiseCanExecute(InWarenkorbCmd);
                return;
            }

            // Bestehende Position finden oder neue erstellen
            var pos = Warenkorb.FirstOrDefault(p => p.ArtikelId == art.Id);
            if (pos is null)
            {
                pos = new Bestellposition
                {
                    ArtikelId = art.Id,
                    Name = art.Name,
                    Einzelpreis = art.Preis,
                    Menge = 1
                };
                Warenkorb.Add(pos);
            }
            else
            {
                pos.Menge += 1;
            }

            RechneGesamtsumme();
            RaiseCanExecute(InWarenkorbCmd);
            RaiseCanExecute(MengePlusCmd);
            RaiseCanExecute(MengeMinusCmd);
        }

        private void AusWarenkorb(Bestellposition? pos)
        {
            if (pos is null) return;

            if (pos.Menge > 1)
                pos.Menge -= 1; // Menge reduzieren
            else
                Warenkorb.Remove(pos); // Position entfernen wenn Menge 0

            RechneGesamtsumme();
            RaiseCanExecute(BelegSpeichernCmd);
            RaiseCanExecute(MengePlusCmd);
            RaiseCanExecute(MengeMinusCmd);
            RaiseCanExecute(InWarenkorbCmd);
        }

        private void MengePlus(Bestellposition? pos)
        {
            if (pos is null) return;
            var art = FindeArtikel(pos.ArtikelId);
            if (art is null) return;

            var mengeImWarenkorb = MengeImWarenkorb(pos.ArtikelId);
            if (mengeImWarenkorb >= art.Menge) return; // Lagerprüfung

            pos.Menge += 1;

            RechneGesamtsumme();
            RaiseCanExecute(MengePlusCmd);
            RaiseCanExecute(MengeMinusCmd);
            RaiseCanExecute(InWarenkorbCmd);
        }

        private void MengeMinus(Bestellposition? pos)
        {
            if (pos is null) return;

            if (pos.Menge > 1)
                pos.Menge -= 1;
            else
                Warenkorb.Remove(pos);

            RechneGesamtsumme();
            RaiseCanExecute(MengePlusCmd);
            RaiseCanExecute(MengeMinusCmd);
            RaiseCanExecute(InWarenkorbCmd);
            RaiseCanExecute(BelegSpeichernCmd);
        }

        private void PositionLoeschen(Bestellposition? pos)
        {
            if (pos is null) return;

            Warenkorb.Remove(pos);

            RechneGesamtsumme();
            RaiseCanExecute(MengePlusCmd);
            RaiseCanExecute(MengeMinusCmd);
            RaiseCanExecute(InWarenkorbCmd);
            RaiseCanExecute(BelegSpeichernCmd);
        }

        // =========================
        // 9) BELEG-FUNKTIONEN
        // =========================
        private void RechneGesamtsumme()
        {
            // Zwischensumme berechnen
            Zwischensumme = Math.Round(Warenkorb.Sum(p => p.Gesamt), 2);

            // Rabatt berechnen
            var rabattProzent = BitAndBiteCard ? 10m : 0m;
            RabattBetrag = Math.Round(Zwischensumme * (rabattProzent / 100m), 2);

            var netto = Zwischensumme - RabattBetrag;

            // Mehrwertsteuer berechnen
            MwstBetrag = MwstAktiv ? Math.Round(netto * (MwstSatz / 100m), 2) : 0m;

            // Endsumme berechnen
            Gesamtsumme = Math.Round(netto + MwstBetrag, 2);

            OnPropertyChanged(nameof(ArtikelAnzahl));
        }

        private void BelegSpeichern()
        {
            // Bestätigung dialog
            if (MessageBox.Show("Bestellung jetzt bezahlen und speichern?",
                    "Kauf", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;
            if (Warenkorb.Count == 0) return;

            var rabattProzent = BitAndBiteCard ? 10m : 0m;

            // Neuen Beleg erstellen
            var beleg = new Beleg
            {
                Datum = DateTime.Now,
                Positionen = Warenkorb.Select(p => new Bestellposition
                {
                    ArtikelId = p.ArtikelId,
                    Name = p.Name,
                    Einzelpreis = p.Einzelpreis,
                    Menge = p.Menge
                }).ToList(),
                Zwischensumme = Zwischensumme,
                RabattProzent = rabattProzent,
                RabattBetrag = RabattBetrag,
                MwstProzent = MwstAktiv ? MwstSatz : 0m,
                MwstBetrag = MwstBetrag,
                Endsumme = Gesamtsumme
            };

            // Lagerbestand aktualisieren
            foreach (var pos in Warenkorb)
            {
                var art = ArtikelListe.FirstOrDefault(a => a.Id == pos.ArtikelId);
                if (art != null) art.Menge = Math.Max(0, art.Menge - pos.Menge);
            }

            // Beleg speichern
            var pfad = BelegDateiService.Speichern(beleg);

            // Warenkorb leeren
            Warenkorb.Clear();
            RechneGesamtsumme();
            RaiseCanExecute(BelegSpeichernCmd);
            RaiseCanExecute(AusWarenkorbCmd);
            RaiseCanExecute(InWarenkorbCmd);

            // Erfolgsmeldung anzeigen
            MessageBox.Show(
                $"Beleg gespeichert:\n{pfad}\n\n" +
                $"Zwischensumme: {Zwischensumme:F2} €\n" +
                (rabattProzent > 0 ? $"Rabatt ({rabattProzent:F0}%): −{RabattBetrag:F2} €\n" : "") +
                (MwstAktiv ? $"MwSt ({MwstSatz:F0}%): +{MwstBetrag:F2} €\n" : "") +
                $"Endsumme: {Gesamtsumme:F2} €",
                "Beleg", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BelegeLaden()
        {
            Belege.Clear();
            foreach (var b in BelegDateiService.LadenAlle())
                Belege.Add(b);
        }

        private void BelegOrdnerOeffnen()
        {
            var pfad = BelegDateiService.OrdnerPfad();
            try
            {
                Process.Start(new ProcessStartInfo { FileName = pfad, UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ordner kann nicht geöffnet werden:\n{ex.Message}",
                    "Belege", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}