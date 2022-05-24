using System;
using System.Collections.Generic;

namespace DuckGame.PropHunt
{
    public abstract class PHTool : Equipment
    {
        public StateBinding _specialTimerBinding = new StateBinding("_specialTimer");
        public StateBinding _fireTimerBinding = new StateBinding("_fireTimer");
        public StateBinding _zoomBinding = new StateBinding("_zoom");
        protected SpriteMap _sprite;
        protected SpriteMap _teamMarker;
        protected BitmapFont _targetFont;

        public Thing _target;

        private bool _fired;
        protected float _fireTimer = 0f;
        public bool canFire = true;
        public float fireCooldown = 4f;

        private bool _special;
        public float specialCooldown = 6f;
        protected float _specialTimer = 0f;

        protected float animationSpeed = 0.3f;

        public float _zoom = 1f;

        protected int _health;

        public int Health
        {
            get
            {
                return _health;
            }
        }

        public virtual Color TeamColor
        {
            get
            {
                return Color.White;
            }
        }

        public PHTool(float xval, float yval) : base(xval, yval)
        {
            _teamMarker = new SpriteMap(GetPath("sprites/teamMarker"), 16, 16);
            _teamMarker.AddAnimation("default", 0.1f, true, 0, 1, 2, 3, 4, 5);
            _teamMarker.SetAnimation("default");
            _teamMarker.CenterOrigin();
            _teamMarker.scale *= 0.3f;

            _targetFont = new BitmapFont("smallFont", 4, 3);

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
            _special = false;
        }

        public override void Update()
        {
            base.Update();
            if (_fireTimer > 0f) _fireTimer -= 0.01f;
            if (_specialTimer > 0f) _specialTimer -= 0.01f;

            if (equippedDuck != null)
            {
                InputProfile input = equippedDuck.inputProfile;
                if (input == null) return;
                bool fire = input.Down("SHOOT");
                if (fire && !_fired)
                {
                    _fired = true;
                    if (canFire)
                    {
                        Fire();
                    }
                }
                if (!fire && _fired) _fired = false;

                bool specialI = input.Down("QUACK");
                if(specialI && !_special && _specialTimer <= 0f)
                {
                    _special = true;
                    _specialTimer = specialCooldown;
                    Special();
                }

                if (!specialI && _special) _special = false;

                PickTarget();
                ReduceVision();

                bool t1 = Network.isActive && DuckNetwork.localProfile.duck.Equals(equippedDuck);
                bool t2 = !Network.isActive && equippedDuck.team.Equals(Teams.Player1);

                if ((t1 || t2) && PropHunt.core.Data != null && PropHunt.core.Data.Tool == null) PropHunt.core.Data.Tool = this;
            }
        }

        public virtual void Fire()
        {
            _sprite.SetAnimation("idle");
        }

        public virtual void Special()
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
                return (equippedDuck.ragdoll != null ? equippedDuck.ragdoll.position : equippedDuck.position) - CameraSize * 0.5f;
            }
        }

        public virtual Vec2 CameraSize
        {
            get
            {
                return new Vec2(350f / _zoom, (350f / _zoom) * DuckGame.Graphics.aspect);
            }
        }

        public virtual void DrawTeamMates(List<Duck> teamMates)
        {
        }

        public abstract void PickTarget();

        public virtual string TargetText
        {
            get
            {
                return "@SHOOT@";
            }
        }

        public virtual float TargetMarkerRadius
        {
            get
            {
                return 6f;
            }
        }

        public virtual void DrawTarget()
        {
            if (_fireTimer > 0f)
            {
                string text = _fireTimer.ToString("0.00") + "s";
                Graphics.DrawString(text, equippedDuck.position - new Vec2(Graphics.GetStringWidth(text, scale: 0.5f) * 0.5f, 15f), TeamColor, scale: 0.5f);
            }
            else if (_target != null)
            {
                Vec2 tPosition = _target.position;
                if (this is PHHunterTool && _target is Duck)
                {
                    PHHiderTool tool = (PHHiderTool)((Duck)_target).GetEquipment(typeof(PHHiderTool));
                    if (tool != null && tool._disguised == true)
                    {
                        tPosition.y = tool._textY;
                    }
                }
                _targetFont.Draw(TargetText, tPosition - new Vec2(6f, 15f), TeamColor, input: equippedDuck.inputProfile);
            }
        }

        public virtual void ReduceVision()
        {
            if (equippedDuck == null) return;
            if (Network.isActive && DuckNetwork.localProfile.duck == null) return; 

            bool t1 = Network.isActive && DuckNetwork.localProfile.duck.Equals(equippedDuck);
            bool t2 = !Network.isActive && equippedDuck.team.Equals(Teams.Player1);
            bool t3 = equippedDuck != null && Level.current.camera != null;

            if (!t3) return;
            if (!t1 && !t2) return;


            Level.current.camera.position = CameraPosition;
            Level.current.camera.size = CameraSize;

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
            if (PropHunt.core.IsPHLevel && isLocal && equippedDuck != null)
            {
                DoDrawTeamMates();
                DrawTarget();
            }
        }


    }
}