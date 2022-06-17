using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Westwind.Utilities;

namespace Westwind.AspNetCore.Services
{
    /// <summary>
    /// Implements the Gravatar API for retrieving a Gravatar image to display
    /// </summary>
    public static class Gravatar
    {
        public static string GravatarBaseUrl = "https://www.gravatar.com/avatar.php";

        /// <summary>
        /// Returns a Gravatar image url for an email address
        /// </summary>
        /// <param name="email">Email address to display Gravatar for</param>
        /// <param name="Size">Size in pixels (square image) (80)</param>
        /// <param name="rating">Parental Guidance rating of image (PG)</param>
        /// <param name="defaultImageUrl">Url to image if no match is found. 
        ///  If not passed gravatar provides default image</param>
        public static string GetGravatarLink(string email, int Size = 80,
                                             string rating = "PG", 
                                             string defaultImageUrl = null)
        {
            byte[] hash;

            if (string.IsNullOrEmpty(email))
                hash = new byte[] { 0 };
            else
            {
                var provider = MD5.Create();
                hash = provider.ComputeHash(Encoding.UTF8.GetBytes(email));
            }

            StringBuilder sb = new System.Text.StringBuilder();
            for (int x = 0; x < hash.Length; x++)
            {
                sb.Append(hash[x].ToString("x2"));
            }

            if (!string.IsNullOrEmpty(defaultImageUrl))
                defaultImageUrl = "&default=" + defaultImageUrl;
            else
                defaultImageUrl = "";

            return string.Format("{0}?gravatar_id={1}&size={2}&rating={3}{4}",
                                   GravatarBaseUrl, sb.ToString(), Size, rating, defaultImageUrl);
        }

        /// <summary>
        /// Returns a Gravatar Image Tag that can be directly embedded into
        /// an HTML document.
        /// </summary>
        /// <param name="Email">Email address to display Gravatar for</param>
        /// <param name="Size">Size in pixels (square image) (80)</param>
        /// <param name="Rating">Parental Guidance rating of image (PG)</param>
        /// <param name="ExtraImageAttributes">Any extra attributes to stick on the img tag</param>
        /// <param name="DefaultImageUrl">Url to image if no match is found. 
        ///  If not passed gravatar provides default image</param>
        /// <returns></returns>
        public static string GetGravatarImage(string Email, int Size = 80, string Rating = "PG",
                                              string ExtraImageAttributes = null,
                                              string DefaultImageUrl = null)
        {
            string Url = GetGravatarLink(Email, Size, Rating, DefaultImageUrl);
            return string.Format("<img src='{0}' {1}>", Url, ExtraImageAttributes, DefaultImageUrl);
        }

    }


}
