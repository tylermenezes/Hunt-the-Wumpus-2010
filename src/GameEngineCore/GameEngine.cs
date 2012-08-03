using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using HuntTheWumpus.UICore;
using HuntTheWumpus.Utilities;

namespace HuntTheWumpus.GameEngineCore
{
    public class GameEngine
    {

        List<GameState> _gameStates;

        public GameState ActiveState
        {
            get
            {
                GameState active = null;
                for (int i = _gameStates.Count - 1; i >=0; i--)
                    if (!_gameStates[i].PleaseDestroy)
                    {
                        active = _gameStates[i];
                        break;
                    }
                return active;
            }
        }
        internal GameStates.IPlayable GetPlayState()
        {
            GameState active = null;
            for (int i = _gameStates.Count - 1; i >= 0; i--)
                if (_gameStates[i] is GameStates.IPlayable)
                {
                    active = _gameStates[i];
                    break;
                }
            return (GameStates.IPlayable)active;
        }

        public GameState TopState
        {
            get
            {
                return _gameStates[_gameStates.Count - 1];
            }
        }
        private static GameEngine _singleton;
        public static GameEngine Singleton
        {
            get
            {
                if (_singleton == null)
                    _singleton = new GameEngine();
                return _singleton;
            }
        }
        private GameEngine()
        {
            _gameStates = new List<GameState>();
        }

        /// <summary>
        /// Loads GameEngine and sets first GameState
        /// </summary>
        public void Load()
        {
            _gameStates.Add(new GameStates.MainMenuState());
            
            ActiveState.Load();
        }
        
        public void AddState(GameState state)
        {
            _gameStates.Add(state);
        }
        public void AddAndLoad(GameState state)
        {
            _gameStates.Add(state);
            state.Load();
        }

        private void RemoveState(int n)
        {
            _gameStates[n].PleaseDestroy = true;
        }

        private void DestroyAndRemove(int n)
        {
            RemoveState(n);
        }

        public void RemoveState()
        {
            ActiveState.PleaseDestroy = true;           
        }

        public void DestroyAndRemove()
        {            
            RemoveState();
        }

        public void RemoveStates(int n)
        {
            for (int i = 0; i < n; i++)
                RemoveState(_gameStates.Count - i);
        }
        public void DestroyAndRemoveStates(int n)
        {
            for (int i = 1; i <= n; i++)
                DestroyAndRemove(_gameStates.Count - i);
        }
        public bool Update(GameTime time)
        {
            if (_gameStates[_gameStates.Count - 1].PleaseDestroy)
            {
                _gameStates[_gameStates.Count - 1].Destroy();
                _gameStates.RemoveAt(_gameStates.Count - 1);
            }
            if (_gameStates.Count > 0)
                ActiveState.Update(time);
            else return false;
            return true;
        }
        public bool Draw(GameTime time, SpriteBatch spriteBatch)
        {

            if (_gameStates.Count > 0)
            {
                var startDrawingAt = _gameStates.Count - 1;
                for (int i = startDrawingAt; i >= 0; i--)
                {
                    if (!_gameStates[i].Transparent)
                    {
                        startDrawingAt = i;
                        break;
                    }
                }
                for (int i = startDrawingAt; i < _gameStates.Count; i++)
                    _gameStates[i].Draw(time, spriteBatch);
            }
            else
                return false;
            return true;
        }

        public void DestroyAll()
        {
            DestroyAndRemoveStates(
                _gameStates.Count);
        }
    }
}
