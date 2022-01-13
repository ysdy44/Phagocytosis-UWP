using System.Xml.Linq;

namespace Phagocytosis.Sprites
{
    public static partial class XML
    {

        public static XElement SaveSprite(string elementName, ISprite sprite)
        {
            XElement element = new XElement(elementName);

            // SaveWith
            {
                element.Add(new XAttribute("X", sprite.X));
                element.Add(new XAttribute("Y", sprite.Y));
                element.Add(new XAttribute("Level", sprite.Level));
                element.Add(new XAttribute("Type", sprite.Type));
            }
            return element;
        }

        public static ISprite LoadSprite(XElement element)
        {
            Sprite sprite = new Sprite();
            if (element.Attribute("X") is XAttribute width) sprite.X = (float)width;
            if (element.Attribute("Y") is XAttribute height) sprite.Y = (float)height;
            if (element.Attribute("Level") is XAttribute level) sprite.Level = (int)level;
            if (element.Attribute("Type") is XAttribute type) sprite.Type = (int)type;

            return sprite;
        }

    }
}