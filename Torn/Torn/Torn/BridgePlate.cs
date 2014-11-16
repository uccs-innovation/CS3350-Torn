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
    class BridgePlate : DrawableGameComponent
    {
        protected String textureFileBridge;
        protected String textureFilePlate;
        protected Texture2D bridge;
        protected Texture2D plate;
        protected Vector2 positionBridge;
        protected Vector2 positionPlate;
        protected Vector2 centerBridge;
        protected Vector2 centerPlate;
        protected Color color; 
        protected Random random;
        bool platePressed;
        Rectangle bridgeRecOn, bridgeRecOff;

        public Vector2 PositionBridge
        {
            get { return positionBridge; }
            set { positionBridge = value; }
        }

        public bool PlatePressed
        {
            get { return platePressed; }
            set { platePressed = value; }
        }

        public BridgePlate(Game game, String textureFileBridge, Vector2 positionBridge, String textureFilePlate, Vector2 positionPlate)
            : base(game)
        {
            this.textureFileBridge = textureFileBridge;
            this.textureFilePlate = textureFilePlate;
            this.positionBridge = positionBridge;
            this.positionPlate = positionPlate;
            color = Color.White;
            random = new Random();
            platePressed = false;
            int aux = (int) MyGlobals.realBlockSize;
            bridgeRecOff = new Rectangle(aux, 0, aux, aux);
            bridgeRecOn = new Rectangle(0, 0, aux, aux);
            LoadContent();

        }

        protected override void LoadContent()
        {
            bridge = Game.Content.Load<Texture2D>(textureFileBridge);
            plate = Game.Content.Load<Texture2D>(textureFilePlate);
            centerBridge = new Vector2(bridge.Width * 0.25f, bridge.Height * 0.5f);
            centerPlate = new Vector2(plate.Width * 0.5f, plate.Height * 0.5f);
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
            if(platePressed)
                sb.Draw(bridge, positionBridge, bridgeRecOn, color, 0f, centerBridge, MyGlobals.scale, SpriteEffects.None, 0f);
            else
                sb.Draw(bridge, positionBridge, bridgeRecOff, color, 0f, centerBridge , MyGlobals.scale, SpriteEffects.None, 0f);
            sb.Draw(plate, positionPlate, null, color, 0f, centerPlate, MyGlobals.scale, SpriteEffects.None, 0f);
            sb.End();

            base.Draw(gameTime);
        }

        public bool isPlatePressed(Vector2 position)
        {
            if (position == this.positionPlate)
            {
                platePressed = true;
                return true;
            }
            else
            {
                platePressed = false;
                return false;
            }
                
        }
    }
}
