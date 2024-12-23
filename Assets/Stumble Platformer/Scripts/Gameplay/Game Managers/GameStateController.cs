using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Enums;
using Stateless;

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

        private readonly StateMachine<State, Trigger> _gameStateMachine;
        private readonly StateMachine<State, Trigger>.TriggerWithParameters<EndResult> _endLevelTrigger;
        private readonly StateMachine<State, Trigger>.TriggerWithParameters<EndResult> _endGameTrigger;

        public GameStateController()
        {
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
        }

        private void OnWatching()
        {

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
