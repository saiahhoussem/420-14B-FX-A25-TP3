using System;
using _420_14B_FX_A25_TP3.classes;
using _420_14B_FX_A25_TP3.enums;
using Xunit;

namespace _420_14B_FX_A25_TP3_Tests
{
    public class EvenementTests
    {
        /// <summary>
        /// Crée un événement valide par défaut pour réutilisation dans les tests.
        /// </summary>
        private Evenement CreerEvenementValide(
            uint id = 1,
            string nom = "Spectacle Test",
            TypeEvenement type = TypeEvenement.Musique,
            decimal prix = 50m,
            int nbPlaces = 100,
            string imagePath = "test.png")
        {
            return new Evenement(id, nom, type, DateTime.Now.AddDays(5), prix, nbPlaces, imagePath);
        }


        [Fact]
        public void Constructeur_Complet_Devrait_Assigner_Valeurs_Correctes_Quand_Parametres_Valides()
        {
            // Arrange
            DateTime date = DateTime.Now.AddDays(10);

            // Act
            var evt = new Evenement(1, "Gala d'ouverture", TypeEvenement.Musique, date, 75m, 250, "concert.png");

            // Assert
            Assert.Equal(1u, evt.Id);
            Assert.Equal("Gala d'ouverture", evt.Nom);
            Assert.Equal(TypeEvenement.Musique, evt.Type);
            Assert.Equal(date, evt.DateHeure);
            Assert.Equal(75m, evt.Prix);
            Assert.Equal(250, evt.NbPlaces);
            Assert.Equal("concert.png", evt.ImagePath);
        }

       

        [Fact]
        public void Nom_Set_Devrait_Lancer_ArgumentNullException_Quand_Nom_Null()
        {
            //Arange
            Evenement evenement = CreerEvenementValide();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => evenement.Nom = null);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Nom_Set_Devrait_Lancer_ArgumentException_Quand_Nom_Vide_Ou_Espaces(string nom)
        {
            //Arange
            Evenement evenement = CreerEvenementValide();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => evenement.Nom = nom);
        }

        [Fact]
        public void Nom_Set_Devrait_Assigner_Valeur_Trimmee_Quand_Nom_Valide()
        {
            //Arange
            Evenement evenement = CreerEvenementValide();

            // Act
            evenement.Nom = "  Conférence Tech  ";

            // Assert
            Assert.Equal("Conférence Tech", evenement.Nom);
        }



        [Fact]
        public void Type_Set_Devrait_Assigner_Valeur_Quand_Type_Valide()
        {

            // Arrange & Act
            var evt = CreerEvenementValide();

            // Assert
            Assert.Equal(TypeEvenement.Musique, evt.Type);
        }

        [Fact]
        public void Type_Set_Devrait_Lancer_ArgumentException_Quand_Type_Invalide()
        {
            // Arrange
            Evenement evt = CreerEvenementValide();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => evt.Type = (TypeEvenement)999);
        }


        [Theory]
        [InlineData(-1)]
        [InlineData(-0.01)]
        public void Prix_Set_Devrait_Lancer_ArgumentOutOfRangeException_Quand_Valeur_Negative(decimal prix)
        {
            // Arrange
            Evenement evt = CreerEvenementValide();

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => evt.Prix=  prix);
        }

        [Fact]
        public void Prix_Set_Devrait_Assigner_Valeur_Quand_Valeur_Valide()
        {
            // Act
            var evt = CreerEvenementValide();

            // Assert
            Assert.Equal(50m, evt.Prix);
        }

       
        [Theory]
        [InlineData(Evenement.NB_PLACES_MIN-1)]
        [InlineData(Evenement.NB_PLACES_MAX + 1)]
        public void NbPlaces_Set_Devrait_Lancer_ArgumentOutOfRangeException_Quand_Hors_Limites(int nbPlaces)
        {
            // Arrange
            Evenement evt = CreerEvenementValide();

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => evt.NbPlaces = nbPlaces);
        }

        [Fact]
        public void NbPlaces_Set_Devrait_Assigner_Valeur_Quand_Valeur_Valide()
        {
            // Act
            var evt = CreerEvenementValide();

            // Assert
            Assert.Equal(100, evt.NbPlaces);
        }

   
        [Theory]
        [InlineData("")]
        [InlineData("document.pdf")]
        [InlineData("photo.gif")]
        public void ImagePath_Set_Devrait_Lancer_ArgumentException_Quand_Fichier_Invalide(string chemin)
        {
            // Arrange
            Evenement evt = CreerEvenementValide();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => evt.ImagePath = chemin);
        }

        [Fact]
        public void ImagePath_Set_Devrait_Assigner_Valeur_Quand_Fichier_Valide()
        {
            // Act
            var evt = CreerEvenementValide();

            // Assert
            Assert.Equal("test.png", evt.ImagePath);
        }
    }
}
