using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;

namespace StumblePlatformer.Scripts.UI.Gameplay.MainPanels
{
    public class PlayGamePanel : MonoBehaviour
    {
        [SerializeField] private CountDown countDown;
        [SerializeField] private GameObject playGameObject;
        [SerializeField] private GameObject levelNameHolder;
        [SerializeField] private GameObject qualifyCount;
        [SerializeField] private TMP_Text levelName;
        [SerializeField] private TMP_Text levelObjective;

        public void ResetCountdown()
        {
            if (countDown)
                countDown.ResetCountdown(false).Forget();
        }

        public async UniTask CountDown()
        {
            if (countDown)
                await countDown.Countdown();
        }

        public void SetPlayObjectActive(bool active)
        {
            playGameObject.SetActive(active);
        }

        public void SetLevelNameActive(bool active)
        {
            levelNameHolder.SetActive(active);
        }

        public void SetQualifyCountActive(bool active)
        {
            qualifyCount.SetActive(active);
        }

        public void SetLevelName(string levelName)
        {
            this.levelName.text = levelName;
        }

        public void SetLevelObjective(string levelObjective)
        {
            this.levelObjective.text = levelObjective;
        }
    }
}