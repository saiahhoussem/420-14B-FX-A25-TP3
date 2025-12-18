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
            }
        }

        //A ne pas utiliser non plus! (Pas d'ajout)
        private void InitialiserChampsVides()
        {   
            txtNom.Clear();
            cboType.SelectedIndex = -1;
            dpDate.SelectedDate = null;
            tpHeure.Value = null;
            txtNbPlaces.Clear();
            txtImage.Clear();
            imgApercu.Source = null;
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

       
        private void btnAction_Click(object sender, RoutedEventArgs e)
        {
            if(_etat == EtatFormulaire.Supprimer)
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
            else if (_etat == EtatFormulaire.Modifier)
            {
                if (_evenement == null)
                {
                    MessageBox.Show("Aucun événement à modifier.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtNom.Text))
                {
                    MessageBox.Show("Le nom est obligatoire.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (cboType.SelectedItem == null)
                {
                    MessageBox.Show("Le type est obligatoire.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (!dpDate.SelectedDate.HasValue || tpHeure.Value == null)
                {
                    MessageBox.Show("La date et l'heure sont obligatoires.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(txtNbPlaces.Text, out int nbPlaces) || nbPlaces <= 0)
                {
                    MessageBox.Show("NbPlaces invalide.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(txtPrix.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal prix) || prix < 0)
                {
                    MessageBox.Show("Prix invalide.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _evenement.Nom = txtNom.Text.Trim();
                _evenement.Type = (TypeEvenement)Enum.Parse(typeof(TypeEvenement), cboType.SelectedItem.ToString());
                _evenement.DateHeure = dpDate.SelectedDate.Value.Date + tpHeure.Value.Value.TimeOfDay;
                _evenement.NbPlaces = nbPlaces;
                _evenement.Prix = prix;
                _evenement.ImagePath = txtImage.Text.Trim();

                DialogResult = true;
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
