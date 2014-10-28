using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Torn
{
    [Serializable]
    class AnimatedSpriteSave: SpriteSave
    {
        public int directionX, directionY;

        public AnimatedSpriteSave(String textureFile, Vector2 position, Vector2 center, Color color, int directionX, int directionY):base(textureFile,position,center,color)
        {
            this.directionX = directionX;
            this.directionY = directionY;
        }

        public void SaveToFile(String saveFileName)
        {
            Stream saveFileStream = File.Create(saveFileName);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(saveFileStream, this);
            saveFileStream.Close();
        }

        public static void LoadFromFile(String loadFileName, ref AnimatedSprite sprite)
        {
            if (File.Exists(loadFileName))
            {
                Stream loadFileStream = File.OpenRead(loadFileName);
                BinaryFormatter deserializer = new BinaryFormatter();
                AnimatedSpriteSave spriteSave = (AnimatedSpriteSave)deserializer.Deserialize(loadFileStream);
                sprite.LoadSprite(spriteSave);
                loadFileStream.Close();
            }
        }
    }
}
