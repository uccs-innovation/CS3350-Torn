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
    //Creates a sprite controlled by the user using the keyboard or the mouse
    class UserControlledSprite : Sprite
    {
        KeyboardState keyboard;
        bool update, moving, isArms, isLegs, isVisible, pulling, stoped;
        char direction;
        int steps;
        int obstacleIndex;
        List<Sprite> obstacles;
        List<Sprite> trench;

        public UserControlledSprite(Game game, String textureFile, Vector2 position, bool isArms, bool isLegs)
            : base(game, textureFile, position)
        {
            //Initializes all the attributes with false in order to control the execution of the program
            update = false;
            moving = false;
            pulling = false;
            stoped = true;
            //Attribute to control how many steps the body part will walk during each interaction
            steps = MyGlobals.steps;
            this.isArms = isArms;
            this.isLegs = isLegs;
            isVisible = true;
        }

        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }

        }

        public bool Stoped
        {
            get { return stoped; }
            set { stoped = value; }
        }
        public List<Sprite> Obstacles
        {
            get { return obstacles; }
            set { obstacles = value; }
        }

        public List<Sprite> Trench
        {
            get { return trench; }
            set { trench = value; }
        }

        public override void Update(GameTime gameTime)
        {
            
            if (update)
            {
                keyboard = Keyboard.GetState();

                /*Makes the sprite walk accondingly with the keyboard
                 *checking if there are any other parts moving or if the this part will no collide with another one
                 */
                if (keyboard.IsKeyDown(Keys.Down) && !moving) 
                {
                    if (keyboard.IsKeyDown(Keys.LeftShift))
                        pulling = true;
                    direction = 'd';
                    moving = true;
                    stoped = true;
                }
                else if (keyboard.IsKeyDown(Keys.Up) && !moving)
                {
                    if (keyboard.IsKeyDown(Keys.LeftShift))
                        pulling = true;
                    direction = 'u';
                    moving = true;
                    stoped = true;
                }
                else if (keyboard.IsKeyDown(Keys.Left) && !moving)
                {
                    if (keyboard.IsKeyDown(Keys.LeftShift))
                        pulling = true;
                    direction = 'l';
                    moving = true;
                    stoped = true;
                }
                else if (keyboard.IsKeyDown(Keys.Right) && !moving) 
                {
                    if (keyboard.IsKeyDown(Keys.LeftShift))
                        pulling = true;
                    direction = 'r';
                    moving = true;
                    stoped = true;
                }

                //makes the body part 'walk' to the destination with the grid effect
                if (moving && steps > 0)
                {
                    //moves 3 pixels according with the last key pressed
                    switch (direction)
                    {
                        case 'd':
                            if (hastrench(trench, 'd'))
                                steps = 0;
                            else if (hasObstacle(obstacles, 'd') && !isArms)
                                steps = 0;
                            else if (hasObstacle(obstacles, 'd') && isArms && obstacleIndex == -1)
                                steps = 0;
                            else if (pull(obstacles, 'd') && pulling)
                            {
                                obstacles[obstacleIndex].Y += MyGlobals.blockSize / 10;
                                this.position.Y += MyGlobals.blockSize / 10;
                                steps--;
                            }
                            else if (hasObstacle(obstacles, 'd') && isArms && obstacleIndex != -1)
                            {
                                obstacles[obstacleIndex].Y += MyGlobals.blockSize / 10;
                                this.position.Y += MyGlobals.blockSize / 10;
                                steps--;
                            }
                            else if (!hasObstacle(obstacles, 'd'))
                            {
                                this.position.Y += MyGlobals.blockSize / 10;
                                steps--;
                            }
                            break;
                        case 'u':
                            if (hastrench(trench, 'u'))
                                steps = 0;
                            else if (hasObstacle(obstacles, 'u') && !isArms)
                                steps = 0;
                            else if (hasObstacle(obstacles, 'u') && isArms && obstacleIndex == -1)
                                steps = 0;
                            else if (pull(obstacles, 'u') && pulling)
                            {
                                obstacles[obstacleIndex].Y -= MyGlobals.blockSize / 10;
                                this.position.Y -= MyGlobals.blockSize / 10;
                                steps--;
                            }
                            else if (hasObstacle(obstacles, 'u') && isArms && obstacleIndex != -1)
                            {
                                obstacles[obstacleIndex].Y -= MyGlobals.blockSize / 10;
                                this.position.Y -= MyGlobals.blockSize / 10;
                                steps--;
                            }
                            else if (!hasObstacle(obstacles, 'u'))
                            {
                                this.position.Y -= MyGlobals.blockSize / 10;
                                steps--;
                            }
                            break;
                        case 'r':
                            if (hastrench(trench, 'r'))
                                steps = 0;
                            else if (hasObstacle(obstacles, 'r') && !isArms)
                                steps = 0;
                            else if (hasObstacle(obstacles, 'r') && isArms && obstacleIndex == -1)
                                steps = 0;
                            else if (pull(obstacles, 'r') && pulling)
                            {
                                obstacles[obstacleIndex].X += MyGlobals.blockSize / 10;
                                this.position.X += MyGlobals.blockSize / 10;
                                steps--;
                            }
                            else if (hasObstacle(obstacles, 'r') && isArms && obstacleIndex != -1)
                            {
                                obstacles[obstacleIndex].X += MyGlobals.blockSize / 10;
                                this.position.X += MyGlobals.blockSize / 10;
                                steps--;
                            }
                            else if (!hasObstacle(obstacles, 'r'))
                            {
                                this.position.X += MyGlobals.blockSize / 10;
                                steps--;
                            }
                            break;
                        case 'l':
                            if (hastrench(trench, 'l'))
                                steps = 0;
                            else if (hasObstacle(obstacles, 'l') && !isArms)
                                steps = 0;
                            else if (hasObstacle(obstacles, 'l') && isArms && obstacleIndex == -1)
                                steps = 0;
                            else if (pull(obstacles, 'l') && pulling)
                            {
                                obstacles[obstacleIndex].X -= MyGlobals.blockSize / 10;
                                this.position.X -= MyGlobals.blockSize / 10;
                                steps--;
                            }
                            else if (hasObstacle(obstacles, 'l') && isArms && obstacleIndex != -1)
                            {
                                obstacles[obstacleIndex].X -= MyGlobals.blockSize / 10;
                                this.position.X -= MyGlobals.blockSize / 10;
                                steps--;
                            }
                            else if (!hasObstacle(obstacles, 'l'))
                            {
                                this.position.X -= MyGlobals.blockSize / 10;
                                steps--;
                            }
                            break;
                    }
                }

                /*Stop after 10 interactions of 3 pixels
                 * set the direction with a meaningless caracter and return the other attributes to their default values*/
                if (steps == 0)
                {
                    pulling = false;
                    moving = false;
                    direction = '-';
                    steps = MyGlobals.steps;
                }

                if (keyboard.IsKeyDown(Keys.Down) && keyboard.IsKeyDown(Keys.LeftShift))
                    jump('d');
                else if (keyboard.IsKeyDown(Keys.Up) && keyboard.IsKeyDown(Keys.LeftShift))
                    jump('u');
                else if (keyboard.IsKeyDown(Keys.Right) && keyboard.IsKeyDown(Keys.LeftShift))
                    jump('r');
                else if (keyboard.IsKeyDown(Keys.Left) && keyboard.IsKeyDown(Keys.LeftShift))
                    jump('l');
            }
            base.Update(gameTime);
        }

        public bool change
        {
            get { return update; }
            set { update = value; }
        }

        public bool Moving
        {
            get { return moving; }
        }

        public bool hastrench(List<Sprite> trench, char direction)
        {
            switch (direction)
            {
                case 'd':
                    for (int i = 0; i < trench.Count; i++)
                    {
                        if (this.Position.Y + MyGlobals.blockSize == trench[i].Position.Y && this.Position.X == trench[i].Position.X)
                        {
                            for (int j = 0; j < obstacles.Count; j++)
                            {
                                if (trench[i].Position == obstacles[j].Position)
                                    return false;
                            }
                            return true;
                        }
                    }
                break;
                case 'u':
                    for (int i = 0; i < trench.Count; i++)
                    {
                        if (this.Position.Y - MyGlobals.blockSize == trench[i].Position.Y && this.Position.X == trench[i].Position.X)
                        {
                            for (int j = 0; j < obstacles.Count; j++)
                            {
                                if (trench[i].Position == obstacles[j].Position)
                                    return false;
                            }
                            return true;
                        }
                    }
                break;
                case 'r':
                    for (int i = 0; i < trench.Count; i++)
                    {
                        if (this.Position.X + MyGlobals.blockSize == trench[i].Position.X && this.Position.Y == trench[i].Position.Y)
                        {
                            for (int j = 0; j < obstacles.Count; j++)
                            {
                                if (trench[i].Position == obstacles[j].Position)
                                    return false;
                            }
                            return true;
                        }
                    }
                break;
                case 'l':
                    for (int i = 0; i < trench.Count; i++)
                    {
                        if (this.Position.X - MyGlobals.blockSize == trench[i].Position.X && this.Position.Y == trench[i].Position.Y)
                        {
                            for (int j = 0; j < obstacles.Count; j++)
                            {
                                if (trench[i].Position == obstacles[j].Position)
                                    return false;
                            }
                            return true;
                        }
                    }
                break;
            }

            return false;
        }

        public bool hasObstacle(List<Sprite> obstacles, char direction)
        {
            int i, j;

            switch (direction)
            {
                case 'd':
                    for (i = 0; i < obstacles.Count; i++)
                    {
                        if (this.Position.Y + MyGlobals.blockSize == obstacles[i].Position.Y && this.Position.X == obstacles[i].Position.X)
                        {
                            for (j = 0; j < trench.Count; j++)
                            {
                                if (obstacles[i].Position == trench[j].Position)
                                    return false;
                            }
                            for (j = 0; j < obstacles.Count; j++)
                            {
                                if (!pulling && (obstacles[i].Position.Y + MyGlobals.blockSize == obstacles[j].Position.Y && obstacles[i].Position.X == obstacles[j].Position.X || obstacles[i].Position.Y>= MyGlobals.heigh - MyGlobals.blockSize - MyGlobals.blockSize/2))
                                {
                                    //Return -1 to indicate that there is a double obstacle line.
                                    obstacleIndex = -1;
                                    return true;
                                }
                            }
                            obstacleIndex = i;
                            return true;
                        }
                    }
                    break;
                case 'u':
                    for (i = 0; i < obstacles.Count; i++)
                    {
                        if (this.Position.Y - MyGlobals.blockSize == obstacles[i].Position.Y && this.Position.X == obstacles[i].Position.X)
                        {
                            for (j = 0; j < trench.Count; j++)
                            {
                                if (obstacles[i].Position == trench[j].Position)
                                    return false;
                            }
                            for (j = 0; j < obstacles.Count; j++)
                            {
                                if (!pulling && (obstacles[i].Position.Y - MyGlobals.blockSize == obstacles[j].Position.Y && obstacles[i].Position.X == obstacles[j].Position.X || obstacles[i].Position.Y <= MyGlobals.blockSize + MyGlobals.blockSize/2))
                                {
                                    //Return -1 to indicate that there is a double obstacle line.
                                    obstacleIndex = -1;
                                    return true;
                                }
                            }
                            obstacleIndex = i;
                            return true;
                        }
                    }
                    break;
                case 'r':
                    for (i = 0; i < obstacles.Count; i++)
                    {
                        if (this.Position.X + MyGlobals.blockSize == obstacles[i].Position.X && this.Position.Y == obstacles[i].Position.Y)
                        {
                            for (j = 0; j < trench.Count; j++)
                            {
                                if (obstacles[i].Position == trench[j].Position)
                                    return false;
                            }
                            for (j = 0; j < obstacles.Count; j++)
                            {
                                if (!pulling && (obstacles[i].Position.X + MyGlobals.blockSize == obstacles[j].Position.X && obstacles[i].Position.Y == obstacles[j].Position.Y || obstacles[i].Position.X >= MyGlobals.width - MyGlobals.blockSize - MyGlobals.blockSize / 2))
                                {
                                    //Return -1 to indicate that there is a double obstacle line.
                                    obstacleIndex = -1;
                                    return true;
                                }
                            }
                            obstacleIndex = i;
                            return true;
                        }
                    }
                    break;
                case 'l':
                    for (i = 0; i < obstacles.Count; i++)
                    {
                        if (this.Position.X - MyGlobals.blockSize == obstacles[i].Position.X && this.Position.Y == obstacles[i].Position.Y)
                        {
                            for (j = 0; j < trench.Count; j++)
                            {
                                if (obstacles[i].Position == trench[j].Position)
                                    return false;
                            }
                            for (j = 0; j < obstacles.Count; j++)
                            {
                                if (!pulling && (obstacles[i].Position.X - MyGlobals.blockSize == obstacles[j].Position.X && obstacles[i].Position.Y == obstacles[j].Position.Y || obstacles[i].Position.X <= MyGlobals.blockSize + MyGlobals.blockSize / 2))
                                {
                                    //Return -1 to indicate that there is a double obstacle line.
                                    obstacleIndex = -1;
                                    return true;
                                }
                            }
                            obstacleIndex = i;
                            return true;
                        }
                    }
                    break;
            }
            return false;
        }

        public bool kick(Sprite body, char direction)
        {
            keyboard = Keyboard.GetState();
            if (this.isArms || this.isLegs)
            {
                if (this.position.Y + MyGlobals.blockSize == body.Position.Y && this.position.X == body.Position.X && direction == 'd')
                {
                    for (int i = 0; i < trench.Count; i++)
                    {
                        if (body.Position.Y + MyGlobals.blockSize * 2 == trench[i].Position.Y && body.Position.X == trench[i].Position.X)
                        {
                            for (int j = 0; j < obstacles.Count; j++)
                            {
                                if (trench[i].Position == obstacles[j].Position)
                                    return true;
                            }
                            return false;
                        }
                    }
                    return true;
                }
                else if (this.position.Y - MyGlobals.blockSize == body.Position.Y && this.position.X == body.Position.X && direction == 'u')
                {
                    for (int i = 0; i < trench.Count; i++)
                    {
                        if (body.Position.Y - MyGlobals.blockSize * 2 == trench[i].Position.Y && body.Position.X == trench[i].Position.X)
                        {
                            for (int j = 0; j < obstacles.Count; j++)
                            {
                                if (trench[i].Position == obstacles[j].Position)
                                    return true;
                            }
                            return false;
                        }
                    }
                    return true;
                }
                else if (this.position.X - MyGlobals.blockSize == body.Position.X && this.position.Y == body.Position.Y && direction == 'l')
                {
                    for (int i = 0; i < trench.Count; i++)
                    {
                        if (body.Position.X - MyGlobals.blockSize * 2 == trench[i].Position.X && body.Position.Y == trench[i].Position.Y)
                        {
                            for (int j = 0; j < obstacles.Count; j++)
                            {
                                if (trench[i].Position == obstacles[j].Position)
                                    return true;
                            }
                            return false;
                        }
                    }
                    return true;
                }
                else if (this.position.X + MyGlobals.blockSize == body.Position.X && this.position.Y == body.Position.Y && direction == 'r')
                {
                    for (int i = 0; i < trench.Count; i++)
                    {
                        if (body.Position.X + MyGlobals.blockSize * 2 == trench[i].Position.X && body.Position.Y == trench[i].Position.Y)
                        {
                            for (int j = 0; j < obstacles.Count; j++)
                            {
                                if (trench[i].Position == obstacles[j].Position)
                                    return true;
                            }
                            return false;
                        }
                    }
                    return true;
                }
                else
                    return false;
            }
            return false;
        }

        public void jump(char direction)
        {
            int i, j;

            if (isLegs)
            {
                switch (direction)
                {
                    case 'd':
                        if (hastrench(this.trench, 'd') && this.position.Y + MyGlobals.blockSize * 2 < MyGlobals.heigh - MyGlobals.blockSize/2)
                        {
                            for (i = 0; i < trench.Count; i++)
                            {
                                if (this.position.Y + MyGlobals.blockSize * 2 == trench[i].Position.Y && this.position.X == trench[i].Position.X)
                                {
                                    for (j = 0; j < obstacles.Count; j++)
                                    {
                                        if (trench[i].Position == obstacles[j].Position)
                                        {
                                            this.position.Y += MyGlobals.blockSize;
                                            return;
                                        }
                                    }
                                    return;
                                }
                            }
                            this.position.Y += MyGlobals.blockSize;
                        }
                        break;
                    case 'u':
                        if (hastrench(this.trench, 'u') && this.position.Y - MyGlobals.blockSize * 2 > MyGlobals.blockSize/2)
                        {
                            for (i = 0; i < trench.Count; i++)
                            {
                                if (this.position.Y - MyGlobals.blockSize * 2 == trench[i].Position.Y && this.position.X == trench[i].Position.X)
                                {
                                    for (j = 0; j < obstacles.Count; j++)
                                    {
                                        if (trench[i].Position == obstacles[j].Position)
                                        {
                                            this.position.Y -= MyGlobals.blockSize;
                                            return;
                                        }
                                    }
                                    return;
                                }
                            }
                            this.position.Y -= MyGlobals.blockSize;
                        }
                        break;
                    case 'l':
                        if (hastrench(this.trench, 'l') && this.position.X - MyGlobals.blockSize * 2 > MyGlobals.blockSize/2)
                        {
                            for (i = 0; i < trench.Count; i++)
                            {
                                if (this.position.X - MyGlobals.blockSize * 2 == trench[i].Position.X && this.position.Y == trench[i].Position.Y)
                                {
                                    for (j = 0; j < obstacles.Count; j++)
                                    {
                                        if (trench[i].Position == obstacles[j].Position)
                                        {
                                            this.position.X -= MyGlobals.blockSize;
                                            return;
                                        }
                                    }
                                    return;
                                }
                            }
                            this.position.X -= MyGlobals.blockSize;
                        }
                        break;
                    case 'r':
                        if (hastrench(this.trench, 'r') && this.position.X + MyGlobals.blockSize * 2 < MyGlobals.width - MyGlobals.blockSize / 2)
                        {
                            for (i = 0; i < trench.Count; i++)
                            {
                                if (this.position.X + MyGlobals.blockSize * 2 == trench[i].Position.X && this.position.Y == trench[i].Position.Y)
                                {
                                    for (j = 0; j < obstacles.Count; j++)
                                    {
                                        if (trench[i].Position == obstacles[j].Position)
                                        {
                                            this.position.X += MyGlobals.blockSize;
                                            return;
                                        }
                                    }
                                    return;
                                }
                            }
                            this.position.X += MyGlobals.blockSize;
                        }
                        break;
                }
            }
        }

        public bool pull(List<Sprite> obstacles, char direction)
        {
            if(isArms)
            {
                switch (direction)
                {
                    case 'd':
                        if (!hastrench(trench, 'd') && !hasObstacle(obstacles, 'd') && hasObstacle(obstacles, 'u') && this.position.Y < MyGlobals.heigh - MyGlobals.blockSize - MyGlobals.blockSize / 2)
                            return true;
                        break;
                    case 'u':
                        if (!hastrench(trench, 'u') && !hasObstacle(obstacles, 'u') && hasObstacle(obstacles, 'd') && this.position.Y > MyGlobals.blockSize + MyGlobals.blockSize / 2)
                            return true;
                        break;
                    case 'l':
                        if (!hastrench(trench, 'l') && !hasObstacle(obstacles, 'l') && hasObstacle(obstacles, 'r') && this.position.X > MyGlobals.blockSize + MyGlobals.blockSize / 2)
                            return true;
                        break;
                    case 'r':
                        if (!hastrench(trench, 'r') && !hasObstacle(obstacles, 'r') && hasObstacle(obstacles, 'l') && this.position.Y < MyGlobals.width - MyGlobals.blockSize - MyGlobals.blockSize / 2)
                            return true;
                        break;
                }
            }
            return false;
        }
    }
}
