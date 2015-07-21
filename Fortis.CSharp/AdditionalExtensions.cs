//-----------------------------------------------------------------------
// <copyright file="AdditionalExtensions.cs" company="Open Source">
//     Defensive coding libaries
//     Code defensively
// </copyright>
// <author>Huw Simpson</author>
//-----------------------------------------------------------------------

namespace Fortis.CSharp
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Additional Extension methods for various types.
    /// </summary>
    public static class AdditionalExtensions
    {
        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="key">The key.</param>
        /// <returns>An Option of TValue</returns>
        public static Option<TValue> TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            Contract.Requires(source != null);
            Contract.Requires(key != null);

            return Option
                .OfValue(key)
                .AndThen<TKey, TValue>((k, col) => source.TryGetValue(k, out col.Value));
        }
    }
}
