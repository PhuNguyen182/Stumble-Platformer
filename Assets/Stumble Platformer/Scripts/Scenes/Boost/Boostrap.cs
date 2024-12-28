using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalScripts.SceneUtils;
using Cysharp.Threading.Tasks;

namespace StumblePlatformer.Scripts.Scenes.Boost
{
    public class Boostrap : MonoBehaviour
    {
        private void Start()
        {
            SceneLoader.LoadScene(SceneConstants.Loading).Forget();
        }
    }
}
