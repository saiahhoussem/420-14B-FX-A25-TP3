using _420_14B_FX_A25_TP3.classes;
using _420_14B_FX_A25_TP3.dal;
using _420_14B_FX_A25_TP3.enums;
using Microsoft.Win32;
using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace _420_14B_FX_A25_TP3
{
    public partial class FormEvenement : Window
    {
        private EtatFormulaire _etat;
        private Evenement _evenement;

        public Evenement Evenement
        {
            get { return _evenement; }
            set { _evenement = value; }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitialiserFormulaire();
        }
        private void ChargerTypeEvenement()
        {
            cboType.ItemsSource = Enum.GetNames(typeof(TypeEvenement));
        }

        private void InitialiserFormulaire()
        {
            switch (_etat)
            {
                case EtatFormulaire.Modifier:
                    this.Title = "Modifier un événémént";
                    btnAction.Content = "Modifier";
                    PreRemplirChamps();
                    break;

                case EtatFormulaire.Supprimer:
                    this.Title = "Supprimer un événémént";
                    btnAction.Content = "Supprimer";
                    PreRemplirChamps();
                    DesactiverChamps();
                    break;

                case EtatFormulaire.Ajouter:
                    this.Title = "Ajouter un événement";
                    btnAction.Content = "Ajouter";
                    InitialiserChampsVides();
                    ChargerTypeEvenement();
                    break;
            }
        }
 
        private void InitialiserChampsVides()
        {   
            txtNom.Clear();
            cboType.SelectedIndex = -1;
            dpDate.SelectedDate = null;
            tpHeure.Value = null;
            txtNbPlaces.Clear();
            txtImage.Clear();
            imgApercu.Source = null;
            btnParcourir.IsEnabled = true;
        }

        private void DesactiverChamps()
        {
            ChargerTypeEvenement();
            txtNom.IsEnabled = false;
            cboType.IsEnabled = false;
            dpDate.IsEnabled = false;
            tpHeure.IsEnabled = false;
            txtNbPlaces.IsEnabled = false;
            txtPrix.IsEnabled = false;
            txtImage.IsEnabled = false;
            AfficherImageApercu();
            btnParcourir.IsEnabled = false;
        }

        private void PreRemplirChamps()
        {
            if (_evenement != null)
            {
                ChargerTypeEvenement();
                txtNom.Text = _evenement.Nom;
                cboType.SelectedIndex =(int)_evenement.Type;
                dpDate.SelectedDate = _evenement.DateHeure.Date;
                tpHeure.Value = DateTime.Today.Add(_evenement.DateHeure.TimeOfDay);
                txtPrix.Text = _evenement.Prix.ToString("0.00", CultureInfo.CurrentCulture);
                txtNbPlaces.Text = _evenement.NbPlaces.ToString();
                txtImage.Text = _evenement.ImagePath;
                AfficherImageApercu();

     
            }
        }
        public FormEvenement(EtatFormulaire etat, Evenement evenement = null)
        {
            InitializeComponent();
            _evenement = evenement;
            _etat = etat;
            InitialiserFormulaire();
        }

        public bool ValiderEvenement()
        {
            string messageErreur = "";
            if (string.IsNullOrWhiteSpace(txtNom.Text))
            {
                messageErreur += "- Vous devez saisir le nom de l'événement.\n";
            }

            if (cboType.SelectedIndex == -1)
            {
                messageErreur += "- Vous devez choisir le type de l'événement.\n";
            }

            if (!dpDate.SelectedDate.HasValue || tpHeure.Value == null)
            {
                messageErreur += "-Vous devez choisir la date et l'heure de l'événement.";     
            }

            int nbPlaces;
            if (!int.TryParse(txtNbPlaces.Text, out nbPlaces) || nbPlaces < Evenement.NB_PLACES_MIN && nbPlaces > Evenement.NB_PLACES_MAX)
            {
                messageErreur += $"-NbPlaces doit être entre {Evenement.NB_PLACES_MIN} et {Evenement.NB_PLACES_MAX}.";
            }

            decimal prix;
            if (!decimal.TryParse(txtPrix.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out prix) || prix < 0)
            {
                messageErreur += "-Prix invalide.";
            }

            if (messageErreur != "")
            {
                MessageBox.Show(messageErreur, "Enregistrement");
                return false;
            }

            return true;
        }
       
        private void btnAction_Click(object sender, RoutedEventArgs e)
        {
            if(_etat == EtatFormulaire.Ajouter)
            {
                if (ValiderEvenement())
                {

                    _evenement = new Evenement(
                                               txtNom.Text.Trim(),
                                               (TypeEvenement)Enum.Parse(typeof(TypeEvenement), 
                                               cboType.SelectedItem.ToString()),
                                               dpDate.SelectedDate.Value.Date + tpHeure.Value.Value.TimeOfDay,
                                               decimal.Parse(txtPrix.Text),
                                               int.Parse(txtNbPlaces.Text),
                                               txtImage.Text.Trim()
                                               );

                    MessageBox.Show("Événement ajouté avec succès !", "Confirmation d'enregistrement");

                    DialogResult = true;
                    Close();
                }

            }
            else if (_etat == EtatFormulaire.Modifier)
            {
                if (ValiderEvenement())
                {
                    if (_evenement != null)
                    {
                        _evenement.Nom = txtNom.Text.Trim();
                        _evenement.Type = (TypeEvenement)Enum.Parse(typeof(TypeEvenement), cboType.SelectedItem.ToString());
                        _evenement.DateHeure = dpDate.SelectedDate.Value.Date + tpHeure.Value.Value.TimeOfDay;
                        _evenement.Prix = decimal.Parse(txtPrix.Text, CultureInfo.CurrentCulture);
                        _evenement.NbPlaces = int.Parse(txtNbPlaces.Text);
                        _evenement.ImagePath = txtImage.Text.Trim();

                        try
                        {
                            BilleterieDAL.ModifierEvenement(_evenement);
                            MessageBox.Show("Événement modifié avec succès !", "Confirmation d'enregistrement");
                            DialogResult = true;
                            Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Erreur lors de la modification: {ex.Message}", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            else 
            {
                MessageBoxResult result = MessageBox.Show(
                "Êtes-vous sûr de vouloir supprimer cet évenement ?",
                "Confirmation de suppression",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    DialogResult = true;
                }
            }

        }

        private void btnParcourir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Veuillez sélectionner une image";
                ofd.Filter = "Image (*.jpg)|*.jpg| Image (*.png)|*.png";

                if (ofd.ShowDialog() == true)
                {
                    AfficherImage(ofd.FileName);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Une erreur s'est produite :\n" + ex.Message, "Ajout d'une image");
            }
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void AfficherImageApercu()
        {
            imgApercu.Source = null;

            if (_evenement == null || string.IsNullOrWhiteSpace(_evenement.ImagePath))
                return;

            string nomFichier = System.IO.Path.GetFileName(_evenement.ImagePath);
            string cheminComplet = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "images",
                nomFichier
            );

            if (File.Exists(cheminComplet))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(cheminComplet, UriKind.Absolute);
                bitmap.EndInit();
                imgApercu.Source = bitmap;
            }
        }

        private void AfficherImage(string cheminFichier)
        {
            txtImage.Text = cheminFichier;

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.UriSource = new Uri(cheminFichier, UriKind.Absolute);
            bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bi.EndInit();

            imgApercu.Source = bi;

        }



    }
}
