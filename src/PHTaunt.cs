using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.PropHunt
{
    public class PHTaunt
    {
        private string _path;
        private Sprite _icon;
        private int _index;

        public int Index
        {
            get
            {
                return _index;
            }
        }

        public string Path
        {
            get
            {
                return _path;
            }
        }
        
        public Sprite Icon
        {
            get
            {
                return _icon;
            }
        }

        public PHTaunt(string path, int index) : this(path,index, GeneratePlaceHolder())
        {
        }

        public PHTaunt(string path, int index, Sprite icon)
        {
            _path = path;
            _icon = icon;
            _index = index;
            _icon.CenterOrigin();
        }

        public void Play()
        {
            SFX.Play(_path);
            DevConsole.Log(ToString());
        }

        public override string ToString()
        {
            return _path;
        }

        private static Sprite GeneratePlaceHolder()
        {
            Sprite placeHolder = new Sprite(Mod.GetPath<PropHunt>("sprites/tauntPlaceholder"),32,32);
            return placeHolder;
        }
    }
}
