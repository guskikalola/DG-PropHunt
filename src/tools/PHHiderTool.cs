using System;
using System.Collections.Generic;

namespace DuckGame.PropHunt
{
    [EditorGroup("PropHunt|tool")]
    public class PHHiderTool : PHTool
    {
        public float _checkRadius = 7f;
        private Holdable _attachedProp;

        public Vec2 PropPosition
        {
            get
            {
                //  + equippedDuck.collisionSize.y - _attachedProp.collisionSize.y +_attachedProp.collisionOffset.y
                return new Vec2(equippedDuck.position.x, (equippedDuck.position.y - equippedDuck.collisionOffset.y));
            }
        }

        public PHHiderTool(float xval, float yval) : base(xval, yval)
        {
            _sprite.color = Color.Blue;
            _teamMarker.color = Color.LightBlue;
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
                Holdable t = GetNearestProp();
                if (t != null)
                {
                    _fireTimer = fireCooldown; // Starts the cooldown timer
                    _sprite.SetAnimation("active");

                    DisguiseAsProp(t);
                }
            }
        }

        private Holdable GetNearestProp()
        {
            IEnumerable<Holdable> props = Level.CheckCircleAll<Holdable>(this.position, _checkRadius);
            foreach (Holdable p in props)
            {
                bool validProp = p != this && p.canPickUp && !(p is IAmADuck) && (p.equippedDuck == null);
                if (validProp) return p;
            }
            return null;
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

            if (equippedDuck != null)
            {
                // Use the RSTICK or LSTICK to open the taunt menu
                if (!equippedDuck.dead && (equippedDuck.inputProfile.Pressed("RSTICK") || equippedDuck.inputProfile.Pressed("LSTICK")))
                {
                    // Only open for local duck
                    if (equippedDuck.profile.Equals(DuckNetwork.localProfile)) OpenTauntMenu();
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

        private void OpenTauntMenu()
        {
            DevConsole.Log("[PH] Opening taunt menu");
        }

        public override void Thrown()
        {
            base.Thrown();
            Presto();
        }

    }
}