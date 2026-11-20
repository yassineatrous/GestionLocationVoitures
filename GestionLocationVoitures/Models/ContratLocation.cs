using System;

namespace GestionLocationVoitures.Models
{
    public class ContratLocation
    {
        public int Id { get; set; }
        public Reservation Reservation { get; set; } = null!;
        public Agent Agent { get; set; } = null!;
        public DateTime DateSignature { get; set; }
        public DateTime? DateRetourEffective { get; set; }
        public int KilometrageDepart { get; set; }
        public int? KilometrageRetour { get; set; }
        public decimal Caution { get; set; }
        public decimal MontantTotal { get; set; }
        public bool EstCloture { get; set; }

        public ContratLocation() { }

        public ContratLocation(int id, Reservation reservation, Agent agent, decimal caution)
        {
            if (reservation.Statut != StatutReservation.Confirmee)
                throw new InvalidOperationException("La réservation doit être confirmée avant de créer un contrat.");

            Id = id;
            Reservation = reservation;
            Agent = agent;
            DateSignature = DateTime.Now;
            KilometrageDepart = reservation.Vehicule.Kilometrage;
            Caution = caution;
            MontantTotal = reservation.CalculerDureeJours() * reservation.Vehicule.PrixParJour;
            EstCloture = false;
            reservation.Vehicule.Statut = StatutVehicule.Loue;
        }

        public void CloturerContrat(DateTime dateRetour, int kmRetour)
        {
            DateRetourEffective = dateRetour;
            KilometrageRetour = kmRetour;
            Reservation.Vehicule.Kilometrage = kmRetour;

            int joursRetard = (dateRetour.Date - Reservation.DateFin.Date).Days;
            if (joursRetard > 0)
            {
                MontantTotal += joursRetard * 50m;
            }

            Reservation.Vehicule.Statut = StatutVehicule.Disponible;
            EstCloture = true;
        }
    }
}