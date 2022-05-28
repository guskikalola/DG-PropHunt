using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DuckGame;

namespace DuckGame.PropHunt
{
    public class PHDraw : IDrawable
    {
        public static PHDraw instance = null;

        private Sprite GUI_tauntMenu;
        private Sprite GUI_status;

        private Depth GUI_statusDepth = (Depth)0.8f;
        private Depth GUI_tauntMenuDepth = (Depth)0.8f;

        protected BitmapFont _tauntFount;

        public PHDraw()
        {
            instance = this;

            // GUI Sprites

            _tauntFount = new BitmapFont("smallFont", 8);

            GUI_tauntMenu = new Sprite(Mod.GetPath<PropHunt>("sprites/GUI/tauntMenu"), 34, 96)
            {
                color = Color.Crimson,
                depth = GUI_tauntMenuDepth
            };

            GUI_tauntMenu.CenterOrigin();

            GUI_status = new Sprite(Mod.GetPath<PropHunt>("sprites/GUI/status"), 128, 36)
            {
                color = Color.BurlyWood,
                depth = GUI_statusDepth
            };

            GUI_status.CenterOrigin();

        }

        public bool Visible
        {
            get
            {
                return true;
            }
        }

        public int DrawOrder
        {
            get
            {
                return 1;
            }
        }

        public event EventHandler<EventArgs> VisibleChanged;
        public event EventHandler<EventArgs> DrawOrderChanged;

        protected virtual void OnVisibleChanged(EventArgs e)
        {
            // Just to shut the warning
            VisibleChanged.ToString();
        }

        protected virtual void OnDrawOrderChanged(EventArgs e)
        {
            // Just to shut the warning
            DrawOrderChanged.ToString();
        }

        public static float CalculateScreenXCenter(string text, float scale)
        {
            float tWidth = Graphics.GetStringWidth(text, scale: scale);
            float xCenter = MonoMain.screenWidth / 2;
            return xCenter - tWidth / 2;
        }

        public static float CalculateScreenXCenter(string text)
        {
            return CalculateScreenXCenter(text, 1f);
        }

        public void DrawStatus(PHData data)
        {
            string gameStatus;
            Color statusColor = Color.Coral;
            double remainingTime = data.RemainingTime;
            switch (data.Status)
            {
                case PHGameStatus.CREATED:
                    gameStatus = "CREATED";
                    break;
                case PHGameStatus.HIDING:
                    gameStatus = "HIDING";
                    statusColor = Color.LightBlue;
                    break;
                case PHGameStatus.HUNTING:
                    gameStatus = "HUNTING";
                    statusColor = Color.Red;
                    break;
                case PHGameStatus.ENDED:
                    gameStatus = "ENDED";
                    break;
                default:
                    gameStatus = "?";
                    break;
            }

            Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Resolution.getTransformationMatrix());
            if (data.Status != PHGameStatus.ENDED)
                Graphics.Draw(GUI_status, MonoMain.screenWidth / 2, GUI_status.h + 10f, scaleX: 4f, scaleY: 4f);
            Graphics.screen.End();


            Graphics.screen.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Resolution.getTransformationMatrix());

            if (data.Status != PHGameStatus.ENDED)
            {
                string tStatus = gameStatus;
                string tTime = remainingTime.ToString("0.0") + "s";
                string huntersAlive = data.HuntersAlive.ToString();
                string hidersAlive = data.HidersAlive.ToString();

                float scale = 2f;

                float statusY = 46f;
                float timeY = 70f;

                float aliveSeparator = 170f;
                float aliveY = 50f;

                Graphics.DrawString(tStatus, new Vec2(CalculateScreenXCenter(tStatus, scale), statusY), statusColor, scale: 2f, depth: GUI_statusDepth + 1);
                Graphics.DrawString(tTime, new Vec2(CalculateScreenXCenter(tTime, scale), timeY), Color.Coral, scale: 2f, depth: GUI_statusDepth + 1);
                Graphics.DrawString(huntersAlive, new Vec2(CalculateScreenXCenter(huntersAlive, scale) - aliveSeparator, aliveY), Color.Red, scale: 2f, depth: GUI_statusDepth + 1);
                Graphics.DrawString(hidersAlive, new Vec2(CalculateScreenXCenter(hidersAlive, scale) + aliveSeparator + 2f, aliveY), Color.LightBlue, scale: 2f, depth: GUI_statusDepth + 1);
            }

