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
        public Duck localDuck = null;
        public PHDraw()
        {
            instance = this;
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

        private float CalculateScreenXCenter(string text)
        {
            float tWidth = Graphics.GetStringWidth(text);
            float xCenter = MonoMain.screenWidth / 2;
            return xCenter - tWidth / 2;
        }

        public void Draw(GameTime gameTime)
        {
            if (PropHunt.core.IsPHLevel)
            {
                PHData data = PropHunt.core.Data;

                string gameStatus;
                double remainingTime = Math.Round(data.RemainingTime);
                switch (data.Status)
                {
                    case PHGameStatus.CREATED:
                        gameStatus = "CREATED";
                        break;
                    case PHGameStatus.HIDING:
                        gameStatus = "HIDING";
                        break;
                    case PHGameStatus.HUNTING:
                        gameStatus = "HUNTING";
                        break;
                    case PHGameStatus.ENDED:
                        gameStatus = "ENDED";
                        break;
                    default:
                        gameStatus = "?";
                        break;
                }


                Graphics.screen.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Resolution.getTransformationMatrix());

                if (data.Status != PHGameStatus.ENDED)
                {
                    string tStatus = "Status: " + gameStatus;
                    string tTime = "Remaining time: " + remainingTime;
                    Graphics.DrawString(tStatus, new Vec2(CalculateScreenXCenter(tStatus), 50), Color.Coral);
                    Graphics.DrawString(tTime, new Vec2(CalculateScreenXCenter(tTime), 50 + 10), Color.Coral);
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
                    Graphics.DrawFancyString(winner, new Vec2(CalculateScreenXCenter(winner), 70),color);

                }
                Graphics.screen.End();
            }

        }

    }
}