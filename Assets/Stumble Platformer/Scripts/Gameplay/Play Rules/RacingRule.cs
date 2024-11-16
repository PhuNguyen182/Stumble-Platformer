using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StumblePlatformer.Scripts.Gameplay.PlayRules
{
    public class RacingRule : BasePlayRule
    {
        public override void OnPlayerFall()
        {
            Debug.Log("Player Fall In Racing");
        }

        public override void OnPlayerFinish()
        {
            Debug.Log("Race Finish");
        }

        public override void OnPlayerLose()
        {
            Debug.Log("<color=#ff0000>Race Lose Game</color>");
        }

        public override void OnPlayerWin()
        {
            Debug.Log("<color=#00ff00>Race Win Game</color>");
        }
    }
}
