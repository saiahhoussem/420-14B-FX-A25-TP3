using _420_14B_FX_A25_TP3.classes;
using _420_14B_FX_A25_TP3.enums;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _420_14B_FX_A25_TP3.dal
{
    public static class BilleterieDAL
    {
        private const string APPSETTINGS_FILE = "appsettings.json";
        private const string CONNECTION_STRING = "DefaultConnection";
        private const string IMAGE_PATH = "Images:Path";

        private static readonly IConfiguration _config = new ConfigurationBuilder()
    .AddJsonFile(APPSETTINGS_FILE, optional: false, reloadOnChange: true)
    .Build();

        /// <summary>
        /// Crée et retourne un objet MySqlConnection basé sur la configuration.
        /// </summary>
        /// <returns>Nouvelle connexion MySQL non ouverte.</returns>
        private static MySqlConnection CreerConnection()
        {
            string connectionString = _config.GetConnectionString(CONNECTION_STRING);
            return new MySqlConnection(connectionString);
        }

        /// <summary>
        /// Ferme et libère correctement une connexion MySQL si elle est ouverte.
        /// </summary>
        /// <param name="cn">Connexion à fermer.</param>
        private static void FermerConnection(MySqlConnection cn)
        {
            if (cn != null && cn.State == System.Data.ConnectionState.Open)
            {
                cn.Close();
                cn.Dispose();
            }
        }

        /// <summary>
        /// Retourne le chemin du répertoire contenant les images des événements.
        /// </summary>
        /// <returns>Chemin du dossier en chaîne de caractères.</returns>
        private static string CheminImages()
        {
            string imagesPath = _config[IMAGE_PATH];
            if (string.IsNullOrWhiteSpace(imagesPath))
            {
                imagesPath = "images";
            }
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagesPath);
        }

        /// <summary>
        /// Ajoute un nouvel événement à la base de données.
        /// </summary>
        /// <param name="e">Evenement</param>
        public static void AjouterEvenement(Evenement e)
        {
            if (e is null)
            {
                throw new ArgumentNullException(nameof(e), "L'événement ne doit pas être null.");
            }


            MySqlConnection cn = null;
            string imageCopiee = null;
            try
            {
                cn = CreerConnection();
                cn.Open();

                string nouveauNomImage = null;
                if (!string.IsNullOrEmpty(e.ImagePath) && File.Exists(e.ImagePath))
                {
                    string dossierImages = CheminImages();

                    if (!Directory.Exists(dossierImages))
                    {
                        Directory.CreateDirectory(dossierImages);
                    }

                    string extension = Path.GetExtension(e.ImagePath);
                    nouveauNomImage = $"{Guid.NewGuid()}{extension}";
                    string cheminDestination = Path.Combine(dossierImages, nouveauNomImage);

                    File.Copy(e.ImagePath, cheminDestination, true);
                    imageCopiee = cheminDestination;
                }

                string requete = "INSERT INTO Evenements (nom, type, dateHeure, prix, nbPlaces, imagePath)"+
                                 " VALUES (@nom, @type, @dateHeure, @prix, @nbPlaces, @imagePath);";

                MySqlCommand cmd = new MySqlCommand(requete, cn);
                cmd.Parameters.AddWithValue("@nom", e.Nom);
                cmd.Parameters.AddWithValue("@type", (int)e.Type);
                cmd.Parameters.AddWithValue("@dateHeure", e.DateHeure);
                cmd.Parameters.AddWithValue("@prix", e.Prix);
                cmd.Parameters.AddWithValue("@nbPlaces", e.NbPlaces);

                if (nouveauNomImage != null)
                {
                    cmd.Parameters.AddWithValue("@imagePath", nouveauNomImage);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@imagePath", DBNull.Value);
                }

      
                cmd.ExecuteNonQuery();

         
                e.Id = Convert.ToUInt32(cmd.LastInsertedId);

                if (nouveauNomImage != null)
                {
                    e.ImagePath = nouveauNomImage;
                }
            }
            catch (Exception ex)
            {
                if (imageCopiee != null && File.Exists(imageCopiee))
                {
                    File.Delete(imageCopiee);
                }

                throw new Exception("Erreur lors de l'ajout de l'événement", ex);
            }
            finally
            {
                FermerConnection(cn);
            }
        }

        /// <summary>
        /// Recherche et retourne une facture à partir de son identifiant unique.
        /// </summary>
        /// <param name="idFacture">Identifiant de la facture</param>
        /// <returns>Facture</returns>
        public static Facture ObtenirFacture(uint idFacture)
        {
            MySqlConnection cn = null;
            MySqlDataReader dr = null;

            try
            {
                cn = CreerConnection();
                cn.Open();

                string requete = @"SELECT Id, `Date` as DateCreation FROM Factures WHERE Id = @idFacture";

                MySqlCommand cmd = new MySqlCommand(requete, cn);
                cmd.Parameters.AddWithValue("@idFacture", idFacture);

                dr = cmd.ExecuteReader(); 

                dr.Read();
                uint factureId = dr.GetUInt32(0);
                DateTime factureDate = dr.GetDateTime(1);
                dr.Close(); 

                Facture facture = new Facture(factureId, factureDate, new List<Billet>());

                
                requete = "SELECT b.Id, b.Quantite, e.Id,e.Nom, e.Type, e.DateHeure, e.Prix, e.NbPlaces, e.ImagePath " 
                        + "FROM Billets b INNER JOIN Evenements e ON b.IdEvenement = e.Id "
                        + "WHERE b.IdFacture = @idFacture ORDER BY e.Nom";

                cmd.CommandText = requete;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@idFacture", idFacture);

                dr = cmd.ExecuteReader(); 

                while (dr.Read())
                {
                   
                    Evenement evenement = new Evenement(
                        dr.GetUInt32(2),                   
                        dr.GetString(3),                   
                        (TypeEvenement)dr.GetInt32(4),     
                        dr.GetDateTime(5),                 
                        dr.GetDecimal(6),                  
                        dr.GetInt32(7),
                        dr.GetString(8)                       
                    );

                    Billet billet = new Billet(
                        dr.GetUInt32(0),                   
                        evenement,
                        dr.GetInt32(1)                     
                    );

                    facture.AjouterBillet(billet);
                }
                dr.Close(); 

                return facture;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération de la facture ID {idFacture}", ex);
            }
            finally
            {
                dr.Close();
                FermerConnection(cn);
            }
        }


        public static List<Evenement> ObtenirListeEvenements(string nom = "", TypeEvenement? type = null)
        {
            List<Evenement> evenements = new List<Evenement>();
            MySqlConnection cn = null;
            try
            {
                cn = CreerConnection();
                cn.Open();

                string requete = @"SELECT id, nom, type, dateHeure, prix, nbPlaces, imagePath 
                                   FROM Evenements 
                                   WHERE (@nom = '' OR nom LIKE @nom)
                                   AND (@type IS NULL OR type = @type)
                                   ORDER BY nom";

                MySqlCommand cmd = new MySqlCommand(requete, cn);

                if (string.IsNullOrEmpty(nom))
                {
                    cmd.Parameters.AddWithValue("@nom", "");
                }
                else
                {
                    cmd.Parameters.AddWithValue("@nom", "%" + nom + "%");
                }

                if (type.HasValue)
                {
                    cmd.Parameters.AddWithValue("@type", (int)type.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@type", DBNull.Value);
                }

                MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Evenement evenement = new Evenement(
                        dr.GetUInt32(0),
                        dr.GetString(1),
                        (TypeEvenement)dr.GetInt32(2),
                        dr.GetDateTime(3),
                        dr.GetDecimal(4),
                        dr.GetInt32(5),
                        dr.GetString(6)
                    );
                    evenements.Add(evenement);
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la récupération des événements", ex);
            }
            finally
            {
                FermerConnection(cn);
            }

            return evenements;
        }

      
        public static bool EvenementExiste(Evenement e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e), "L'événement ne doit pas être null.");
            }

            MySqlConnection cn = null;
            try
            {
                cn = CreerConnection();
                cn.Open();

                string requete = "SELECT COUNT(*) FROM Evenements" + 
                                 " WHERE Nom = @nom DateHeure = @dateHeure";

                MySqlCommand cmd = new MySqlCommand(requete, cn);
                cmd.Parameters.AddWithValue("@nom", e.Nom);
                cmd.Parameters.AddWithValue("@dateHeure", e.DateHeure);

                long compteur = Convert.ToInt64(cmd.ExecuteScalar());

                return compteur > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la vérification de l'existence de l'événement.", ex);
            }
            finally
            {
                FermerConnection(cn);
            }
        }

     
        public static bool EvenementEnConflit(Evenement e, uint? idIgnore = null)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            MySqlConnection cn = null;
            try
            {
                cn = CreerConnection();
                cn.Open();

                string requete = @"SELECT COUNT(*) 
                           FROM Evenements 
                           WHERE dateHeure = @dateHeure";

                if (idIgnore.HasValue)
                {
                    requete += " AND id != @idIgnore";
                }

                MySqlCommand cmd = new MySqlCommand(requete, cn);
                cmd.Parameters.AddWithValue("@dateHeure", e.DateHeure);

                if (idIgnore.HasValue)
                {
                    cmd.Parameters.AddWithValue("@idIgnore", idIgnore.Value);
                }

                int compteur = (int)cmd.ExecuteScalar();

                return compteur > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la vérification du conflit d'événement", ex);
            }
            finally
            {
                FermerConnection(cn);
            }
        }

        
        public static bool SupprimerEvenement(Evenement e)
        {
            if (e is null)
                throw new ArgumentNullException(nameof(e));

            if (e.Id <= 0)
                throw new ArgumentException("L'événement doit avoir un Id valide pour être supprimé.");

            MySqlConnection cn = null;

            try
            {
                cn = CreerConnection();
                cn.Open();

                
                string requete = "SELECT COUNT(*) FROM Billets WHERE IdEvenement = @id";
                MySqlCommand cmd = new MySqlCommand(requete, cn);
                cmd.Parameters.AddWithValue("@id", e.Id);

                long compteurBillets = Convert.ToInt64(cmd.ExecuteScalar());
                if (compteurBillets > 0)
                    return false;

                
                requete = "SELECT ImagePath FROM Evenements WHERE Id = @id";
                cmd.CommandText = requete;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@id", e.Id);

                string imagePath = cmd.ExecuteScalar() as string;

                
                requete = "DELETE FROM Evenements WHERE Id = @id";
                cmd.CommandText = requete;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@id", e.Id);

                int lignesSupprimees = cmd.ExecuteNonQuery();
                if (lignesSupprimees == 0)
                    return false;

                
                if (!string.IsNullOrWhiteSpace(imagePath))
                {
                    
                    string cheminImage = imagePath;

                    
                    if (!Path.IsPathRooted(cheminImage))
                    {
                        cheminImage = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images", imagePath);
                    }

                    if (File.Exists(cheminImage))
                    {
                        File.Delete(cheminImage);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la suppression de l'événement", ex);
            }
            finally
            {
                FermerConnection(cn);
            }
        }


        public static void ModifierEvenement(Evenement e)
        {
            if (e is null)
                throw new ArgumentNullException(nameof(e));

            if (e.Id == 0)
                throw new ArgumentException("Id invalide");

            MySqlConnection cn = null;
            try
            {
                cn = CreerConnection();
                cn.Open();

                string requete = "SELECT ImagePath FROM Evenements WHERE Id = @id";
                MySqlCommand cmd = new MySqlCommand(requete, cn);
                cmd.Parameters.AddWithValue("@id", e.Id);
                string ancienNomImage = cmd.ExecuteScalar() as string;

                string dossierImages = CheminImages();

                string nouveauNomImage = ancienNomImage;

                if (!string.IsNullOrWhiteSpace(e.ImagePath) && e.ImagePath != ancienNomImage)
                {
                    string nomFichier = Path.GetFileName(e.ImagePath);
                    string cheminCompletSource = e.ImagePath;
                    string cheminCompletCible = Path.Combine(dossierImages, nomFichier);

                    if (!cheminCompletSource.StartsWith(dossierImages, StringComparison.OrdinalIgnoreCase))
                    {

                        string extension = Path.GetExtension(e.ImagePath);
                        nouveauNomImage = $"{Guid.NewGuid()}{extension}";
                        cheminCompletCible = Path.Combine(dossierImages, nouveauNomImage);

                        if (!Directory.Exists(dossierImages))
                            Directory.CreateDirectory(dossierImages);

                        File.Copy(cheminCompletSource, cheminCompletCible, true);
                    }
                    else
                    {
                        nouveauNomImage = nomFichier;
                    }
                }

                requete = @"UPDATE Evenements SET 
                           Nom = @nom, Type = @type, DateHeure = @dateHeure,
                           Prix = @prix, NbPlaces = @nbPlaces, ImagePath = @imagePath
                           WHERE Id = @id";

                cmd.CommandText = requete;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@id", e.Id);
                cmd.Parameters.AddWithValue("@nom", e.Nom);
                cmd.Parameters.AddWithValue("@type", (int)e.Type);
                cmd.Parameters.AddWithValue("@dateHeure", e.DateHeure);
                cmd.Parameters.AddWithValue("@prix", e.Prix);
                cmd.Parameters.AddWithValue("@nbPlaces", e.NbPlaces);
                if (nouveauNomImage != null)
                {
                    cmd.Parameters.AddWithValue("@imagePath", nouveauNomImage);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@imagePath", DBNull.Value);
                }

                cmd.ExecuteNonQuery();

                if (!string.IsNullOrEmpty(ancienNomImage) && ancienNomImage != nouveauNomImage)
                {
                    string ancienCheminComplet = Path.Combine(dossierImages, ancienNomImage);
                    if (File.Exists(ancienCheminComplet))
                        File.Delete(ancienCheminComplet);
                }

                if (!string.IsNullOrEmpty(nouveauNomImage))
                {

                    e.ImagePath = nouveauNomImage;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur modification: {ex.Message}", ex);
            }
            finally
            {
                FermerConnection(cn);
            }
        }


        
        public static void AjouterFacture(Facture f)
        {
            if (f is null)
                throw new ArgumentNullException(nameof(f), "La facture ne peut pas être null.");

            if (f.Billets == null || f.Billets.Count == 0)
                throw new InvalidOperationException("La facture doit contenir au moins un billet.");

            MySqlConnection cn = null;

            try
            {
                cn = CreerConnection();
                cn.Open();

                decimal sousTotal = f.SousTotal;
                decimal tps = f.TPS;
                decimal tvq = f.TVQ;
                decimal total = f.Total;

                string requete = "INSERT INTO factures (DateCreation, SousTotal, TPS, TVQ, Total) " +
                                       "VALUES (@dateCreation, @sousTotal, @tps, @tvq, @total)";

                MySqlCommand cmd = new MySqlCommand(requete, cn);

                cmd.Parameters.AddWithValue("@dateCreation", f.Date);
                cmd.Parameters.AddWithValue("@sousTotal", sousTotal);
                cmd.Parameters.AddWithValue("@tps", tps);
                cmd.Parameters.AddWithValue("@tvq", tvq);
                cmd.Parameters.AddWithValue("@total", total);

                cmd.ExecuteNonQuery();

                f.Id = (uint)cmd.LastInsertedId;

                requete = "INSERT INTO billets (IdFacture, IdEvenement, Quantite) " +
                                      "VALUES (@factureId, @evenementId, @quantite)";
                cmd.CommandText = requete;

                foreach (Billet billet in f.Billets)
                {
                    MySqlCommand cmdBillet = new MySqlCommand(requete, cn);
                    cmdBillet.Parameters.AddWithValue("@factureId", f.Id);
                    cmdBillet.Parameters.AddWithValue("@evenementId", billet.Evenement.Id);
                    cmdBillet.Parameters.AddWithValue("@quantite", billet.Quantite);

                    cmdBillet.ExecuteNonQuery();

                    billet.Id = (uint)cmdBillet.LastInsertedId;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'ajout de la facture: {ex.Message}", ex);
            }
            finally
            {
                FermerConnection(cn);
            }
        }

      
    }
}
