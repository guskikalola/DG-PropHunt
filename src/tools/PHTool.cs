using System;
using System.Collections.Generic;

namespace DuckGame.PropHunt
{
    public abstract class PHTool : Equipment
    {
        public StateBinding _fireTimerBinding = new StateBinding("_fireTimer");
        private Sprite _sprite;
        private bool _fired;
        protected float _fireTimer = 0f;
        public bool canFire = true;

        public float fireCooldown = 4f;


        public PHTool(float xval, float yval) : base(xval, yval)
        {
            _sprite = new Sprite("chestPlatePickup");
            _sprite.CenterOrigin();
            //graphic = _sprite;

            destructive = false;
            _fired = false;

        }

        public override void Update()
        {
            base.Update();
            if (PHDraw.instance != null)
            {
                if (PHDraw.instance.localDuck != this.equippedDuck) PHDraw.instance.localDuck = this.equippedDuck;
            }
            if (_fireTimer > 0f) _fireTimer -= 0.01f;
            if (this.equippedDuck != null)
            {
                InputProfile input = this.equippedDuck.inputProfile;
                bool fire = input.Down("SHOOT");
                if (fire && !_fired)
                {
                    _fired = true;
                    if (canFire)
                    {
                        if (equippedDuck != null) Fire();
                    }
                }
                if (!fire && _fired) _fired = false;
            }
            ReduceVision();
        }

        public virtual void Fire()
        {

        }

        public override void Thrown()
        {
            base.Thrown();
            this.Presto();
        }

        public virtual void DoDrawTeamMates()
        {
            if (PropHunt.core.IsPHLevel)
            {
                List<Duck> teamMates = new List<Duck>();

                List<Team> teams = Teams.active;
                foreach (Team team in teams)
                {
                    List<Profile> profiles = team.activeProfiles;
                    foreach (Profile profile in profiles)
                    {
                        Duck d = profile.duck;
                        if (this is PHHiderTool)
                        {
                            if (d.GetEquipment(typeof(PHHiderTool)) != null)
                            {
                                teamMates.Add(d);
                            }
                        }
                        else if (this is PHHunterTool)
                        {
                            if (d.GetEquipment(typeof(PHHunterTool)) != null)
                            {
                                teamMates.Add(d);
                            }
                        }
                    }
                }

                DrawTeamMates(teamMates);
            }
        }

        public virtual Vec2 CameraPosition
        {
            get
            {
                return equippedDuck.cameraPosition + equippedDuck.collisionOffset * 2 - new Vec2(45f, 7f);
            }
        }

        public virtual Vec2 CameraSize
        {
            get
            {
                return new Vec2(100f, 100f * DuckGame.Graphics.aspect);
            }
        }

        public virtual void DrawTeamMates(List<Duck> teamMates)
        {
        }

        public virtual void ReduceVision()
        {
            if (Level.current.camera != null && DuckNetwork.localProfile.duck.Equals(equippedDuck))
            {
                Level.current.camera.position = CameraPosition;
                Level.current.camera.size = CameraSize;
            }

            // Hide players outside of the camera view
            List<Team> teams = Teams.active;
            foreach (Team team in teams)
            {
                List<Profile> profiles = team.activeProfiles;
                foreach (Profile profile in profiles)
                {
                    Duck d = profile.duck;
                    if(d != null)
                    {
                        if (d.ShouldDrawIcon() && PropHunt.core.Data.Status != PHGameStatus.ENDED)
                        {
                            d.localSpawnVisible = false;
                        }
                        else d.localSpawnVisible = true;
                    }
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
            if (PropHunt.core.IsPHLevel && isLocal)
            {
                DoDrawTeamMates();
            }
        }


    }
}