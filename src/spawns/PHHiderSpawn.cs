using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.PropHunt
{
    [EditorGroup("PropHunt|spawns")]
    class PHHiderSpawn : PHSpawn
    {
        public PHHiderSpawn(float xval, float yval) : base(xval, yval)
        {
            this.graphic.color = Color.Blue;

            this.editorTooltip = "Spawn point for hiders";
        }
    }
}
