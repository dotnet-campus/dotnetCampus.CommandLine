using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnetCampus.Cli.Utils
{
    internal static class NamingHelper
    {
        /// <summary>
        /// Check if the specified <paramref name="value"/> is a PascalCase string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool CheckIsPascalCase(string value)
        {
            var first = value[0];
            if (!char.IsUpper(first))
            {
                return false;
            }

            foreach (var letter in value)
            {
                if (!char.IsLetterOrDigit(letter))
                {
                    return false;
                }
            }

            if (value.Length >= 3)
            {
                var allUpper = value.All(x => char.IsUpper(x));
                if (allUpper)
                {
                    return false;
                }
            }

            return true;
        }

        internal static string MakePascalCase(string oldName)
        {
            var builder = new StringBuilder();

            var isFirstLetter = true;
            var isWordStart = true;
            foreach (char c in oldName)
            {
                if (!char.IsLetterOrDigit(c))
                {
                    // Append nothing because PascalCase has no special characters.
                    isWordStart = true;
                    continue;
                }

                if (isFirstLetter)
                {
                    if (char.IsDigit(c))
                    {
                        // PascalCase does not support digital as the first letter.
                        isWordStart = true;
                        continue;
                    }
                    else if (!char.IsUpper(c))
                    {
                        isFirstLetter = false;
                        isWordStart = false;
                        builder.Append(char.ToUpperInvariant(c));
                    }
                    else
                    {
                        isFirstLetter = false;
                        isWordStart = false;
                        builder.Append(c);
                    }
                }
                else
                {
                    if (char.IsDigit(c))
                    {
                        // PascalCase does not support digital as the first letter.
                        isWordStart = true;
                        builder.Append(c);
                    }
                    else if (!char.IsUpper(c))
                    {
                        builder.Append(isWordStart
                            ? char.ToUpperInvariant(c)
                            : c);
                        isWordStart = false;
                    }
                    else
                    {
                        isWordStart = false;
                        builder.Append(c);
                    }
                }
            }

            return builder.ToString();
        }

        internal static string MakeKebabCase(string oldName)
        {
            var builder = new StringBuilder();

            var isFirstLetter = true;
            var isWordStart = true;
            foreach (char c in oldName)
            {
                if (!char.IsLetterOrDigit(c))
                {
                    // Append nothing because kebab-case has no special characters.
                    isWordStart = true;
                    continue;
                }

                if (isFirstLetter)
                {
                    if (char.IsDigit(c))
                    {
                        // kebab-case does not support digital as the first letter.
                        isWordStart = true;
                        continue;
                    }
                    else if (char.IsUpper(c))
                    {
                        isFirstLetter = false;
                        isWordStart = false;
                        builder.Append(char.ToLowerInvariant(c));
                    }
                    else
                    {
                        isFirstLetter = false;
                        isWordStart = false;
                        builder.Append(c);
                    }
                }
                else
                {
                    if (char.IsDigit(c))
                    {
                        // kebab-case does not support digital as the first letter.
                        isWordStart = true;
                        builder.Append(c);
                    }
                    else if (char.IsUpper(c))
                    {
                        if (isWordStart)
                        {
                            builder.Append('-');
                        }
                        builder.Append(char.ToLowerInvariant(c));
                        isWordStart = false;
                    }
                    else
                    {
                        if (isWordStart)
                        {
                            builder.Append('-');
                        }
                        builder.Append(c);
                        isWordStart = false;
                    }
                }
            }

            return builder.ToString();
        }
    }
}
