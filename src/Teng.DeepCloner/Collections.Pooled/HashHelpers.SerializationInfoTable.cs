// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Used by Hashtable and Dictionary's SeralizationInfo .ctor's to store the SeralizationInfo
// object until OnDeserialization is called.

using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;

namespace Collections.Pooled
{
    internal static partial class HashHelpers
    {
        private static ConditionalWeakTable<object, SerializationInfo>? _serializationInfoTable;

        public static ConditionalWeakTable<object, SerializationInfo> SerializationInfoTable
        {
            get
            {
                if (_serializationInfoTable == null)
                    Interlocked.CompareExchange(ref _serializationInfoTable, new ConditionalWeakTable<object, SerializationInfo>(), null);

                return _serializationInfoTable;
            }
        }
    }
}