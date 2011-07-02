using System;
using System.IO;

namespace Torshify.Client.Infrastructure
{
    public class AppConstants
    {
        public static readonly string AppDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "torshify");

        public static readonly string BackdropCacheFolder = Path.Combine(
            AppDataFolder, "Backdrops");

        public static readonly string CoverArtCacheFolder = Path.Combine(
            AppDataFolder, "CoverArts");
    }
}