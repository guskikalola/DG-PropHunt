using System;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using DuckGame;

namespace DuckGame.PropHunt
{
    public class PHCore : Thing, IEngineUpdatable
    {
        private Level _prevLevel;
        private PHGameMode _gamemode;
        private bool _isPHLevel;

        public bool IsPHLevel { get => _isPHLevel;}
        public PHGameMode Gamemode { get => _gamemode; }

        public PHCore()
        {
            MonoMain.RegisterEngineUpdatable(this);
            (typeof(Game).GetField("drawableComponents", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).GetValue(MonoMain.instance) as List<IDrawable>).Add(new PHDraw());
        }

        private void InvokeLevelChanged(Level level)
        {
            OnLevelChanged(level);
        }

        public void OnLevelChanged(Level level)
        {
            DevConsole.Log("[PH] Level changed to " + level.level);
            DestroyPreviousGM();
            PHMode phMode = GetLevelPHMode(level);
            if (phMode != null)
            {
                DevConsole.Log("[PH] Current level is a PH level");
                _isPHLevel = true;
                CreateNewGM(phMode);
            }
            else
            {
                DevConsole.Log("[PH] Current level is NOT a PH level");
                _isPHLevel = false;
            }
        }

        private void CreateNewGM(PHMode phMode)
        {
            _gamemode = new PHGameMode(phMode);
            Level.current.AddThing(_gamemode);
        }

        private void DestroyPreviousGM()
        {
            if (_gamemode != null)
            {
                _gamemode.active = false;
                _gamemode = null;
            }
        }

        public PHMode GetLevelPHMode(Level level)
        {
            foreach (Thing t in level.things)
            {
                if (t is PHMode mode) return mode;
            }
            return null;
        }

        public void PreUpdate()
        {
        }

        public override void Update()
        {
            base.Update();
            if (Level.current != _prevLevel && Level.current.things.Count > 0)
            {
                // Level exists ( Duck in level )
                if (Level.current is GameLevel || Level.current is DuckGameTestArea)
                {
                    _prevLevel = Level.current;
                    InvokeLevelChanged(Level.current);
                }
            }
        }

        public void PostUpdate()
        {

        }

        public void OnDrawLayer(Layer pLayer)
        {
        }
    }
}