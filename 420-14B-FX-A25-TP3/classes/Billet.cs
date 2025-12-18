using System;

namespace _420_14B_FX_A25_TP3.classes
{
    /// <summary>
    /// Représente un billet associé à un événement.
    /// Chaque billet correspond à un événement acheté et à une quantité.
    /// </summary>
    public class Billet
    {

        public const int QUANTITE_MIN = 1;
        public const int QUANTITE_MAX = 10;

        private uint _id;
        private Evenement _evenement;
        private int _quantite;

        /// <summary>
        /// Identifiant unique du billet.
        /// </summary>
        public uint Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Événement associé à ce billet.
        /// </summary>
        public Evenement Evenement
        {
            get { return _evenement; }
            set
            {
                if (value is null)
                    throw new ArgumentNullException(nameof(value), "L'événement ne peut pas être null.");
                _evenement = value;
            }
        }

        /// <summary>
        /// Nombre de billets achetés pour cet événement.
        /// </summary>
        public int Quantite
        {
            get { return _quantite; }
            set
            {
                if (value < QUANTITE_MIN || value > QUANTITE_MAX)
                    throw new ArgumentOutOfRangeException(nameof(value), $"La quantité doit être entre {QUANTITE_MIN} et {QUANTITE_MAX}.");
                _quantite = value;
            }
        }

        /// <summary>
        /// Constructeur complet avec identifiant.
        /// </summary>
        public Billet(uint id, Evenement evenement, int quantite = QUANTITE_MIN)
        {
            Id = id;
            Evenement = evenement;
            Quantite = quantite;
        }

        /// <summary>
        /// Constructeur pour un nouveau billet pour cet événement avec une quantité minimum
        /// </summary>
        public Billet(Evenement evenement)
        {
            Id = 0;                    
            Evenement = evenement;     
            Quantite = QUANTITE_MIN;
        }

        /// <summary>
        /// Deux billets sont égaux s’ils concernent le même événement.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is Billet other)
                return Evenement.Id == other.Evenement.Id;
            return false;
        }

        public static bool operator ==(Billet gauche, Billet droite)
        {
            if (gauche is null) 
                return droite is null;
            return gauche.Equals(droite);
        }

        public static bool operator !=(Billet gauche, Billet droite)
        {
            return !(gauche == droite);
        }
    }
}