            if (PropHunt.core.Data.Status == PHGameStatus.ENDED)
            {
                string winner;
                Color color;
                switch (PropHunt.core.Data.winner)
                {
                    case 0: // Hunters
                        winner = "HUNTERS";
                        color = Color.Red;
                        break;
                    case 1: // Hiders
                        winner = "HIDERS";
                        color = Color.Blue;
                        break;
                    default:
                        winner = "?";
                        color = Color.SaddleBrown;
                        break;
                }
                winner += " WIN!";
                Graphics.DrawFancyString(winner, new Vec2(CalculateScreenXCenter(winner), 70), color, scale: 2f);
            }
            Graphics.screen.End();

        }

        public void DrawHUD(PHData data)
        {
            if (data.Tool == null) return;

            // Taunt menu
            if (data.Tool is PHHiderTool)
            {
                PHHiderTool hiderTool = (PHHiderTool)data.Tool;
                PHTaunt previousTaunt = PropHunt.taunts[Math.Abs((hiderTool._tauntIndex - 1)) % PropHunt.taunts.Count];
                PHTaunt currentTaunt = PropHunt.taunts[hiderTool._tauntIndex];
                PHTaunt nextTaunt = PropHunt.taunts[(hiderTool._tauntIndex + 1) % PropHunt.taunts.Count];

                // Taunt icon
                float tauntsGap = 2f;

                float othersScale = 1.7f;
                float currentScale = 2f;

                Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Resolution.getTransformationMatrix());

                previousTaunt.Icon.depth = GUI_tauntMenuDepth - 1;
                currentTaunt.Icon.depth = GUI_tauntMenuDepth - 1;
                nextTaunt.Icon.depth = GUI_tauntMenuDepth - 1;


                Graphics.Draw(previousTaunt.Icon, MonoMain.screenWidth - nextTaunt.Icon.width, MonoMain.screenHeight * 0.5f - currentTaunt.Icon.height * currentScale - tauntsGap, scaleX: othersScale, scaleY: othersScale);

                Graphics.Draw(currentTaunt.Icon, MonoMain.screenWidth - nextTaunt.Icon.width, MonoMain.screenHeight * 0.5f, scaleX: currentScale, scaleY: currentScale);

                Graphics.Draw(nextTaunt.Icon, MonoMain.screenWidth - nextTaunt.Icon.width, MonoMain.screenHeight * 0.5f + currentTaunt.Icon.height * currentScale + tauntsGap, scaleX: othersScale, scaleY: othersScale);

                Graphics.screen.End();

                // Taunt menu gui
                Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Resolution.getTransformationMatrix());
                Graphics.Draw(GUI_tauntMenu, MonoMain.screenWidth - GUI_tauntMenu.width, MonoMain.screenHeight * 0.5f, scaleX: 2f, scaleY: 2f);
                Graphics.screen.End();

                Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Resolution.getTransformationMatrix());
                Vec2 textPos = new Vec2(MonoMain.screenWidth - GUI_tauntMenu.width - 60f, MonoMain.screenHeight * 0.5f);

                InputProfile input = Network.isActive ? DuckNetwork.localProfile.inputProfile : Profiles.DefaultPlayer1.inputProfile;

                _tauntFount.Draw("@LSTICK@", textPos + new Vec2(55f,-110f), data.Tool.TeamColor, GUI_tauntMenuDepth + 1, input);
                _tauntFount.Draw("@RSTICK@", textPos + new Vec2(55f,110f), data.Tool.TeamColor, GUI_tauntMenuDepth + 1, input);

                string cooldownText = data.Tool.SpecialTimer.ToString("0.00") + "s";
                float cooldownOffset = Graphics.GetStringWidth(cooldownText);
                if (data.Tool.SpecialTimer <= 0f) _tauntFount.Draw("@QUACK@", textPos, data.Tool.TeamColor, GUI_tauntMenuDepth + 1, input);
                else Graphics.DrawString(cooldownText, textPos + new Vec2(-cooldownOffset, 0), data.Tool.TeamColor, depth: GUI_tauntMenuDepth + 1);
                Graphics.screen.End();

            }

            // Health
            Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Resolution.getTransformationMatrix());
            string health = "Health: " + data.Tool.Health;
            float healthScale = 2f;
            Graphics.DrawFancyString(health, new Vec2(CalculateScreenXCenter(health, healthScale), MonoMain.screenHeight - 20f), data.Tool.TeamColor, scale: healthScale);
            Graphics.screen.End();

        }

        public void Draw(GameTime gameTime)
        {
            if (PropHunt.core.IsPHLevel && PropHunt.core.Data != null)
            {
                PHData data = PropHunt.core.Data;
                DrawHUD(data);
                DrawStatus(data);
            }

        }

    }
}