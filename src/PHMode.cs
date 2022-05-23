using System;
using System.Collections.Generic;
using System.Linq;
using DuckGame;

namespace DuckGame.PropHunt
{
    [EditorGroup("PropHunt")]
    public class PHMode : Thing
    {
        public EditorProperty<int> hidingTime = new EditorProperty<int>(val: 15, min: 15, max: 60);
        public EditorProperty<int> huntingTime = new EditorProperty<int>(val: 120, min: 45, max: 620);
        public EditorProperty<int> huntersAmount = new EditorProperty<int>(val: 1, min: 1, max: 8);
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