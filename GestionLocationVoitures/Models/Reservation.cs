using System;

namespace GestionLocationVoitures.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public StatutReservation Statut { get; set; }
        public Client Client { get; set; }
        public Vehicule Vehicule { get; set; }

        public Reservation(int id, Client client, Vehicule vehicule, DateTime dateDebut, DateTime dateFin)
        {
            if (dateFin <= dateDebut)
                throw new ArgumentException("La date de fin doit être postérieure à la date de début.");

            Id = id;
            Client = client;
            Vehicule = vehicule;
            DateDebut = dateDebut;
            DateFin = dateFin;
            Statut = StatutReservation.EnAttente;
        }

        public int CalculerDureeJours()
        {
            return (DateFin.Date - DateDebut.Date).Days;
        }

        public void Confirmer()
        {
            if (Vehicule.Statut != StatutVehicule.Disponible)
                throw new InvalidOperationException("Le véhicule n'est pas disponible.");

            Statut = StatutReservation.Confirmee;
            Vehicule.Statut = StatutVehicule.Reserve;
        }

        public void Annuler()
        {
            Statut = StatutReservation.Annulee;
            if (Vehicule.Statut == StatutVehicule.Reserve)
            {
                Vehicule.Statut = StatutVehicule.Disponible;
            }
        }
    }
}