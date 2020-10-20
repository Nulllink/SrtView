using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace SrtView
{
    public static class ZipArchiveExtension
    {
        public static void ExtractToDirectory(this ZipArchive archive, string destinationDirectoryName)
        {
            foreach (ZipArchiveEntry file in archive.Entries)
            {
                string completeFileName = Path.Combine(destinationDirectoryName, file.FullName);
                if (file.Name == "")
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(completeFileName));
                    continue;
                }
                file.ExtractToFile(completeFileName, true);
            }
        }
    }
}
