using _420_14B_FX_A25_TP3.classes;
using _420_14B_FX_A25_TP3.dal;
using _420_14B_FX_A25_TP3.enums;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace _420_14B_FX_A25_TP3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            ChargerTypeEvenement();
            ChargerEvenements();

        }

        /// <summary>
        /// Ouvre un formulaire pour ajouter un nouvel événement.
        /// </summary>
        private void btnAjouterEvenement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FormEvenement formEvenement = new FormEvenement(EtatFormulaire.Ajouter);
                if (formEvenement.ShowDialog() == true)
                {
                    Evenement nouveauEvenement = formEvenement.Evenement;
                    BilleterieDAL.AjouterEvenement(nouveauEvenement);
                    ChargerEvenements();
                }
            }
            catch(Exception ex)
            {
                throw new ArgumentException("Une erreur s'est produite lors de l'ajout de l'événement.");
            }
        }

        /// <summary>
        /// Lance la recherche d'une facture à partir de son identifiant unique.
        /// </summary>
        private void btnRechercheFacture_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                uint idFacture;
                bool saisieValide = uint.TryParse(txtNoFacture.Text, out idFacture) && idFacture > 0;
                btnPayer.IsEnabled = false;

                if (saisieValide)
                {
                    Facture facture = BilleterieDAL.ObtenirFacture(idFacture);

                    if (facture != null)
                    {
                        lblDateFacture.Text = $"Date : {facture.Date:yyyy/MM/dd HH:mm}";

                        lvFacture.ItemsSource = facture.Billets;


                        lblSousTotal.Text = $"{facture.SousTotal:C2}";
                        lblTotal.Text = $"{facture.Total:C2}";
                        lblTPS.Text = $"{facture.TPS:C2}";
                        lblTVQ.Text = $"{facture.TVQ:C2}";
                    }
                    else
                    {
                        MessageBox.Show($"Aucune facture trouvée avec le numéro {idFacture}.",
                                      "Facture introuvable",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Information);

                        lblDateFacture.Text = "";
                        lvFacture.ItemsSource = null;
                        lblSousTotal.Text = $"";
                        lblTotal.Text = $"";
                        lblTPS.Text = $"";
                        lblTVQ.Text = $"";
                    }
                }
                else
                {
                    MessageBox.Show("Veuillez entrer un numéro de facture valide (nombre positif).",
                                  "Numéro invalide",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Warning);

                    lblDateFacture.Text = "";
                    lvFacture.ItemsSource = null;
                    lblSousTotal.Text = $"";
                    lblTotal.Text = $"";
                    lblTPS.Text = $"";
                    lblTVQ.Text = $"";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la recherche de la facture : {ex.Message}",
                              "Erreur",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void btnModifierEvenement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btnModif = sender as Button;
                Evenement evenement = btnModif.Tag as Evenement;

                if (evenement != null)
                {
                    FormEvenement formEvenement = new FormEvenement(EtatFormulaire.Modifier, evenement);

                    if (formEvenement.ShowDialog() == true)
                    {
                        Evenement evenementModifie = formEvenement.Evenement;
                        BilleterieDAL.ModifierEvenement(evenementModifie);
                        ChargerEvenements();
                    }

                }
                else
                {
                    MessageBox.Show("Veuillez sélectionner un événement à modifier",
                        "Modification d'événement", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur: " + ex.Message, "Modification d'événement",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
       
        private void btnSupprimerEvenement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                Evenement evenement = btn.Tag as Evenement;

                bool evenementASupprimer = (evenement != null);

                if (evenementASupprimer)
                {
                    FormEvenement formEvenement = new FormEvenement(EtatFormulaire.Supprimer, evenement);

                    if(formEvenement.ShowDialog() == true)
                    {
                        evenement = formEvenement.Evenement;
                        try
                        {
                            bool ok = BilleterieDAL.SupprimerEvenement(evenement);

                            if (!ok)
                            {
                                MessageBox.Show(
                                    "Suppression impossible : des billets sont associés à cet événement (ou rien n’a été supprimé).",
                                    "Suppression",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);
                                return;
                            }

                            ChargerEvenements();

                            MessageBox.Show("Evenement supprimé avec succès", "Succès",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Erreur lors de la suppression : {ex.Message}", "Erreur",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                    }

                }
                
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show("Erreur: " + ex.Message, "Suppression d'événement", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void btnRechercheEvenement_Click(object sender, RoutedEventArgs e)
        {  

        }
        private void btnNouvelleFacture_Click(object sender, RoutedEventArgs e)
        {

        }
        
        private void btnPayer_Click(object sender, RoutedEventArgs e)
        {
          
        }

        private void ChargerTypeEvenement()
        {
            cbTypeEvenements.ItemsSource = Enum.GetNames(typeof(TypeEvenement));
        }

       private void ChargerEvenements()
       {
           List<Evenement> evenements = BilleterieDAL.ObtenirListeEvenements(); 

           wpEvenements.Children.Clear();  

           foreach (Evenement e in evenements)
           {
                Border border = new Border
                {
                    Width = 210,
                    MinHeight = 260,
                    Margin = new Thickness(10), 
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(10),
                    Background = Brushes.White,
                    Tag = e, 
                    Cursor = Cursors.Hand
                };

 
                StackPanel stack = new StackPanel();

                Image img = new Image
                {
                    Width = 130,
                    Height = 130,
                    Margin = new Thickness(10),
                    Stretch = Stretch.UniformToFill
                };

        
                if (!string.IsNullOrEmpty(e.ImagePath))
                {
            
                    string cheminImage = e.ImagePath;
            
            
                    if (!Path.IsPathRooted(cheminImage))
                    {
                        cheminImage = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", e.ImagePath);
                    }
            
                    if (File.Exists(cheminImage))
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(cheminImage, UriKind.Absolute);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad; 
                        bitmap.EndInit();
                        img.Source = bitmap;
                    }
                }

                stack.Children.Add(img);

                TextBlock txtNom = new TextBlock
                {
                    Text = e.Nom,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(3)
                };

                TextBlock txtDate = new TextBlock
                {
                    Text = e.DateHeure.ToString("dd MMMM yyyy HH:mm", new System.Globalization.CultureInfo("fr-FR")),
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(3)
                };

                TextBlock txtPrix = new TextBlock
                {
                    Text = $"{e.Prix:F2}$", 
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.Blue,  
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(3)
                };

        
                TextBlock txtPlaces = new TextBlock
                {
                    Text = $"Places: {e.NbPlaces}",
                    FontWeight = FontWeights.SemiBold,
                    Foreground = e.NbPlaces > 0 ? Brushes.Green : Brushes.Red,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(3, 0, 3, 3)
                };

                stack.Children.Add(txtNom);
                stack.Children.Add(txtDate);
                stack.Children.Add(txtPrix);
                stack.Children.Add(txtPlaces);

                StackPanel panelIcones = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(5, 5, 5, 15)
                };

                Button iconModifier = new Button
                {
                    Content = new Image
                    {
                        Source = new BitmapImage(new Uri("/Resources/edit.png",UriKind.Relative)),
                        Width = 25,
                        Height = 25
                    },
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Margin = new Thickness(10, 0, 20, 0),
                    Cursor = Cursors.Hand,
                    ToolTip = "Modifier cet événement",
                    Tag = e
                };

                iconModifier.Click += btnModifierEvenement_Click;

                panelIcones.Children.Add(iconModifier);

                Button iconSupprimer = new Button
                {
                    Content = new Image{
                        Source = new BitmapImage(new Uri("/Resources/delete.png", UriKind.Relative)),
                        Width = 25,
                        Height = 25
                    },
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Margin = new Thickness(0, 0, 10, 0),
                    Cursor = Cursors.Hand,
                    ToolTip = "Supprimer cet événement",
                    Tag = e
                };

                iconSupprimer.Click += btnSupprimerEvenement_Click;

                panelIcones.Children.Add(iconSupprimer);

                stack.Children.Add(panelIcones);
                panelIcones.Margin = new Thickness(0, 5, 0, 15);

                border.Child = stack;
                wpEvenements.Children.Add(border);
           }
        }
    }
}