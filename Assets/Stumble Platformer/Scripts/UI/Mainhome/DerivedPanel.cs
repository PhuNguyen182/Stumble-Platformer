using UnityEngine;
using StumblePlatformer.Scripts.UI.Mainhome.MainPanels;

namespace StumblePlatformer.Scripts.UI.Mainhome
{
    public class DerivedPanel : BasePanel
    {
        public void BackToMain()
        {
            ExitPanel();
            MainhomeManager.Instance.MainPanel.EnterPanel();
        }
    }
}
