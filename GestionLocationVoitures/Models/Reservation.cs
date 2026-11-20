using System;

namespace GestionLocationVoitures.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public Client Client { get; set; } = null!;
        public Vehicule Vehicule { get; set; } = null!;
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public StatutReservation Statut { get; set; }

        public Reservation() { }

        public Reservation(int id, Client client, Vehicule vehicule, DateTime dateDebut, DateTime dateFin)
        {
            Id = id;
            Client = client;
            Vehicule = vehicule;
            DateDebut = dateDebut;
            DateFin = dateFin;
            Statut = StatutReservation.EnAttente;
        }

        public int CalculerDureeJours()
        {
            int jours = (DateFin.Date - DateDebut.Date).Days;
            return jours > 0 ? jours : 1;
        }

        public void Confirmer()
        {
            if (Vehicule.Statut != StatutVehicule.Disponible)
                throw new InvalidOperationException("Ce véhicule n'est pas disponible.");

            Statut = StatutReservation.Confirmee;
            Vehicule.Statut = StatutVehicule.Reserve;
        }

        public void Annuler()
        {
            Statut = StatutReservation.Annulee;
            if (Vehicule.Statut == StatutVehicule.Reserve)
                Vehicule.Statut = StatutVehicule.Disponible;
        }
    }
}