using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.LevelPlatforms
{
    public class SpawnCharacterArea : MonoBehaviour
    {
        [SerializeField] private Vector3 mainCharacterSpawnPosition;
        [SerializeField] private Vector3[] characterSpawnPositions;
        
        public Vector3 MainCharacterSpawnPosition => mainCharacterSpawnPosition + transform.position;
        public Vector3[] CharacterSpawnPositions => characterSpawnPositions;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(mainCharacterSpawnPosition + transform.position, 0.075f);
            
            if (characterSpawnPositions == null || characterSpawnPositions.Length <= 0)
                return;

            for (int i = 0; i < characterSpawnPositions.Length; i++)
            {
                Vector3 position = characterSpawnPositions[i] + transform.position;
                Gizmos.DrawSphere(position, 0.075f);
            }
        }
#endif
    }
}
