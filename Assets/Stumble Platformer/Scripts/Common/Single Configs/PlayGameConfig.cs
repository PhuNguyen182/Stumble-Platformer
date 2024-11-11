using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Common.SingleConfigs
{
    public class PlayGameConfig : BaseSingleConfig<PlayGameConfig>
    {
        public override bool IsAvailable => Current != null;

        public string PlayLevelName;
    }
}
