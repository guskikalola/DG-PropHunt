using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;

namespace DuckGame.PropHunt
{
    public class PHCore : Thing, IEngineUpdatable
    {
        private Level _prevLevel;
        private PHGameModeHandler _gmHandler;
        private PHData _data;
        private bool _isPHLevel;

        public bool IsPHLevel
        {
            get
            {
                return _isPHLevel;
            }
        }

        public PHGameModeHandler GMHandler
        {
            get
            {
                return _gmHandler;
            }
        }

        public PHData Data
        {
            get
            {
                return _data;
            }
        }

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
            if (Level.current is GameLevel || Level.current is DuckGameTestArea)
            {
                PHMode phMode = GetLevelPHMode(level);
                if (phMode != null)
                {
                    DevConsole.Log("[PH] Current level is a PH level");
                    CreateNewGM(phMode);
                }
                else
                {
                    DevConsole.Log("[PH] Current level is NOT a PH level");
                }
            }
        }

        private void CreateNewGM(PHMode phMode)
        {
            _isPHLevel = true;
            _data = new PHData();
            if (Network.isServer)
            {
                _gmHandler = new PHGameModeHandler(phMode);
                Level.current.AddThing(_gmHandler);
            }
        }

        private void DestroyPreviousGM()
        {
            _isPHLevel = false;
            _data = null;
            if (_gmHandler != null)
            {
                Level.Remove(_gmHandler);
                _gmHandler.active = false;
                _gmHandler = null;
            }
        }

        public PHMode GetLevelPHMode(Level level)
        {
            foreach (PHMode t in level.things[typeof(PHMode)])
            {
                return t;
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
                _prevLevel = Level.current;
                InvokeLevelChanged(Level.current);
            }
            if (Data != null)
            {
                Data.Update();
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