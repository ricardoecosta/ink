using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HamstasKitties.Mechanics;
using HamstasKitties.Constants;
using HamstasKitties.Management;
using HamstasKitties.UI;

namespace HamstasKitties.Utils
{
    /// <summary>
    /// Utility methods for text and UI operations.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Gets data from dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns>Value if key exists or default value if key not exists.</returns>
        public static T GetDataFromDictionary<T>(IDictionary<string, object> data, string key, T defaultValue)
        {
            if (data.ContainsKey(key))
            {
                return (T)data[key];
            }

            return defaultValue;
        }

        /// <summary>
        /// Applies ellipsis to text if it exceeds the maximum width.
        /// </summary>
        /// <param name="text">The text to truncate.</param>
        /// <param name="font">The font to measure with.</param>
        /// <param name="maxWidth">The maximum width in pixels.</param>
        /// <returns>The truncated text with ellipsis if needed.</returns>
        public static string ApplyEllipsis(string text, SpriteFont font, int maxWidth)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            Vector2 textSize = font.MeasureString(text);
            if (textSize.X <= maxWidth)
            {
                return text;
            }

            // Binary search for the right length
            int min = 0;
            int max = text.Length;
            string result = text;

            while (min < max)
            {
                int mid = (min + max + 1) / 2;
                string candidate = text.Substring(0, mid) + "...";
                Vector2 candidateSize = font.MeasureString(candidate);

                if (candidateSize.X <= maxWidth)
                {
                    min = mid;
                    result = candidate;
                }
                else
                {
                    max = mid - 1;
                }
            }

            return result;
        }

        /// <summary>
        /// Splits text into multiple lines based on maximum width.
        /// </summary>
        /// <param name="text">The text to wrap.</param>
        /// <param name="font">The font to measure with.</param>
        /// <param name="maxWidth">The maximum width in pixels.</param>
        /// <param name="maxLines">The maximum number of lines to return.</param>
        /// <returns>List of StringBuilder objects representing each line.</returns>
        public static List<StringBuilder> GetTextLines(string text, SpriteFont font, int maxWidth, int maxLines)
        {
            List<StringBuilder> lines = new List<StringBuilder>();
            if (string.IsNullOrEmpty(text))
            {
                lines.Add(new StringBuilder());
                return lines;
            }

            string[] words = text.Split(' ');
            StringBuilder currentLine = new StringBuilder();

            foreach (string word in words)
            {
                string testLine = currentLine.Length > 0 ? currentLine.ToString() + " " + word : word;
                Vector2 testSize = font.MeasureString(testLine);

                if (testSize.X <= maxWidth)
                {
                    if (currentLine.Length > 0)
                    {
                        currentLine.Append(" ");
                    }
                    currentLine.Append(word);
                }
                else
                {
                    if (currentLine.Length > 0)
                    {
                        lines.Add(currentLine);
                        currentLine = new StringBuilder(word);
                    }
                    else
                    {
                        // Word is too long, need to split it
                        currentLine.Append(word);
                        lines.Add(currentLine);
                        currentLine = new StringBuilder();
                    }

                    if (lines.Count >= maxLines)
                    {
                        break;
                    }
                }
            }

            if (currentLine.Length > 0 && lines.Count < maxLines)
            {
                lines.Add(currentLine);
            }

            // Ensure we have exactly maxLines
            while (lines.Count < maxLines)
            {
                lines.Add(new StringBuilder());
            }

            return lines;
        }

        /// <summary>
        /// Gets countdown timer character textures.
        /// </summary>
        /// <returns>Dictionary mapping digit characters to their textures.</returns>
        public static Dictionary<char, UI.Texture> GetCountdownCharacters()
        {
            Dictionary<char, UI.Texture> textures = new Dictionary<char, UI.Texture>(11);
            // TODO: Add countdown number textures to TextureAssets enum when assets are available
            // For now, return empty dictionary - textures need to be defined in GameDirector.TextureAssets
            return textures;
        }

        /// <summary>
        /// Gets level cleared screen character textures.
        /// </summary>
        /// <returns>Dictionary mapping digit characters to their textures.</returns>
        public static Dictionary<char, UI.Texture> GetLevelClearedCharacters()
        {
            Dictionary<char, UI.Texture> textures = new Dictionary<char, UI.Texture>(10);
            // TODO: Add level cleared number textures to TextureAssets enum when assets are available
            // For now, return empty dictionary - textures need to be defined in GameDirector.TextureAssets
            return textures;
        }

        /// <summary>
        /// Gets the score value for a removed block based on its type and special type.
        /// </summary>
        /// <param name="block">The block to calculate score for.</param>
        /// <returns>The score value for removing this block.</returns>
        public static long GetScore(Block block)
        {
            if (block == null)
            {
                return 0;
            }

            switch (block.Type)
            {
                case Block.BlockTypes.UnmovableBlock:
                    return ScoreConstants.UnmovableBlockPoints;

                case Block.BlockTypes.GoldenBlock:
                    return ScoreConstants.GoldenBlockPoints;

                default:
                    // For regular blocks, check if they have a special type power-up
                    return (block.CurrentSpecialType != Block.SpecialTypes.None)
                        ? ScoreConstants.RemovedPowerUpBlockPoints
                        : ScoreConstants.RemovedRegularBlockPoints;
            }
        }

        /// <summary>
        /// Gets the number of digits needed to display a score value.
        /// </summary>
        /// <param name="score">The score value.</param>
        /// <param name="minDigits">Minimum number of digits.</param>
        /// <param name="maxDigits">Maximum number of digits.</param>
        /// <returns>The number of digits, clamped between min and max.</returns>
        public static int GetDigits(long score, int minDigits, int maxDigits)
        {
            int digits = 0;
            int step = 1;
            while (step <= score)
            {
                digits++;
                step *= 10;
            }
            return (int)MathHelper.Clamp(digits, (minDigits < 1 ? 1 : minDigits), maxDigits);
        }

        public const string EllipsisChars = "...";
        public const char SpaceChar = ' ';
        public const char BreakLineChar = '\n';
    }
}
