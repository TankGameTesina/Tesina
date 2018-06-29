using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace provaXNAGame
{
    class Button
    {
        private Texture2D normalState;
        private Texture2D enterState;
        private Texture2D texture;
        private Vector2 position;
        System.Drawing.Size size;
        public int Width;
        public int Height;
        private MouseState mouseState;
        private MouseState previousState;
        Rectangle buttonRectangle;

        public Vector2 Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
            }
        }

        public Button()
        {

        }

        public virtual void Init(Texture2D normalState, Texture2D enterState, Vector2 position, System.Drawing.Size size)
        {
            this.normalState = normalState;
            this.enterState = enterState;
            this.Position = position;
            this.size = size;

            buttonRectangle = new Rectangle((int)position.X, (int)position.Y, size.Width, size.Height);
            texture = normalState;

            Width = normalState.Width;
            Height = normalState.Height;
        }

        public virtual void Init(Texture2D normalState, Texture2D enterState, Vector2 position)
        {
            this.normalState = normalState;
            this.enterState = enterState;
            this.Position = position;
            this.size = new System.Drawing.Size(normalState.Width, normalState.Height);

            buttonRectangle = new Rectangle((int)position.X, (int)position.Y, size.Width, size.Height);
            texture = normalState;

            Width = normalState.Width;
            Height = normalState.Height;
        }

        public void setSize(System.Drawing.Size size)
        {
            this.size = size;
            buttonRectangle = new Rectangle((int)Position.X, (int)Position.Y, size.Width, size.Height);
        }

        public void setPosition(Vector2 position)
        {
            this.Position = position;
            buttonRectangle = new Rectangle((int)position.X, (int)position.Y, size.Width, size.Height);
        }
        public virtual bool Update()
        {
            return MouseClick();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, buttonRectangle, Color.White);
        }

        public bool MouseClick()
        {
            //creates a rectangle of 10x10 around the place where the mouse was clicked
            mouseState = Mouse.GetState();
            Rectangle mouseClickRect = new Rectangle(mouseState.X, mouseState.Y, 10, 10);
                if (mouseClickRect.Intersects(buttonRectangle)) //player clicked start button
                {
                    texture = enterState;
                    if (previousState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed &&
                    mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                    {
                        return true;
                    }
                }
                else
                    texture = normalState;

                previousState = mouseState;

                return false;

        }

    }
}
