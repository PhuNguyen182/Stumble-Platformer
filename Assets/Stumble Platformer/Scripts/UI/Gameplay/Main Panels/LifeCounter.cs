using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace StumblePlatformer.Scripts.UI.Gameplay.MainPanels
{
    public class LifeCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text lifeText;

        public void UpdateLife(int life)
        {
            lifeText.text = $"{life}";
        }
    }
}
