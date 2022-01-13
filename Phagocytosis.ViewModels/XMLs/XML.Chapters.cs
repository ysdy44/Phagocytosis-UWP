using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Phagocytosis.ViewModels
{
    public static partial class XML
    {

        public static XDocument SaveChapters(IEnumerable<Chapter> chapters)
        {
            return new XDocument
            (
                // Set the document definition for xml.
                new XDeclaration("1.0", "utf-8", "no"),
                new XElement
                (
                    "Root",
                    from chapter
                    in chapters
                    select XML.SaveChapter("Chapter", chapter)
                )
            );
        }

        public static IEnumerable<Chapter> LoadChapters(XDocument document)
        {
            if (document.Element("Root") is XElement root)
            {
                if (root.Elements("Chapter") is IEnumerable<XElement> chapters)
                {
                    return
                        from chapter
                        in chapters
                        select XML.LoadChapter(chapter);
                }
            }

            return null;
        }

    }
}