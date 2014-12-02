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
    class UserControlledSpriteSave : SpriteSave
    {
        public UserControlledSpriteSave(String textureFile, Vector2 position, Vector2 center, Color color)
            : base(textureFile, position, center, color)
        {
           
        }

        public void SaveToFile(String saveFileName)
        {
            Stream saveFileStream = File.Create(saveFileName);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(saveFileStream, this);
            saveFileStream.Close();
        }

        public static void LoadFromFile(String loadFileName, ref UserControlledSprite sprite)
        {
            if (File.Exists(loadFileName))
            {
                Stream loadFileStream = File.OpenRead(loadFileName);
                BinaryFormatter deserializer = new BinaryFormatter();
                UserControlledSpriteSave spriteSave = (UserControlledSpriteSave)deserializer.Deserialize(loadFileStream);
                sprite.LoadSprite(spriteSave);
                loadFileStream.Close();
            }
        }
    }
}
