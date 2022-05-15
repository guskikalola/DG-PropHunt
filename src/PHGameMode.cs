using System;
using System.Collections.Generic;

namespace DuckGame.PropHunt
{
    public enum PHGameStatus
    {
        CREATED,
        HIDING,
        HUNTING,
        ENDED
    }
    public class PHGameMode : Thing
    {
        private readonly PHModeConfig _config;
        private float _hidingTimer;
        private float _huntingTimer;
        private PHGameStatus _status = PHGameStatus.CREATED;
        private List<Duck> _hunters;
        private List<Duck> _hiders;
        private bool _gmInitialized = false;

        public List<Duck> Hunters { get => _hunters; }
        public List<Duck> Hiders { get => _hiders; }
        public PHGameStatus Status { get => _status; }

        public float RemainingTime
        {
            get
            {
                if (_status == PHGameStatus.HIDING) return _hidingTimer;
                else if (_status == PHGameStatus.HUNTING) return _huntingTimer;
                else return 0f;
            }
        }

        public int HuntersAlive
        {
            get
            {
                int count = Hunters.Count;
                foreach (Duck hunter in Hunters)
                {
                    if (hunter.dead) count--;
                }
                return count;
            }
        }

        public int HidersAlive
        {
            get
            {
                int count = Hiders.Count;
                foreach (Duck hider in Hiders)
                {
                    if (hider.dead) count--;
                }
                return count;
            }
        }

        public PHGameMode(PHMode phMode) : base()
        {
            _config = phMode.PHConfig;
            _hidingTimer = _config.HidingTime;
        }

        public void SelectHunters(int amount)
        {
            _hunters = new List<Duck>();
            _hiders = new List<Duck>();
            List<Team> teams = Teams.allRandomized;
            foreach (Team team in teams)
            {
                List<Profile> profiles = team.activeProfiles;
                foreach (Profile profile in profiles)
                {
                    Duck d = profile.duck;
                    if (_hunters.Count < amount) _hunters.Add(d);
                    else _hiders.Add(d);
                }
            }

        }

        private void InitializeGM()
        {
            DevConsole.Log("[PH] Configuration " + _config.ToString());
            _gmInitialized = true;
            DevConsole.Log("[PH] Initializing...");
            SelectHunters(_config.HuntersAmount);
            InvokeStartHiding();
        }

        private void InvokeStartHiding()
        {
            OnStartHiding();
        }


        private void InvokeStartHunting()
        {
            OnStartHunting();
        }

        private void InvokeRoundEnded()
        {
            OnRoundEnded();
        }

        private void OnStartHiding()
        {
            GiveWeapons();
            MakeHuntersInvincibleAndFreeze();
            _status = PHGameStatus.HIDING;
            _hidingTimer = _config.HidingTime;
            DevConsole.Log("[PH] Hiding...");
        }

        private void OnStartHunting()
        {
            _status = PHGameStatus.HUNTING;
            MakeHuntersMortalAndUnFreeze();
            _huntingTimer = _config.HidingTime;
            DevConsole.Log("[PH] Hunting...");
        }

        private void OnRoundEnded()
        {
            _status = PHGameStatus.ENDED;
            int huntersAlive = HuntersAlive;
            int hidersAlive = HidersAlive;
            DevConsole.Log(String.Format("[PH] Round ended. Hiders {0} | Hunters {1}", hidersAlive, huntersAlive));
        }

        private void MakeHuntersInvincibleAndFreeze()
        {
            foreach (Duck hunter in _hunters)
            {
                hunter.moveLock = true;
                hunter.invincible = true;
            }
        }

        private void MakeHuntersMortalAndUnFreeze()
        {
            foreach (Duck hunter in _hunters)
            {
                hunter.moveLock = false;
                hunter.invincible = false;
            }
        }

        private void GiveWeapons()
        {
            foreach (Duck hider in _hiders)
            {
                PHHiderTool tool = new PHHiderTool(hider.position.x, hider.position.y);
                Level.Add(tool);
                hider.Equip(tool);
            }
            foreach (Duck hunter in _hunters)
            {
                PHHunterTool tool = new PHHunterTool(hunter.position.x, hunter.position.y);
                Level.Add(tool);
                hunter.Equip(tool);
            }
        }

        public bool IsHunter(Duck duck)
        {
            if(duck == null) return false;
            foreach (Duck d in Hunters)
            {
                if (d.Equals(duck)) return true;
            }
            return false;
        }
        public bool IsHider(Duck duck)
        {
            if(duck == null) return false;
            foreach (Duck d in Hiders)
            {
                if (d.Equals(duck)) return true;
            }
            return false;
        }
        public override void Update()
        {
            base.Update();
            switch (_status)
            {
                case PHGameStatus.CREATED:
                    if (!_gmInitialized) InitializeGM();
                    break;
                case PHGameStatus.HIDING:
                    if (_hidingTimer > 0f)
                    {
                        _hidingTimer -= 0.01f;
                    }
                    else
                    {
                        InvokeStartHunting();
                    }
                    break;
                case PHGameStatus.HUNTING:
                    if (_huntingTimer > 0f)
                    {
                        _huntingTimer -= 0.01f;
                    }
                    else
                    {
                        InvokeRoundEnded();
                    }
                    break;
            }
        }

        public override void Draw()
        {
            base.Draw();
        }

    }
}