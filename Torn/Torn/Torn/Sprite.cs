﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Torn
{
    class Sprite : DrawableGameComponent
    {
        protected String textureFile;
        protected Texture2D texture;
        protected Vector2 position;
        protected Vector2 center;
        protected Vector2 center2;
        protected Color color;
        protected Random random;
        protected Rectangle rectangle;

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public float X
        {
            get { return position.X; }
            set { position.X = value; }
        }

        public float Y
        {
            get { return position.Y; }
            set { position.Y = value; }
        }

        public Rectangle Rectangle
        {
            get { return rectangle; }
            set { rectangle = value; }
        }

        public Sprite(Game game, String textureFile, Vector2 position)
            : base(game)
        {
            this.textureFile = textureFile;
            this.position = position;
            color = Color.White;
            random = new Random();
            rectangle = new Rectangle(0,0,0,0);
            center2 = new Vector2(MyGlobals.realBlockSize / 2, MyGlobals.realBlockSize / 2);
            LoadContent();
        }

        protected override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>(textureFile);
            center = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
            base.LoadContent();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch sb = ((Game1)this.Game).spriteBatch;

            sb.Begin();
            if(rectangle.Equals(new Rectangle(0,0,0,0)))
                sb.Draw(texture, position, null, color, 0f, center, MyGlobals.scale, SpriteEffects.None, 0f);
            else
                sb.Draw(texture, position, rectangle, color, 0f, center2, MyGlobals.scale, SpriteEffects.None, 0f);
            sb.End();
            

            base.Draw(gameTime);
        }


        public SpriteSave SaveSprite()
        {
            SpriteSave spriteSave = new SpriteSave(textureFile, position, center, color);
            return spriteSave;
        }

        public void LoadSprite(SpriteSave spriteSave)
        {
            textureFile = spriteSave.textureFile;
            position = spriteSave.position;
            center = spriteSave.center;
            color = spriteSave.color;
        }
        
    }
}
