using System;

namespace GestionLocationVoitures.Models
{
    public class Entretien
    {
        public int Id { get; set; }
        public DateTime DateEntretien { get; set; }
        public string TypeEntretien { get; set; } = string.Empty;
        public decimal Cout { get; set; }
        public string Notes { get; set; } = string.Empty;

        public Entretien() { }

        public Entretien(int id, DateTime dateEntretien, string typeEntretien, decimal cout, string notes)
        {
            Id = id;
            DateEntretien = dateEntretien;
            TypeEntretien = typeEntretien;
            Cout = cout;
            Notes = notes;
        }
    }
}