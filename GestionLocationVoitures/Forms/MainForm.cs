using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using GestionLocationVoitures.Data;
using GestionLocationVoitures.Models;

namespace GestionLocationVoitures.Forms
{
    public class MainForm : Form
    {
        // --- Contrôles Onglet 1 : Clients ---
        private DataGridView dgvClients = null!;
        private TextBox txtNom = null!;
        private TextBox txtPrenom = null!;
        private TextBox txtTelephone = null!;
        private TextBox txtEmail = null!;
        private TextBox txtPermis = null!;
        private TextBox txtRechercheClient = null!;
        private Button btnAjouterClient = null!;
        private Button btnModifierClient = null!;
        private Button btnSupprimerClient = null!;
        private Label lblModeClient = null!;
        private int? selectedClientId = null;

        // --- Contrôles Onglet 2 : Véhicules ---
        private DataGridView dgvVehicules = null!;
        private TextBox txtImmat = null!;
        private TextBox txtMarque = null!;
        private TextBox txtModele = null!;
        private NumericUpDown numKm = null!;
        private NumericUpDown numPrixJour = null!;
        private ComboBox cmbStatut = null!;
        private TextBox txtRechercheVehicule = null!;
        private Button btnAjouterVehicule = null!;
        private Button btnModifierVehicule = null!;
        private Button btnSupprimerVehicule = null!;
        private Label lblModeVehicule = null!;
        private int? selectedVehiculeId = null;

        public MainForm()
        {
            Text = "Gestion d'une Location de Voitures — EF Core & SQLite";
            Size = new Size(1060, 660);
            MinimumSize = new Size(1020, 600);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9.5F);

            InitialiserInterface();
            InitialiserDonneesSiVide();
            ChargerClients();
            ChargerVehicules();
            MettreAJourEtatBoutonsClient();
            MettreAJourEtatBoutonsVehicule();
        }

        private void InitialiserInterface()
        {
            TabControl tabs = new TabControl { Dock = DockStyle.Fill };

            TabPage tabClients = new TabPage("Gestion des Clients");
            TabPage tabVehicules = new TabPage("Gestion des Véhicules");

            ConstruireOngletClients(tabClients);
            ConstruireOngletVehicules(tabVehicules);

            tabs.TabPages.Add(tabClients);
            tabs.TabPages.Add(tabVehicules);
            tabs.SelectedIndexChanged += (s, e) =>
            {
                if (selectedClientId == null) dgvClients.ClearSelection();
                if (selectedVehiculeId == null) dgvVehicules.ClearSelection();
            };

            Controls.Add(tabs);
        }

        private static void ConfigurerDataGridView(DataGridView dgv)
        {
            dgv.Dock = DockStyle.Fill;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.FixedSingle;
            dgv.RowHeadersWidth = 28;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(246, 248, 252);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(235, 240, 248);
        }

        // =========================================================
        // ONGLET 1 : GESTION DES CLIENTS (CRUD)
        // =========================================================
        private void ConstruireOngletClients(TabPage page)
        {
            page.Padding = new Padding(12);

            GroupBox grpForm = new GroupBox
            {
                Text = "Informations du Client",
                Dock = DockStyle.Top,
                Height = 190,
                Padding = new Padding(10)
            };

            // Ligne 1 : Nom, Prénom, N° Permis
            Label lblNom = new Label { Text = "Nom :", Location = new Point(15, 30), AutoSize = true };
            txtNom = new TextBox { Location = new Point(105, 27), Width = 195 };

            Label lblPrenom = new Label { Text = "Prénom :", Location = new Point(325, 30), AutoSize = true };
            txtPrenom = new TextBox { Location = new Point(415, 27), Width = 195 };

            Label lblPermis = new Label { Text = "N° Permis :", Location = new Point(635, 30), AutoSize = true };
            txtPermis = new TextBox { Location = new Point(735, 27), Width = 260 };

            // Ligne 2 : Téléphone, Email
            Label lblTel = new Label { Text = "Téléphone :", Location = new Point(15, 68), AutoSize = true };
            txtTelephone = new TextBox { Location = new Point(105, 65), Width = 195 };

            Label lblEmail = new Label { Text = "Email :", Location = new Point(325, 68), AutoSize = true };
            txtEmail = new TextBox { Location = new Point(415, 65), Width = 340 };

            // Ligne 3 : Boutons d'action + Barre de recherche espacée
            btnAjouterClient = new Button { Text = "Ajouter", Location = new Point(15, 108), Width = 110, Height = 32 };
            btnModifierClient = new Button { Text = "Modifier", Location = new Point(135, 108), Width = 110, Height = 32 };
            btnSupprimerClient = new Button { Text = "Supprimer", Location = new Point(255, 108), Width = 110, Height = 32 };
            Button btnVider = new Button { Text = "Vider / Nouveau", Location = new Point(375, 108), Width = 145, Height = 32 };

            Label lblSearch = new Label { Text = "Rechercher :", Location = new Point(545, 114), AutoSize = true };
            txtRechercheClient = new TextBox
            {
                Location = new Point(655, 111),
                Width = 340,
                PlaceholderText = "Filtrer par nom, prénom ou N° permis..."
            };

            // Ligne 4 : Guide d'utilisation + Indicateur de mode
            Label lblGuide = new Label
            {
                Text = "Guide : Remplissez les champs vides pour Ajouter • Cliquez sur une ligne du tableau pour Modifier ou Supprimer.",
                Location = new Point(15, 154),
                AutoSize = true,
                ForeColor = Color.FromArgb(75, 85, 99),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Italic)
            };

