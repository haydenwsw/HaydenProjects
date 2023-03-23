using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace CarSearcher
{
    /// <summary>
    /// Converts a cookie string into a data type
    /// </summary>
    internal static class CookieConvert
    {
        /// <summary>
        /// Converts a cookie into a dictionary
        /// It will add the first recurring cookie
        /// </summary>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static Dictionary<string, string> CookieToDictionary(string cookies)
        {
            Dictionary<string, string> cookieDictionary = new Dictionary<string, string>();

            var cookie = cookies.Split(';').Select(x => splitCookie(x));

            foreach (KeyValuePair<string, string> cookiePair in cookie)
            {
                cookieDictionary.TryAdd(cookiePair.Key, cookiePair.Value);
            }

            return cookieDictionary;
        }

        public static Dictionary<string, string> CookieToDictionary(IEnumerable<string> cookiesList)
        {
            return CookieToDictionary(cookiesList.Aggregate((x, y) => $"{x}; {y}"));
        }

        /// <summary>
        /// Converts dictionart<string, string> into a cookie string
        /// </summary>
        /// <param name="cookieDictionary"></param>
        /// <returns></returns>
        public static string DictionaryToCookie(Dictionary<string, string> cookieDictionary)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (KeyValuePair<string, string> cookie in cookieDictionary)
            {
                stringBuilder.Append(cookie.Key);
                stringBuilder.Append('=');

                stringBuilder.Append(cookie.Value);
                stringBuilder.Append("; ");
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        ///  Deserializes a cookie into an object
        ///  It will add the last recurring cookie
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static T DeserializeCookie<T>(string cookies)
        {
            T returnClass = (T)typeof(T).GetConstructors().First().Invoke(null);

            var cookie = cookies.Split(';').Select(x => splitCookie(x));

            foreach (PropertyInfo property in typeof(T).GetRuntimeProperties())
            {
                JsonPropertyAttribute jsonAttribute = property.GetCustomAttribute<JsonPropertyAttribute>();

                if (jsonAttribute != null)
                {
                    KeyValuePair<string, string> c = cookie.FirstOrDefault(x => x.Key == jsonAttribute.PropertyName);
                    property.SetValue(returnClass, c.Value);
                }
                else
                {
                    KeyValuePair<string, string> c = cookie.FirstOrDefault(x => x.Key == property.Name);
                    property.SetValue(returnClass, c.Value);
                }
            }

            return returnClass;
        }

        public static T DeserializeCookie<T>(IEnumerable<string> cookiesList)
        {
            return DeserializeCookie<T>(cookiesList.Aggregate((x, y) => $"{x}; {y}"));
        }

        /// <summary>
        /// Serializes a cookie object into a string
        /// </summary>
        /// <param name="cookieClass"></param>
        /// <returns></returns>
        public static string SerializeCookie(object cookieObject)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (PropertyInfo property in cookieObject.GetType().GetRuntimeProperties())
            {
                string name = property.Name;
                stringBuilder.Append(name);
                stringBuilder.Append('=');

                string value = property.GetValue(cookieObject) as string;
                stringBuilder.Append(value);
                stringBuilder.Append("; ");
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Splits a cookie into a keyValuePair
        /// Adds null as value if split is unsuccesful
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        private static KeyValuePair<string, string> splitCookie(string cookie)
        {
            string[] c = cookie.Split('=', 2);
            return new KeyValuePair<string, string>(c.First().TrimStart(), c.ElementAtOrDefault(1));
        }
    }
}
