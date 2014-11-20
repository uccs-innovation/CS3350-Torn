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
        Sprite blindness, highlightedGrass;
        int[,] field;
        List<Vector2> blockSight;
        List<Sprite> trench;
        List<Sprite> obstacles;
        List<BridgePlate> bridgePlates;
        List<Vector2> bridgesPosition;
        List<Vector2> platesPosition;
        List<Sprite> brokenBridges;
        List<Sprite> grasses;
        List<Sprite> walls;
        List<Sprite> plates;
        List<Sprite> bridges;
        int[] finished;
        Sprite aux, textBox;
        List<Vector2> indexesBellow, indexesRight, indexesLeft, indexesAbove;
        List<Sprite> hiddenAbove, hiddenBellow, hiddenRight, hiddenLeft;
        Song ambientSound;
        SoundEffect kick, nice, pressurePlateOn, thrownHead, didIt;
        Text text;
        String tutorial;
        int countBellow, countAbove, countLeft, countRight, messageIndex;
        KeyboardState old;
        int levelNumber, levelAux, bridgesNumber;
        Sprite enter;
        float counter;
        List<Sprite> bridgeOn, bridgeOff;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //graphics.IsFullScreen = true;

            ambientSound = Content.Load<Song>(@"Sounds\ambientSound");
            MediaPlayer.Play(ambientSound);
            MediaPlayer.IsRepeating = true;



            counter = 300;

            //Initialize all the global variables
            MyGlobals.width = 480 * 2;
            MyGlobals.heigh = 420 * 2;
            MyGlobals.realBlockSize = 30;
            MyGlobals.blockSize = MyGlobals.realBlockSize * 2;
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
            MyGlobals.numberOfBlocksX = 16;
            MyGlobals.numberOfBlocksY = 14;

            tutorial = "Welcome to Torn. Currently, your body is separated into three pieces. You \ncan alternate between these parts pressing A to select Arms, H to select \nhead and L to select legs.";

            graphics.PreferredBackBufferWidth = MyGlobals.width;
            graphics.PreferredBackBufferHeight = MyGlobals.heigh;

            messageIndex = 0;
            levelNumber = 1;
            levelAux = levelNumber;
            bridgesNumber = 0;
            
            body = new UserControlledSprite[3];
            started = false;
            field = new int[MyGlobals.numberOfBlocksY, MyGlobals.numberOfBlocksX];
            trench = new List<Sprite>();
            obstacles = new List<Sprite>();
            bridgePlates = new List<BridgePlate>();
            bridgesPosition = new List<Vector2>();
            platesPosition = new List<Vector2>();
            brokenBridges = new List<Sprite>();
            blockSight = new List<Vector2>();
            walls = new List<Sprite>();
            plates = new List<Sprite>();
            bridges = new List<Sprite>();
            bridgeOn = new List<Sprite>();
            bridgeOff = new List<Sprite>();

            indexesAbove = new List<Vector2>();
            indexesBellow = new List<Vector2>();
            indexesLeft = new List<Vector2>();
            indexesRight = new List<Vector2>();

            hiddenAbove = new List<Sprite>();
            hiddenBellow = new List<Sprite>();
            hiddenLeft = new List<Sprite>();
            hiddenRight = new List<Sprite>();

            finished = new int[3] { 0, 0, 0 };
            

            old = new KeyboardState();

        }
        protected override void Initialize()
        {
            //Makes the mouse pointer visible
            this.IsMouseVisible = true;
            base.Initialize();
        }
        public void readField(String levelName)
        {
            string line;
            int i = 0, j = 0, k = 0, lastJ = 0, lastI = 0, result = 0, rest = 0;

            System.IO.StreamReader file = new System.IO.StreamReader(@"Levels\level"+levelName+".txt");

            while ((line = file.ReadLine()) != null)
            {
                while (k < line.Length)
                {
                    if (k == line.Length)
                    {
                        field[i, j] = int.Parse(line[k].ToString());
                        j++;
                        k++;
                    }
                    else if(k < line.Length - 1 && line[k+1] == ' ')
                    {
                        field[i, j] = int.Parse(line[k].ToString());
                        j++;
                        k++;
                    }
                    else if(k == line.Length - 1)
                    {
                        field[i, j] = int.Parse(line[k].ToString());
                        j++;
                        k++;
                    }
                    else if(line[k] == ' ')
                    {
                        k++;
                    }
                    else
                    {
                        field[i, j] = int.Parse(line[k].ToString() + line[k + 1].ToString());
                        j++;
                        k = k + 2;
                    }
                }
                lastJ = j;
                j = 0;
                k = 0;
                i++;
            }

            lastI = i;

            if (lastJ < MyGlobals.numberOfBlocksX)
            {
                int aux = MyGlobals.numberOfBlocksX - lastJ;

                result = aux / 2;
                rest = aux - result;

                for (i = 0; i < MyGlobals.numberOfBlocksY; i++)
                {
                    for (j = MyGlobals.numberOfBlocksX - rest -1; j >= result; j--)
                    {
                        field[i, j] = field[i, j - result];
                    }
                }

                for (i = 0; i < MyGlobals.numberOfBlocksY; i++)
                {
                    for (j = 0; j < result; j++)
                    {
                        field[i, j] = MyGlobals.outOfSize;
                    }
                    for (j = MyGlobals.numberOfBlocksX -1; j > MyGlobals.numberOfBlocksX - rest - 1; j--)
                    {
                        field[i, j] = MyGlobals.outOfSize;
                    }
                }
            }
            MyGlobals.zeroX = MyGlobals.blockSize * result;
            MyGlobals.realWidth = MyGlobals.blockSize * lastJ + MyGlobals.blockSize * result;

            if (lastI < MyGlobals.numberOfBlocksY)
            {
                int aux = MyGlobals.numberOfBlocksY - lastI;
                
                result = aux / 2;
                rest = aux - result;

                for (i = 0; i < MyGlobals.numberOfBlocksX; i++)
                {
                    for (j = MyGlobals.numberOfBlocksY - rest - 1; j >= result; j--)
                    {
                        field[j, i] = field[j - result, i];
                    }
                }

                for (i = 0; i < MyGlobals.numberOfBlocksX; i++)
                {
                    for (j = 0; j < result; j++)
                    {
                        field[j, i] = MyGlobals.outOfSize;
                    }
                    for (j = MyGlobals.numberOfBlocksY - 1; j > MyGlobals.numberOfBlocksY - rest - 1; j--)
                    {
                        field[j, i] = MyGlobals.outOfSize;
                    }
                }
            }

            MyGlobals.zeroY = MyGlobals.blockSize * result;
            MyGlobals.realHeigh = MyGlobals.blockSize * lastI + MyGlobals.blockSize * result;
            file.Close();

            using (TextWriter tw = new StreamWriter(@"Levels\levelTeste.txt"))
            {
                for (j = 0; j < MyGlobals.numberOfBlocksY; j++)
                {
                    for (i = 0; i < MyGlobals.numberOfBlocksX; i++)
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
            
            spriteBatch = new SpriteBatch(GraphicsDevice);

            

            readField(levelNumber.ToString());
            highlightedGrass = new Sprite(this, @"Images\highlightedGrass", new Vector2(0, 0));
            grasses = new List<Sprite>();

            if (!started)
            {
                kick = Content.Load<SoundEffect>(@"Sounds\Kick");
                nice = Content.Load<SoundEffect>(@"Sounds\Nice");
                pressurePlateOn = Content.Load<SoundEffect>(@"Sounds\PressurePlateOn");
                thrownHead = Content.Load<SoundEffect>(@"Sounds\ThrownHead");
                didIt = Content.Load<SoundEffect>(@"Sounds\didIt");

                initialPosition.X = graphics.PreferredBackBufferWidth / 2;
                initialPosition.Y = graphics.PreferredBackBufferHeight / 2;
                initialScreen = new Sprite(this, @"Images\InitialScreen", initialPosition);
                Components.Add(initialScreen);
            }
            else if(levelFinished())
            {
                for (int i = Components.Count -1; i > 0 ; i--)
                {
                    Components.RemoveAt(i);
                }

                trench = new List<Sprite>();
                obstacles = new List<Sprite>();
                bridgesPosition = new List<Vector2>();
                platesPosition = new List<Vector2>();
                brokenBridges = new List<Sprite>();
                blockSight = new List<Vector2>();
                walls = new List<Sprite>();
                plates = new List<Sprite>();
                bridges = new List<Sprite>();
                bridgeOn = new List<Sprite>();
                grasses = new List<Sprite>();

                indexesAbove = new List<Vector2>();
                indexesBellow = new List<Vector2>();
                indexesLeft = new List<Vector2>();
                indexesRight = new List<Vector2>();

                hiddenAbove = new List<Sprite>();
                hiddenBellow = new List<Sprite>();
                hiddenLeft = new List<Sprite>();
                hiddenRight = new List<Sprite>();


                finished[0] = 0;
                finished[1] = 0;
                finished[2] = 0;
                readField(levelNumber.ToString());
            }
            
            if (started)
            {
                //Takes the initial screen off and displays the body parts
                Components.Remove(initialScreen);

                for (int i = 0; i < MyGlobals.numberOfBlocksY; i++)
                {
                    for (int j = 0; j < MyGlobals.numberOfBlocksX; j++)
                    {
                        initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                        initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                        aux = new Sprite(this, @"Images\grass", initialPosition);
                        grasses.Add(aux);
                        Components.Add(aux);
                    }
                }

                for (int i = 0; i < MyGlobals.numberOfBlocksY; i++)
                {
                    for (int j = 0; j < MyGlobals.numberOfBlocksX; j++)
                    {
                        if (field[i, j] == MyGlobals.outOfSize)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            aux = new Sprite(this, @"Images\edge", initialPosition);
                            Components.Add(aux);
                            walls.Add(aux);
                        }
                    }
                }

                for (int i = 0; i < MyGlobals.numberOfBlocksY; i++)
                {
                    for (int j = 0; j < MyGlobals.numberOfBlocksX; j++)
                    {
                        if (field[i, j] > MyGlobals.plates * 10 && field[i, j] / 10 == MyGlobals.plates)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            aux = new Sprite(this, @"Images\pressureplate_grass", initialPosition);
                            
                            for (int k = 0; k < MyGlobals.numberOfBlocksY; k++)
                            {
                                for (int l = 0; l < MyGlobals.numberOfBlocksX; l++)
                                {
                                    if (field[k, l] / 10 == MyGlobals.bridges && field[k, l] % 10 == field[i, j] % 10)
                                    {
                                        initialPosition.X = (l + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                                        initialPosition.Y = (k + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                                        aux.EquivBridges.Add(initialPosition);
                                    }
                                }
                            }
                            plates.Add(aux);
                        }
                        else if (field[i, j] == MyGlobals.plates)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            aux = new Sprite(this, @"Images\pressureplate_grass", initialPosition);
                            for (int k = 0; k < MyGlobals.numberOfBlocksY; k++)
                            {
                                for (int l = 0; l < MyGlobals.numberOfBlocksX; l++)
                                {
                                    if (field[k, l] / 10 == MyGlobals.bridges)
                                    {
                                        initialPosition.X = (l + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                                        initialPosition.Y = (k + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                                        aux.EquivBridges.Add(initialPosition);
                                    }
                                }
                            }
                            plates.Add(aux);
                        }
                    }
                }

                for (int i = 0; i < plates.Count; i++)
                {
                    for (int j = 0; j < plates[i].EquivBridges.Count; j++)
                    {
                        aux = new Sprite(this, @"Images\bridge", plates[i].EquivBridges[j]);
                        aux.Rectangle = new Rectangle(0, 0, (int)MyGlobals.realBlockSize, (int)MyGlobals.realBlockSize);
                        walls.Add(aux);
                        bridges.Add(aux); 
                        Components.Add(aux);
                    }
                    Components.Add(plates[i]);
                }

                for (int i = 0; i < MyGlobals.numberOfBlocksY; i++)
                {
                    for (int j = 0; j < MyGlobals.numberOfBlocksX; j++)
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
                for (int i = 0; i < MyGlobals.numberOfBlocksY; i++)
                {
                    for (int j = 0; j < MyGlobals.numberOfBlocksX; j++)
                    {
                        if (field[i, j] == MyGlobals.obstacle)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            aux = new Sprite(this, @"Images\Block", initialPosition);
                            aux.Rectangle = new Rectangle(0, 0, (int)MyGlobals.realBlockSize, (int)MyGlobals.realBlockSize);
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

                for (int i = 0; i < MyGlobals.numberOfBlocksY; i++)
			    {
                    for (int j = 0; j < MyGlobals.numberOfBlocksX; j++)
			        {
                        if (field[i, j] == MyGlobals.head)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            body[0] = new UserControlledSprite(this, @"Images\Head", initialPosition, false, false);
                            Components.Add(body[0]);
                        }
                        if (field[i, j] == MyGlobals.arms)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            initialPosition.Y = (i+ 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            body[1] = new UserControlledSprite(this, @"Images\Arms", initialPosition, true, false);
                            Components.Add(body[1]);
                        }
                        if (field[i, j] == MyGlobals.legs)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize/2;
                            body[2] = new UserControlledSprite(this, @"Images\Legs", initialPosition, false , true);
                            Components.Add(body[2]);
                        }
                    }
                }

                for (int i = 0; i < MyGlobals.numberOfBlocksY; i++)
                {
                    for (int j = 0; j < MyGlobals.numberOfBlocksX; j++)
                    {
                        if (field[i, j] == MyGlobals.blocksVision)
                        {
                            initialPosition.X = (j + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            initialPosition.Y = (i + 1) * MyGlobals.blockSize - MyGlobals.blockSize / 2;
                            aux = new Sprite(this, @"Images\BlockVision", initialPosition);
                            blockSight.Add(aux.Position);
                            //trench.Add(aux);
                            walls.Add(aux);
                            Components.Add(aux);
                        }
                    }
                }

                //Initializes the blind sprinte with the same position of the head
               blindness = new Sprite(this, @"Images\Blindness", new Vector2(body[0].Position.X, body[0].Position.Y));

                body[0].Trench = trench;
                body[1].Trench = trench;
                body[2].Trench = trench;

                body[0].Walls = walls;
                body[1].Walls = walls;
                body[2].Walls = walls;

                body[0].Walls.Add(body[1]);
                body[0].Walls.Add(body[2]);

                body[1].Walls.Add(body[0]);
                body[1].Walls.Add(body[2]);

                body[2].Walls.Add(body[0]);
                body[2].Walls.Add(body[1]);


                body[1].Obstacles = obstacles;

                //Components.Add(blindness);
                //TUTORIAL
                if(levelNumber == 1)
                {
                    textBox = new Sprite(this, @"Images\blankBox", new Vector2(MyGlobals.width / 2, MyGlobals.blockSize * 2));
                    Components.Add(textBox);
                    text = new Text(this, new Vector2(20f, 80f), @"Verdana", tutorial, Color.Black);
                    Components.Add(text);
                    enter = new Sprite(this, @"Images\enter", new Vector2(MyGlobals.width - MyGlobals.blockSize / 2 , 3 * MyGlobals.blockSize - MyGlobals.blockSize / 2));
                }
                

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

            //Waits for the first ENTER to initialize the game
            if (keyboard.IsKeyDown(Keys.Enter) && !started)
            {
                started = true;
                //Call the method LoadContent() in order to initialize the body parts
                LoadContent();
            }
             
            if(started)
            {
                body[0].Obstacles = body[1].Obstacles;
                body[2].Obstacles = body[1].Obstacles;

                counter -= gameTime.ElapsedGameTime.Seconds;
            }

            //Tutorial
            if (started && levelNumber == 1)
            {
                if (body[0].Position == new Vector2(4 * MyGlobals.blockSize - MyGlobals.blockSize / 2, 8 * MyGlobals.blockSize - MyGlobals.blockSize / 2))
                {
                    switch (messageIndex)
                    {
                        case 1:
                            text.TextContent = "Welcome to Torn. Currently, your body is separated into three pieces. You \ncan alternate between these parts pressing A to select Arms, H to select \nhead and L to select legs. You can also restart the level pressing R.";
                            if(Components.IndexOf(enter) == -1)    
                                Components.Add(enter);
                            break;
                        case 2:
                            text.TextContent = "As you can see, some areas of the maze are invisible and you will have to \nmove the head in order to see them.";
                            break;
                        case 3:
                            text.TextContent = "Press H to select the head and then use the arrow keys to move it to the \nhighlighted area.";
                            Components.Remove(enter);
                            for (int i = 0; i < grasses.Count; i++)
                            {
                                if (grasses[i].Position == new Vector2(7 * MyGlobals.blockSize + MyGlobals.blockSize / 2, 8 * MyGlobals.blockSize + MyGlobals.blockSize / 2))
                                    grasses[i].Color = Color.LightSeaGreen;
                            }
                            messageIndex = 4;
                            break;
                    }

                    if (!keyboard.IsKeyDown(Keys.Enter) && old.IsKeyDown(Keys.Enter) && messageIndex < 3)
                        messageIndex++;

                    old = keyboard;

                }
                else if (body[0].Position == new Vector2(7 * MyGlobals.blockSize + MyGlobals.blockSize / 2, 8 * MyGlobals.blockSize + MyGlobals.blockSize / 2))
                {
                    for (int i = 0; i < grasses.Count; i++)
                    {
                        if (grasses[i].Position == new Vector2(7 * MyGlobals.blockSize + MyGlobals.blockSize / 2, 8 * MyGlobals.blockSize + MyGlobals.blockSize / 2))
                            grasses[i].Color = Color.White;
                    }
                    switch (messageIndex)
                    {
                        case 4:
                            text.TextContent = "Now you can see and control your arms. Arms are able to push and pull \nblocks. They can also throw body parts. To throw or pull using your arms, \npress the Left Shift and the direction arrow desired.";
                            if(Components.IndexOf(enter) == -1)
                                Components.Add(enter);
                            break;
                        case 5:
                            text.TextContent = "Now, press A to select the arms and then move them to the highlighted \narea.";
                            Components.Remove(enter);
                            for (int i = 0; i < grasses.Count; i++)
                            {
                                if (grasses[i].Position == new Vector2(7 * MyGlobals.blockSize + MyGlobals.blockSize / 2, 7 * MyGlobals.blockSize + MyGlobals.blockSize / 2))
                                    grasses[i].Color = Color.LightSeaGreen;
                            }
                            break;
                    }

                    if (!keyboard.IsKeyDown(Keys.Enter) && old.IsKeyDown(Keys.Enter) && messageIndex > 3 && messageIndex < 5)
                        messageIndex++;

                    old = keyboard;
                }
                else if (body[0].Position == new Vector2(10 * MyGlobals.blockSize + MyGlobals.blockSize / 2, 7 * MyGlobals.blockSize + MyGlobals.blockSize / 2))
                {
                    for (int i = 0; i < grasses.Count; i++)
                    {
                        if (grasses[i].Position == new Vector2(8 * MyGlobals.blockSize + MyGlobals.blockSize / 2, 7 * MyGlobals.blockSize + MyGlobals.blockSize / 2))
                            grasses[i].Color = Color.White;
                    }
                    text.TextContent = "Now place the head on the pressure plate in order to restore the bridge \non the top of the level. Doing that you will be able to jump your legs \nover the trenches";
                }
                else if (body[0].Position == new Vector2(11 * MyGlobals.blockSize + MyGlobals.blockSize / 2, 6 * MyGlobals.blockSize + MyGlobals.blockSize / 2) && body[2].Position == new Vector2(4 * MyGlobals.blockSize + MyGlobals.blockSize / 2, 4 * MyGlobals.blockSize + MyGlobals.blockSize / 2))
                {
                    text.TextContent = "Excellent! Now press L to select the legs and then jump over the trenches \npressing Left shift and the direction arrows.";
                }


                if (body[1].Position == new Vector2(7 * MyGlobals.blockSize + MyGlobals.blockSize / 2, 7 * MyGlobals.blockSize + MyGlobals.blockSize / 2))
                {
                    for (int i = 0; i < grasses.Count; i++)
                    {
                        if (grasses[i].Position == new Vector2(7 * MyGlobals.blockSize + MyGlobals.blockSize / 2, 7 * MyGlobals.blockSize + MyGlobals.blockSize / 2))
                            grasses[i].Color = Color.White;
                    }
                    for (int i = 0; i < grasses.Count; i++)
                    {
                        if (grasses[i].Position == new Vector2(8 * MyGlobals.blockSize + MyGlobals.blockSize / 2, 7 * MyGlobals.blockSize + MyGlobals.blockSize / 2))
                            grasses[i].Color = Color.LightSeaGreen;
                    }
                    text.TextContent = "Now move the head to the highlighted area and then use the arm to throw it \nover the broken bridge.";
                }

                if (body[2].Position == new Vector2(8 * MyGlobals.blockSize + MyGlobals.blockSize / 2, 4 * MyGlobals.blockSize + MyGlobals.blockSize / 2))
                {
                    text.TextContent = "Now, place the legs on the other pressure plate at the botton of the level. \nDoing this, will enable you to join all of your body parts \nat the final portal.";
                }

            }

            
            //Checks which part will move according with the user input, if the game is already started and if there is any other body part moving
            if (keyboard.IsKeyDown(Keys.H) && started && !isBodyMoving())
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
                    {
                        body[0].Position = new Vector2(body[0].Position.X, body[0].Position.Y + MyGlobals.blockSize * 2);
                        thrownHead.Play();
                    }
                    else if (body[2].kick(body[1], 'd'))
                    {
                        body[1].Position = new Vector2(body[1].Position.X, body[1].Position.Y + MyGlobals.blockSize * 2);
                            kick.Play();
                    }
                }
                else if (keyboard.IsKeyDown(Keys.LeftShift) && keyboard.IsKeyDown(Keys.Up))
                {
                    if (body[2].kick(body[0], 'u'))
                    {
                        body[0].Position = new Vector2(body[0].Position.X, body[0].Position.Y - MyGlobals.blockSize * 2);
                        thrownHead.Play();
                    }
                    else if (body[2].kick(body[1], 'u'))
                    {
                        body[1].Position = new Vector2(body[1].Position.X, body[1].Position.Y - MyGlobals.blockSize * 2);
                        kick.Play();
                    }
                }
                else if (keyboard.IsKeyDown(Keys.LeftShift) && keyboard.IsKeyDown(Keys.Left))
                {
                    if (body[2].kick(body[0], 'l'))
                    {
                        body[0].Position = new Vector2(body[0].Position.X - MyGlobals.blockSize * 2, body[0].Position.Y);
                        thrownHead.Play();
                    }
                    else if (body[2].kick(body[1], 'l'))
                    {
                        body[1].Position = new Vector2(body[1].Position.X - MyGlobals.blockSize * 2, body[1].Position.Y);
                        kick.Play();
                    }
                }
                else if (keyboard.IsKeyDown(Keys.LeftShift) && keyboard.IsKeyDown(Keys.Right))
                {
                    if (body[2].kick(body[0], 'r'))
                    {
                        body[0].Position = new Vector2(body[0].Position.X + MyGlobals.blockSize * 2, body[0].Position.Y);
                        thrownHead.Play();
                    }
                    else if (body[2].kick(body[1], 'r'))
                    {
                        body[1].Position = new Vector2(body[1].Position.X + MyGlobals.blockSize * 2, body[1].Position.Y);
                        kick.Play();
                    }
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
                    {
                        body[0].Position = new Vector2(body[0].Position.X, body[0].Position.Y + MyGlobals.blockSize * 2);
                        thrownHead.Play();
                    }
                    else if (body[1].kick(body[2], 'd'))
                        body[2].Position = new Vector2(body[2].Position.X, body[2].Position.Y + MyGlobals.blockSize * 2);
                }
                else if (keyboard.IsKeyDown(Keys.LeftShift) && keyboard.IsKeyDown(Keys.Up))
                {
                    if (body[1].kick(body[0], 'u'))
                    {
                        body[0].Position = new Vector2(body[0].Position.X, body[0].Position.Y - MyGlobals.blockSize * 2);
                        thrownHead.Play();
                    }
                    else if (body[1].kick(body[2], 'u'))
                        body[2].Position = new Vector2(body[2].Position.X, body[2].Position.Y - MyGlobals.blockSize * 2);
                }
                else if (keyboard.IsKeyDown(Keys.LeftShift) && keyboard.IsKeyDown(Keys.Left))
                {
                    if (body[1].kick(body[0], 'l'))
                    {
                        body[0].Position = new Vector2(body[0].Position.X - MyGlobals.blockSize * 2, body[0].Position.Y);
                        thrownHead.Play();
                    }
                    else if (body[1].kick(body[2], 'l'))
                        body[2].Position = new Vector2(body[2].Position.X - MyGlobals.blockSize * 2, body[2].Position.Y);
                }
                else if (keyboard.IsKeyDown(Keys.LeftShift) && keyboard.IsKeyDown(Keys.Right))
                {
                    if (body[1].kick(body[0], 'r'))
                    {
                        body[0].Position = new Vector2(body[0].Position.X + MyGlobals.blockSize * 2, body[0].Position.Y);
                        thrownHead.Play();
                    }
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
                int count;
                indexesBellow = body[0].hidden(blockSight, 'd');
                indexesRight = body[0].hidden(blockSight, 'r');
                indexesLeft = body[0].hidden(blockSight, 'l');
                indexesAbove = body[0].hidden(blockSight, 'u');

                //Bellow
                if (indexesBellow.Count > 0)
                {
                    for (int i = 0; i < indexesBellow.Count; i++)
                    {
                        bool exist = false;
                        initialPosition = indexesBellow[i];
                        count = (int)((MyGlobals.realHeigh - initialPosition.Y) / MyGlobals.blockSize);
                        for (int j = 0; j < count; j++)
                        {
                            initialPosition.Y += MyGlobals.blockSize;
                            aux = new Sprite(this, @"Images\BlockVision", initialPosition);
                            for (int k = 0; k < hiddenBellow.Count; k++)
                            {
                                if (hiddenBellow[k].Position == aux.Position)
                                {
                                    exist = true;
                                }
                            }
                            if (!exist)
                            {
                                hiddenBellow.Add(aux);
                                Components.Add(aux);
                                countBellow = indexesBellow.Count;
                            }

                        }
                    }
                }
                if (indexesBellow.Count < countBellow && hiddenBellow.Count > 0)
                {
                    count = hiddenBellow.Count;
                    for (int i = 0; i < count; i++)
                    {
                        Components.Remove(hiddenBellow[i]);
                    }
                    for (int i = count - 1; i >= 0; i--)
                    {
                        hiddenBellow.RemoveAt(i);
                    }
                }

                //Above
                if (indexesAbove.Count > 0)
                {
                    for (int i = 0; i < indexesAbove.Count; i++)
                    {
                        bool exist = false;
                        initialPosition = indexesAbove[i];
                        count = (int)((initialPosition.Y - MyGlobals.zeroY) / MyGlobals.blockSize);
                        for (int j = 0; j < count; j++)
                        {
                            initialPosition.Y -= MyGlobals.blockSize;
                            aux = new Sprite(this, @"Images\BlockVision", initialPosition);
                            for (int k = 0; k < hiddenAbove.Count; k++)
                            {
                                if (hiddenAbove[k].Position == aux.Position)
                                {
                                    exist = true;
                                }
                            }
                            if (!exist)
                            {
                                hiddenAbove.Add(aux);
                                Components.Add(aux);
                                countAbove = indexesAbove.Count;
                            }

                        }
                    }
                }
                if (indexesAbove.Count < countAbove && hiddenAbove.Count > 0)
                {
                    count = hiddenAbove.Count;
                    for (int i = 0; i < count; i++)
                    {
                        Components.Remove(hiddenAbove[i]);
                    }
                    for (int i = count - 1; i >= 0; i--)
                    {
                        hiddenAbove.RemoveAt(i);
                    }
                }

                //Left
                if (indexesLeft.Count > 0)
                {
                    for (int i = 0; i < indexesLeft.Count; i++)
                    {
                        bool exist = false;
                        initialPosition = indexesLeft[i];
                        count = (int)((initialPosition.X - MyGlobals.zeroX) / MyGlobals.blockSize);
                        for (int j = 0; j < count; j++)
                        {
                            initialPosition.X -= MyGlobals.blockSize;
                            aux = new Sprite(this, @"Images\BlockVision", initialPosition);
                            for (int k = 0; k < hiddenLeft.Count; k++)
                            {
                                if (hiddenLeft[k].Position == aux.Position)
                                {
                                    exist = true;
                                }
                            }
                            if (!exist)
                            {
                                hiddenLeft.Add(aux);
                                Components.Add(aux);
                                countLeft = indexesLeft.Count;
                            }
                        }
                    }
                }
                if (indexesLeft.Count < countLeft && hiddenLeft.Count > 0)
                {
                    count = hiddenLeft.Count;
                    for (int i = 0; i < count; i++)
                    {
                        Components.Remove(hiddenLeft[i]);
                    }
                    for (int i = count - 1; i >= 0; i--)
                    {
                        hiddenLeft.RemoveAt(i);
                    }
                }

                //RIGHT
                if (indexesRight.Count > 0)
                {
                    for (int i = 0; i < indexesRight.Count; i++)
                    {
                        bool exist = false;
                        initialPosition = indexesRight[i];
                        count = (int)((MyGlobals.realWidth - initialPosition.X) / MyGlobals.blockSize);
                        for (int j = 0; j < count; j++)
                        {
                            initialPosition.X += MyGlobals.blockSize;
                            aux = new Sprite(this, @"Images\BlockVision", initialPosition);
                            for (int k = 0; k < hiddenRight.Count; k++)
                            {
                                if (hiddenRight[k].Position == aux.Position)
                                {
                                    exist = true;
                                }
                            }
                            if (!exist)
                            {
                                hiddenRight.Add(aux);
                                Components.Add(aux);
                                countRight = indexesRight.Count;
                            }
                        }
                    }
                }
                if (indexesRight.Count < countRight && hiddenRight.Count > 0)
                {
                    count = hiddenRight.Count;
                    for (int i = 0; i < count; i++)
                    {
                        Components.Remove(hiddenRight[i]);
                    }
                    for (int i = count - 1; i >= 0; i--)
                    {
                        hiddenRight.RemoveAt(i);
                    }
                }
            }

            if (started)
            {
                //Give to blindness sprite, the same position of the head
                blindness.Position = body[0].Position;

                if(finalized(body[0].Position))
                {
                    Components.Remove(body[0]);
                    body[1].Walls.Remove(body[0]);
                    body[2].Walls.Remove(body[0]);
                    finished[0] = 1;
                }
                if (finalized(body[1].Position))
                {
                    Components.Remove(body[1]);
                    body[0].Walls.Remove(body[1]);
                    body[2].Walls.Remove(body[1]);
                    finished[1] = 1;
                }
                if (finalized(body[2].Position))
                {
                    Components.Remove(body[2]);
                    body[0].Walls.Remove(body[2]);
                    body[1].Walls.Remove(body[2]);
                    finished[2] = 1;
                }

                if(started && keyboard.IsKeyDown(Keys.R))
                {

                    body[1].Walls.Remove(body[0]);
                    body[2].Walls.Remove(body[0]);
                    finished[0] = 1;

                    Components.Remove(body[1]);
                    body[0].Walls.Remove(body[1]);
                    body[2].Walls.Remove(body[1]);
                    finished[1] = 1;

                    Components.Remove(body[2]);
                    body[0].Walls.Remove(body[2]);
                    body[1].Walls.Remove(body[2]);
                    finished[2] = 1;


                    body[0].Obstacles = new List<Sprite>();
                    body[0].Trench = new List<Sprite>();
                    body[0].Walls = new List<Sprite>();
                    body[0].Bridge = new List<Sprite>();
                    body[0].Indexes = new List<Vector2>();
                    LoadContent();
                }

                if(levelFinished())
                {
                    body[0].Obstacles = new List<Sprite>();
                    body[0].Trench = new List<Sprite>();
                    body[0].Walls = new List<Sprite>();
                    body[0].Bridge = new List<Sprite>();
                    body[0].Indexes = new List<Vector2>();
                    levelNumber++;
                    if (levelNumber > 5)
                    {
                        this.Exit();
                    }
                    didIt.Play();
                    
                    LoadContent();                    
                }

                List<Vector2> auxList = platePressed();

                if (auxList.Count >= bridgesNumber)
                {
                    for (int i = 0; i < auxList.Count; i++)
                    {
                        for (int j = 0; j < bridges.Count; j++)
                        {
                            if(auxList[i] == bridges[j].Position)
                            {
                                bridges[j].Rectangle = new Rectangle((int)MyGlobals.realBlockSize, 0, (int)MyGlobals.realBlockSize, (int)MyGlobals.realBlockSize);
                            }
                        }
                    }
                    for (int i = 0; i < auxList.Count; i++)
                    {
                        for (int j = 0; j < walls.Count; j++)
                        {
                            if (walls[j].Position == auxList[i])
                            {
                                bridgeOn.Add(walls[j]);
                                walls.RemoveAt(j);
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < bridges.Count; i++)
                    {
                        bridges[i].Rectangle = new Rectangle(0, 0, (int)MyGlobals.realBlockSize, (int)MyGlobals.realBlockSize);
                    }
                    for (int i = 0; i < bridgeOn.Count; i++)
                    {
                        walls.Add(bridgeOn[i]);
                    }
                    bridgeOn = new List<Sprite>();
                }

                bridgesNumber = auxList.Count;
                if (started)
                {
                    body[0].Walls = walls;
                    body[1].Walls = walls;
                    body[2].Walls = walls;

                    body[0].Bridge = bridges;
                    body[1].Bridge = bridges;
                    body[2].Bridge = bridges;
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
            if (finished[0] != 0 && finished[1] != 0 && finished[2] != 0)
            {
                return true;
            }
            
            return false;
        }
        public List<Vector2> platePressed()
        {
            List<Vector2> temp = new List<Vector2>();
            for (int i = 0; i < plates.Count; i++)
            {
                if(plates[i].Position == body[0].Position || plates[i].Position == body[1].Position || plates[i].Position == body[2].Position)
                {
                    temp.AddRange(plates[i].EquivBridges);
                }
            }

            return temp;
        }
    }
}
