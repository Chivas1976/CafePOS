using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// CafePOS.Domain/Models/Benutzer.cs
namespace CafePOS.Domain.Models;

/// <summary>
/// Einfaches Benutzerobjekt für Login und Rollensteuerung.
/// </summary>
public class Benutzer
{
    /// <summary>Anzeigename (z. B. "Anna Schmidt").</summary>
    public string Anzeigename { get; set; } = string.Empty;

    /// <summary>Loginname (z. B. "anna").</summary>
    public string Loginname { get; set; } = string.Empty;

    /// <summary>Klartext-Passwort (für die Projektwoche ausreichend).</summary>
    public string Passwort { get; set; } = string.Empty;

    /// <summary>Rolle bestimmt die Berechtigungen (Mitarbeiter/Chef).</summary>
    public Rolle Rolle { get; set; } = Rolle.Mitarbeiter;
}