using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Netcode;

namespace StumblePlatformer.Scripts.Multiplayers.Datas
{
    public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
    {
        public bool IsServer;
        public ulong ClientID;
        public FixedString64Bytes PlayerSkin;
        public FixedString64Bytes PlayerName;
        public FixedString64Bytes PlayerID;

        public bool Equals(PlayerData other)
        {
            if (IsServer != other.IsServer)
                return false;

            if (ClientID != other.ClientID) 
                return false;

            if (string.CompareOrdinal(PlayerSkin.Value, other.PlayerSkin.Value) != 0)
                return false;

            if (string.CompareOrdinal(PlayerName.Value, other.PlayerName.Value) != 0)
                return false;

            if (string.CompareOrdinal(PlayerID.Value, other.PlayerID.Value) != 0)
                return false;

            return true;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref IsServer);
            serializer.SerializeValue(ref ClientID);
            serializer.SerializeValue(ref PlayerSkin);
            serializer.SerializeValue(ref PlayerName);
            serializer.SerializeValue(ref PlayerID);
        }
    }
}
