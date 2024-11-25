using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Gameplay.Databases;

namespace StumblePlatformer.Scripts.Gameplay.GameManagers
{
    public class PlayDataCollectionInitializer : MonoBehaviour
    {
        [SerializeField] public CharacterVisualDatabase CharacterVisualDatabase;

        public void Initialize()
        {
            CharacterVisualDatabase.Initialize();
        }
    }
}
