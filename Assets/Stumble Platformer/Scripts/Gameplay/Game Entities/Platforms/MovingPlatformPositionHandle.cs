using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.Platforms
{
    [CustomEditor(typeof(MovingPlatform)), CanEditMultipleObjects]
    public class MovingPlatformPositionHandle : Editor
    {
        protected virtual void OnSceneGUI()
        {
            MovingPlatform movingPlatform = (MovingPlatform)target;

            EditorGUI.BeginChangeCheck();
            Vector3 newStartPosition = Handles.PositionHandle(movingPlatform.StartPosition, Quaternion.identity);
            Vector3 newEndPosition = Handles.PositionHandle(movingPlatform.EndPosition, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                movingPlatform.StartPosition = newStartPosition;
                movingPlatform.EndPosition = newEndPosition;
            }
        }
    }
}
#endif
