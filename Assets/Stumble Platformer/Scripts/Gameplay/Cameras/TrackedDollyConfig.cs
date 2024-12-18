using UnityEngine;
using Cinemachine;

namespace StumblePlatformer.Scripts.Gameplay.Cameras
{
    [CreateAssetMenu(fileName = "Tracked Dolly Config", menuName = "Scriptable Objects/Virtual Camera Config/Tracked Dolly Config")]
    public class TrackedDollyConfig : ScriptableObject
    {
        public CinemachinePathBase.PositionUnits PositionUnits;
        public Vector3 PathOffset;
        [Range(0, 20f)] public float XDamping = 1;
        [Range(0, 20f)] public float YDamping = 1;
        [Range(0, 20f)] public float ZDamping = 1;
        public CinemachineTrackedDolly.CameraUpMode CameraUpMode;
        [Range(0, 20f)] public float PitchDamping;
        [Range(0, 20f)] public float YawDamping;
        [Range(0, 20f)] public float RollDamping;
        public bool AutoDolly;
        public float PositionOffset = 0;
        public int SearchRadius = 2;
        public int SearchResolution = 5;
    }
}
