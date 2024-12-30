using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Enums;

namespace StumblePlatformer.Scripts.UI.Gameplay.MainPanels
{
    public class EndGamePanel : MonoBehaviour
    {
        [SerializeField] private GameObject winObject;
        [SerializeField] private GameObject loseObject;
        [SerializeField] private GameObject roundOverObject;

        public void SetLevelEndBannerActive(EndResult endResult, bool active)
        {
            switch (endResult)
            {
                case EndResult.None:
                    SetWinBannerActive(false);
                    SetLoseBannerActive(false);
                    break;
                case EndResult.Win:
                    SetWinBannerActive(active);
                    break;
                case EndResult.Lose:
                    SetLoseBannerActive(active);
                    break;
            }
        }

        private void SetWinBannerActive(bool active)
        {
            winObject.SetActive(active);
        }

        private void SetLoseBannerActive(bool active)
        {
            loseObject.SetActive(active);
        }
    }
}