            lblModeClient = new Label
            {
                Location = new Point(745, 153),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            btnAjouterClient.Click += BtnAjouterClient_Click;
            btnModifierClient.Click += BtnModifierClient_Click;
            btnSupprimerClient.Click += BtnSupprimerClient_Click;
            btnVider.Click += (s, e) => ViderFormulaireClient();
            txtRechercheClient.TextChanged += (s, e) => ChargerClients(txtRechercheClient.Text.Trim());

            grpForm.Controls.AddRange(new Control[]
            {
                lblNom, txtNom, lblPrenom, txtPrenom, lblPermis, txtPermis,
                lblTel, txtTelephone, lblEmail, txtEmail,
                btnAjouterClient, btnModifierClient, btnSupprimerClient, btnVider,
                lblSearch, txtRechercheClient, lblGuide, lblModeClient
            });

            dgvClients = new DataGridView();
            ConfigurerDataGridView(dgvClients);
            dgvClients.CellClick += DgvClients_CellClick;
            dgvClients.DataBindingComplete += (s, e) =>
            {
                if (selectedClientId == null) dgvClients.ClearSelection();
            };

            Panel pnlGrid = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 10, 0, 0) };
            pnlGrid.Controls.Add(dgvClients);

            page.Controls.Add(pnlGrid);
            page.Controls.Add(grpForm);
        }

        private void MettreAJourEtatBoutonsClient()
        {
            bool enEdition = selectedClientId != null;
            btnAjouterClient.Enabled = !enEdition;
            btnModifierClient.Enabled = enEdition;
            btnSupprimerClient.Enabled = enEdition;

            if (enEdition)
            {
                lblModeClient.Text = $"Mode : Édition (Client #{selectedClientId})";
                lblModeClient.ForeColor = Color.FromArgb(29, 78, 216);
            }
            else
            {
                lblModeClient.Text = "Mode : Nouvel ajout";
                lblModeClient.ForeColor = Color.FromArgb(21, 128, 61);
            }
        }

        private void ChargerClients(string filtre = "")
        {
            using var db = new AppDbContext();
            var query = db.Clients.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtre))
            {
                string f = filtre.ToLower();
                query = query.Where(c =>
                    c.Nom.ToLower().Contains(f) ||
                    c.Prenom.ToLower().Contains(f) ||
                    c.NumeroPermis.ToLower().Contains(f));
            }

            dgvClients.DataSource = query
                .Select(c => new
                {
                    c.Id,
                    c.Nom,
                    Prénom = c.Prenom,
                    Téléphone = c.Telephone,
                    c.Email,
                    Permis = c.NumeroPermis
                })
                .ToList();

            if (selectedClientId == null)
                dgvClients.ClearSelection();
        }

        private void DgvClients_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvClients.Rows[e.RowIndex];
            selectedClientId = Convert.ToInt32(row.Cells["Id"].Value);
            txtNom.Text = row.Cells["Nom"].Value?.ToString() ?? "";
            txtPrenom.Text = row.Cells["Prénom"].Value?.ToString() ?? "";
            txtTelephone.Text = row.Cells["Téléphone"].Value?.ToString() ?? "";
            txtEmail.Text = row.Cells["Email"].Value?.ToString() ?? "";
            txtPermis.Text = row.Cells["Permis"].Value?.ToString() ?? "";
            MettreAJourEtatBoutonsClient();
        }

        private void BtnAjouterClient_Click(object? sender, EventArgs e)
        {
            if (selectedClientId != null)
            {
                MessageBox.Show("Une ligne est actuellement sélectionnée. Cliquez sur 'Modifier' ou sur 'Vider / Nouveau' pour créer un nouveau client.", "Action bloquée", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNom.Text) || string.IsNullOrWhiteSpace(txtPrenom.Text) || string.IsNullOrWhiteSpace(txtPermis.Text))
            {
                MessageBox.Show("Veuillez remplir au minimum le Nom, le Prénom et le N° de Permis.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nomNorm = txtNom.Text.Trim().ToLower();
            string prenomNorm = txtPrenom.Text.Trim().ToLower();
            string telNorm = txtTelephone.Text.Trim().ToLower();
            string emailNorm = txtEmail.Text.Trim().ToLower();
            string permisNorm = txtPermis.Text.Trim().ToLower();

            using var db = new AppDbContext();

            // Vérification insensible à la casse : même N° de permis OU (même Nom+Prénom avec au moins un contact/permis identique)
            var clientExistant = db.Clients.AsEnumerable().FirstOrDefault(c =>
                c.NumeroPermis.Trim().ToLower() == permisNorm ||
                (c.Nom.Trim().ToLower() == nomNorm &&
                 c.Prenom.Trim().ToLower() == prenomNorm &&
                 ((!string.IsNullOrEmpty(permisNorm) && c.NumeroPermis.Trim().ToLower() == permisNorm) ||
                  (!string.IsNullOrEmpty(telNorm) && c.Telephone.Trim().ToLower() == telNorm) ||
                  (!string.IsNullOrEmpty(emailNorm) && c.Email.Trim().ToLower() == emailNorm))));

            if (clientExistant != null)
            {
                var rep = MessageBox.Show(
                    $"Le client « {clientExistant.Prenom} {clientExistant.Nom} » (Permis : {clientExistant.NumeroPermis}) existe déjà dans la base de données.\n\nVoulez-vous modifier ses informations avec celles saisies ?",
                    "Client déjà existant",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (rep == DialogResult.Yes)
                {
                    clientExistant.Nom = txtNom.Text.Trim();
                    clientExistant.Prenom = txtPrenom.Text.Trim();
                    clientExistant.Telephone = txtTelephone.Text.Trim();
                    clientExistant.Email = txtEmail.Text.Trim();
                    clientExistant.NumeroPermis = txtPermis.Text.Trim();
                    db.SaveChanges();

                    ChargerClients(txtRechercheClient.Text.Trim());
                    ViderFormulaireClient();
                    MessageBox.Show("Les informations du client existant ont été mises à jour.", "Modification effectuée", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }

            var client = new Client
            {
                Nom = txtNom.Text.Trim(),
                Prenom = txtPrenom.Text.Trim(),
                Telephone = txtTelephone.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                NumeroPermis = txtPermis.Text.Trim()
            };

            db.Clients.Add(client);
            db.SaveChanges();

            ChargerClients(txtRechercheClient.Text.Trim());
            ViderFormulaireClient();
            MessageBox.Show("Client ajouté avec succès dans SQLite.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnModifierClient_Click(object? sender, EventArgs e)
        {
            if (selectedClientId == null)
            {
                MessageBox.Show("Sélectionnez d'abord un client dans la liste.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNom.Text) || string.IsNullOrWhiteSpace(txtPrenom.Text) || string.IsNullOrWhiteSpace(txtPermis.Text))
            {
                MessageBox.Show("Le Nom, le Prénom et le N° de Permis sont obligatoires.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var db = new AppDbContext();
            var client = db.Clients.Find(selectedClientId.Value);
            if (client == null) return;

            client.Nom = txtNom.Text.Trim();
            client.Prenom = txtPrenom.Text.Trim();
            client.Telephone = txtTelephone.Text.Trim();
            client.Email = txtEmail.Text.Trim();
            client.NumeroPermis = txtPermis.Text.Trim();

            db.SaveChanges();
            ChargerClients(txtRechercheClient.Text.Trim());
            ViderFormulaireClient();
            MessageBox.Show("Client modifié avec succès.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnSupprimerClient_Click(object? sender, EventArgs e)
        {
            if (selectedClientId == null)
            {
                MessageBox.Show("Sélectionnez d'abord un client à supprimer.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Confirmer la suppression de ce client ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            using var db = new AppDbContext();
            var client = db.Clients.Find(selectedClientId.Value);
            if (client == null) return;

            db.Clients.Remove(client);
            db.SaveChanges();

            ChargerClients(txtRechercheClient.Text.Trim());
            ViderFormulaireClient();
        }

        private void ViderFormulaireClient()
        {
            selectedClientId = null;
            txtNom.Clear();
            txtPrenom.Clear();
            txtTelephone.Clear();
            txtEmail.Clear();
            txtPermis.Clear();
            dgvClients.ClearSelection();
            MettreAJourEtatBoutonsClient();
        }

        // =========================================================
        // ONGLET 2 : GESTION DES VÉHICULES (CRUD)
        // =========================================================
        private void ConstruireOngletVehicules(TabPage page)
        {
            page.Padding = new Padding(12);

            GroupBox grpVeh = new GroupBox
            {
                Text = "Informations du Véhicule",
                Dock = DockStyle.Top,
                Height = 190,
                Padding = new Padding(10)
            };

            // Ligne 1 : Matricule, Marque, Modèle
            Label lblImmat = new Label { Text = "Matricule :", Location = new Point(15, 30), AutoSize = true };
            txtImmat = new TextBox { Location = new Point(115, 27), Width = 185 };

            Label lblMarque = new Label { Text = "Marque :", Location = new Point(325, 30), AutoSize = true };
            txtMarque = new TextBox { Location = new Point(425, 27), Width = 185 };

            Label lblModele = new Label { Text = "Modèle :", Location = new Point(635, 30), AutoSize = true };
            txtModele = new TextBox { Location = new Point(720, 27), Width = 275 };

            // Ligne 2 : Kilométrage, Prix / Jour, Statut
            Label lblKm = new Label { Text = "Kilométrage :", Location = new Point(15, 68), AutoSize = true };
            numKm = new NumericUpDown { Location = new Point(115, 65), Width = 185, Maximum = 1000000, Increment = 500 };

            Label lblPrix = new Label { Text = "Prix / Jour :", Location = new Point(325, 68), AutoSize = true };
            numPrixJour = new NumericUpDown { Location = new Point(425, 65), Width = 185, Maximum = 5000, DecimalPlaces = 2, Value = 100 };

            Label lblStatut = new Label { Text = "Statut :", Location = new Point(635, 68), AutoSize = true };
            cmbStatut = new ComboBox
            {
                Location = new Point(720, 65),
                Width = 275,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DataSource = Enum.GetValues(typeof(StatutVehicule))
            };

            // Ligne 3 : Boutons + Recherche espacée
            btnAjouterVehicule = new Button { Text = "Ajouter", Location = new Point(15, 108), Width = 110, Height = 32 };
            btnModifierVehicule = new Button { Text = "Modifier", Location = new Point(135, 108), Width = 110, Height = 32 };
            btnSupprimerVehicule = new Button { Text = "Supprimer", Location = new Point(255, 108), Width = 110, Height = 32 };
            Button btnClearV = new Button { Text = "Vider / Nouveau", Location = new Point(375, 108), Width = 145, Height = 32 };

            Label lblSearchV = new Label { Text = "Rechercher :", Location = new Point(545, 114), AutoSize = true };
            txtRechercheVehicule = new TextBox
            {
                Location = new Point(655, 111),
                Width = 340,
                PlaceholderText = "Filtrer par matricule, marque ou modèle..."
            };

            // Ligne 4 : Guide d'utilisation + Indicateur de mode
            Label lblGuideV = new Label
            {
                Text = "Guide : Remplissez les champs vides pour Ajouter • Cliquez sur une ligne du tableau pour Modifier ou Supprimer.",
                Location = new Point(15, 154),
                AutoSize = true,
                ForeColor = Color.FromArgb(75, 85, 99),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Italic)
            };

            lblModeVehicule = new Label
            {
                Location = new Point(745, 153),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            btnAjouterVehicule.Click += BtnAjouterVehicule_Click;
            btnModifierVehicule.Click += BtnModifierVehicule_Click;
            btnSupprimerVehicule.Click += BtnSupprimerVehicule_Click;
            btnClearV.Click += (s, e) => ViderFormulaireVehicule();
            txtRechercheVehicule.TextChanged += (s, e) => ChargerVehicules(txtRechercheVehicule.Text.Trim());

            grpVeh.Controls.AddRange(new Control[]
            {
                lblImmat, txtImmat, lblMarque, txtMarque, lblModele, txtModele,
                lblKm, numKm, lblPrix, numPrixJour, lblStatut, cmbStatut,
                btnAjouterVehicule, btnModifierVehicule, btnSupprimerVehicule, btnClearV,
                lblSearchV, txtRechercheVehicule, lblGuideV, lblModeVehicule
            });

            dgvVehicules = new DataGridView();
            ConfigurerDataGridView(dgvVehicules);
            dgvVehicules.CellClick += DgvVehicules_CellClick;
            dgvVehicules.DataBindingComplete += (s, e) =>
            {
                if (selectedVehiculeId == null) dgvVehicules.ClearSelection();
            };

            Panel pnlGridV = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 10, 0, 0) };
            pnlGridV.Controls.Add(dgvVehicules);

            page.Controls.Add(pnlGridV);
            page.Controls.Add(grpVeh);
        }

        private void MettreAJourEtatBoutonsVehicule()
        {
            bool enEdition = selectedVehiculeId != null;
            btnAjouterVehicule.Enabled = !enEdition;
            btnModifierVehicule.Enabled = enEdition;
            btnSupprimerVehicule.Enabled = enEdition;

            if (enEdition)
            {
                lblModeVehicule.Text = $"Mode : Édition (Véhicule #{selectedVehiculeId})";
                lblModeVehicule.ForeColor = Color.FromArgb(29, 78, 216);
            }
            else
            {
                lblModeVehicule.Text = "Mode : Nouvel ajout";
                lblModeVehicule.ForeColor = Color.FromArgb(21, 128, 61);
            }
        }

        private void ChargerVehicules(string filtre = "")
        {
            using var db = new AppDbContext();
            var query = db.Vehicules.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtre))
            {
                string f = filtre.ToLower();
                query = query.Where(v =>
                    v.Immatriculation.ToLower().Contains(f) ||
                    v.Marque.ToLower().Contains(f) ||
                    v.Modele.ToLower().Contains(f));
            }

            dgvVehicules.DataSource = query
                .Select(v => new
                {
                    v.Id,
                    Immatriculation = v.Immatriculation,
                    v.Marque,
                    Modèle = v.Modele,
                    Kilométrage = v.Kilometrage,
                    PrixParJour = v.PrixParJour,
                    v.Statut
                })
                .ToList();

            if (selectedVehiculeId == null)
                dgvVehicules.ClearSelection();
        }

        private void DgvVehicules_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvVehicules.Rows[e.RowIndex];
            selectedVehiculeId = Convert.ToInt32(row.Cells["Id"].Value);
            txtImmat.Text = row.Cells["Immatriculation"].Value?.ToString() ?? "";
            txtMarque.Text = row.Cells["Marque"].Value?.ToString() ?? "";
            txtModele.Text = row.Cells["Modèle"].Value?.ToString() ?? "";
            numKm.Value = Convert.ToDecimal(row.Cells["Kilométrage"].Value);
            numPrixJour.Value = Convert.ToDecimal(row.Cells["PrixParJour"].Value);
            if (row.Cells["Statut"].Value is StatutVehicule st)
                cmbStatut.SelectedItem = st;
            MettreAJourEtatBoutonsVehicule();
        }

        private void BtnAjouterVehicule_Click(object? sender, EventArgs e)
        {
            if (selectedVehiculeId != null)
            {
                MessageBox.Show("Une ligne est actuellement sélectionnée. Cliquez sur 'Modifier' ou sur 'Vider / Nouveau' pour ajouter un nouveau véhicule.", "Action bloquée", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtImmat.Text) || string.IsNullOrWhiteSpace(txtMarque.Text))
            {
                MessageBox.Show("Veuillez saisir au minimum l'immatriculation et la marque.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string immatNorm = txtImmat.Text.Trim().ToLower();

            using var db = new AppDbContext();
            var vehiculeExistant = db.Vehicules.AsEnumerable()
                .FirstOrDefault(v => v.Immatriculation.Trim().ToLower() == immatNorm);

            if (vehiculeExistant != null)
            {
                var rep = MessageBox.Show(
                    $"Un véhicule avec le matricule « {vehiculeExistant.Immatriculation} » ({vehiculeExistant.Marque} {vehiculeExistant.Modele}) existe déjà.\n\nVoulez-vous modifier ses informations avec celles saisies ?",
                    "Véhicule déjà existant",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (rep == DialogResult.Yes)
                {
                    vehiculeExistant.Immatriculation = txtImmat.Text.Trim();
                    vehiculeExistant.Marque = txtMarque.Text.Trim();
                    vehiculeExistant.Modele = txtModele.Text.Trim();
                    vehiculeExistant.Kilometrage = (int)numKm.Value;
                    vehiculeExistant.PrixParJour = numPrixJour.Value;
                    vehiculeExistant.Statut = (StatutVehicule)(cmbStatut.SelectedItem ?? StatutVehicule.Disponible);
                    db.SaveChanges();

                    ChargerVehicules(txtRechercheVehicule.Text.Trim());
                    ViderFormulaireVehicule();
                    MessageBox.Show("Les informations du véhicule existant ont été mises à jour.", "Modification effectuée", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }

            var v = new Vehicule
            {
                Immatriculation = txtImmat.Text.Trim(),
                Marque = txtMarque.Text.Trim(),
                Modele = txtModele.Text.Trim(),
                Kilometrage = (int)numKm.Value,
                PrixParJour = numPrixJour.Value,
                Statut = (StatutVehicule)(cmbStatut.SelectedItem ?? StatutVehicule.Disponible)
            };

            db.Vehicules.Add(v);
            db.SaveChanges();

            ChargerVehicules(txtRechercheVehicule.Text.Trim());
            ViderFormulaireVehicule();
            MessageBox.Show("Véhicule enregistré dans SQLite.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnModifierVehicule_Click(object? sender, EventArgs e)
        {
            if (selectedVehiculeId == null)
            {
                MessageBox.Show("Sélectionnez d'abord un véhicule dans la liste.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtImmat.Text) || string.IsNullOrWhiteSpace(txtMarque.Text))
            {
                MessageBox.Show("L'immatriculation et la marque sont obligatoires.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var db = new AppDbContext();
            var v = db.Vehicules.Find(selectedVehiculeId.Value);
            if (v == null) return;

            v.Immatriculation = txtImmat.Text.Trim();
            v.Marque = txtMarque.Text.Trim();
            v.Modele = txtModele.Text.Trim();
            v.Kilometrage = (int)numKm.Value;
            v.PrixParJour = numPrixJour.Value;
            v.Statut = (StatutVehicule)(cmbStatut.SelectedItem ?? StatutVehicule.Disponible);

            db.SaveChanges();
            ChargerVehicules(txtRechercheVehicule.Text.Trim());
            ViderFormulaireVehicule();
            MessageBox.Show("Véhicule mis à jour.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnSupprimerVehicule_Click(object? sender, EventArgs e)
        {
            if (selectedVehiculeId == null)
            {
                MessageBox.Show("Sélectionnez d'abord un véhicule à supprimer.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Confirmer la suppression de ce véhicule ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            using var db = new AppDbContext();
            var v = db.Vehicules.Find(selectedVehiculeId.Value);
            if (v == null) return;

            db.Vehicules.Remove(v);
            db.SaveChanges();

            ChargerVehicules(txtRechercheVehicule.Text.Trim());
            ViderFormulaireVehicule();
        }

        private void ViderFormulaireVehicule()
        {
            selectedVehiculeId = null;
            txtImmat.Clear();
            txtMarque.Clear();
            txtModele.Clear();
            numKm.Value = 0;
            numPrixJour.Value = 100;
            cmbStatut.SelectedItem = StatutVehicule.Disponible;
            dgvVehicules.ClearSelection();
            MettreAJourEtatBoutonsVehicule();
        }

        private void InitialiserDonneesSiVide()
        {
            using var db = new AppDbContext();
            db.Database.Migrate();

            if (!db.Clients.Any())
            {
                db.Clients.AddRange(
                    new Client { Nom = "Chafei", Prenom = "Rayen", Telephone = "20000002", Email = "rayen@mail.tn", NumeroPermis = "TN-987654" },
                    new Client { Nom = "Latrous", Prenom = "Yassine", Telephone = "20000001", Email = "yassine@mail.tn", NumeroPermis = "TN-123456" }
                );
            }

            if (!db.Vehicules.Any())
            {
                db.Vehicules.AddRange(
                    new Vehicule { Immatriculation = "230 TN 4512", Marque = "Volkswagen", Modele = "Golf 8", Kilometrage = 42450, PrixParJour = 120m, Statut = StatutVehicule.Disponible },
                    new Vehicule { Immatriculation = "224 TN 8810", Marque = "Peugeot", Modele = "208", Kilometrage = 31000, PrixParJour = 95m, Statut = StatutVehicule.Disponible }
                );
            }

            db.SaveChanges();
        }
    }
}