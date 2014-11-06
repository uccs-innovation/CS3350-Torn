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
    //AnimatedSprite: Creates a sprite that runs through the display
    class AnimatedSprite : Sprite
    {
        //Define how many pixels the sprite will move itself on the x-axis and y-axis
        int directionX, directionY;
        public AnimatedSprite(Game game, String textureFile, Vector2 position, int directionX, int directionY)
            : base(game, textureFile, position)
        {
            this.directionX = directionX;
            this.directionY = directionY;
        }

        public override void Update(GameTime gameTime)
        {
            //Makes the sprite walk accordingly with the direction previously established
            this.position.X += directionX;
            this.position.Y += directionY;


            //Detect if the sprite hit the border. If yes, changes the direction
            if (this.position.Y < 0)
                directionY *= -1;
            if (this.position.X < 0)
                directionX *= -1;
            if (this.position.Y > 450)
                directionY *= -1;
            if (this.position.X > 750)
                directionX *= -1;

            base.Update(gameTime);
        }
    }
}
