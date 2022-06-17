using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Westwind.AspNetCore.Extensions
{

    /// <summary>
    /// Extends Query Collections to make it easier to retrieve collection values
    /// either individually or multi-values.
    ///
    /// Based on concepts from Khalid Abuhakmeh:
    /// https://khalidabuhakmeh.com/read-and-convert-querycollection-values-in-aspnet
    /// </summary>
    public static class IQueryCollectionExtensions
    {
        /// <summary>
        /// Retrieves multiple selection values for a key
        /// and returns them as strings.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetAll(this IQueryCollection collection, string key, string defaultValue = null)
        {
            return GetAll(collection, key);
        }


        /// <summary>
        /// Retrieves multiple selection values for a key
        /// as an enumerable value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue">Default value if the value is invalid or missing</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAll<T>(this IQueryCollection collection, string key, T defaultValue = default)
        {
            var values = new List<T>();

            if (collection.TryGetValue(key, out var results))
            {
                foreach (var s in results)
                {
                    try
                    {
                        var result = (T)Convert.ChangeType(s, typeof(T));
                        values.Add(result);
                    }
                    catch (Exception)
                    {
                        values.Add(defaultValue);
                    }
                }
            }

            // return an array with at least one
            return values;
        }



        /// <summary>
        /// Retrieves a single value and casts it to
        /// a non-string value. For string use the non-Generic version
        /// </summary>
        /// <typeparam name="T">Type t</typeparam>
        /// <param name="collection">StringCollection </param>
        /// <param name="key">Key to retrieve</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string Get(this IQueryCollection collection,
            string key,
            string defaultValue = null)
        {
            var value = defaultValue;

            if (collection.TryGetValue(key, out var results))
            {
                value = results.FirstOrDefault();
            }

            return value;
        }

        /// <summary>
        /// Retrieves a single value and casts it to
        /// a non-string value. For string use the non-Generic version
        /// </summary>
        /// <typeparam name="T">Type t</typeparam>
        /// <param name="collection"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T Get<T>(this IQueryCollection collection,
            string key,
            T defaultValue = default)
        {
            var value = defaultValue;

            if (collection.TryGetValue(key, out var results))
            {
                string val = results.FirstOrDefault();
                try
                {
                    value = (T)Convert.ChangeType(val, typeof(T));
                }
                catch
                {
                    value = defaultValue;
                }
            }

            return value;
        }
    }
}
