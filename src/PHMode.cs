using System;
using System.Collections.Generic;
using System.Linq;
using DuckGame;

namespace DuckGame.PropHunt
{
    [EditorGroup("PropHunt")]
    public class PHMode : Thing
    {
        public EditorProperty<int> hidingTime = new EditorProperty<int>(val:15, min:5, max:45);
        public EditorProperty<int> huntingTime = new EditorProperty<int>(val:30, min:10,max:240);
        public EditorProperty<int> huntersAmount = new EditorProperty<int>(val:1, min:1, max:8);
        private PHModeConfig _config;
        public PHMode()
        {
            graphic = new Sprite("challengeIcon");
            center = new Vec2(8f, 8f);
            _collisionSize = new Vec2(16f, 16f);
            _collisionOffset = new Vec2(-8f, -8f);
            base.depth = 0.9f;
            base.layer = Layer.Foreground;
            _editorName = "Prop Hunt";
            _canFlip = false;
            _canHaveChance = false;

            _config = new PHModeConfig();

        }

        public PHModeConfig PHConfig 
        {
            get
            {
                return _config;
            }
        }

        public override void EditorPropertyChanged(object property)
        {
            base.EditorPropertyChanged(property);
            if(hidingTime.Equals(property))
            {
                // update hiding time
                _config.HidingTime = hidingTime.value;
            }
            else if(huntersAmount.Equals(property))
            {
                // update the hunters amount
                _config.HuntersAmount = huntersAmount.value;
            }
        }

        public override void Draw()
        {
            if (Level.current is Editor)
            {
                base.Draw();
            }
        }
    }
}