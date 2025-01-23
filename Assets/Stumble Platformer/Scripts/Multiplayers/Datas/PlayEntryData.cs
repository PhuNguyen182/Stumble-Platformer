using System;
using Unity.Netcode;
using Unity.Collections;

namespace StumblePlatformer.Scripts.Multiplayers.Datas
{
    public struct PlayEntryData : IEquatable<PlayEntryData>, INetworkSerializable
    {
        public FixedString64Bytes PlayLevelName;

        public bool Equals(PlayEntryData other)
        {
            return string.CompareOrdinal(PlayLevelName.Value, other.PlayLevelName.Value) == 0;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref PlayLevelName);
        }
    }
}
