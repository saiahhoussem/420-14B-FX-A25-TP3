using _420_14B_FX_A25_TP3.enums;
using System;

namespace _420_14B_FX_A25_TP3.classes
{
    /// <summary>
    /// Représente un événement offert dans la billetterie.
    /// </summary>
    public class Evenement
    {

        public const int NB_PLACES_MIN = 1;
        public const int NB_PLACES_MAX = 500;

        private uint _id;
        private string _nom;
        private TypeEvenement _type;
        private DateTime _dateHeure;
        private decimal _prix;
        private int _nbPlaces;
        private string _imagePath;

        /// <summary>
        /// Identifiant unique de l'événement.
        /// </summary>
        public uint Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Nom de l'événement.
        /// </summary>
        /// <exception cref="ArgumentNullException">Lancé si le nom est nul.</exception>
        /// <exception cref="ArgumentException">Lancé si le nom est vide ou ne contient que des espaces.</exception>
        public string Nom
        {
            get { return _nom; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value), "Le nom de l'événement ne peut pas être nul.");

                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Le nom de l'événement ne peut pas être vide ou ne contenir que des espaces.", nameof(value));

                _nom = value.Trim();
            }
        }

        /// <summary>
        /// Type de l'événement.
        /// </summary>
        /// <exception cref="ArgumentException">Lancé si le type est invalide.</exception>
        public TypeEvenement Type
        {
            get { return _type; }
            set
            {
                if (value < TypeEvenement.Musique || value > TypeEvenement.Autre)
                    throw new ArgumentException("Le type d'événement spécifié est invalide.", nameof(value));

                _type = value;
            }
        }

        /// <summary>
        /// Date et heure de l'événement.
        /// </summary>
        public DateTime DateHeure
        {
            get { return _dateHeure; }
            set { _dateHeure = value; }
        }

        /// <summary>
        /// Prix du billet pour l'événement.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Lancé si le prix est négatif.</exception>
        public decimal Prix
        {
            get { return _prix; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Le prix doit être supérieur ou égal à zéro.");

                _prix = value;
            }
        }

        /// <summary>
        /// Nombre de places disponibles pour l'événement.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Lancé si la valeur n’est pas comprise entre <see cref="NB_PLACES_MIN"/> et <see cref="NB_PLACES_MAX"/>.</exception>
        public int NbPlaces
        {
            get { return _nbPlaces; }
            set
            {
                if (value < NB_PLACES_MIN || value > NB_PLACES_MAX)
                    throw new ArgumentOutOfRangeException(nameof(value),
                        $"Le nombre de places doit être entre {NB_PLACES_MIN} et {NB_PLACES_MAX}.");

                _nbPlaces = value;
            }
        }

        /// <summary>
        /// Chemin vers l'image associée à l'événement.
        /// </summary>
        /// <exception cref="ArgumentException">Lancé si le fichier image est invalide ou inexistant.</exception>
        public string ImagePath
        {
            get { return _imagePath; }
            set { _imagePath = value; }
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Evenement"/> avec un identifiant.
        /// </summary>
        public Evenement(uint id, string nom, TypeEvenement type, DateTime date, decimal prix, int nbPlaces, string imagePath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Evenement"/>.
        /// </summary>
        public Evenement(string nom, TypeEvenement type, DateTime date, decimal prix, int nbPlaces, string imagePath)
        {
            throw new NotImplementedException();
        }
    }
}
