//-----------------------------------------------------------------------
// <copyright file="IEnumerableExtension.cs" company="Open Source">
//     Defensive coding libaries
//     Code defensively
// </copyright>
// <author>Huw Simpson</author>
//-----------------------------------------------------------------------

namespace Fortis.CSharp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// IEnumerable Extension Methods.
    /// </summary>
    public static class IEnumerableExtension
    {
        /// <summary>
        /// Picks one from the specified source.
        /// </summary>
        /// <typeparam name="TInValue">The type of the in value.</typeparam>
        /// <typeparam name="TOutValue">The type of the out value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="choice">The choice.</param>
        /// <returns>An Option of TOutValue</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "The design requires it")]
        public static Option<TOutValue> Pick<TInValue, TOutValue>(
            this IEnumerable<TInValue> source, Func<TInValue, Option<TOutValue>> choice)
        {
            Contract.Requires(source != null);
            Contract.Requires(choice != null);

            var picked =
                source
                .Select(v => choice(v))
                .FirstOrDefault(option => option.Exists());

            return (picked == null) ? Option.None<TOutValue>() : picked;
        }

        /// <summary>
        /// Chooses from the specified source.
        /// </summary>
        /// <typeparam name="TInValue">The type of the in value.</typeparam>
        /// <typeparam name="TOutValue">The type of the out value.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="choice">The choice.</param>
        /// <returns>An IEnumerable of TOutValue</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "The design requires it")]
        public static IEnumerable<TOutValue> Choose<TInValue, TOutValue>(
            this IEnumerable<TInValue> source, Func<TInValue, Option<TOutValue>> choice)
        {
            Contract.Requires(source != null);
            Contract.Requires(choice != null);

            var chosen =
                from item in source
                let picked = choice(item)
                where picked.Exists()
                select ((Option<TOutValue>.Some)picked).Value;

            return chosen;
        }
    }
}
