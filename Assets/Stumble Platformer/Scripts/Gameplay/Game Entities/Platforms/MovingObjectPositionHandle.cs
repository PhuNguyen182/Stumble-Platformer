using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Platforms
{
    [CustomEditor(typeof(MultiplePointsMovingPlatform)), CanEditMultipleObjects]
    public class MovingObjectPositionHandle : Editor
    {
        protected virtual void OnSceneGUI()
        {
            MultiplePointsMovingPlatform movingPlatform = (MultiplePointsMovingPlatform)target;
            int positionCount = movingPlatform.Positions.Length;
            Vector3[] positions = new Vector3[positionCount];

            EditorGUI.BeginChangeCheck();
            if (positionCount <= 0)
                return;

            for (int i = 0; i < positionCount; i++)
            {
                positions[i] = Handles.PositionHandle(movingPlatform.Positions[i], Quaternion.identity);
            }

            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < positionCount; i++)
                {
                    movingPlatform.Positions[i] = positions[i];
                }
            }
        }
    }
}
#endif
