using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace WebCamImageCollector.Background.Capturing
{
    internal static class _StorageFolderExtensions
    {
        public static async Task<StorageFolder> FindFolderAsync(this StorageFolder parent, string folderName)
        {
            try
            {
                StorageFolder item = await parent.TryGetItemAsync(folderName) as StorageFolder;
                return item;
            }
            catch (Exception)
            {
                // Should never get here 
                return null;
            }
        }
    }
}
