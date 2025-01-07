using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.Databases;

namespace StumblePlatformer.Scripts.Multiplayers.Carriers
{
    public struct PlayEntryData
    {
        public string PlayLevelName;
    }

    public class PlayEntryCarrier : CarrierBehaviour<PlayEntryData>
    {
        [SerializeField] private LevelNameCollection levelNameCollection;

        public string GetRandomLevelName()
        {
            return levelNameCollection.GetRandomName();
        }
    }
}
