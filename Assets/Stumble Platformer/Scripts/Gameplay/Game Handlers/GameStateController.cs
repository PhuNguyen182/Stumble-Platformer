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

        public GameStateController()
        {
            _gameStateMachine = new StateMachine<State, Trigger>(State.Start);
            _endGameTrigger = _gameStateMachine.SetTriggerParameters<EndResult>(Trigger.EndGame);

            _gameStateMachine.Configure(State.Start)
                             .OnEntry(PlayerStart)
                             .Permit(Trigger.Play, State.Playing);

            _gameStateMachine.Configure(State.Playing)
                             .OnEntry(PlayGame)
                             .Permit(Trigger.Finish, State.Finished);

            _gameStateMachine.Configure(State.Finished)
                             .OnEntryFrom(Trigger.Finish, OnFinished)
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

        private void OnFinished()
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

        public void FinishLevel()
        {
            if (_gameStateMachine.CanFire(Trigger.Finish))
            {
                _gameStateMachine.Fire(Trigger.Finish);
            }
        }

        public void EndGame(EndResult result)
        {
            if (_gameStateMachine.CanFire(_endGameTrigger.Trigger))
            {
                _gameStateMachine.Fire(_endGameTrigger, result);
            }
        }
    }
}
