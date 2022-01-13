using Phagocytosis.Sprites;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Phagocytosis.ViewModels
{
    public static partial class XML
    {

        public static XElement SaveChapter(string elementName, Chapter chapter)
        {
            XElement element = new XElement(elementName);

            // SaveWith
            {
                element.Add(new XAttribute("Width", chapter.Width));
                element.Add(new XAttribute("Height", chapter.Height));
                element.Add(new XAttribute("MaximumFoods", chapter.MaximumFoods));
                element.Add(new XAttribute("IncreaseFoods", chapter.IncreaseFoods));
                if (chapter.Restricteds != null && chapter.Restricteds.Count() != 0)
                {
                    element.Add(new XElement
                    (
                        "Restricteds",
                        from rect
                        in chapter.Restricteds
                        select new XElement("Rect", new XAttribute("X", rect.X), new XAttribute("Y", rect.Y), new XAttribute("Width", rect.Width), new XAttribute("Height", rect.Height))
                    ));
                }
                if (chapter.FriendSprites != null && chapter.FriendSprites.Count() != 0)
                {
                    element.Add(new XElement
                    (
                        "FriendSprites",
                        from friendSprite
                        in chapter.FriendSprites
                        select Sprites.XML.SaveSprite("Sprite", friendSprite)
                    ));
                }
                if (chapter.EnemySprites != null && chapter.EnemySprites.Count() != 0)
                {
                    element.Add(new XElement
                    (
                        "EnemySprites",
                        from enemySprite
                        in chapter.EnemySprites
                        select Sprites.XML.SaveSprite("Sprite", enemySprite)
                    ));
                }
            }
            return element;
        }

        public static Chapter LoadChapter(XElement element)
        {
            Chapter project = new Chapter();
            if (element.Attribute("Width") is XAttribute width) project.Width = (int)width;
            if (element.Attribute("Height") is XAttribute height) project.Height = (int)height;
            if (element.Attribute("MaximumFoods") is XAttribute maximumFoods) project.MaximumFoods = (int)maximumFoods;
            if (element.Attribute("IncreaseFoods") is XAttribute increaseFoods) project.IncreaseFoods = (int)increaseFoods;
            if (element.Element("Restricteds") is XElement restricteds)
            {
                if (restricteds.Elements() is IEnumerable<XElement> restricteds2)
                {
                    project.Restricteds =
                        from restricted
                        in restricteds2
                        select new Rect2
                        (
                            x: (int)restricted.Attribute("X"),
                            y: (int)restricted.Attribute("Y"),
                            width: (int)restricted.Attribute("Width"),
                            height: (int)restricted.Attribute("Height")
                        );
                }
            }
            if (element.Element("FriendSprites") is XElement friendSprites)
            {
                if (friendSprites.Elements() is IEnumerable<XElement> friendSprite2)
                {
                    project.FriendSprites =
                        from friendSprite
                        in friendSprite2
                        select Sprites.XML.LoadSprite(friendSprite);
                }
            }
            if (element.Element("EnemySprites") is XElement enemySprites)
            {
                if (enemySprites.Elements() is IEnumerable<XElement> enemySprites2)
                {
                    project.EnemySprites =
                        from enemySprite
                        in enemySprites2
                        select Sprites.XML.LoadSprite(enemySprite);
                }
            }

            return project;
        }

    }
}