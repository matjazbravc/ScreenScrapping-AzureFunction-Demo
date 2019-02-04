using System;

namespace ScreenScrappingAzureFunctionDemo.Services.Extensions
{
    /// <summary>
    ///     This represents the extension entity for generic.
    /// </summary>
    public static class GenericExtensions
    {
        /// <summary>
        ///     Checks whether the given instance is <c>null</c> or empty.
        /// </summary>
        /// <typeparam name="T">Type of instance.</typeparam>
        /// <param name="instance">Instance to check.</param>
        /// <returns>Returns <c>True</c>, if the original instance is <c>null</c> or empty; otherwise returns <c>False</c>.</returns>
        public static bool IsNullOrDefault<T>(this T instance)
        {
            return instance == null || instance.Equals(default(T));
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentNullException" /> if the given instance is <c>null</c> or empty.
        /// </summary>
        /// <typeparam name="T">Type of instance.</typeparam>
        /// <param name="instance">Instance to check.</param>
        /// <returns>
        ///     Returns the original instance, if the instance is NOT <c>null</c>; otherwise throws an
        ///     <see cref="ArgumentNullException" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="instance" /> is <see langword="null" /></exception>
        public static T ThrowIfNullOrDefault<T>(this T instance)
        {
            if (instance.IsNullOrDefault())
            {
                throw new ArgumentNullException(nameof(instance));
            }
            return instance;
        }
    }
}
