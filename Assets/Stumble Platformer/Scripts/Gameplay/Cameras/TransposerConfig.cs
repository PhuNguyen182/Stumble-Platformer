using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace StumblePlatformer.Scripts.Gameplay.Cameras
{
    [CreateAssetMenu(fileName = "Transposer Config", menuName = "Scriptable Objects/Virtual Camera Config/Transposer Config")]
    public class TransposerConfig : ScriptableObject
    {
        public CinemachineTransposer.BindingMode BindingMode;
        public Vector3 FollowOffset;
        [Range(0, 20f)] public float XDamping = 1;
        [Range(0, 20f)] public float YDamping = 1;
        [Range(0, 20f)] public float ZDamping = 1;
        public CinemachineTransposer.AngularDampingMode AngularDampingMode;
        [Range(0, 20f)] public float PitchDamping;
        [Range(0, 20f)] public float YawDamping;
        [Range(0, 20f)] public float RollDamping;
    }
}
