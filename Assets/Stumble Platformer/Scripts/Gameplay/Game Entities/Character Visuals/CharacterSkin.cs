using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace StumblePlatformer.Scripts.Gameplay.GameEntities.CharacterVisuals
{
    [CreateAssetMenu(fileName = "Character Skin", menuName = "Scriptable Objects/Characters/Character Skin")]
    public class CharacterSkin : ScriptableObject
    {
        [PreviewField(80, Alignment = ObjectFieldAlignment.Left)]
        [SerializeField] public Mesh SkinMesh;
        [SerializeField] public Avatar SkinAvatar;
        [SerializeField] public Material[] SkinMaterials;
    }
}
