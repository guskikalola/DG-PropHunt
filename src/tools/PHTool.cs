using System;
using System.Collections.Generic;

namespace DuckGame.PropHunt
{
    public abstract class PHTool : Equipment
    {
        public StateBinding _fireTimerBinding = new StateBinding("_fireTimer");
        protected SpriteMap _sprite;
        protected SpriteMap _teamMarker;
        private bool _fired;
        protected float _fireTimer = 0f;
        public bool canFire = true;

        public float fireCooldown = 4f;

        protected float animationSpeed = 0.3f;

        public PHTool(float xval, float yval) : base(xval, yval)
        {

            _teamMarker = new SpriteMap(GetPath("sprites/teamMarker"), 16, 16);
            _teamMarker.AddAnimation("default", 0.1f, true, 0, 1, 2, 3, 4, 5);
            _teamMarker.SetAnimation("default");
            _teamMarker.CenterOrigin();
            _teamMarker.scale *= 0.3f;

            _sprite = new SpriteMap(GetPath("sprites/PHTool"), 16, 16);
            _sprite.AddAnimation("idle", 1, false, 0);
            _sprite.AddAnimation("active", animationSpeed, false, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 0);
            _sprite.SetAnimation("idle");
            _sprite.CenterOrigin();
            _sprite.scale *= 5f;

            graphic = (Sprite)_sprite;

            destructive = false;

            collisionOffset = new Vec2(-6f, -4f);
            collisionSize = new Vec2(11f, 8f);
            _equippedCollisionOffset = new Vec2(-7f, -5f);
            _equippedCollisionSize = new Vec2(12f, 11f);
            _hasEquippedCollision = false;
            center = new Vec2(8f, 8f);
            physicsMaterial = PhysicsMaterial.Metal;


            _fired = false;

        }

        public override void Update()
        {
            base.Update();
            if (PHDraw.instance != null)
            {
                if (PHDraw.instance.localDuck != this.equippedDuck) PHDraw.instance.localDuck = this.equippedDuck;
            }
            if (_fireTimer > 0f)
            {
                _fireTimer -= 0.01f;
            }
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
            _sprite.SetAnimation("idle");
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
                        if (this is PHHiderTool && d != null)
                        {
                            if (d.GetEquipment(typeof(PHHiderTool)) != null)
                            {
                                teamMates.Add(d);
                            }
                        }
                        else if (this is PHHunterTool && d != null)
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
            if (!Network.isActive) return;

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
                    if (d != null)
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