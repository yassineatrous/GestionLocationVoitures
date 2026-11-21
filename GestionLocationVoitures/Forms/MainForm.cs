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

        // --- Contrôles Onglet 3 : Réservations ---
        private DataGridView dgvReservations = null!;
        private ComboBox cmbResClient = null!;
        private ComboBox cmbResVehicule = null!;
        private DateTimePicker dtpDebut = null!;
        private DateTimePicker dtpFin = null!;
        private ComboBox cmbResStatut = null!;
        private Label lblEstimation = null!;
        private TextBox txtRechercheRes = null!;
        private Button btnAjouterRes = null!;
        private Button btnConfirmerRes = null!;
        private Button btnAnnulerRes = null!;
        private Button btnSupprimerRes = null!;
        private Label lblModeRes = null!;
        private int? selectedReservationId = null;

        // --- Barre d'état KPI en bas ---
        private ToolStripStatusLabel lblKpiClients = null!;
        private ToolStripStatusLabel lblKpiDispo = null!;
        private ToolStripStatusLabel lblKpiReserves = null!;
        private ToolStripStatusLabel lblKpiMaint = null!;
        private ToolStripStatusLabel lblKpiReservations = null!;

        private sealed class ComboItem
        {
            public int Id { get; init; }
            public string Libelle { get; init; } = string.Empty;
            public decimal PrixParJour { get; init; }
            public override string ToString() => Libelle;
        }

        public MainForm()
        {
            Text = "Gestion d'une Location de Voitures — EF Core & SQLite";
            Size = new Size(1120, 700);
            MinimumSize = new Size(1060, 640);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9.5F);

            InitialiserInterface();
            InitialiserDonneesSiVide();

            ChargerClients();
            ChargerVehicules();
            ChargerCombosReservation();
            ChargerReservations();
            ActualiserKpis();

            MettreAJourEtatBoutonsClient();
            MettreAJourEtatBoutonsVehicule();
            MettreAJourEtatBoutonsReservation();
            MettreAJourEstimationReservation();
        }

        private void InitialiserInterface()
        {
            TabControl tabs = new TabControl { Dock = DockStyle.Fill };

            TabPage tabClients = new TabPage("Gestion des Clients");
            TabPage tabVehicules = new TabPage("Gestion des Véhicules");
            TabPage tabReservations = new TabPage("Gestion des Réservations");

            ConstruireOngletClients(tabClients);
            ConstruireOngletVehicules(tabVehicules);
            ConstruireOngletReservations(tabReservations);

            tabs.TabPages.Add(tabClients);
            tabs.TabPages.Add(tabVehicules);
            tabs.TabPages.Add(tabReservations);
            tabs.SelectedIndexChanged += (s, e) =>
            {
                if (selectedClientId == null) dgvClients.ClearSelection();
                if (selectedVehiculeId == null) dgvVehicules.ClearSelection();
                if (selectedReservationId == null) dgvReservations.ClearSelection();
            };

            StatusStrip statusBar = new StatusStrip
            {
                BackColor = Color.FromArgb(241, 245, 249),
                SizingGrip = false
            };

            lblKpiClients = new ToolStripStatusLabel { Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(30, 41, 59) };
            lblKpiDispo = new ToolStripStatusLabel { Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(21, 128, 61) };
            lblKpiReserves = new ToolStripStatusLabel { Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(29, 78, 216) };
            lblKpiMaint = new ToolStripStatusLabel { Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(180, 83, 9) };
            lblKpiReservations = new ToolStripStatusLabel { Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(88, 28, 135) };

            statusBar.Items.AddRange(new ToolStripItem[]
            {
                lblKpiClients,
                new ToolStripSeparator(),
                lblKpiDispo,
                new ToolStripSeparator(),
                lblKpiReserves,
                new ToolStripSeparator(),
                lblKpiMaint,
                new ToolStripSeparator(),
                lblKpiReservations
            });

            Controls.Add(tabs);
            Controls.Add(statusBar);
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

        private void ActualiserKpis()
        {
            using var db = new AppDbContext();
            int nbClients = db.Clients.Count();
            int nbDispo = db.Vehicules.Count(v => v.Statut == StatutVehicule.Disponible);
            int nbResLoue = db.Vehicules.Count(v => v.Statut == StatutVehicule.Reserve || v.Statut == StatutVehicule.Loue);
            int nbMaint = db.Vehicules.Count(v => v.Statut == StatutVehicule.EnMaintenance);
            int nbResActives = db.Reservations.Count(r => r.Statut != StatutReservation.Annulee);

            lblKpiClients.Text = $"  Clients : {nbClients}  ";
            lblKpiDispo.Text = $"  Véhicules Disponibles : {nbDispo}  ";
            lblKpiReserves.Text = $"  Réservés / Loués : {nbResLoue}  ";
            lblKpiMaint.Text = $"  En Maintenance : {nbMaint}  ";
            lblKpiReservations.Text = $"  Réservations Actives : {nbResActives}  ";
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

            Label lblNom = new Label { Text = "Nom :", Location = new Point(15, 30), AutoSize = true };
            txtNom = new TextBox { Location = new Point(105, 27), Width = 205 };

            Label lblPrenom = new Label { Text = "Prénom :", Location = new Point(335, 30), AutoSize = true };
            txtPrenom = new TextBox { Location = new Point(425, 27), Width = 205 };

            Label lblPermis = new Label { Text = "N° Permis :", Location = new Point(655, 30), AutoSize = true };
            txtPermis = new TextBox { Location = new Point(755, 27), Width = 285 };

            Label lblTel = new Label { Text = "Téléphone :", Location = new Point(15, 68), AutoSize = true };
            txtTelephone = new TextBox { Location = new Point(105, 65), Width = 205 };

            Label lblEmail = new Label { Text = "Email :", Location = new Point(335, 68), AutoSize = true };
            txtEmail = new TextBox { Location = new Point(425, 65), Width = 360 };

            btnAjouterClient = new Button { Text = "Ajouter", Location = new Point(15, 108), Width = 110, Height = 32 };
            btnModifierClient = new Button { Text = "Modifier", Location = new Point(135, 108), Width = 110, Height = 32 };
            btnSupprimerClient = new Button { Text = "Supprimer", Location = new Point(255, 108), Width = 110, Height = 32 };
            Button btnVider = new Button { Text = "Vider / Nouveau", Location = new Point(375, 108), Width = 145, Height = 32 };

            Label lblSearch = new Label { Text = "Rechercher :", Location = new Point(550, 114), AutoSize = true };
            txtRechercheClient = new TextBox
            {
                Location = new Point(660, 111),
                Width = 380,
                PlaceholderText = "Filtrer par nom, prénom ou N° permis..."
            };

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
                Location = new Point(785, 153),
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
                    ChargerCombosReservation();
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
            ChargerCombosReservation();
            ActualiserKpis();
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

            string permisNorm = txtPermis.Text.Trim().ToLower();
            using var db = new AppDbContext();

            bool conflitPermis = db.Clients.AsEnumerable()
                .Any(c => c.Id != selectedClientId.Value && c.NumeroPermis.Trim().ToLower() == permisNorm);

            if (conflitPermis)
            {
                MessageBox.Show("Un autre client possède déjà ce numéro de permis.", "Doublon détecté", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var client = db.Clients.Find(selectedClientId.Value);
            if (client == null) return;

            client.Nom = txtNom.Text.Trim();
            client.Prenom = txtPrenom.Text.Trim();
            client.Telephone = txtTelephone.Text.Trim();
            client.Email = txtEmail.Text.Trim();
            client.NumeroPermis = txtPermis.Text.Trim();

            db.SaveChanges();
            ChargerClients(txtRechercheClient.Text.Trim());
            ChargerCombosReservation();
            ChargerReservations(txtRechercheRes.Text.Trim());
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

            using var db = new AppDbContext();
            if (db.Reservations.Any(r => r.Client.Id == selectedClientId.Value))
            {
                MessageBox.Show("Impossible de supprimer ce client car il est lié à une ou plusieurs réservations.", "Suppression bloquée", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Confirmer la suppression de ce client ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            var client = db.Clients.Find(selectedClientId.Value);
            if (client == null) return;

            db.Clients.Remove(client);
            db.SaveChanges();

            ChargerClients(txtRechercheClient.Text.Trim());
            ChargerCombosReservation();
            ActualiserKpis();
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

            Label lblImmat = new Label { Text = "Matricule :", Location = new Point(15, 30), AutoSize = true };
            txtImmat = new TextBox { Location = new Point(115, 27), Width = 195 };

            Label lblMarque = new Label { Text = "Marque :", Location = new Point(335, 30), AutoSize = true };
            txtMarque = new TextBox { Location = new Point(435, 27), Width = 195 };

            Label lblModele = new Label { Text = "Modèle :", Location = new Point(655, 30), AutoSize = true };
            txtModele = new TextBox { Location = new Point(740, 27), Width = 300 };

            Label lblKm = new Label { Text = "Kilométrage :", Location = new Point(15, 68), AutoSize = true };
            numKm = new NumericUpDown { Location = new Point(115, 65), Width = 195, Maximum = 1000000, Increment = 500 };

            Label lblPrix = new Label { Text = "Prix / Jour :", Location = new Point(335, 68), AutoSize = true };
            numPrixJour = new NumericUpDown { Location = new Point(435, 65), Width = 195, Maximum = 5000, DecimalPlaces = 2, Value = 100 };

            Label lblStatut = new Label { Text = "Statut :", Location = new Point(655, 68), AutoSize = true };
            cmbStatut = new ComboBox
            {
                Location = new Point(740, 65),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DataSource = Enum.GetValues(typeof(StatutVehicule))
            };

            btnAjouterVehicule = new Button { Text = "Ajouter", Location = new Point(15, 108), Width = 110, Height = 32 };
            btnModifierVehicule = new Button { Text = "Modifier", Location = new Point(135, 108), Width = 110, Height = 32 };
            btnSupprimerVehicule = new Button { Text = "Supprimer", Location = new Point(255, 108), Width = 110, Height = 32 };
            Button btnClearV = new Button { Text = "Vider / Nouveau", Location = new Point(375, 108), Width = 145, Height = 32 };

            Label lblSearchV = new Label { Text = "Rechercher :", Location = new Point(550, 114), AutoSize = true };
            txtRechercheVehicule = new TextBox
            {
                Location = new Point(660, 111),
                Width = 380,
                PlaceholderText = "Filtrer par matricule, marque ou modèle..."
            };

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
                Location = new Point(785, 153),
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
                    ChargerCombosReservation();
                    ActualiserKpis();
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
            ChargerCombosReservation();
            ActualiserKpis();
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

            string immatNorm = txtImmat.Text.Trim().ToLower();
            using var db = new AppDbContext();

            bool conflitImmat = db.Vehicules.AsEnumerable()
                .Any(v => v.Id != selectedVehiculeId.Value && v.Immatriculation.Trim().ToLower() == immatNorm);

            if (conflitImmat)
            {
                MessageBox.Show("Un autre véhicule possède déjà cette immatriculation.", "Doublon détecté", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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
            ChargerCombosReservation();
            ChargerReservations(txtRechercheRes.Text.Trim());
            ActualiserKpis();
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

            using var db = new AppDbContext();
            if (db.Reservations.Any(r => r.Vehicule.Id == selectedVehiculeId.Value))
            {
                MessageBox.Show("Impossible de supprimer ce véhicule car il est lié à une ou plusieurs réservations.", "Suppression bloquée", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Confirmer la suppression de ce véhicule ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            var v = db.Vehicules.Find(selectedVehiculeId.Value);
            if (v == null) return;

            db.Vehicules.Remove(v);
            db.SaveChanges();

            ChargerVehicules(txtRechercheVehicule.Text.Trim());
            ChargerCombosReservation();
            ActualiserKpis();
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

        // =========================================================
        // ONGLET 3 : GESTION DES RÉSERVATIONS
        // =========================================================
        private void ConstruireOngletReservations(TabPage page)
        {
            page.Padding = new Padding(12);

            GroupBox grpRes = new GroupBox
            {
                Text = "Nouvelle Réservation && Suivi",
                Dock = DockStyle.Top,
                Height = 195,
                Padding = new Padding(10)
            };

            // Ligne 1 : Client et Véhicule
            Label lblClient = new Label { Text = "Client :", Location = new Point(15, 30), AutoSize = true };
            cmbResClient = new ComboBox
            {
                Location = new Point(100, 27),
                Width = 340,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            Label lblVeh = new Label { Text = "Véhicule :", Location = new Point(465, 30), AutoSize = true };
            cmbResVehicule = new ComboBox
            {
                Location = new Point(550, 27),
                Width = 490,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Ligne 2 : Dates, Statut initial et Coût estimé
            Label lblDebut = new Label { Text = "Date Début :", Location = new Point(15, 68), AutoSize = true };
            dtpDebut = new DateTimePicker
            {
                Location = new Point(105, 65),
                Width = 175,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today
            };

            Label lblFin = new Label { Text = "Date Fin :", Location = new Point(300, 68), AutoSize = true };
            dtpFin = new DateTimePicker
            {
                Location = new Point(375, 65),
                Width = 175,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddDays(3)
            };

            Label lblStatutRes = new Label { Text = "Statut :", Location = new Point(570, 68), AutoSize = true };
            cmbResStatut = new ComboBox
            {
                Location = new Point(635, 65),
                Width = 145,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DataSource = new[] { StatutReservation.Confirmee, StatutReservation.EnAttente }
            };

            lblEstimation = new Label
            {
                Location = new Point(800, 68),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(21, 128, 61)
            };

            // Ligne 3 : Boutons d'action + Recherche
            btnAjouterRes = new Button { Text = "Créer Réservation", Location = new Point(15, 108), Width = 145, Height = 32 };
            btnConfirmerRes = new Button { Text = "Confirmer", Location = new Point(170, 108), Width = 100, Height = 32 };
            btnAnnulerRes = new Button { Text = "Annuler", Location = new Point(280, 108), Width = 95, Height = 32 };
            btnSupprimerRes = new Button { Text = "Supprimer", Location = new Point(385, 108), Width = 95, Height = 32 };
            Button btnClearRes = new Button { Text = "Vider / Nouveau", Location = new Point(490, 108), Width = 135, Height = 32 };

            Label lblSearchRes = new Label { Text = "Rechercher :", Location = new Point(645, 114), AutoSize = true };
            txtRechercheRes = new TextBox
            {
                Location = new Point(745, 111),
                Width = 295,
                PlaceholderText = "Client, matricule ou statut..."
            };

            // Ligne 4 : Guide & Mode
            Label lblGuideRes = new Label
            {
                Text = "Guide : Choisissez un client, un véhicule et une période libre • Sélectionnez une ligne pour Confirmer ou Annuler.",
                Location = new Point(15, 156),
                AutoSize = true,
                ForeColor = Color.FromArgb(75, 85, 99),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Italic)
            };

            lblModeRes = new Label
            {
                Location = new Point(820, 155),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            cmbResVehicule.SelectedIndexChanged += (s, e) => MettreAJourEstimationReservation();
            dtpDebut.ValueChanged += (s, e) => MettreAJourEstimationReservation();
            dtpFin.ValueChanged += (s, e) => MettreAJourEstimationReservation();

            btnAjouterRes.Click += BtnAjouterReservation_Click;
            btnConfirmerRes.Click += BtnConfirmerReservation_Click;
            btnAnnulerRes.Click += BtnAnnulerReservation_Click;
            btnSupprimerRes.Click += BtnSupprimerReservation_Click;
            btnClearRes.Click += (s, e) => ViderFormulaireReservation();
            txtRechercheRes.TextChanged += (s, e) => ChargerReservations(txtRechercheRes.Text.Trim());

            grpRes.Controls.AddRange(new Control[]
            {
                lblClient, cmbResClient, lblVeh, cmbResVehicule,
                lblDebut, dtpDebut, lblFin, dtpFin, lblStatutRes, cmbResStatut, lblEstimation,
                btnAjouterRes, btnConfirmerRes, btnAnnulerRes, btnSupprimerRes, btnClearRes,
                lblSearchRes, txtRechercheRes, lblGuideRes, lblModeRes
            });

            dgvReservations = new DataGridView();
            ConfigurerDataGridView(dgvReservations);
            dgvReservations.CellClick += DgvReservations_CellClick;
            dgvReservations.DataBindingComplete += (s, e) =>
            {
                if (selectedReservationId == null) dgvReservations.ClearSelection();
            };

            Panel pnlGridR = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 10, 0, 0) };
            pnlGridR.Controls.Add(dgvReservations);

            page.Controls.Add(pnlGridR);
            page.Controls.Add(grpRes);
        }

        private void ChargerCombosReservation()
        {
            using var db = new AppDbContext();

            var clients = db.Clients
                .OrderBy(c => c.Nom)
                .Select(c => new ComboItem
                {
                    Id = c.Id,
                    Libelle = $"{c.Nom} {c.Prenom} ({c.NumeroPermis})"
                })
                .ToList();

            var vehicules = db.Vehicules
                .OrderBy(v => v.Statut == StatutVehicule.EnMaintenance)
                .ThenBy(v => v.Marque)
                .Select(v => new ComboItem
                {
                    Id = v.Id,
                    Libelle = $"{v.Immatriculation} — {v.Marque} {v.Modele} ({v.PrixParJour:0.00} DT/j) [{v.Statut}]",
                    PrixParJour = v.PrixParJour
                })
                .ToList();

            cmbResClient.DataSource = clients;
            cmbResVehicule.DataSource = vehicules;
            MettreAJourEstimationReservation();
        }

        private void MettreAJourEstimationReservation()
        {
            if (cmbResVehicule.SelectedItem is not ComboItem veh)
            {
                lblEstimation.Text = "Durée : - | Total : 0,00 DT";
                return;
            }

            int jours = (dtpFin.Value.Date - dtpDebut.Value.Date).Days;
            if (jours <= 0) jours = 1;
            decimal total = jours * veh.PrixParJour;
            lblEstimation.Text = $"Durée : {jours} j | Total : {total:0.00} DT";
        }

        private void SetFormulaireReservationEnabled(bool enabled)
        {
            cmbResClient.Enabled = enabled;
            cmbResVehicule.Enabled = enabled;
            dtpDebut.Enabled = enabled;
            dtpFin.Enabled = enabled;
            cmbResStatut.Enabled = enabled;
        }

        private void MettreAJourEtatBoutonsReservation(StatutReservation? statutSelectionne = null)
        {
            bool selectionne = selectedReservationId != null;
            SetFormulaireReservationEnabled(!selectionne);

            btnAjouterRes.Enabled = !selectionne;
            btnConfirmerRes.Enabled = selectionne && statutSelectionne == StatutReservation.EnAttente;
            btnAnnulerRes.Enabled = selectionne && statutSelectionne != StatutReservation.Annulee;
            btnSupprimerRes.Enabled = selectionne;

            if (selectionne)
            {
                lblModeRes.Text = $"Mode : Réservation #{selectedReservationId}";
                lblModeRes.ForeColor = Color.FromArgb(29, 78, 216);
            }
            else
            {
                lblModeRes.Text = "Mode : Nouvelle réservation";
                lblModeRes.ForeColor = Color.FromArgb(21, 128, 61);
            }
        }

        private static Reservation? TrouverConflitReservation(AppDbContext db, int vehiculeId, DateTime debut, DateTime fin, int? excludeReservationId = null)
        {
            return db.Reservations
                .Include(r => r.Client)
                .Where(r => r.Vehicule.Id == vehiculeId
                         && r.Id != (excludeReservationId ?? -1)
                         && r.Statut == StatutReservation.Confirmee)
                .AsEnumerable()
                .FirstOrDefault(r => debut.Date <= r.DateFin.Date && fin.Date >= r.DateDebut.Date);
        }

        private static void SynchroniserStatutVehicule(AppDbContext db, Vehicule vehicule, int? excludeReservationId = null)
        {
            if (vehicule.Statut == StatutVehicule.EnMaintenance) return;

            bool aAutreReservationActive = db.Reservations.Any(r =>
                r.Vehicule.Id == vehicule.Id &&
                r.Id != (excludeReservationId ?? -1) &&
                r.Statut == StatutReservation.Confirmee);

            vehicule.Statut = aAutreReservationActive ? StatutVehicule.Reserve : StatutVehicule.Disponible;
        }

        private void ChargerReservations(string filtre = "")
        {
            using var db = new AppDbContext();
            var list = db.Reservations
                .Include(r => r.Client)
                .Include(r => r.Vehicule)
                .OrderByDescending(r => r.Id)
                .ToList();

            if (!string.IsNullOrWhiteSpace(filtre))
            {
                string f = filtre.ToLower();
                list = list.Where(r =>
                    r.Client.Nom.ToLower().Contains(f) ||
                    r.Client.Prenom.ToLower().Contains(f) ||
                    r.Vehicule.Immatriculation.ToLower().Contains(f) ||
                    r.Vehicule.Marque.ToLower().Contains(f) ||
                    r.Statut.ToString().ToLower().Contains(f)).ToList();
            }

            dgvReservations.DataSource = list
                .Select(r =>
                {
                    int duree = r.CalculerDureeJours();
                    return new
                    {
                        r.Id,
                        Client = $"{r.Client.Prenom} {r.Client.Nom}",
                        Véhicule = $"{r.Vehicule.Immatriculation} ({r.Vehicule.Marque} {r.Vehicule.Modele})",
                        Début = r.DateDebut.ToString("dd/MM/yyyy"),
                        Fin = r.DateFin.ToString("dd/MM/yyyy"),
                        Durée = $"{duree} j",
                        MontantTotal = $"{duree * r.Vehicule.PrixParJour:0.00} DT",
                        r.Statut
                    };
                })
                .ToList();

            if (selectedReservationId == null)
                dgvReservations.ClearSelection();
        }

        private void DgvReservations_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            int id = Convert.ToInt32(dgvReservations.Rows[e.RowIndex].Cells["Id"].Value);

            using var db = new AppDbContext();
            var res = db.Reservations
                .Include(r => r.Client)
                .Include(r => r.Vehicule)
                .FirstOrDefault(r => r.Id == id);

            if (res == null) return;

            selectedReservationId = res.Id;
            dtpDebut.Value = res.DateDebut;
            dtpFin.Value = res.DateFin;

            foreach (ComboItem item in cmbResClient.Items)
            {
                if (item.Id == res.Client.Id) { cmbResClient.SelectedItem = item; break; }
            }
            foreach (ComboItem item in cmbResVehicule.Items)
            {
                if (item.Id == res.Vehicule.Id) { cmbResVehicule.SelectedItem = item; break; }
            }
            if (res.Statut == StatutReservation.Confirmee || res.Statut == StatutReservation.EnAttente)
            {
                cmbResStatut.SelectedItem = res.Statut;
            }

            MettreAJourEstimationReservation();
            MettreAJourEtatBoutonsReservation(res.Statut);
        }

        private void BtnAjouterReservation_Click(object? sender, EventArgs e)
        {
            if (cmbResClient.SelectedItem is not ComboItem clientItem || cmbResVehicule.SelectedItem is not ComboItem vehItem)
            {
                MessageBox.Show("Veuillez sélectionner un client et un véhicule.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dtpFin.Value.Date < dtpDebut.Value.Date)
            {
                MessageBox.Show("La date de fin doit être supérieure ou égale à la date de début.", "Dates invalides", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var db = new AppDbContext();
            var client = db.Clients.Find(clientItem.Id);
            var vehicule = db.Vehicules.Find(vehItem.Id);
            if (client == null || vehicule == null) return;

            if (vehicule.Statut == StatutVehicule.EnMaintenance)
            {
                MessageBox.Show("Ce véhicule est actuellement en maintenance.", "Véhicule indisponible", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var conflit = TrouverConflitReservation(db, vehicule.Id, dtpDebut.Value, dtpFin.Value);
            if (conflit != null)
            {
                MessageBox.Show(
                    $"Ce véhicule est déjà réservé par {conflit.Client.Prenom} {conflit.Client.Nom} du {conflit.DateDebut:dd/MM/yyyy} au {conflit.DateFin:dd/MM/yyyy}.",
                    "Créneau indisponible",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var statutSouhaite = (StatutReservation)(cmbResStatut.SelectedItem ?? StatutReservation.Confirmee);
            var reservation = new Reservation(0, client, vehicule, dtpDebut.Value.Date, dtpFin.Value.Date);

            if (statutSouhaite == StatutReservation.Confirmee)
            {
                if (vehicule.Statut != StatutVehicule.Disponible)
                    vehicule.Statut = StatutVehicule.Disponible;

                reservation.Confirmer();
            }

            db.Reservations.Add(reservation);
            db.SaveChanges();

            ChargerVehicules(txtRechercheVehicule.Text.Trim());
            ChargerCombosReservation();
            ChargerReservations(txtRechercheRes.Text.Trim());
            ActualiserKpis();
            ViderFormulaireReservation();
            MessageBox.Show("Réservation enregistrée avec succès.", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnConfirmerReservation_Click(object? sender, EventArgs e)
        {
            if (selectedReservationId == null) return;

            using var db = new AppDbContext();
            var res = db.Reservations.Include(r => r.Vehicule).FirstOrDefault(r => r.Id == selectedReservationId.Value);
            if (res == null) return;

            if (res.Statut == StatutReservation.Confirmee)
            {
                MessageBox.Show("Cette réservation est déjà confirmée.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (res.Vehicule.Statut == StatutVehicule.EnMaintenance)
            {
                MessageBox.Show("Ce véhicule est actuellement en maintenance.", "Confirmation impossible", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var conflit = TrouverConflitReservation(db, res.Vehicule.Id, res.DateDebut, res.DateFin, res.Id);
            if (conflit != null)
            {
                MessageBox.Show(
                    $"Impossible de confirmer : ce véhicule est déjà réservé par {conflit.Client.Prenom} {conflit.Client.Nom} du {conflit.DateDebut:dd/MM/yyyy} au {conflit.DateFin:dd/MM/yyyy}.",
                    "Conflit de dates",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (res.Vehicule.Statut != StatutVehicule.Disponible)
                res.Vehicule.Statut = StatutVehicule.Disponible;

            res.Confirmer();
            db.SaveChanges();

            ChargerVehicules(txtRechercheVehicule.Text.Trim());
            ChargerCombosReservation();
            ChargerReservations(txtRechercheRes.Text.Trim());
            ActualiserKpis();
            ViderFormulaireReservation();
            MessageBox.Show("Réservation confirmée et véhicule marqué comme Réservé.", "Confirmée", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnAnnulerReservation_Click(object? sender, EventArgs e)
        {
            if (selectedReservationId == null) return;

            using var db = new AppDbContext();
            var res = db.Reservations.Include(r => r.Vehicule).FirstOrDefault(r => r.Id == selectedReservationId.Value);
            if (res == null) return;

            if (res.Statut == StatutReservation.Annulee)
            {
                MessageBox.Show("Cette réservation est déjà annulée.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            res.Annuler();
            SynchroniserStatutVehicule(db, res.Vehicule, res.Id);
            db.SaveChanges();

            ChargerVehicules(txtRechercheVehicule.Text.Trim());
            ChargerCombosReservation();
            ChargerReservations(txtRechercheRes.Text.Trim());
            ActualiserKpis();
            ViderFormulaireReservation();
            MessageBox.Show("Réservation annulée et statut du véhicule mis à jour.", "Annulée", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnSupprimerReservation_Click(object? sender, EventArgs e)
        {
            if (selectedReservationId == null) return;

            if (MessageBox.Show("Supprimer définitivement cette réservation ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            using var db = new AppDbContext();
            var res = db.Reservations.Include(r => r.Vehicule).FirstOrDefault(r => r.Id == selectedReservationId.Value);
            if (res == null) return;

            var vehicule = res.Vehicule;
            int deletedId = res.Id;

            db.Reservations.Remove(res);
            SynchroniserStatutVehicule(db, vehicule, deletedId);
            db.SaveChanges();

            ChargerVehicules(txtRechercheVehicule.Text.Trim());
            ChargerCombosReservation();
            ChargerReservations(txtRechercheRes.Text.Trim());
            ActualiserKpis();
            ViderFormulaireReservation();
        }

        private void ViderFormulaireReservation()
        {
            selectedReservationId = null;
            dtpDebut.Value = DateTime.Today;
            dtpFin.Value = DateTime.Today.AddDays(3);
            if (cmbResStatut.Items.Count > 0) cmbResStatut.SelectedIndex = 0;
            dgvReservations.ClearSelection();
            MettreAJourEtatBoutonsReservation();
            MettreAJourEstimationReservation();
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