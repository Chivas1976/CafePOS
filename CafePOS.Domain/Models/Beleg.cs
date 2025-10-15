using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace CafePOS.Domain.Models
{
   
    public class Beleg
    {
        public Guid BelegId { get; set; } = Guid.NewGuid();
        public DateTime Datum { get; set; } = DateTime.Now;

        public List<Bestellposition> Positionen { get; set; } = new();

       
        public decimal Zwischensumme { get; set; }   
        public decimal RabattProzent { get; set; }  
        public decimal RabattBetrag { get; set; }    
        public decimal MwstProzent { get; set; }     
        public decimal MwstBetrag { get; set; }      
        public decimal Endsumme { get; set; }       

       
        [JsonIgnore]
        public decimal Summe => Math.Round(Positionen.Sum(p => p.Gesamt), 2);
    }
}