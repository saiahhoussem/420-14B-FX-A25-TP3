using System;

namespace _420_14B_FX_A25_TP3.classes
{
    /// <summary>
    /// Représente un billet associé à un événement.
    /// Chaque billet correspond à un événement acheté et à une quantité.
    /// </summary>
    public class Billet
    {
        

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
            set { _evenement = value; }
        }

        /// <summary>
        /// Nombre de billets achetés pour cet événement.
        /// </summary>
        public int Quantite
        {
            get { return _quantite; }
            set { _quantite = value; }
        }

        /// <summary>
        /// Constructeur complet avec identifiant.
        /// </summary>
        public Billet(uint id, Evenement evenement, int quantite = QUANTITE_MIN)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Constructeur pour un nouveau billet pour cet événement avec une quantité minimum
        /// </summary>
        public Billet(Evenement evenement)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deux billets sont égaux s’ils concernent le même événement.
        /// </summary>
        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(Billet gauche, Billet droite)
        {
            throw new NotImplementedException();
        }

        public static bool operator !=(Billet gauche, Billet droite)
        {
            throw new NotImplementedException();
        }
    }
}
