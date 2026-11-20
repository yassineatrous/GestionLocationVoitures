using System.Collections.Generic;

namespace GestionLocationVoitures.Models
{
    public class Vehicule
    {
        public int Id { get; set; }
        public string Immatriculation { get; set; } = string.Empty;
        public string Marque { get; set; } = string.Empty;
        public string Modele { get; set; } = string.Empty;
        public int Kilometrage { get; set; }
        public decimal PrixParJour { get; set; }
        public StatutVehicule Statut { get; set; }
        public List<Entretien> HistoriqueEntretiens { get; set; } = new List<Entretien>();

        public Vehicule() { }

        public Vehicule(int id, string immatriculation, string marque, string modele, int kilometrage, decimal prixParJour)
        {
            Id = id;
            Immatriculation = immatriculation;
            Marque = marque;
            Modele = modele;
            Kilometrage = kilometrage;
            PrixParJour = prixParJour;
            Statut = StatutVehicule.Disponible;
        }

        public void AjouterEntretien(Entretien e)
        {
            HistoriqueEntretiens.Add(e);
            Statut = StatutVehicule.EnMaintenance;
        }

        public void TerminerEntretien()
        {
            if (Statut == StatutVehicule.EnMaintenance)
            {
                Statut = StatutVehicule.Disponible;
            }
        }
    }
}