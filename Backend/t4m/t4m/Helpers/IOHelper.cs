using System;
using System.Text;

namespace t4m.Helpers
{
    public static class IOHelper
    {
        public static string GetSafeFileName(string fileName)
        {
            // Remove any characters not supported by the file system
            char[] invalidChars = Path.GetInvalidFileNameChars();
            fileName = string.Join("", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));

            // Remove characters that can cause issues when it's used in file system
            string[] invalidNames = { "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };
            fileName = invalidNames.Contains(fileName.ToUpper()) ? "_" + fileName : fileName;

            // Truncate the file name if it's too long
            int maxLength = 100;
            if (fileName.Length > maxLength)
            {
                fileName = fileName.Substring(0, maxLength);
            }

            return fileName.Replace(' ', '_').Normalize(NormalizationForm.FormD);
        }
    }
}

