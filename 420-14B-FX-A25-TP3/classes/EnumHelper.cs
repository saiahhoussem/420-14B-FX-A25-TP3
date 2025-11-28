
using System.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
namespace _420_14B_FX_A25_TP3.classes
{

        /// <summary>
        /// Classe utilitaire pour les énumérations. Fournit des méthodes permettant d'obtenir les
        /// descriptions associées aux constantes des énumérations lorsque celles-ci sont disponibles.
        /// NOTE POUR LES ÉTUDIANTS : N'essayez pas de comprendre le code suivant.
        /// </summary>
        public static class EnumHelper
        {
            
            /// <summary>
            /// Extension permettant d'obtenir la description pour une constante d'une énumération, si disponible.
            /// S'il n'y a pas de description associée à la constante de l'énumération, permet d'obtenir la valeur
            /// de celle-ci.
            /// Source : https://msmvps.com/blogs/deborahk/archive/2009/07/10/enum-binding-to-the-description-attribute.aspx
            /// </summary>
            /// <param name="currentEnum">Énumération pour laquelle on désire obtenir une description.</param>
            /// <returns>
            /// Description associée à la constante de l'énumération ou bien la valeur s'il n'y a pas de description.
            /// </returns>
            public static string ObtenirDescription(this Enum currentEnum)
            {
                string description;
                DescriptionAttribute da;

                FieldInfo fi = currentEnum.GetType().GetField(currentEnum.ToString());
                da = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
                if (da != null)
                    description = da.Description;
                else
                    description = currentEnum.ToString();

                return description;
            }

            /// <summary>
            /// Permet d'obtenir toutes les descriptions associées aux constantes d'une énumération, si disponible.
            /// S'il n'y a pas de description associée à une constante de l'énumération, permet d'obtenir
            /// la valeur de celle-ci.
            /// </summary>
            /// <returns>
            /// Les descriptions associées aux constantes de l'énumération ou bien les valeurs
            /// s'il n'y a pas de description.
            /// </returns>
            public static string[] ObtenirDescriptions<T>()
            {
                Type enumType = typeof(T);
                List<String> lesDescriptions = new List<String>();
                foreach (Enum valeur in Enum.GetValues(enumType))
                {
                    lesDescriptions.Add(valeur.ObtenirDescription());
                }
                return lesDescriptions.ToArray();
            }

        /// <summary>
        /// Retourne la valeur d’énumération correspondant à un nom ou une description.
        /// </summary>
        /// <typeparam name="TEnum">Type d’énumération.</typeparam>
        /// <param name="texte">Nom ou description à rechercher.</param>
        /// <returns>Valeur correspondante de l’énumération.</returns>
        /// <exception cref="ArgumentException">Si aucune correspondance n’est trouvée.</exception>
        public static TEnum ObtenirValeurDepuisTexte<TEnum>(string texte) where TEnum : Enum
        {
            if (string.IsNullOrWhiteSpace(texte))
                throw new ArgumentException("Le texte fourni est vide.", nameof(texte));

            var type = typeof(TEnum);
            foreach (var valeur in Enum.GetValues(type))
            {
                var enumValue = (TEnum)valeur;
                string nom = enumValue.ToString();
                string description = ObtenirDescription(enumValue);

                if (string.Equals(texte, nom, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(texte, description, StringComparison.OrdinalIgnoreCase))
                {
                    return enumValue;
                }
            }

            throw new ArgumentException($"Aucune valeur de l’énumération {type.Name} ne correspond à '{texte}'.");
        }


    }
 }  

