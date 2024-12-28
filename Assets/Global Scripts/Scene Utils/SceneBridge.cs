using Cysharp.Threading.Tasks;

namespace GlobalScripts.SceneUtils
{
    public static class SceneBridge
    {
        public static string Bridge;

        /// <summary>
        /// Load target scene via a transition scene
        /// </summary>
        /// <param name="destinationSceneName">Desired scene name to load</param>
        /// <returns></returns>
        public static async UniTask LoadNextScene(string destinationSceneName)
        {
            Bridge = destinationSceneName;
            await SceneLoader.LoadScene(SceneConstants.Transition);
        }
    }
}
