// CafePOS.Domain/Services/SpeisekarteService.cs
using CafePOS.Domain.Models;

namespace CafePOS.Domain.Services;

/// <summary>
/// Liefert eine einfache, feste Speisekarte (für den Start).
/// </summary>
public static class SpeisekarteService
{
    public static List<Artikel> LadeStandard()
    {
        return new List<Artikel>
        {
            new Artikel { Id = 1,  Name = "Walnuss (Walnusscreme)", Preis = 3.00m, Menge = 7 },
            new Artikel { Id = 2,  Name = "Schwarz-Weiß (Mandel & Kokos)", Preis = 3.10m, Menge = 8 },
            new Artikel { Id = 3,  Name = "Pfirsich & Ricotta", Preis = 3.00m, Menge = 4 },
            new Artikel { Id = 4,  Name = "Pistazie", Preis = 3.20m, Menge = 3 },
            new Artikel { Id = 5,  Name = "Karamell", Preis = 3.10m, Menge = 3 },
            new Artikel { Id = 6,  Name = "Nutela", Preis = 4.00m, Menge=7 },
            new Artikel { Id = 101, Name = "Espresso", Preis = 1.80m, Menge = 33 },
            new Artikel { Id = 102, Name = "Tassenkaffee", Preis = 2.20m, Menge = 20 },
            new Artikel { Id = 103, Name = "Cappuccino", Preis = 3.60m, Menge = 65 },
            new Artikel { Id = 111, Name = "Mineralwasser", Preis = 1.50m, Menge = 60 },
            new Artikel {Id = 112, Name = "Cola",Preis = 2.20m, Menge=72},
        };
    }
}