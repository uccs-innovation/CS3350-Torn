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
using System.IO;

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
        int[,] field;
        List<Vector2> blockSight;
        List<Sprite> trench;
        List<Sprite> obstacles;
        List<BridgePlate> bridgePlates;
        List<Vector2> bridgesPosition;
        List<Vector2> platesPosition;
        List<Sprite> brokenBridges;
        int[] finished;
        Sprite levelEnd, aux;
        Sprite textBox;
        SpriteFont text;
        List<Sprite> hiddenBellow, hiddenAbove, hiddenRight, hiddenLeft;
        List<Vector2> indexesBellow, indexesAbove, indexesRight, indexesLeft;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //graphics.IsFullScreen = true;
            
            //Initialize all the global variables
            MyGlobals.width = 420 * 2;
            MyGlobals.heigh = 420 * 2;
            MyGlobals.blockSize = 30 * 2;
            MyGlobals.scale = 1.0f * 2;

            MyGlobals.steps = 10;
            MyGlobals.empty = 0;
            MyGlobals.trench = 1;
            MyGlobals.obstacle = 2;
            MyGlobals.head = 3;
            MyGlobals.arms = 4;
            MyGlobals.legs = 5;
            MyGlobals.final = 6;
            MyGlobals.blocksVision = 7;
            MyGlobals.plates = 8;
            MyGlobals.bridges = 9;
            MyGlobals.outOfSize = 10;
            MyGlobals.numberOfBlocks = 14;
            

            graphics.PreferredBackBufferWidth = MyGlobals.width;
            graphics.PreferredBackBufferHeight = MyGlobals.heigh;

            body = new UserControlledSprite[3];
            started = false;
            field = new int[MyGlobals.numberOfBlocks, MyGlobals.numberOfBlocks];
            trench = new List<Sprite>();
            obstacles = new List<Sprite>();
            bridgePlates = new List<BridgePlate>();
            bridgesPosition = new List<Vector2>();
            platesPosition = new List<Vector2>();
            brokenBridges = new List<Sprite>();
            blockSight = new List<Vector2>();
            hiddenBellow = new List<Sprite>();
            hiddenAbove = new List<Sprite>();
            hiddenLeft = new List<Sprite>();
            hiddenRight = new List<Sprite>();
            indexesBellow = new List<Vector2>();
            indexesAbove = new List<Vector2>();
            indexesLeft = new List<Vector2>();
            indexesRight = new List<Vector2>();
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
            int i = 0, j = 0, lastJ = 0, lastI = 0, result, rest;

            System.IO.StreamReader file = new System.IO.StreamReader(@"Levels\tutorial.txt");
            while ((line = file.ReadLine()) != null)
            {
                while (j * 2 < line.Length)
                {
                    field[i, j] = Int32.Parse(line[j * 2].ToString());
                    j++;
                }
                lastJ = j;
                j = 0;
                i++;
            }

            lastI = i;

            if (lastJ < MyGlobals.numberOfBlocks)
            {
                int aux = MyGlobals.numberOfBlocks - lastJ;

                result = aux / 2;
                rest = aux - result;

                for (i = 0; i < MyGlobals.numberOfBlocks; i++)
                {
                    for (j = MyGlobals.numberOfBlocks - rest -1; j >= result; j--)
                    {
                        field[i, j] = field[i, j - result];
                    }
                }

                for (i = 0; i < MyGlobals.numberOfBlocks; i++)
                {
                    for (j = 0; j < result; j++)
                    {
                        field[i, j] = MyGlobals.outOfSize;
                    }
                    for (j = MyGlobals.numberOfBlocks -1; j > MyGlobals.numberOfBlocks - rest - 1; j--)
                    {
                        field[i, j] = MyGlobals.outOfSize;
                    }
                }
            }

            if (lastI < MyGlobals.numberOfBlocks)
            {
                int aux = MyGlobals.numberOfBlocks - lastI;
                
                result = aux / 2;
                rest = aux - result;

                for (i = 0; i < MyGlobals.numberOfBlocks; i++)
                {
                    for (j = MyGlobals.numberOfBlocks - rest - 1; j >= result; j--)
                    {
                        field[j, i] = field[j - result, i];
                    }
                }

                for (i = 0; i < MyGlobals.numberOfBlocks; i++)
                {
                    for (j = 0; j < result; j++)
                    {
                        field[j, i] = MyGlobals.outOfSize;
                    }
                    for (j = MyGlobals.numberOfBlocks - 1; j > MyGlobals.numberOfBlocks - rest - 1; j--)
                    {
                        field[j, i] = MyGlobals.outOfSize;
                    }
                }
            }
            file.Close();

            using (TextWriter tw = new StreamWriter(@"Levels\levelTeste.txt"))
            {
                for (j = 0; j < MyGlobals.numberOfBlocks; j++)
                {
                    for (i = 0; i < MyGlobals.numberOfBlocks; i++)
                    {
                        tw.Write(field[j, i] + " ");
                    }
                    tw.WriteLine();
                }
            }
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
                //Takes the initial screen off and displays the body parts
                Components.Remove(initialScreen);

                for (int i = 0; i < MyGlobals.numberOfBlocks; i++)
                {
                    for (int j = 0; j < MyGlobals.numberOfBlocks; j++)
                    {
                        initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                        initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                        aux = new Sprite(this, @"Images\grass", initialPosition);
                        Components.Add(aux);
                    }
                }

                for (int i = 0; i < MyGlobals.numberOfBlocks; i++)
                {
                    for (int j = 0; j < MyGlobals.numberOfBlocks; j++)
                    {
                        if (field[i, j] == MyGlobals.outOfSize)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            aux = new Sprite(this, @"Images\edge", initialPosition);
                            Components.Add(aux);
                            trench.Add(aux);
                        }
                    }
                }

                for (int i = 0; i < MyGlobals.numberOfBlocks; i++)
                {
                    for (int j = 0; j < MyGlobals.numberOfBlocks; j++)
                    {
                        if (field[i, j] == MyGlobals.blocksVision)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            aux = new Sprite(this, @"Images\BlockVision", initialPosition);
                            trench.Add(aux);
                            blockSight.Add(aux.Position);
                            Components.Add(aux);
                        }
                    }
                }

                for (int i = 0; i < MyGlobals.numberOfBlocks; i++)
                {
                    for (int j = 0; j < MyGlobals.numberOfBlocks; j++)
                    {
                        if (field[i, j] == MyGlobals.plates)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            platesPosition.Add(initialPosition);
                        }
                    }
                }

                for (int i = 0; i < MyGlobals.numberOfBlocks; i++)
                {
                    for (int j = 0; j < MyGlobals.numberOfBlocks; j++)
                    {
                        if (field[i, j] == MyGlobals.bridges)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            bridgesPosition.Add(initialPosition);
                            brokenBridges.Add(new Sprite(this, @"Images\bridge", initialPosition));
                        }
                    }
                }

                for (int i = 0; i < brokenBridges.Count; i++)
                {
                    trench.Add(brokenBridges[i]);
                }

                for (int i = 0; i < bridgesPosition.Count; i++)
                {
                    bridgePlates.Add(new BridgePlate(this, @"Images\bridge", bridgesPosition[i], @"Images\pressureplate_grass", platesPosition[i]));
                    Components.Add(bridgePlates[i]);
                }
                
                //Initializes the body parts in divisible by three positions, in order to make the grid effect during the movement
                for (int i = 0; i < MyGlobals.numberOfBlocks; i++)
                {
                    for (int j = 0; j < MyGlobals.numberOfBlocks; j++)
                    {
                        if (field[i, j] == MyGlobals.trench)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            aux = new Sprite(this, @"Images\pit", initialPosition);
                            trench.Add(aux);
                            Components.Add(aux);
                        }
                    }
                }
                for (int i = 0; i < MyGlobals.numberOfBlocks; i++)
                {
                    for (int j = 0; j < MyGlobals.numberOfBlocks; j++)
                    {
                        if (field[i, j] == MyGlobals.obstacle)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            aux = new Sprite(this, @"Images\Block", initialPosition);
                            obstacles.Add(aux);
                            Components.Add(aux);
                        }
                        else if (field[i, j] == MyGlobals.final)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            finalPosition = initialPosition;
                            aux = new Sprite(this, @"Images\final", initialPosition);
                            Components.Add(aux);
                        }
                    }
                }

                for (int i = 0; i < MyGlobals.numberOfBlocks; i++)
			    {
                    for (int j = 0; j < MyGlobals.numberOfBlocks; j++)
			        {
                        if (field[i, j] == MyGlobals.head)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            body[0] = new UserControlledSprite(this, @"Images\Head", initialPosition, false, false);
                            Components.Add(body[0]);
                        }
                        else if (field[i, j] == MyGlobals.arms)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            initialPosition.Y = (i+ 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            body[1] = new UserControlledSprite(this, @"Images\Arms", initialPosition, true, false);
                            Components.Add(body[1]);
                        }
                        else if (field[i, j] == MyGlobals.legs)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            body[2] = new UserControlledSprite(this, @"Images\Legs", initialPosition, false , true);
                            Components.Add(body[2]);
                        }
                    }
                }

                //Initializes the blind sprinte with the same position of the head
               blindness = new Sprite(this, @"Images\Blindness", new Vector2(body[0].Position.X, body[0].Position.Y));

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

                Components.Add(blindness);
                
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

            if (started && !isBodyMoving())
            {
                for (int i = 0; i < indexesRight.Count; i++)
                {
                    indexesRight.RemoveAt(i);
                }
                for (int i = 0; i < indexesLeft.Count; i++)
                {
                    indexesLeft.RemoveAt(i);
                }
                for (int i = 0; i < indexesAbove.Count; i++)
                {
                    indexesAbove.RemoveAt(i);
                }
                for (int i = 0; i < indexesBellow.Count; i++)
                {
                    indexesBellow.RemoveAt(i);
                }

                //RIght
                if (visionBlocked(blockSight, 'r'))
                {
                    for (int i = 0; i < indexesRight.Count; i++)
                    {
                        initialPosition = indexesRight[i];
                        int aux1 = (int)((MyGlobals.width - initialPosition.X) / MyGlobals.blockSize);
                        for (int j = 0; j < aux1; j++)
                        {
                            initialPosition.X += MyGlobals.blockSize;
                            aux = new Sprite(this, @"Images\BlockVision", initialPosition);
                            hiddenRight.Add(aux);
                            Components.Add(aux);
                        }
                    }

                }
                else if (hiddenRight.Count > 0)
                {
                    for (int i = 0; i < hiddenRight.Count; i++)
                    {
                        Components.Remove(hiddenRight[i]);
                        hiddenRight.RemoveAt(i);
                    }
                }

                //LEFT
                if (visionBlocked(blockSight, 'l'))
                {
                    for (int i = 0; i < indexesLeft.Count; i++)
                    {
                        initialPosition = indexesLeft[i];
                        int aux1 = (int)((initialPosition.X) / MyGlobals.blockSize);
                        for (int j = 0; j < aux1; j++)
                        {
                            initialPosition.X -= MyGlobals.blockSize;
                            aux = new Sprite(this, @"Images\BlockVision", initialPosition);
                            hiddenLeft.Add(aux);
                            Components.Add(aux);
                        }
                    }

                }
                else if (hiddenLeft.Count > 0)
                {
                    for (int i = 0; i < hiddenLeft.Count; i++)
                    {
                        Components.Remove(hiddenLeft[i]);
                        hiddenLeft.RemoveAt(i);
                    }
                }
                //UP
                if (visionBlocked(blockSight, 'u'))
                {
                    for (int i = 0; i < indexesAbove.Count; i++)
                    {
                        initialPosition = indexesAbove[i];
                        int aux1 = (int)((initialPosition.Y) / MyGlobals.blockSize);
                        for (int j = 0; j < aux1; j++)
                        {
                            initialPosition.Y -= MyGlobals.blockSize;
                            aux = new Sprite(this, @"Images\BlockVision", initialPosition);
                            hiddenAbove.Add(aux);
                            Components.Add(aux);
                        }
                    }

                }
                else if (hiddenAbove.Count > 0)
                {
                    for (int i = 0; i < hiddenAbove.Count; i++)
                    {
                        Components.Remove(hiddenAbove[i]);
                        hiddenAbove.RemoveAt(i);
                    }
                }
                //down
                if (visionBlocked(blockSight, 'd'))
                {
                    for (int i = 0; i < indexesBellow.Count; i++)
                    {
                        initialPosition = indexesBellow[i];
                        int aux1 = (int)((MyGlobals.heigh - initialPosition.Y) / MyGlobals.blockSize);
                        for (int j = 0; j < aux1; j++)
                        {
                            initialPosition.Y += MyGlobals.blockSize;
                            aux = new Sprite(this, @"Images\BlockVision", initialPosition);
                            hiddenBellow.Add(aux);
                            Components.Add(aux);
                        }
                    }

                }
                else if (hiddenBellow.Count > 0)
                {
                    for (int i = 0; i < hiddenBellow.Count; i++)
                    {
                        Components.Remove(hiddenBellow[i]);
                        hiddenBellow.RemoveAt(i);
                    }
                }


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
            //Kicks the body parts
            if (started && body[2].change)
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
            //Throws the body parts
            if (started && body[1].change)
            {
                if (keyboard.IsKeyDown(Keys.LeftShift) && keyboard.IsKeyDown(Keys.Down))
                {
                    if (body[1].kick(body[0], 'd'))
                        body[0].Position = new Vector2(body[0].Position.X, body[0].Position.Y + MyGlobals.blockSize * 2);
                    else if (body[1].kick(body[2], 'd'))
                        body[2].Position = new Vector2(body[2].Position.X, body[2].Position.Y + MyGlobals.blockSize * 2);
                }
                else if (keyboard.IsKeyDown(Keys.LeftShift) && keyboard.IsKeyDown(Keys.Up))
                {
                    if (body[1].kick(body[0], 'u'))
                        body[0].Position = new Vector2(body[0].Position.X, body[0].Position.Y - MyGlobals.blockSize * 2);
                    else if (body[1].kick(body[2], 'u'))
                        body[2].Position = new Vector2(body[2].Position.X, body[2].Position.Y - MyGlobals.blockSize * 2);
                }
                else if (keyboard.IsKeyDown(Keys.LeftShift) && keyboard.IsKeyDown(Keys.Left))
                {
                    if (body[1].kick(body[0], 'l'))
                        body[0].Position = new Vector2(body[0].Position.X - MyGlobals.blockSize * 2, body[0].Position.Y);
                    else if (body[1].kick(body[2], 'l'))
                        body[2].Position = new Vector2(body[2].Position.X - MyGlobals.blockSize * 2, body[2].Position.Y);
                }
                else if (keyboard.IsKeyDown(Keys.LeftShift) && keyboard.IsKeyDown(Keys.Right))
                {
                    if (body[1].kick(body[0], 'r'))
                        body[0].Position = new Vector2(body[0].Position.X + MyGlobals.blockSize * 2, body[0].Position.Y);
                    else if (body[1].kick(body[2], 'r'))
                        body[2].Position = new Vector2(body[2].Position.X + MyGlobals.blockSize * 2, body[2].Position.Y);
                }

                if (body[0].Position.X > graphics.PreferredBackBufferWidth - MyGlobals.blockSize)
                    body[0].Position = new Vector2(graphics.PreferredBackBufferWidth - MyGlobals.blockSize + MyGlobals.blockSize / 2, body[0].Position.Y);
                if (body[2].Position.X > graphics.PreferredBackBufferWidth - MyGlobals.blockSize)
                    body[2].Position = new Vector2(graphics.PreferredBackBufferWidth - MyGlobals.blockSize + MyGlobals.blockSize / 2, body[2].Position.Y);

                if (body[0].Position.Y > graphics.PreferredBackBufferHeight - MyGlobals.blockSize)
                    body[0].Position = new Vector2(body[2].Position.X, graphics.PreferredBackBufferHeight - MyGlobals.blockSize + MyGlobals.blockSize / 2);
                if (body[2].Position.Y > graphics.PreferredBackBufferHeight - MyGlobals.blockSize)
                    body[2].Position = new Vector2(body[2].Position.X, graphics.PreferredBackBufferHeight - MyGlobals.blockSize + MyGlobals.blockSize / 2);

                if (body[0].Position.X < MyGlobals.blockSize)
                    body[0].Position = new Vector2(MyGlobals.blockSize + MyGlobals.blockSize / 2, body[0].Position.Y);
                if (body[2].Position.X < MyGlobals.blockSize)
                    body[2].Position = new Vector2(MyGlobals.blockSize + MyGlobals.blockSize / 2, body[2].Position.Y);

                if (body[0].Position.Y < MyGlobals.blockSize)
                    body[0].Position = new Vector2(body[2].Position.X, MyGlobals.blockSize + MyGlobals.blockSize / 2);
                if (body[2].Position.Y < MyGlobals.blockSize)
                    body[2].Position = new Vector2(body[2].Position.X, MyGlobals.blockSize + MyGlobals.blockSize / 2);
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

               for (int i = 0; i < bridgePlates.Count; i++)
               {
                   if (bridgePlates[i].isPlatePressed(body[0].Position) || bridgePlates[i].isPlatePressed(body[1].Position) || bridgePlates[i].isPlatePressed(body[2].Position))
                   {
                       trench.Remove(brokenBridges[i]);
                   }
                   else if (trench.IndexOf(brokenBridges[i]) == -1)
                   {
                       trench.Add(brokenBridges[i]);
                   }
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
        //Fazer o esquema da visao
        public bool visionBlocked(List<Vector2> positions, char direction)
        {

            int i;

            switch (direction)
            {
                case 'u':
                    for (i = 0; i < positions.Count; i++)
                    {
                        if (body[0].Position.Y > positions[i].Y)
                        {
                            indexesAbove.Add(positions[i]);
                        }
                    }
                    if (indexesAbove.Count > 0)
                    {
                        body[0].Stoped = false;
                        return true;
                    }
                    break;
                case 'd':
                    for (i = 0; i < positions.Count; i++)
                    {
                        if (body[0].Position.Y < positions[i].Y)
                        {
                            indexesBellow.Add(positions[i]);
                        }
                    }
                    if (indexesBellow.Count > 0)
                    {
                        body[0].Stoped = false;
                        return true;
                    }
                    break;
                case 'r':
                    for (i = 0; i < positions.Count; i++)
                    {
                        if (body[0].Position.X < positions[i].X)
                        {
                            indexesRight.Add(positions[i]);
                        }
                    }
                    if (indexesRight.Count > 0)
                    {
                        body[0].Stoped = false;
                        return true;
                    }
                    break;
                case 'l':
                    for (i = 0; i < positions.Count; i++)
                    {
                        if (body[0].Position.X > positions[i].X)
                        {
                            indexesLeft.Add(positions[i]);
                        }
                    }
                    if (indexesLeft.Count > 0)
                    {
                        body[0].Stoped = false;
                        return true;
                    }
                    break;
            }

            return false;
        }

    }
}
