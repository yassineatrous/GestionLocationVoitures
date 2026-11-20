using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionLocationVoitures.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Agents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Matricule = table.Column<string>(type: "TEXT", nullable: false),
                    Poste = table.Column<string>(type: "TEXT", nullable: false),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    Prenom = table.Column<string>(type: "TEXT", nullable: false),
                    Telephone = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumeroPermis = table.Column<string>(type: "TEXT", nullable: false),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    Prenom = table.Column<string>(type: "TEXT", nullable: false),
                    Telephone = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vehicules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Immatriculation = table.Column<string>(type: "TEXT", nullable: false),
                    Marque = table.Column<string>(type: "TEXT", nullable: false),
                    Modele = table.Column<string>(type: "TEXT", nullable: false),
                    Kilometrage = table.Column<int>(type: "INTEGER", nullable: false),
                    PrixParJour = table.Column<decimal>(type: "TEXT", nullable: false),
                    Statut = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Entretiens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateEntretien = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TypeEntretien = table.Column<string>(type: "TEXT", nullable: false),
                    Cout = table.Column<decimal>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: false),
                    VehiculeId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entretiens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Entretiens_Vehicules_VehiculeId",
                        column: x => x.VehiculeId,
                        principalTable: "Vehicules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<int>(type: "INTEGER", nullable: false),
                    VehiculeId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateDebut = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateFin = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Statut = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Vehicules_VehiculeId",
                        column: x => x.VehiculeId,
                        principalTable: "Vehicules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContratsLocation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReservationId = table.Column<int>(type: "INTEGER", nullable: false),
                    AgentId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateSignature = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateRetourEffective = table.Column<DateTime>(type: "TEXT", nullable: true),
                    KilometrageDepart = table.Column<int>(type: "INTEGER", nullable: false),
                    KilometrageRetour = table.Column<int>(type: "INTEGER", nullable: true),
                    Caution = table.Column<decimal>(type: "TEXT", nullable: false),
                    MontantTotal = table.Column<decimal>(type: "TEXT", nullable: false),
                    EstCloture = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContratsLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContratsLocation_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContratsLocation_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContratsLocation_AgentId",
                table: "ContratsLocation",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_ContratsLocation_ReservationId",
                table: "ContratsLocation",
                column: "ReservationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Entretiens_VehiculeId",
                table: "Entretiens",
                column: "VehiculeId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ClientId",
                table: "Reservations",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_VehiculeId",
                table: "Reservations",
                column: "VehiculeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContratsLocation");

            migrationBuilder.DropTable(
                name: "Entretiens");

            migrationBuilder.DropTable(
                name: "Agents");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Vehicules");
        }
    }
}
