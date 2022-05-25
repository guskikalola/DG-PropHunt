using System;
using System.Collections.Generic;

namespace DuckGame.PropHunt
{
    [EditorGroup("PropHunt|tools")]
    public class PHHiderTool : PHTool
    {
        public StateBinding _tauntIndexBinding = new StateBinding("_tauntIndex");
        public StateBinding _disguisedBinding = new StateBinding("_disguised");
        public StateBinding _textYBinding = new StateBinding("_textY");
        public float _checkRadius = 7f;
        private Holdable _attachedProp;
        public bool _disguised = false;
        public float _textY = 1f;
        public int _tauntIndex = 0;
        // This is a fix to a bug where, if the prop is solid, you could fly when going ragdoll
        public float _nonSolidTimer = 0f;
        public float nonSolidCooldown = 3f;

        public override Color TeamColor
        {
            get
            {
                return Color.LightBlue;
            }
        }

        public Vec2 PropPosition
        {
            get
            {
                //  + equippedDuck.collisionSize.y - _attachedProp.collisionSize.y +_attachedProp.collisionOffset.y
                return new Vec2(equippedDuck.position.x, (equippedDuck.position.y - equippedDuck.collisionOffset.y));
            }
        }

        public float TextY
        {
            get
            {
                return _textY;
            }
        }

        public bool Disguised
        {
            get
            {
                return _disguised;
            }
        }

        public PHHiderTool(float xval, float yval) : base(xval, yval)
        {
            _sprite.color = TeamColor;
            _teamMarker.color = TeamColor;
            _health = 1;
        }

        public override void Fire()
        {
            base.Fire();
            // Only work when you have you hand empty
            if (equippedDuck.holdObject != null) return;
            // If you are already disguised, undisguise.
            if (_attachedProp != null) UnDisguise();
            else if (_fireTimer <= 0f) // Cooldown timer...
            {
                if (_target != null)
                {
                    _fireTimer = fireCooldown; // Starts the cooldown timer
                    _sprite.SetAnimation("active");

                    DisguiseAsProp((Holdable)_target);
                }
            }
        }

        public override void PickTarget()
        {
            bool found = false;
            IEnumerable<Holdable> props = Level.CheckCircleAll<Holdable>(this.position, _checkRadius);
            foreach (Holdable p in props)
            {
                bool validProp = p != this && p.canPickUp && !(p is IAmADuck) && (p.equippedDuck == null);
                if (validProp)
                {
                    _target = p;
                    found = true;
                    break;
                }
            }
            if (!found || equippedDuck.holdObject != null) _target = null;
        }

        private void DisguiseAsProp(Holdable t)
        {
            equippedDuck.visible = false;
            _attachedProp = t;
            equippedDuck.Fondle(_attachedProp);
            if (_attachedProp.equippedDuck != null) _attachedProp.equippedDuck.Disarm(this.equippedDuck);
            _attachedProp.canPickUp = false;
            _attachedProp.solid = false;
            _attachedProp.enablePhysics = false;
            _disguised = true;
        }

        private void UnDisguise()
        {
            equippedDuck.visible = true;
            if (_attachedProp != null)
            {
                equippedDuck.Fondle(_attachedProp);
                _attachedProp.canPickUp = true;
                _attachedProp.enablePhysics = true;
                _attachedProp.ApplyForce(new Vec2(0, -1f));
                _attachedProp.solid = true;
                if (_attachedProp.equippedDuck != null) _attachedProp.equippedDuck.Disarm(this.equippedDuck);
            }
            _attachedProp = null;
            _disguised = false;
        }

        public override void DrawTeamMates(List<Duck> teamMates)
        {
            foreach (Duck d in teamMates)
            {
                Vec2 pos;
                Equipment eq = d.GetEquipment(GetType());
                PHHiderTool tool = (PHHiderTool)eq;
                pos = tool._attachedProp == null ? d.position : tool._attachedProp.position;
                Graphics.Draw(_teamMarker, pos.x, pos.y - 10f);
            }
        }

        public override void Update()
        {
            base.Update();

            if (_nonSolidTimer > 0f) _nonSolidTimer -= 0.01f;

            if (equippedDuck != null)
            {

                if (equippedDuck.ragdoll != null && _nonSolidTimer <= 0f) _nonSolidTimer = nonSolidCooldown;


                if (_attachedProp != null)
                {
                    if (_nonSolidTimer > 0f) _attachedProp.solid = false;
                    else _attachedProp.solid = true;
                    if (_disguised)
                    {
                        _textY = _attachedProp.position.y;
                    }
                    else
                    {
                        _textY = equippedDuck.position.y;
                    }
                }

                if (PropHunt.core.Data != null)
                {
                    _zoom = PropHunt.core.Data.HidersZoom;
                }
                // Use the RSTICK or LSTICK to switch the taunt
                // Only open for local duck
                bool t1 = Network.isActive && equippedDuck.profile.Equals(DuckNetwork.localProfile);
                bool t2 = !Network.isActive && equippedDuck.team.Equals(Teams.Player1);
                if (!equippedDuck.dead && (t1||t2))
                {
                    if (equippedDuck.inputProfile.Pressed("LSTICK")) 
                    {
                        PreviousTaunt();
                    }
                    else if (equippedDuck.inputProfile.Pressed("RSTICK"))
                    {
                        NextTaunt();
                    }
                }

                if (_attachedProp != null)
                {
                    if (_attachedProp.destroyed) UnDisguise();
                    else
                    {
                        if (equippedDuck.visible) equippedDuck.visible = false;
                        _attachedProp.position = PropPosition;
                    }
                }

            }
        }

        private void NextTaunt()
        {
            _tauntIndex = (_tauntIndex + 1) % (PropHunt.taunts.Count); 
            DevConsole.Log("[PH] Next taunt : " + _tauntIndex);
        }

        private void PreviousTaunt()
        {
            _tauntIndex = (_tauntIndex - 1) % (PropHunt.taunts.Count);
            if (_tauntIndex < 0) _tauntIndex = PropHunt.taunts.Count - 1;
            DevConsole.Log("[PH] Prev taunt : " + _tauntIndex);

        }

        public override void Special()
        {
            base.Special();
            // Taunt
            if (Network.isActive && !equippedDuck.profile.Equals(DuckNetwork.localProfile)) return;
            PHTaunt taunt = PropHunt.taunts[_tauntIndex];
            Send.Message(new NMTaunt(_tauntIndex, equippedDuck.profile.networkIndex));
            taunt.Play();
        }

        public override void Thrown()
        {
            base.Thrown();
            Presto();
        }

    }
}