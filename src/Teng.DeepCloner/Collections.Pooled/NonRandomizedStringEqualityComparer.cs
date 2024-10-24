// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

namespace Collections.Pooled
{
    /// <summary>
    /// NonRandomizedStringEqualityComparer is the comparer used by default with the PooledDictionary.
    /// We use NonRandomizedStringEqualityComparer as default comparer as it doesnt use the randomized string hashing which
    /// keeps the performance not affected till we hit collision threshold and then we switch to the comparer which is using
    /// randomized string hashing.
    /// </summary>
    [Serializable] // Required for compatibility with .NET Core 2.0 as we exposed the NonRandomizedStringEqualityComparer inside the serialization blob
    public sealed class NonRandomizedStringEqualityComparer : EqualityComparer<string>, ISerializable
    {
        private static readonly int s_empyStringHashCode = string.Empty.GetHashCode();

        internal new static IEqualityComparer<string> Default { get; } = new NonRandomizedStringEqualityComparer();

        private NonRandomizedStringEqualityComparer()
        { }

        // This is used by the serialization engine.
        private NonRandomizedStringEqualityComparer(SerializationInfo information, StreamingContext context)
        { }

        public override sealed bool Equals(string x, string y) => string.Equals(x, y);

        public override sealed int GetHashCode(string str)
            => str is null ? 0 : str.Length == 0 ? s_empyStringHashCode : GetNonRandomizedHashCode(str);

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.SetType(typeof(NonRandomizedStringEqualityComparer));
        }

        // Use this if and only if 'Denial of Service' attacks are not a concern (i.e. never used for free-form user input),
        // or are otherwise mitigated.
        // This code was ported from an internal method on String, which relied on private members to get the char* pointer.
        private static int GetNonRandomizedHashCode(string str)
        {
            ReadOnlySpan<char> chars = str.AsSpan();

            uint hash1 = (5381 << 16) + 5381;
            uint hash2 = hash1;

            int length = chars.Length;

            // Process characters in pairs of four bytes
            for (int i = 0; i < length; i += 4)
            {
                if (i + 4 <= length)
                {
                    hash1 = (((hash1 << 5) | (hash1 >> 27)) + hash1) ^ (uint)(chars[i] | (chars[i + 1] << 16));
                    hash2 = (((hash2 << 5) | (hash2 >> 27)) + hash2) ^ (uint)(chars[i + 2] | (chars[i + 3] << 16));
                }
                else if (i + 2 == length)
                {
                    hash2 = (((hash2 << 5) | (hash2 >> 27)) + hash2) ^ (uint)(chars[i] | (chars[i + 1] << 16));
                }
                else if (i + 1 == length)
                {
                    hash2 = (((hash2 << 5) | (hash2 >> 27)) + hash2) ^ (uint)chars[i];
                }
            }

            return (int)(hash1 + (hash2 * 1566083941));
        }
    }
}