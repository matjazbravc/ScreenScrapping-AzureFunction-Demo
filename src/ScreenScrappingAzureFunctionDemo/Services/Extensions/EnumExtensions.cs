using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ScreenScrappingAzureFunctionDemo.Services.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// A generic extension method that aids in reflecting 
        /// and retrieving any attribute that is applied to an `Enum`.
        /// http://codereview.stackexchange.com/questions/5352/getting-the-value-of-a-custom-attribute-from-an-enum
        /// </summary>
        /// <param name="value">Enum value</param>
        public static TAttribute GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name).GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
        }

        /// <summary>
        /// Get Enum description
        /// http://blog.spontaneouspublicity.com/associating-strings-with-enums-in-c
        /// </summary>
        /// <param name="value">Enum value</param>
        /// <returns>Enum description</returns>
        public static string GetDescription(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static string GetHashTag(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes = (HashTagAttribute[])fi.GetCustomAttributes(typeof(HashTagAttribute), false);
            return attributes.Length > 0 ? attributes[0].Value : value.ToString();
        }

        /// <summary>
        /// Gets enum descriptions as string list
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <returns>string list</returns>
        // Usage: List<string> items = GetListOfDescription<CancelReasonEnum>();
        public static List<string> GetListOfDescription<T>() where T : struct
        {
            var t = typeof(T);
            return !t.IsEnum ? null : Enum.GetValues(t).Cast<Enum>().Select(x => x.GetDescription()).ToList();
        }
    }
}
