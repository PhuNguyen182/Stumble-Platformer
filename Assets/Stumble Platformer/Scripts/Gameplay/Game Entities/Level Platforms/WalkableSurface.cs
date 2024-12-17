using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms
{
    // This require a trigger collider
    public class WalkableSurface : MonoBehaviour
    {
        [SerializeField] public float LinearDrag;
        [SerializeField] public float AngularDrag;
        [Range(0f, 1f)]
        [SerializeField] public float JumpRestriction;
    }
}
