using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.Databases;
using StumblePlatformer.Scripts.Multiplayers.Datas;

namespace StumblePlatformer.Scripts.Multiplayers.Carriers
{
    public class PlayEntryCarrier : NetworkCarrierBehaviour<PlayEntryData>
    {
        [SerializeField] private LevelNameCollection levelNameCollection;

        public string GetRandomLevelName()
        {
            return levelNameCollection.GetRandomName();
        }
    }
}
