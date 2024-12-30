using UnityEditor;

namespace StumblePlatformer.Scripts.GameDatas.Editor.GameDataControllers
{
    public class GameDataController : EditorWindow
    {
        [MenuItem("Game Data/Clear Data")]
        public static void ClearData()
        {
            GameDataManager.Instance.DeleteData();
        }
    }
}
