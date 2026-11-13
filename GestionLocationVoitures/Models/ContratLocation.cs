using System;

namespace GestionLocationVoitures.Models
{
    public class ContratLocation
    {
        public int Id { get; set; }
        public DateTime DateSignature { get; set; }
        public DateTime? DateRetourEffective { get; set; }
        public int KilometrageDepart { get; set; }
        public int? KilometrageRetour { get; set; }
        public decimal Caution { get; set; }
        public decimal MontantTotal { get; private set; }
        public bool EstCloture { get; private set; }
        public Reservation Reservation { get; set; }
        public Agent Agent { get; set; }

        public ContratLocation(int id, Reservation reservation, Agent agent, decimal caution)
        {
            if (reservation.Statut != StatutReservation.Confirmee)
                throw new InvalidOperationException("La réservation doit être confirmée avant d'établir un contrat.");

            Id = id;
            Reservation = reservation;
            Agent = agent;
            DateSignature = DateTime.Now;
            KilometrageDepart = reservation.Vehicule.Kilometrage;
            Caution = caution;
            EstCloture = false;

            reservation.Vehicule.Statut = StatutVehicule.Loue;
            MontantTotal = reservation.CalculerDureeJours() * reservation.Vehicule.PrixParJour;
        }

        public void CloturerContrat(DateTime dateRetour, int kmRetour)
        {
            if (kmRetour < KilometrageDepart)
                throw new ArgumentException("Le kilométrage de retour ne peut pas être inférieur au kilométrage de départ.");

            DateRetourEffective = dateRetour;
            KilometrageRetour = kmRetour;
            EstCloture = true;

            int joursRetard = (dateRetour.Date - Reservation.DateFin.Date).Days;
            if (joursRetard > 0)
            {
                decimal penaliteParJour = 50m;
                MontantTotal += joursRetard * (Reservation.Vehicule.PrixParJour + penaliteParJour);
            }

            Reservation.Vehicule.Kilometrage = kmRetour;
            Reservation.Vehicule.Statut = StatutVehicule.Disponible;
        }
    }
}