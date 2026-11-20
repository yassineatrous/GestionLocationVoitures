namespace GestionLocationVoitures.Models
{
    public abstract class Personne
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        protected Personne() { }

        protected Personne(int id, string nom, string prenom, string telephone, string email)
        {
            Id = id;
            Nom = nom;
            Prenom = prenom;
            Telephone = telephone;
            Email = email;
        }

        public virtual string AfficherInfos()
        {
            return $"{Prenom} {Nom} | Tél: {Telephone} | Email: {Email}";
        }
    }

    public class Client : Personne
    {
        public string NumeroPermis { get; set; } = string.Empty;

        public Client() { }

        public Client(int id, string nom, string prenom, string telephone, string email, string numeroPermis)
            : base(id, nom, prenom, telephone, email)
        {
            NumeroPermis = numeroPermis;
        }

        public override string AfficherInfos()
        {
            return $"[Client] {base.AfficherInfos()} | Permis: {NumeroPermis}";
        }
    }

    public class Agent : Personne
    {
        public string Matricule { get; set; } = string.Empty;
        public string Poste { get; set; } = string.Empty;

        public Agent() { }

        public Agent(int id, string nom, string prenom, string telephone, string email, string matricule, string poste)
            : base(id, nom, prenom, telephone, email)
        {
            Matricule = matricule;
            Poste = poste;
        }

        public override string AfficherInfos()
        {
            return $"[Agent {Matricule} - {Poste}] {base.AfficherInfos()}";
        }
    }
}