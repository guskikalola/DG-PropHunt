using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.PropHunt
{
    [EditorGroup("PropHunt|spawns")]
    class PHHunterSpawn : PHSpawn
    {
        public PHHunterSpawn(float xval, float yval) : base(xval, yval)
        {
            _editorName = "Hunter Spawn";

            this.graphic.color = Color.Red;

            this.editorTooltip = "Spawn point for hunters";
        }
    }
}
