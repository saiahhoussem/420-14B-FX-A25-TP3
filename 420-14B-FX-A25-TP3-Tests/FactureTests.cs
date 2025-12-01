using System;
using System.Collections.Generic;
using _420_14B_FX_A25_TP3.classes;
using _420_14B_FX_A25_TP3.enums;
using Xunit;

namespace _420_14B_FX_A25_TP3_Tests
{
    public class FactureTests
    {

        private Evenement CreerEvenementValide(uint id = 1, decimal prix = 50m)
        {
            return new Evenement(id, "Spectacle test", TypeEvenement.Musique, DateTime.Now.AddDays(5), prix, 100, "test.png");
        }

        private Billet CreerBilletValide(uint id = 1, int quantite = 1)
        {
            return new Billet(id, CreerEvenementValide(id), quantite);
        }

        private Facture CreerFactureValide()
        {
            return new Facture
            {
                Id = 1,
                Date = DateTime.Now,
            };
        }


        [Fact]
        public void Constructeur_SansParametre_Devrait_Creer_Facture_Avec_ListeVide()
        {
            // Act
            var facture = new Facture();

            // Assert
            Assert.NotNull(facture.Billets);
            Assert.Empty(facture.Billets);
        }

        [Fact]
        public void Constructeur_Complet_Devrait_Assigner_Valeurs_Quand_Parametres_Valides()
        {
            // Arrange
            var billets = new List<Billet> { CreerBilletValide() };
            var date = DateTime.Now;

            // Act
            var facture = new Facture(1, date, billets);

            // Assert
            Assert.Equal(1u, facture.Id);
            Assert.Equal(date, facture.Date);
            Assert.Single(facture.Billets);
        }


        [Fact]
        public void Date_Set_Devrait_Lancer_ArgumentOutOfRangeException_Quand_DateFuture()
        {
            // Arrange
            var facture = CreerFactureValide();

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => facture.Date = DateTime.Now.AddDays(10));
        }

        [Fact]
        public void Date_Set_Devrait_Assigner_Valeur_Quand_DateValide()
        {
            // Arrange
            var facture = CreerFactureValide();
            var date = DateTime.Now;

            // Act
            facture.Date = date;

            // Assert
            Assert.Equal(date, facture.Date);
        }


        [Fact]
        public void SousTotal_Devrait_Retourner_SommeDesBillets_Quand_BilletsValides()
        {
            // Arrange
            var facture = CreerFactureValide();
            facture.Billets.Add(new Billet(1,CreerEvenementValide(1, 20m), 2 )); 
            facture.Billets.Add(new Billet(2,CreerEvenementValide(2, 30m), 1 )); 

            // Act
            var sousTotal = facture.SousTotal;

            // Assert
            Assert.Equal(70m, sousTotal);
        }

        [Fact]
        public void TPS_Devrait_Retourner_CalculArrondi_Quand_SousTotalValide()
        {
            // Arrange
            var facture = CreerFactureValide();
            facture.Billets.Add(new Billet(CreerEvenementValide(1, 100m))); 

            // Act
            var tps = facture.TPS;

            // Assert
            Assert.Equal(5.00m, tps);
        }

        [Fact]
        public void TVQ_Devrait_Retourner_CalculArrondi_Quand_SousTotalValide()
        {
            // Arrange
            var facture = CreerFactureValide();
            facture.Billets.Add(new Billet(CreerEvenementValide(1, 100m))); // SousTotal = 100

            // Act
            var tvq = facture.TVQ;

            // Assert
            Assert.Equal(9.98m, tvq); // arrondi
        }

        [Fact]
        public void Total_Devrait_Retourner_SommeTotale_Quand_SousTotalEtTaxesValides()
        {
            // Arrange
            var facture = CreerFactureValide();
            facture.Billets.Add(new Billet(CreerEvenementValide(1, 100m)));

            // Act
            var total = facture.Total;

            // Assert
            Assert.Equal(114.98m, total);
        }


        [Fact]
        public void AjouterBillet_Devrait_Ajouter_NouveauBillet_Quand_BilletInexistant()
        {
            // Arrange
            var facture = CreerFactureValide();
            var billet = CreerBilletValide();

            // Act
            facture.AjouterBillet(billet);

            // Assert
            Assert.Single(facture.Billets);
        }

        [Fact]
        public void AjouterBillet_Devrait_Incremente_Quantite_Quand_BilletExistant()
        {
            // Arrange
            var facture = CreerFactureValide();
            var billet = CreerBilletValide();
            facture.AjouterBillet(billet);

            // Act
            facture.AjouterBillet(billet);

            // Assert
            Assert.Equal(2, facture.Billets[0].Quantite);
        }

        [Fact]
        public void AjouterBillet_Devrait_Lancer_ArgumentNullException_Quand_BilletNull()
        {
            // Arrange
            var facture = CreerFactureValide();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => facture.AjouterBillet(null));
        }

        [Fact]
        public void AjouterBillet_Devrait_Lancer_InvalidOperationException_Quand_QuantiteMax_Atteinte()
        {
            // Arrange
            var facture = CreerFactureValide();
            var billet = CreerBilletValide(1, Billet.QUANTITE_MAX);
            facture.AjouterBillet(billet);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => facture.AjouterBillet(billet));
        }


        [Fact]
        public void SupprimerBillet_Devrait_Decrementer_Quantite_Quand_BilletExistant_QuantiteSuperieureMin()
        {
            // Arrange
            var facture = CreerFactureValide();
            var billet = CreerBilletValide();
            billet.Quantite = 3;
            facture.AjouterBillet(billet);

            // Act
            facture.SupprimerBillet(billet);

            // Assert
            Assert.Equal(2, facture.Billets[0].Quantite);
        }

        [Fact]
        public void SupprimerBillet_Devrait_Retirer_Billet_Quand_QuantiteEgaleMin()
        {
            // Arrange
            var facture = CreerFactureValide();
            var billet = CreerBilletValide();
            facture.AjouterBillet(billet);

            // Act
            facture.SupprimerBillet(billet);

            // Assert
            Assert.Empty(facture.Billets);
        }

        [Fact]
        public void SupprimerBillet_Devrait_Lancer_ArgumentNullException_Quand_BilletNull()
        {
            // Arrange
            var facture = CreerFactureValide();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => facture.SupprimerBillet(null!));
        }

        [Fact]
        public void SupprimerBillet_Devrait_Lancer_InvalidOperationException_Quand_BilletInexistant()
        {
            // Arrange
            var facture = CreerFactureValide();
            var billet = CreerBilletValide();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => facture.SupprimerBillet(billet));
        }
    }
}
