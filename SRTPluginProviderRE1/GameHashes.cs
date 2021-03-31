using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace SRTPluginProviderRE1
{
    /// <summary>
    /// SHA256 hashes for the REmake game executables.
    /// </summary>
    public static class GameHashes
    {
        private static readonly byte[] bhdWW_yyyyMMdd_1 = new byte[32] { 0x0A, 0xB6, 0x1B, 0xAD, 0xA3, 0x47, 0x83, 0xA6, 0x84, 0x49, 0x08, 0x58, 0xE2, 0x00, 0x5B, 0xBD, 0x2E, 0x9A, 0x1B, 0x13, 0x53, 0xEA, 0xAA, 0xD4, 0x43, 0x37, 0xBF, 0x7A, 0xBB, 0x77, 0x3B, 0x72 };

        public static GameVersion DetectVersion(string filePath)
        {
            byte[] checksum;
            using (SHA256 hashFunc = SHA256.Create())
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                checksum = hashFunc.ComputeHash(fs);

            if (checksum.SequenceEqual(bhdWW_yyyyMMdd_1))
                return GameVersion.REmake_Latest;
            else
                return GameVersion.Unknown;
        }
    }
}