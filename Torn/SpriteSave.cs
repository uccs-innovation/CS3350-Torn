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
    class SpriteSave
    {
        public String textureFile;
        public Vector2 position;
        public Vector2 center;
        public Color color;

        public SpriteSave(String textureFile, Vector2 position, Vector2 center, Color color)
        {
            this.textureFile = textureFile;
            this.position = position;
            this.center = center;
            this.color = color;
        }

        public void SaveToFile(String saveFileName, Sprite sprite)
        {
            if (sprite != null)
            {
                Stream saveFileStream = File.Create(saveFileName);
                BinaryFormatter serializer = new BinaryFormatter();
                SpriteSave spriteSave = sprite.SaveSprite();
                serializer.Serialize(saveFileStream, spriteSave);
                saveFileStream.Close();
            }
        }

        public void LoadFromFile(String loadFileName, ref Sprite sprite)
        {
            if (File.Exists(loadFileName))
            {
                Stream loadFileStream = File.OpenRead(loadFileName);
                BinaryFormatter deserializer = new BinaryFormatter();
                SpriteSave spriteSave = (SpriteSave)deserializer.Deserialize(loadFileStream);
                sprite.LoadSprite(spriteSave);
                loadFileStream.Close();
            }
        }
        
    }
}
