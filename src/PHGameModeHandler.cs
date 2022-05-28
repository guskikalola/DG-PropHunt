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
    public class PHGameModeHandler : Thing
    {
        private readonly PHMode _config;
        private float _hidingTimer;
        private float _huntingTimer;
        private PHGameStatus _status = PHGameStatus.CREATED;
        private List<Duck> _hunters = new List<Duck>();
        private List<Duck> _hiders = new List<Duck>();

        private int _preAliveHunters = 0;
        private int _preAliveHiders = 0;

        public List<Duck> Hunters
        {
            get
            {
                return _hunters;
            }
        }
        public List<Duck> Hiders
        {
            get
            {
                return _hiders;
            }
        }
        public PHGameStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                float remainingTime;
                if (Status == PHGameStatus.HIDING) remainingTime = _config.hidingTime.value;
                else if (Status == PHGameStatus.HUNTING) remainingTime = _config.huntingTime.value;
                else remainingTime = 0f;

                Send.Message((NetMessage) new NMChangeStatus(Status, remainingTime));
                PropHunt.core.Data.ChangeStatus(Status, remainingTime);
            }
        }

        public float RemainingTime
        {
            get
            {
                if (Status == PHGameStatus.HIDING) return _hidingTimer;
                else if (Status == PHGameStatus.HUNTING) return _huntingTimer;
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

        public PHGameModeHandler(PHMode phMode) : base()
        {
            _config = phMode;
            _hidingTimer = _config.hidingTime.value;
        }

        public void SelectHunters(int amount)
        {
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
        public override void Initialize()
        {
            base.Initialize();
            InitializeGM();
        }
        private void InitializeGM()
        {
            DevConsole.Log("[PH] Initializing...");
            SelectHunters(_config.huntersAmount.value);
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
            Status = PHGameStatus.HIDING;
            _hidingTimer = _config.hidingTime.value;
            DevConsole.Log("[PH] Hiding...");
        }

        private void OnStartHunting()
        {
            Send.Message((NetMessage)new NMChangeZoom(_config.hidersZoom, _config.huntersZoom));
            Status = PHGameStatus.HUNTING;
            MakeHuntersMortalAndUnFreeze();
            _huntingTimer = _config.huntingTime.value;
            DevConsole.Log("[PH] Hunting...");
        }

        private void OnRoundEnded()
        {
            if (Status == PHGameStatus.ENDED) return;
            Status = PHGameStatus.ENDED;
            int huntersAlive = HuntersAlive;
            int hidersAlive = HidersAlive;
            DevConsole.Log(String.Format("[PH] Round ended. Hiders {0} | Hunters {1}", hidersAlive, huntersAlive));
            if(HidersAlive == 0)
            {
                // Hunters WIN
                PropHunt.core.Data.winner = 0;
                Send.Message((NetMessage)new NMRoundEnded(0));
            }
            else if(HuntersAlive == 0 || RemainingTime <= 0f)
            {
                // Hiders WIN
                PropHunt.core.Data.winner = 1;
                Send.Message((NetMessage)new NMRoundEnded(1));
            }

            List<Team> teams = Teams.allRandomized;
            foreach (Team team in teams)
            {
                List<Profile> profiles = team.activeProfiles;
                foreach (Profile profile in profiles)
                {
                    Duck d = profile.duck;
                    if(d != null && !d.dead)
                    {
                        EnergyScimitar weapon = new EnergyScimitar(d.position.x, d.position.y);
                        Level.Add(weapon);
                    }
                }
            }
        }

        private void MakeHuntersInvincibleAndFreeze()
        {
            foreach (Duck hunter in _hunters)
            {
                hunter.moveLock = true;
                hunter.invincible = true;
                Send.Message((NetMessage)new NMMakeHuntersInvincibleAndFreeze(hunter));
            }
        }

        private void MakeHuntersMortalAndUnFreeze()
        {
            foreach (Duck hunter in _hunters)
            {
                hunter.moveLock = false;
                hunter.invincible = false;
                Send.Message((NetMessage)new NMMakeHuntersMortalAndUnFreeze(hunter));
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
            PropHunt.core.Data.ChangeZoom(_config.hidersZoom, _config.huntersZoom);
            Send.Message((NetMessage)new NMChangeZoom(_config.hidersZoom, _config.huntersZoom));
        }

        public void UpdateAliveDucks()
        {
            if(_preAliveHiders != HidersAlive || _preAliveHunters != HuntersAlive)
            {
                _preAliveHiders = HidersAlive;
                _preAliveHunters = HuntersAlive;
                PropHunt.core.Data.UpdateAliveDucks(HidersAlive, HuntersAlive);
                Send.Message(new NMUpdateAlive(HuntersAlive, HidersAlive));
            }

            if (HuntersAlive == 0 || HidersAlive == 0)
            {
                InvokeRoundEnded();
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
            if(!DuckNetwork.allClientsReady) 
            {
                return;
            }
            switch (Status)
            {
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
            UpdateAliveDucks();
        }

        public override void Draw()
        {
            base.Draw();
        }

    }
}