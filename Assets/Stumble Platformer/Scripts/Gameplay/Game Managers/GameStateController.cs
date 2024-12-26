using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Enums;
using Stateless;
using StumblePlatformer.Scripts.UI.Gameplay.MainPanels;

namespace StumblePlatformer.Scripts.Gameplay.GameManagers
{
    public class GameStateController : IDisposable
    {
        private enum State
        {
            Start,
            Playing,
            LevelEnded,
            Watching,
            GameEnded,
            Quit,
        }

        private enum Trigger
        {
            Play,
            EndLevel,
            Watch,
            EndGame,
            Quit
        }

        private CameraHandler _cameraHandler;
        private EnvironmentHandler _environmentHandler;
        private EndGamePanel _endGamePanel;

        // To do: Assign game UIs in this class, use them to control game flow
        private readonly StateMachine<State, Trigger> _gameStateMachine;
        private readonly StateMachine<State, Trigger>.TriggerWithParameters<EndResult> _endLevelTrigger;
        private readonly StateMachine<State, Trigger>.TriggerWithParameters<EndResult> _endGameTrigger;

        public GameStateController(CameraHandler cameraHandler, EnvironmentHandler environmentHandler, EndGamePanel endGamePanel)
        {
            _cameraHandler = cameraHandler;
            _environmentHandler = environmentHandler;
            _endGamePanel = endGamePanel;

            _gameStateMachine = new StateMachine<State, Trigger>(State.Start);
            _endLevelTrigger = _gameStateMachine.SetTriggerParameters<EndResult>(Trigger.EndLevel);
            _endGameTrigger = _gameStateMachine.SetTriggerParameters<EndResult>(Trigger.EndGame);

            _gameStateMachine.Configure(State.Start)
                             .Permit(Trigger.Play, State.Playing)
                             .OnActivate(OnPlayerStart);

            _gameStateMachine.Configure(State.Playing)
                             .Permit(_endLevelTrigger.Trigger, State.LevelEnded)
                             .OnEntry(OnPlayGame);

            _gameStateMachine.Configure(State.Watching)
                             .Permit(_endGameTrigger.Trigger, State.GameEnded)
                             .OnEntry(OnWatching);

            _gameStateMachine.Configure(State.LevelEnded)
                             .Permit(_endGameTrigger.Trigger, State.GameEnded)
                             .OnEntryFrom(_endLevelTrigger, result => OnLevelEnded(result));

            _gameStateMachine.Configure(State.GameEnded)
                             .Permit(Trigger.Quit, State.Quit)
                             .OnEntryFrom(_endGameTrigger, result => OnEndGame(result));

            _gameStateMachine.Configure(State.Quit)
                             .OnEntry(QuitPlay);

            _gameStateMachine.Activate();
        }

        private void OnPlayerStart()
        {
            if (_gameStateMachine.CanFire(Trigger.Play))
            {
                _gameStateMachine.Fire(Trigger.Play);
            }
        }

        private void OnPlayGame()
        {
            Debug.Log("Playing");
        }

        private void OnLevelEnded(EndResult endResult)
        {
            Debug.Log($"Level Ended {endResult}");
            _endGamePanel?.SetLevelEndBannerActive(endResult, true);
        }

        private void OnWatching()
        {
            _endGamePanel?.SetLevelEndBannerActive(EndResult.None, false);
        }

        private void OnEndGame(EndResult result)
        {
            Debug.Log($"End Game {result}");
        }

        private void QuitPlay()
        {

        }

        public void EndLevel(EndResult endResult)
        {
            if (_gameStateMachine.CanFire(_endLevelTrigger.Trigger))
            {
                _gameStateMachine.Fire(_endLevelTrigger, endResult);
            }
        }

        public void EndGame(EndResult result)
        {
            if (_gameStateMachine.CanFire(_endGameTrigger.Trigger))
            {
                _gameStateMachine.Fire(_endGameTrigger, result);
            }
        }

        public void Dispose()
        {
            _gameStateMachine.Deactivate();
        }
    }
}
