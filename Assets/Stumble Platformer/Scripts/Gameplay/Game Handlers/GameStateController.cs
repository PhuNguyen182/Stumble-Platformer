using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StumblePlatformer.Scripts.Common.Enums;
using Stateless;

namespace StumblePlatformer.Scripts.Gameplay.GameHandlers
{
    public class GameStateController
    {
        private enum State
        {
            Start,
            Playing,
            Finished,
            Watching,
            EndGame,
            Quit,
        }

        private enum Trigger
        {
            Play,
            Finish,
            EndGame,
            Watch,
            Quit
        }

        private readonly StateMachine<State, Trigger> _gameStateMachine;
        private readonly StateMachine<State, Trigger>.TriggerWithParameters<EndResult> _endGameTrigger;
        private readonly StateMachine<State, Trigger>.TriggerWithParameters<EndResult> _finishTrigger;

        public GameStateController()
        {
            _gameStateMachine = new StateMachine<State, Trigger>(State.Start);
            _finishTrigger = _gameStateMachine.SetTriggerParameters<EndResult>(Trigger.Finish);
            _endGameTrigger = _gameStateMachine.SetTriggerParameters<EndResult>(Trigger.EndGame);

            _gameStateMachine.Configure(State.Start)
                             .OnEntry(PlayerStart)
                             .Permit(Trigger.Play, State.Playing);

            _gameStateMachine.Configure(State.Playing)
                             .OnEntry(PlayGame)
                             .Permit(_finishTrigger.Trigger, State.Finished);

            _gameStateMachine.Configure(State.Finished)
                             .OnEntryFrom(_finishTrigger, result => OnFinished(result))
                             .Permit(_endGameTrigger.Trigger, State.EndGame)
                             .Permit(Trigger.Watch, State.Watching)
                             .Permit(Trigger.Quit, State.Quit);

            _gameStateMachine.Configure(State.Watching)
                             .OnEntry(OnWatching)
                             .Permit(_endGameTrigger.Trigger, State.EndGame);

            _gameStateMachine.Configure(State.EndGame)
                             .OnEntryFrom(_endGameTrigger, result => OnEndGame(result))
                             .Permit(Trigger.Quit, State.Quit);

            _gameStateMachine.Configure(State.Quit)
                             .OnEntry(QuitPlay);
        }

        private void PlayerStart()
        {

        }

        private void PlayGame()
        {

        }

        private void OnFinished(EndResult result)
        {

        }

        private void OnWatching()
        {

        }

        private void OnEndGame(EndResult result)
        {

        }

        private void QuitPlay()
        {

        }

        public void FinishLevel(EndResult result)
        {
            if (_gameStateMachine.CanFire(_finishTrigger.Trigger))
            {
                _gameStateMachine.Fire(_finishTrigger, result);
            }
        }
    }
}
