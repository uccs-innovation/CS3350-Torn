using System;
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
    class Text : DrawableGameComponent
    {
        SpriteFont font;
        Vector2 position;
        String fontFile;
        String text;
        Color color;

        public Text(Game game, Vector2 position, String fontFile, String text, Color color) : base(game)
        {
            this.position = position;
            this.fontFile = fontFile;
            this.text = text;
            this.color = color;
            LoadContent();
        }

        public String TextContent
        {
            get { return text; }
            set { text = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        protected override void LoadContent()
        {
            font = Game.Content.Load<SpriteFont>(fontFile);
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
            sb.DrawString(font, text, position, color);
            sb.End();


            base.Draw(gameTime);
        }
    }
}
