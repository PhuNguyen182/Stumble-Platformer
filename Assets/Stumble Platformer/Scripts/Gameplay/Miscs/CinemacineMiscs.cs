using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.Miscs
{
    public class CinemacineMiscs : MonoBehaviour
    {
        [SerializeField] private CinemachineBrain cinemachineBrain;

        private void Awake()
        {
            if (cinemachineBrain != null)
                cinemachineBrain.useGUILayout = false;
        }
    }
}
