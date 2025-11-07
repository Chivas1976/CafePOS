using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// CafePOS.Domain/Models/Rolle.cs
namespace CafePOS.Domain.Models;

/// <summary>
/// Rolle eines Benutzers in der Anwendung.
/// </summary>
public enum Rolle
{
    Mitarbeiter = 0,
    Chef = 1
}
