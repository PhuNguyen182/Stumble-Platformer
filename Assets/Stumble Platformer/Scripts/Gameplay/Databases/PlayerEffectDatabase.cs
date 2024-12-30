using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.Databases
{
    [CreateAssetMenu(fileName = "Player Effect Database", menuName = "Scriptable Objects/Databases/Player Effect Database")]
    public class PlayerEffectDatabase : ScriptableObject
    {
        [SerializeField] public ParticleSystem LaserDeadEffect;
    }
}
