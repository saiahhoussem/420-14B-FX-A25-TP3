using System;
using System.Collections.Generic;

namespace _420_14B_FX_A25_TP3.classes
{
    /// <summary>
    /// Représente une facture contenant les billets achetés pour un ou plusieurs événements.
    /// </summary>
    public class Facture
    {

        public const decimal TAUX_TPS = 0.05m;
        public const decimal TAUX_TVQ = 0.09975m;

        private uint _id;
        private DateTime? _date;
        private List<Billet> _billets;

        

        /// <summary>
        /// Identifiant unique de la facture.
        /// </summary>
        public uint Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Date de création de la facture.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Lancée si la date de création est dans le futur.
        /// </exception>
        public DateTime? Date
        {
            get { return _date; }
            set
            {
                if (value.HasValue && value.Value > DateTime.Now)
                    throw new ArgumentOutOfRangeException(nameof(value), "La date ne peut pas être dans le futur.");
                _date = value;
            }
        }

        /// <summary>
        /// Liste des billets associés à la facture.
        /// </summary>
        public List<Billet> Billets
        {
            get { return _billets; }
            //private set { _billets = value ?? new List<Billet>(); }
        }

        /// <summary>
        /// Montant total avant taxes.
        /// </summary>
        public decimal SousTotal
        {
            get
            {
                decimal total = 0m;
                foreach (Billet billet in _billets)
                {
                    total += billet.Evenement.Prix * billet.Quantite;
                }
                return total;
            }
        }

        /// <summary>
        /// Montant de la TPS calculé sur le sous-total.
        /// </summary>
        public decimal TPS
        {
            get
            {
                return Math.Round(SousTotal * TAUX_TPS, 2);
            }
        }

        /// <summary>
        /// Montant de la TVQ calculé sur le sous-total.
        /// </summary>
        public decimal TVQ
        {
            get
            {
                return Math.Round(SousTotal * TAUX_TVQ, 2);
            }
        }

        /// <summary>
        /// Montant total (taxes incluses).
        /// </summary>
        public decimal Total
        {
            get
            {
                return SousTotal + TPS + TVQ;
            }
        }

        /// <summary>
        /// Initialise une facture vide.
        /// </summary>
        public Facture()
        {
            Id = 0;
            Date = DateTime.Now;
            _billets = new List<Billet>();
        }

        /// <summary>
        /// Initialise une facture complète avec ses données.
        /// </summary>
        public Facture(uint id, DateTime date, List<Billet> billets)
        {
            Id = id;
            Date = date;
            _billets = billets ?? new List<Billet>();
        }

        /// <summary>
        /// Ajoute un billet à la facture.
        /// Si un billet du même événement existe déjà, sa quantité est augmentée.
        /// Sinon, le billet est ajouté à la liste.
        /// </summary>
        /// <param name="billet">Billet à ajouter.</param>
        /// <exception cref="ArgumentNullException">
        /// Lancée si le billet est nul.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Lancée si la quantité maximale pour un événement est atteinte.
        /// </exception>
        public void AjouterBillet(Billet billet)
        {
            if (billet is null)
                throw new ArgumentNullException(nameof(billet));

            Billet billetExistant = null;

            for (int i = 0; i < Billets.Count; i++)
            {
                if (Billets[i] == billet)  
                {
                    if (Billets[i].Quantite + billet.Quantite > Billet.QUANTITE_MAX)
                        throw new InvalidOperationException("La quantité maximale pour cet événement est atteinte.");

                    Billets[i].Quantite += billet.Quantite;
                    billetExistant = Billets[i];  
                }
            }

            if (billetExistant is null)  
            {
                if (billet.Quantite > Billet.QUANTITE_MAX)
                    throw new InvalidOperationException("La quantité maximale pour cet événement est atteinte.");

                Billets.Add(billet);
            }
        }

        /// <summary>
        /// Supprime un billet de la facture.
        /// Si plusieurs billets du même événement sont présents, la quantité est diminuée.
        /// Si la quantité tombe à zéro, le billet est retiré de la facture.
        /// </summary>
        /// <param name="billet">Billet à retirer.</param>
        /// <exception cref="ArgumentNullException">
        /// Lancée si le billet est nul.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Lancée si le billet n’existe pas dans la facture.
        /// </exception>
        public void SupprimerBillet(Billet billet)
        {
            Billet billetExistant = null;
            if (billet is null)
                throw new ArgumentNullException(nameof(billet));   

            foreach (Billet b in _billets)
            {
                if (b == billet)
                {
                    billetExistant = b;
                    break;
                }
            }

            if (billetExistant == null)
                throw new InvalidOperationException("Le billet n'existe pas dans la facture.");

            if (billetExistant.Quantite > 1)
            {
                billetExistant.Quantite -= 1;
            }
            else
            {
                // Quantite == 1 → on retire complètement le billet
                _billets.Remove(billetExistant);
            }
        }
    }
}
