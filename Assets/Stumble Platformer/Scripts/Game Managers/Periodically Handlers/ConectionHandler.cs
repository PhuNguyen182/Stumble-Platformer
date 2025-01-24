using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalScripts.SceneUtils;
using StumblePlatformer.Scripts.Common.Constants;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace StumblePlatformer.Scripts.GameManagers.PeriodicallyHandlers
{
    public class ConectionHandler : MonoBehaviour
    {
        public async UniTask<bool> CheckConnection()
        {
            bool isConnected = await IsInternetConnected();
            if (!isConnected)
                await ShowConnectionPopup();

            return isConnected;
        }

        private async UniTask ShowConnectionPopup()
        {
            ConfirmPopup confirmPopup = await ConfirmPopup.CreateFromAddress(CommonPopupPaths.ConfirmPopupPath);
            confirmPopup.AddMessageOK("Notice!", "Disconnected! Check you internet connection again!", BackMainHome)
                        .SetCanvasMode(true).ShowCloseButton(true);
        }

        private async UniTask<bool> IsInternetConnected()
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get("1.1.1.1"))
            {
                bool isConnected = false;
                await webRequest.SendWebRequest();

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.ProtocolError:
                    case UnityWebRequest.Result.DataProcessingError:
                        isConnected = false;
                        break;
                    case UnityWebRequest.Result.Success:
                        isConnected = true;
                        break;
                }

                return isConnected;
            }
        }

        private void BackMainHome()
        {
            OnBackHome().Forget();
        }

        private async UniTask OnBackHome()
        {
            string currentScene = SceneManager.GetActiveScene().name;
            if (string.CompareOrdinal(currentScene, SceneConstants.Mainhome) != 0)
            {
                WaitingPopup.Setup().ShowWaiting();
                await SceneLoader.LoadScene(SceneConstants.Mainhome);
            }
        }
    }
}
