using System;
using _420_14B_FX_A25_TP3.classes;
using _420_14B_FX_A25_TP3.enums;
using Xunit;

namespace _420_14B_FX_A25_TP3_Tests
{
    public class BilletTests
    {
        private Evenement CreerEvenement(uint id = 1)
        {
            return new Evenement(id, "Spectacle Test", TypeEvenement.Musique, DateTime.Now.AddDays(5), 50m, 100, "test.png");
        }

       
        [Fact]
        public void Constructeur_Complet_Devrait_Assigner_Valeurs_Correctes_Quand_Evenement_Valide()
        {
            // Arrange
            var evenement = CreerEvenement();

            // Act
            var billet = new Billet(10, evenement, 3);

            // Assert
            Assert.Equal(10u, billet.Id);
            Assert.Equal(evenement, billet.Evenement);
            Assert.Equal(3, billet.Quantite);
        }

        [Fact]
        public void Constructeur_Principal_Devrait_Assigner_QuantiteMin_Quand_AucuneQuantiteSpecifiee()
        {
            // Arrange
            var evenement = CreerEvenement();

            // Act
            var billet = new Billet(evenement);

            // Assert
            Assert.Equal(Billet.QUANTITE_MIN, billet.Quantite);
        }

        [Fact]
        public void Constructeur_Devrait_Lancer_ArgumentNullException_Quand_Evenement_Null()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new Billet(null));
        }



        [Theory]
        [InlineData(0)]
        [InlineData(11)]
        public void Quantite_Set_Devrait_Lancer_ArgumentOutOfRangeException_Quand_Valeur_Hors_Limite(int quantite)
        {
            // Arrange
            var billet = new Billet(CreerEvenement());

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => billet.Quantite = quantite);
        }

        [Fact]
        public void Quantite_Set_Devrait_Assigner_Valeur_Correcte_Quand_Valeur_Valide()
        {
            // Arrange
            var billet = new Billet(CreerEvenement());

            // Act
            billet.Quantite = 5;

            // Assert
            Assert.Equal(5, billet.Quantite);
        }



        [Fact]
        public void Equals_Devrait_Retourner_True_Quand_Meme_Evenement()
        {
            // Arrange
            var evenement = CreerEvenement();
            var billet1 = new Billet(evenement);
            var billet2 = new Billet(evenement);

            // Act
            bool resultat = billet1.Equals(billet2);

            // Assert
            Assert.True(resultat);
        }

        [Fact]
        public void Equals_Devrait_Retourner_False_Quand_Evenements_Differents()
        {
            // Arrange
            var billet1 = new Billet(CreerEvenement(1));
            var billet2 = new Billet(CreerEvenement(2));

            // Act
            bool resultat = billet1.Equals(billet2);

            // Assert
            Assert.False(resultat);
        }

        [Fact]
        public void Equals_Devrait_Retourner_False_Quand_Objet_NonBillet()
        {
            // Arrange
            var billet = new Billet(CreerEvenement());
            var autreObjet = new object();

            // Act
            bool resultat = billet.Equals(autreObjet);

            // Assert
            Assert.False(resultat);
        }


        [Fact]
        public void OperateurEgalite_Devrait_Retourner_True_Quand_Meme_Evenement()
        {
            // Arrange
            var evenement = CreerEvenement();
            var billet1 = new Billet(evenement);
            var billet2 = new Billet(evenement);

            // Act
            bool resultat = billet1 == billet2;

            // Assert
            Assert.True(resultat);
        }

        [Fact]
        public void OperateurEgalite_Devrait_Retourner_False_Quand_Evenements_Differents()
        {
            // Arrange
            var billet1 = new Billet(CreerEvenement(1));
            var billet2 = new Billet(CreerEvenement(2));

            // Act
            bool resultat = billet1 == billet2;

            // Assert
            Assert.False(resultat);
        }

        [Fact]
        public void OperateurEgalite_Devrait_Retourner_False_Quand_UnDesBillets_EstNull()
        {
            // Arrange
            var billet = new Billet(CreerEvenement());

            // Act & Assert
            Assert.False(billet == null);
            Assert.False(null == billet);
        }

        [Fact]
        public void OperateurEgalite_Devrait_Retourner_True_Quand_Reference_Identique()
        {
            // Arrange
            var billet = new Billet(CreerEvenement());

            // Act
            bool resultat = billet == billet;

            // Assert
            Assert.True(resultat);
        }


        [Fact]
        public void OperateurInegalite_Devrait_Retourner_True_Quand_Evenements_Differents()
        {
            // Arrange
            var billet1 = new Billet(CreerEvenement(1));
            var billet2 = new Billet(CreerEvenement(2));

            // Act
            bool resultat = billet1 != billet2;

            // Assert
            Assert.True(resultat);
        }

        [Fact]
        public void OperateurInegalite_Devrait_Retourner_False_Quand_Meme_Evenement()
        {
            // Arrange
            var evenement = CreerEvenement();
            var billet1 = new Billet(evenement);
            var billet2 = new Billet(evenement);

            // Act
            bool resultat = billet1 != billet2;

            // Assert
            Assert.False(resultat);
        }
    }
}
