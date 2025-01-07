using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms
{
    public class SpawnCharacterArea : MonoBehaviour
    {
        public Color labelColor = Color.black;
        [SerializeField] private Vector3 mainCharacterSpawnPosition;
        [SerializeField] private Vector3[] characterSpawnPositions;
        
        public Vector3 MainCharacterSpawnPosition => mainCharacterSpawnPosition + transform.position;
        public Vector3[] CharacterSpawnPositions => characterSpawnPositions;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Vector3 drawnPosition = mainCharacterSpawnPosition + transform.position;
            Gizmos.DrawSphere(drawnPosition, 0.075f);
            DrawString("Main", drawnPosition + Vector3.up * 0.5f, labelColor);

            if (characterSpawnPositions == null || characterSpawnPositions.Length <= 0)
                return;

            for (int i = 0; i < characterSpawnPositions.Length; i++)
            {
                Vector3 position = characterSpawnPositions[i] + transform.position;
                DrawString($"{i + 1}", position + Vector3.up * 0.25f, labelColor);
                Gizmos.DrawSphere(position, 0.075f);
            }
        }

        private static void DrawString(string text, Vector3 worldPos, Color? colour = null)
        {
            Handles.BeginGUI();

            if (colour.HasValue)
                GUI.color = colour.Value;

            SceneView view = SceneView.currentDrawingSceneView;

            if (view != null)
            {
                Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);
                Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));

                GUI.Label(new Rect(screenPos.x - (size.x / 2)
                                   , -screenPos.y + view.position.height + 4
                                   , size.x, size.y), text);
                Handles.EndGUI();
            }
        }
#endif
    }
}
