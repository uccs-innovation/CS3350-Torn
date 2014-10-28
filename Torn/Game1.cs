using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Torn
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        Sprite initialScreen;
        Vector2 initialPosition, finalPosition;
        bool started;
        KeyboardState keyboard;
        UserControlledSprite[] body;
        Sprite blindness;
        Sprite edgesBlocks;
        int[,] field;
        List<Sprite> trench;
        List<Sprite> obstacles;
        int[] finished;
        Sprite levelEnd;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //graphics.IsFullScreen = true;
            
            //Initialize all the global variables
            MyGlobals.width = 420;
            MyGlobals.heigh = 420;
            MyGlobals.blockSize = 30;
            MyGlobals.scale = 1.0f ;

            MyGlobals.steps = 10;
            MyGlobals.empty = 0;
            MyGlobals.trench = 1;
            MyGlobals.obstacle = 2;
            MyGlobals.head = 3;
            MyGlobals.arms = 4;
            MyGlobals.legs = 5;
            MyGlobals.final = 6;
            

            graphics.PreferredBackBufferWidth = MyGlobals.width;
            graphics.PreferredBackBufferHeight = MyGlobals.heigh;

            body = new UserControlledSprite[3];
            started = false;
            field = new int[12, 12];
            trench = new List<Sprite>();
            obstacles = new List<Sprite>();
            finished = new int[3] { 0, 0, 0 };
            readField();

        }
        
        protected override void Initialize()
        {
            //Makes the mouse pointer visible
            this.IsMouseVisible = true;
            base.Initialize();
        }

        public void readField()
        {
            string line;
            int i = 0;

            System.IO.StreamReader file = new System.IO.StreamReader(@"Levels\level1.txt");
            while ((line = file.ReadLine()) != null)
            {
                for (int j = 0; j < 12; j++)
                {
                    field[i, j] = Int32.Parse(line[j * 2].ToString());
                }
                i++;
            }

            file.Close();
        }

        protected override void LoadContent()
        {
            //Displays the initial screen
            if (!started)
            {
                spriteBatch = new SpriteBatch(GraphicsDevice);
                initialPosition.X = graphics.PreferredBackBufferWidth / 2;
                initialPosition.Y = graphics.PreferredBackBufferHeight / 2;
                initialScreen = new Sprite(this, @"Images\InitialScreen", initialPosition);
                Components.Add(initialScreen);
            }
            else if(levelFinished())
            {
                initialPosition.X = graphics.PreferredBackBufferWidth / 2;
                initialPosition.Y = graphics.PreferredBackBufferHeight / 2;
                levelEnd = new Sprite(this, @"Images\levelEnd1", initialPosition);
                Components.Add(levelEnd);
            }
            else
            {
                Sprite aux;

                //Takes the initial screen off and displays the body parts
                Components.Remove(initialScreen);


                initialPosition.X = 4 * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                initialPosition.Y = 6 * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                for (int i = 0; i < 4; i++)
                {
                    initialPosition.X += MyGlobals.blockSize;
                    aux = new Sprite(this, @"Images\Block", initialPosition);
                    Components.Add(aux);
                }
                
                //Initializes the body parts in divisible by three positions, in order to make the grid effect during the movement
                for (int i = 0; i < 12; i++)
                {
                    for (int j = 0; j < 12; j++)
                    {
                        if (field[i, j] == MyGlobals.trench)
                        {
                            initialPosition.X = (j + 2) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            initialPosition.Y = (i + 2) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            aux = new Sprite(this, @"Images\GrayBlock", initialPosition);
                            trench.Add(aux);
                            Components.Add(aux);
                        }
                    }
                }
                for (int i = 0; i < 12; i++)
                {
                    for (int j = 0; j < 12; j++)
                    {
                        if (field[i, j] == MyGlobals.obstacle)
                        {
                            initialPosition.X = (j + 2) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            initialPosition.Y = (i + 2) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            aux = new Sprite(this, @"Images\Block", initialPosition);
                            obstacles.Add(aux);
                            Components.Add(aux);
                        }
                        else if (field[i, j] == MyGlobals.final)
                        {
                            initialPosition.X = (j + 2) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            initialPosition.Y = (i + 2) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            finalPosition = initialPosition;
                            aux = new Sprite(this, @"Images\final", initialPosition);
                            Components.Add(aux);
                        }
                    }
                }
                        
                for (int i = 0; i < 12; i++)
			    {
			        for (int j = 0; j < 12; j++)
			        {
                        if (field[i, j] == MyGlobals.head)
                        {
                            initialPosition.X = (j + 2) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            initialPosition.Y = (i + 2) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            body[0] = new UserControlledSprite(this, @"Images\Head", initialPosition, false, false);
                            Components.Add(body[0]);
                        }
                        else if (field[i, j] == MyGlobals.arms)
                        {
                            initialPosition.X = (j + 2) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            initialPosition.Y = (i+ 2) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            body[1] = new UserControlledSprite(this, @"Images\Arms", initialPosition, true, false);
                            Components.Add(body[1]);
                        }
                        else if (field[i, j] == MyGlobals.legs)
                        {
                            initialPosition.X = (j + 2) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            initialPosition.Y = (i + 2) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            body[2] = new UserControlledSprite(this, @"Images\Legs", initialPosition, false , true);
                            Components.Add(body[2]);
                        }
                    }
                }

                //Initializes the blind sprinte with the same position of the head
                blindness = new Sprite(this, @"Images\Blindness", new Vector2(body[0].Position.X, body[0].Position.Y));

                //Draw all the blocks on the right edge
                initialPosition.Y = -MyGlobals.blockSize/2;
                for (int i = 0; i < graphics.PreferredBackBufferHeight / MyGlobals.blockSize; i++)
                {
                    initialPosition.X = MyGlobals.blockSize/2;
                    initialPosition.Y += MyGlobals.blockSize;
                    edgesBlocks = new Sprite(this, @"Images\edge",initialPosition);
                    Components.Add(edgesBlocks);
                    trench.Add(edgesBlocks);
                }
                
                //Draw all the blocks on the left edge
                initialPosition.Y = -MyGlobals.blockSize/2;
                for (int i = 0; i < graphics.PreferredBackBufferHeight / MyGlobals.blockSize; i++)
                {
                    initialPosition.X = graphics.PreferredBackBufferWidth - MyGlobals.blockSize/2;
                    initialPosition.Y += MyGlobals.blockSize;
                    edgesBlocks = new Sprite(this, @"Images\edge", initialPosition);
                    Components.Add(edgesBlocks);
                    trench.Add(edgesBlocks);
                }
                
                //Draw all the blocks on the top edge
                initialPosition.X = MyGlobals.blockSize/2;
                for (int i = 0; i < graphics.PreferredBackBufferWidth / MyGlobals.blockSize - 2; i++)
                {
                    initialPosition.X += MyGlobals.blockSize;
                    initialPosition.Y = MyGlobals.blockSize/2;
                    edgesBlocks = new Sprite(this, @"Images\edge", initialPosition);
                    Components.Add(edgesBlocks);
                    trench.Add(edgesBlocks);
                }

                //Draw all the blocks on the bottom edge
                initialPosition.X = MyGlobals.blockSize/2;
                for (int i = 0; i < graphics.PreferredBackBufferWidth / MyGlobals.blockSize; i++)
                {
                    initialPosition.X += MyGlobals.blockSize;
                    initialPosition.Y = graphics.PreferredBackBufferHeight - MyGlobals.blockSize/2;
                    edgesBlocks = new Sprite(this, @"Images\edge", initialPosition);
                    Components.Add(edgesBlocks);
                    trench.Add(edgesBlocks);
                }

                body[0].Trench = trench;
                body[1].Trench = trench;
                body[2].Trench = trench;

                body[0].Trench.Add(body[1]);
                body[0].Trench.Add(body[2]);

                body[1].Trench.Add(body[0]);
                body[1].Trench.Add(body[2]);

                body[2].Trench.Add(body[0]);
                body[2].Trench.Add(body[1]);


                body[1].Obstacles = obstacles;

                //Components.Add(blindness);
                
            }
            
        }


        protected override void UnloadContent()
        {
           
        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();           
            
            keyboard = Keyboard.GetState();

            if(started)
            {
                body[0].Obstacles = body[1].Obstacles;
                body[2].Obstacles = body[1].Obstacles;
            }

            //Waits for the first ENTER to initialize the game
            if (keyboard.IsKeyDown(Keys.Enter) && !started)
            {
                started = true;
                //Call the method LoadContent() in order to initialize the body parts
                LoadContent();
            }
            //Checks which part will move according with the user input, if the game is already started and if there is any other body part moving
            else if (keyboard.IsKeyDown(Keys.H) && started && !isBodyMoving())
            {
                //If the head was selected, set the bool attribute 'change' in body[0] as true and in the other parts as false, in this way, only the head will move 
                body[0].change = true;
                body[1].change = false;
                body[2].change = false;
            }
            else if (keyboard.IsKeyDown(Keys.A) && started && !isBodyMoving())
            {
                //If the arms was selected, set the bool attribute 'change' in body[1] as true and in the other parts as false, in this way, only the arms will move
                body[0].change = false;
                body[1].change = true;
                body[2].change = false;    
            }
            else if (keyboard.IsKeyDown(Keys.L) && started && !isBodyMoving())
            {
                //If the legs was selected, set the bool attribute 'change' in body[2] as true and in the other parts as false, in this way, only the legs will move
                body[0].change = false;
                body[1].change = false;
                body[2].change = true;
            }
            if (started)
            {
                if (keyboard.IsKeyDown(Keys.LeftShift) && keyboard.IsKeyDown(Keys.Down))
                {
                    if (body[2].kick(body[0], 'd'))
                        body[0].Position = new Vector2(body[0].Position.X, body[0].Position.Y + MyGlobals.blockSize * 2);
                    else if (body[2].kick(body[1], 'd'))
                        body[1].Position = new Vector2(body[1].Position.X, body[1].Position.Y + MyGlobals.blockSize * 2);
                }
                else if (keyboard.IsKeyDown(Keys.LeftShift) && keyboard.IsKeyDown(Keys.Up))
                {
                    if (body[2].kick(body[0], 'u'))
                        body[0].Position = new Vector2(body[0].Position.X, body[0].Position.Y - MyGlobals.blockSize * 2);
                    else if (body[2].kick(body[1], 'u'))
                        body[1].Position = new Vector2(body[1].Position.X, body[1].Position.Y - MyGlobals.blockSize * 2);
                }
                else if (keyboard.IsKeyDown(Keys.LeftShift) && keyboard.IsKeyDown(Keys.Left))
                {
                    if (body[2].kick(body[0], 'l'))
                        body[0].Position = new Vector2(body[0].Position.X - MyGlobals.blockSize * 2, body[0].Position.Y);
                    else if (body[2].kick(body[1], 'l'))
                        body[1].Position = new Vector2(body[1].Position.X - MyGlobals.blockSize * 2, body[1].Position.Y);
                }
                else if (keyboard.IsKeyDown(Keys.LeftShift) && keyboard.IsKeyDown(Keys.Right))
                {
                    if (body[2].kick(body[0], 'r'))
                        body[0].Position = new Vector2(body[0].Position.X + MyGlobals.blockSize * 2, body[0].Position.Y);
                    else if (body[2].kick(body[1], 'r'))
                        body[1].Position = new Vector2(body[1].Position.X + MyGlobals.blockSize * 2, body[1].Position.Y);
                }
                if (body[0].Position.X > graphics.PreferredBackBufferWidth - MyGlobals.blockSize)
                    body[0].Position = new Vector2(graphics.PreferredBackBufferWidth - MyGlobals.blockSize + MyGlobals.blockSize/2, body[0].Position.Y);
                if (body[1].Position.X > graphics.PreferredBackBufferWidth - MyGlobals.blockSize)
                    body[1].Position = new Vector2(graphics.PreferredBackBufferWidth - MyGlobals.blockSize + MyGlobals.blockSize/2, body[1].Position.Y);

                if (body[0].Position.Y > graphics.PreferredBackBufferHeight - MyGlobals.blockSize)
                    body[0].Position = new Vector2(body[1].Position.X, graphics.PreferredBackBufferHeight - MyGlobals.blockSize + MyGlobals.blockSize/2);
                if (body[1].Position.Y > graphics.PreferredBackBufferHeight - MyGlobals.blockSize)
                    body[1].Position = new Vector2(body[1].Position.X, graphics.PreferredBackBufferHeight - MyGlobals.blockSize + MyGlobals.blockSize/2);

                if (body[0].Position.X < MyGlobals.blockSize)
                    body[0].Position = new Vector2(MyGlobals.blockSize + MyGlobals.blockSize/2, body[0].Position.Y);
                if (body[1].Position.X < MyGlobals.blockSize)
                    body[1].Position = new Vector2(MyGlobals.blockSize + MyGlobals.blockSize/2, body[1].Position.Y);

                if (body[0].Position.Y < MyGlobals.blockSize)
                    body[0].Position = new Vector2(body[1].Position.X, MyGlobals.blockSize + MyGlobals.blockSize/2);
                if (body[1].Position.Y < MyGlobals.blockSize)
                    body[1].Position = new Vector2(body[1].Position.X, MyGlobals.blockSize + MyGlobals.blockSize/2);


            }

            if (started)
            {
                //Give to blindness sprite, the same position of the head
                blindness.Position = body[0].Position;

                if(finalized(body[0].Position))
                {
                    Components.Remove(body[0]);
                    body[1].Trench.Remove(body[0]);
                    body[2].Trench.Remove(body[0]);
                    finished[0] = 1;
                }
                if (finalized(body[1].Position))
                {
                    Components.Remove(body[1]);
                    body[0].Trench.Remove(body[1]);
                    body[2].Trench.Remove(body[1]);
                    finished[1] = 1;
                }
                if (finalized(body[2].Position))
                {
                    Components.Remove(body[2]);
                    body[0].Trench.Remove(body[2]);
                    body[1].Trench.Remove(body[2]);
                    finished[2] = 1;
                }

               if(levelFinished())
               {
                    LoadContent();
               }

            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            base.Draw(gameTime);
        }

        //Verifies if any part of the body is moving
        public bool isBodyMoving()
        {
            for (int i = 0; i < 3; i++)
            {
                if (body[i].Moving)
                    return true;
            }
            return false;
        }

        public bool finalized(Vector2 position)
        {
            if (position == finalPosition)
                return true;
            else
                return false;
        }

        public bool levelFinished()
        {
            for (int i = 0; i < 3; i++)
            {
                if (finished[i] == 0)
                    return false;
            }
            return true;
        }
    }
}
