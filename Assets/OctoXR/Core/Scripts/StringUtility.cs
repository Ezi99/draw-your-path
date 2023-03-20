using System;
using System.Text;

namespace OctoXR
{
    public static class StringUtility
    {
        public static string GetSpaceSeparatedString(string inputString, params char[] replaceWithSpaceChars)
        {
            return GetSpaceSeparatedString(inputString, false, replaceWithSpaceChars);
        }

        public static string GetSpaceSeparatedString(string inputString, bool insertSpaceBeforeCapitalLetters, params char[] replaceWithSpaceChars)
        {
            if (inputString == null)
            {
                throw new ArgumentNullException(nameof(inputString));
            }

            var stringBuilder = new StringBuilder();
            var builderIndex = -1;

            for (var i = 0; i < inputString.Length; ++i)
            {
                var character = inputString[i];

                if (insertSpaceBeforeCapitalLetters)
                {
                    if (char.IsUpper(character))
                    {
                        AppendSpaceToStringBuilder(stringBuilder, ref builderIndex);
                    }

                    AppendCharToStringBuilder(stringBuilder, character, ref builderIndex);
                }

                if (replaceWithSpaceChars != null)
                {
                    for (var j = 0; j < replaceWithSpaceChars.Length; ++j)
                    {
                        var replaceChar = replaceWithSpaceChars[j];

                        if (replaceChar == ' ')
                        {
                            continue;
                        }

                        if (replaceChar == character)
                        {
                            var previousIndex = builderIndex - 1;

                            if (previousIndex != -1 && stringBuilder[previousIndex] == ' ')
                            {
                                continue;
                            }

                            stringBuilder[builderIndex] = ' ';
                        }
                    }
                }
            }

            return stringBuilder.ToString();
        }

        private static void AppendSpaceToStringBuilder(StringBuilder stringBuilder, ref int builderIndex)
        {
            if (builderIndex != -1 && stringBuilder[builderIndex] != ' ')
            {
                stringBuilder.Append(' ');
                ++builderIndex;
            }
        }

        private static void AppendCharToStringBuilder(StringBuilder stringBuilder, char character, ref int builderIndex)
        {
            stringBuilder.Append(character);
            ++builderIndex;
        }
    }
}
