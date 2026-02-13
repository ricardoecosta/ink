using System;
using System.Collections.Generic;
using System.Text;
using HnK.Management;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HnK.Mechanics;
using HnK.Constants;
using Microsoft.Xna.Framework.GamerServices;

namespace HnK
{
    public class Utils
    {

        /// <summary>
        /// Gets data from dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns>Value if key exists or null if key not exits.</returns>
        public static T GetDataFromDictionary<T>(IDictionary<string, object> data, string key, T defaultValue)
        {
            if (data.ContainsKey(key))
            {
                return (T)data[key];
            }

            return defaultValue;
        }

        /// <summary>
        /// Apply Ellipsis to given string.
        /// </summary>
        /// <param name="text">text to apply ellipsis string is bigger than given rect.</param>
        /// <param name="font">Current font.</param>
        /// <param name="maxWidth">Max width when the string must be.</param>
        /// <returns>New string with Ellipsis or de same string was passed to method.</returns>
        public static String ApplyEllipsis(String text, SpriteFont font, int maxWidth)
        {
            if (text == null || font == null || text.Length == 0)
            {
                return text;
            }

            Vector2 textSize = font.MeasureString(text);
            // rect is large enough to display the whole text 
            if (textSize.X <= maxWidth)
            {
                return text;
            }

            int len = 0;
            int seg = text.Length;
            String fit = null;
            StringBuilder tst = new StringBuilder();

            // find the longest string that fits into
            // the control boundaries using bisection method 
            while (seg > 1)
            {
                seg -= seg / 2;
                int left = len + seg;
                int right = text.Length;

                if (left > right)
                    continue;

                // build and measure a candidate string with ellipsis
                //string tst = text.Substring(0, left) + EllipsisChars + text.Substring(right);
                tst.Append(text.Substring(0, left));
                tst.Append(EllipsisChars);
                tst.Append(text.Substring(right));
                textSize = font.MeasureString(tst);

                // candidate string fits into control boundaries, 
                // try a longer string
                // stop when seg <= 1 
                if (textSize.X <= maxWidth)
                {
                    len += seg;
                    fit = tst.ToString();
                }
                tst.Length = 0;

            }
            if (len == 0) // string can't fit into control
            { return EllipsisChars; }

            return fit;
        }

        /// <summary>
        /// Gets lines texts of string that fits on given MaxLines and MaxWidth.
        /// This receives max width and lines limit.
        /// </summary>
        /// <param name="text">Text to analyze.</param>
        /// <param name="font">Font of given text.</param>
        /// <param name="maxWidth">Max width of rectangle that will be to contains the text.</param>
        /// <param name="maxLines">Maximum number of lines.</param>
        /// <param name="totalWidth">(out parameter) Text lines list.</param>
        /// <returns>List with strings that will be appear on given limits.</returns>
        public static List<StringBuilder> GetTextLines(String text, SpriteFont font, int maxWidth, int maxLines)
        {
            List<StringBuilder> lines = new List<StringBuilder>(maxLines);
            if (text.Length == 0 || font == null ||
                maxWidth <= 0 || maxLines <= 0)
            {
                return lines;
            }

            //fill all lines strings.
            for (int i = 0; i < maxLines; i++)
            {
                lines.Add(new StringBuilder());
            }

            int linesCounter = 0;
            char[] chars = text.ToCharArray();
            float lineWidth = 0.0f;
            StringBuilder builder = new StringBuilder();

            int currentIndex = 0;
            int indexOfBreak = 0;
            foreach (char c in chars)
            {
                if (linesCounter == maxLines)
                { return lines; }

                if (c == Utils.BreakLineChar)
                {
                    linesCounter++;
                    lineWidth = 0;
                    builder.Length = 0; // clears the String builder
                    currentIndex++;
                    indexOfBreak = currentIndex;
                    continue;
                }

                Vector2 charSize = font.MeasureString(c.ToString());
                if ((lineWidth + charSize.X) > maxWidth)
                {
                    lineWidth = charSize.X;
                    linesCounter++;
                    indexOfBreak = currentIndex;
                    if (c != Utils.SpaceChar)
                    {
                        Vector2 strSize = font.MeasureString(builder.ToString());
                        lineWidth += strSize.X;
                        indexOfBreak -= builder.Length;
                        if (linesCounter < lines.Count)
                        {
                            lines[linesCounter].Append(builder.ToString());
                            lines[linesCounter - 1].Remove((lines[linesCounter - 1].Length) - builder.Length, builder.Length);
                            lines[linesCounter].Append(c);
                        }
                    }
                    else
                    {
                        lineWidth = 0;
                        if (linesCounter < lines.Count)
                        { lines[linesCounter].Append(c); }
                    }
                    builder.Length = 0; // clears the String builder
                }
                else
                {
                    builder.Append(c);
                    lineWidth += charSize.X;
                    if (linesCounter < lines.Count)
                    {
                        lines[linesCounter].Append(c);
                    }
                }

                if (c == Utils.SpaceChar)
                {
                    builder.Length = 0; // clears the String builder
                }
                currentIndex++;
            }
            return lines;
        }

        public static Dictionary<char, GameLibrary.UI.Texture> GetCountdownCharacters()
        {
            Dictionary<char, GameLibrary.UI.Texture> textures = new Dictionary<char, GameLibrary.UI.Texture>(11);
            textures['0'] = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.Countdown0);
            textures['1'] = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.Countdown1);
            textures['2'] = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.Countdown2);
            textures['3'] = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.Countdown3);
            textures['4'] = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.Countdown4);
            textures['5'] = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.Countdown5);
            textures['6'] = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.Countdown6);
            textures['7'] = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.Countdown7);
            textures['8'] = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.Countdown8);
            textures['9'] = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.Countdown9);
            textures[':'] = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.CountdownSeparator);
            return textures;
        }

        public static Dictionary<char, GameLibrary.UI.Texture> GetLevelClearedCharacters()
        {
            Dictionary<char, GameLibrary.UI.Texture> textures = new Dictionary<char, GameLibrary.UI.Texture>(11);
            textures['0'] = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.LevelClearedNumber0);
            textures['1'] = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.LevelClearedNumber1);
            textures['2'] = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.LevelClearedNumber2);
            textures['3'] = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.LevelClearedNumber3);
            textures['4'] = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.LevelClearedNumber4);
            textures['5'] = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.LevelClearedNumber5);
            textures['6'] = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.LevelClearedNumber6);
            textures['7'] = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.LevelClearedNumber7);
            textures['8'] = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.LevelClearedNumber8);
            textures['9'] = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.LevelClearedNumber9);
            return textures;
        }

        /// <summary>
        /// Gets scores by block type and special type.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public static long GetScore(Block block)
        {
            long blockPoints = 0;

            if (block != null)
            {
                switch (block.Type)
                {
                    case Block.BlockTypes.UnmovableBlock:
                        blockPoints = ScoreConstants.UnmovableBlockPoints;
                        break;

                    case Block.BlockTypes.GoldenBlock:
                        blockPoints = ScoreConstants.GoldenBlockPoints;
                        break;

                    default:
                        blockPoints = (block.CurrentSpecialType != Block.SpecialTypes.None) ? ScoreConstants.RemovedPowerUpBlockPoints : ScoreConstants.RemovedRegularBlockPoints;
                        break;
                }
            }


            return blockPoints;
        }

        /// <summary>
        /// Gets the number of digits of given value.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
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
