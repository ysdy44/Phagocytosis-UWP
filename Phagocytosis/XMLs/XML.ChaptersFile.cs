using Phagocytosis.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Phagocytosis
{
    public static partial class XML
    {

        public static async Task<IEnumerable<Chapter>> ConstructChaptersFile()
        {
            StorageFile file = null;

            // Read the file from the local folder.
            IStorageItem item = await ApplicationData.Current.LocalFolder.TryGetItemAsync("Chapters.xml");
            if (item is StorageFile file2)
            {
                file = file2;
            }

            if (file == null)
            {
                // Read the file from the package.
                file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///XMLs/Chapters.xml"));

                // Copy to the local folder.
                await file.CopyAsync(ApplicationData.Current.LocalFolder);
            }

            if (file == null) return null;

            using (Stream stream = await file.OpenStreamForReadAsync())
            {
                try
                {
                    XDocument document = XDocument.Load(stream);

                    IEnumerable<Chapter> source = Phagocytosis.ViewModels.XML.LoadChapters(document);
                    return source;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static async Task SaveChaptersFile(IEnumerable<Chapter> chapters)
        {
            XDocument document = Phagocytosis.ViewModels.XML.SaveChapters(chapters);

            // Save the Chapter xml file.      
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Chapters.xml", CreationCollisionOption.ReplaceExisting);
            using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                using (Stream stream = fileStream.AsStream())
                {
                    document.Save(stream);
                }
            }
        }

    }
}