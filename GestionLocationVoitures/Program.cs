using System;
using GestionLocationVoitures.Models;

namespace GestionLocationVoitures
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Agent agent = new Agent(1, "Latrous", "Yassine", "20000001", "yassine@agence.tn", "AG-01", "Responsable Agence");
            Client client = new Client(2, "Chafei", "Rayen", "20000002", "rayen@mail.tn", "TN-987654");
            Vehicule voiture = new Vehicule(1, "230 TN 4512", "Volkswagen", "Golf 8", 42000, 120m);

            Console.WriteLine(agent.AfficherInfos());
            Console.WriteLine(client.AfficherInfos());

            Reservation resa = new Reservation(1, client, voiture, DateTime.Today, DateTime.Today.AddDays(3));
            resa.Confirmer();
            Console.WriteLine($"Réservation #{resa.Id} confirmée ({resa.CalculerDureeJours()} jours) - Statut véhicule : {voiture.Statut}");

            ContratLocation contrat = new ContratLocation(1, resa, agent, 500m);
            Console.WriteLine($"Contrat #{contrat.Id} établi par {contrat.Agent.Prenom} - Montant initial : {contrat.MontantTotal} DT");

            contrat.CloturerContrat(DateTime.Today.AddDays(3), 42450);
            Console.WriteLine($"Contrat clôturé - Kilométrage véhicule : {voiture.Kilometrage} km - Statut : {voiture.Statut}");
        }
    }
}
