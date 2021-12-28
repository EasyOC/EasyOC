using Castle.Core.Internal;
using EasyOC.Core;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    //
    // 摘要:
    //     Extension methods for String class.
    public static class StringExtensions
    {
        //
        // 摘要:
        //     Adds a char to end of given string if it does not ends with the char.
        public static string EnsureEndsWith(this string str, char c, StringComparison comparisonType = StringComparison.Ordinal)
        {
            Check.NotNull<string>(str, "str");
            if (str.EndsWith(c.ToString(), comparisonType))
            {
                return str;
            }

            return str + c;
        }

        //
        // 摘要:
        //     Adds a char to beginning of given string if it does not starts with the char.
        public static string EnsureStartsWith(this string str, char c, StringComparison comparisonType = StringComparison.Ordinal)
        {
            Check.NotNull<string>(str, "str");
            if (str.StartsWith(c.ToString(), comparisonType))
            {
                return str;
            }

            return c + str;
        }

        //
        // 摘要:
        //     Indicates whether this string is null or an System.String.Empty string.
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        //
        // 摘要:
        //     indicates whether this string is null, empty, or consists only of white-space
        //     characters.
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        //
        // 摘要:
        //     Gets a substring of a string from beginning of the string.
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     Thrown if str is null
        //
        //   T:System.ArgumentException:
        //     Thrown if len is bigger that string's length
        public static string Left(this string str, int len)
        {
            Check.NotNull<string>(str, "str");
            if (str.Length < len)
            {
                throw new ArgumentException("len argument can not be bigger than given string's length!");
            }

            return str.Substring(0, len);
        }

        //
        // 摘要:
        //     Converts line endings in the string to System.Environment.NewLine.
        public static string NormalizeLineEndings(this string str)
        {
            return str.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", Environment.NewLine);
        }

        //
        // 摘要:
        //     Gets index of nth occurrence of a char in a string.
        //
        // 参数:
        //   str:
        //     source string to be searched
        //
        //   c:
        //     Char to search in str
        //
        //   n:
        //     Count of the occurrence
        public static int NthIndexOf(this string str, char c, int n)
        {
            Check.NotNull<string>(str, "str");
            int num = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == c && ++num == n)
                {
                    return i;
                }
            }

            return -1;
        }

        //
        // 摘要:
        //     Removes first occurrence of the given postfixes from end of the given string.
        //
        // 参数:
        //   str:
        //     The string.
        //
        //   postFixes:
        //     one or more postfix.
        //
        // 返回结果:
        //     Modified string or the same string if it has not any of given postfixes
        public static string RemovePostFix(this string str, params string[] postFixes)
        {
            return str.RemovePostFix(StringComparison.Ordinal, postFixes);
        }

        //
        // 摘要:
        //     Removes first occurrence of the given postfixes from end of the given string.
        //
        // 参数:
        //   str:
        //     The string.
        //
        //   comparisonType:
        //     String comparison type
        //
        //   postFixes:
        //     one or more postfix.
        //
        // 返回结果:
        //     Modified string or the same string if it has not any of given postfixes
        public static string RemovePostFix(this string str, StringComparison comparisonType, params string[] postFixes)
        {
            if (str.IsNullOrEmpty())
            {
                return str;
            }

            if (postFixes.IsNullOrEmpty())
            {
                return str;
            }

            foreach (string text in postFixes)
            {
                if (str.EndsWith(text, comparisonType))
                {
                    return str.Left(str.Length - text.Length);
                }
            }

            return str;
        }

        //
        // 摘要:
        //     Removes first occurrence of the given prefixes from beginning of the given string.
        //
        // 参数:
        //   str:
        //     The string.
        //
        //   preFixes:
        //     one or more prefix.
        //
        // 返回结果:
        //     Modified string or the same string if it has not any of given prefixes
        public static string RemovePreFix(this string str, params string[] preFixes)
        {
            return str.RemovePreFix(StringComparison.Ordinal, preFixes);
        }

        //
        // 摘要:
        //     Removes first occurrence of the given prefixes from beginning of the given string.
        //
        // 参数:
        //   str:
        //     The string.
        //
        //   comparisonType:
        //     String comparison type
        //
        //   preFixes:
        //     one or more prefix.
        //
        // 返回结果:
        //     Modified string or the same string if it has not any of given prefixes
        public static string RemovePreFix(this string str, StringComparison comparisonType, params string[] preFixes)
        {
            if (str.IsNullOrEmpty())
            {
                return str;
            }

            if (preFixes.IsNullOrEmpty())
            {
                return str;
            }

            foreach (string text in preFixes)
            {
                if (str.StartsWith(text, comparisonType))
                {
                    return str.Right(str.Length - text.Length);
                }
            }

            return str;
        }

        public static string ReplaceFirst(this string str, string search, string replace, StringComparison comparisonType = StringComparison.Ordinal)
        {
            Check.NotNull<string>(str, "str");
            int num = str.IndexOf(search, comparisonType);
            if (num < 0)
            {
                return str;
            }

            return str.Substring(0, num) + replace + str.Substring(num + search.Length);
        }

        //
        // 摘要:
        //     Gets a substring of a string from end of the string.
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     Thrown if str is null
        //
        //   T:System.ArgumentException:
        //     Thrown if len is bigger that string's length
        public static string Right(this string str, int len)
        {
            Check.NotNull<string>(str, "str");
            if (str.Length < len)
            {
                throw new ArgumentException("len argument can not be bigger than given string's length!");
            }

            return str.Substring(str.Length - len, len);
        }

        //
        // 摘要:
        //     Uses string.Split method to split given string by given separator.
        public static string[] Split(this string str, string separator)
        {
            return str.Split(new string[1]
            {
                separator
            }, StringSplitOptions.None);
        }

        //
        // 摘要:
        //     Uses string.Split method to split given string by given separator.
        public static string[] Split(this string str, string separator, StringSplitOptions options)
        {
            return str.Split(new string[1]
            {
                separator
            }, options);
        }

        //
        // 摘要:
        //     Uses string.Split method to split given string by System.Environment.NewLine.
        public static string[] SplitToLines(this string str)
        {
            return Split(str, Environment.NewLine);
        }

        //
        // 摘要:
        //     Uses string.Split method to split given string by System.Environment.NewLine.
        public static string[] SplitToLines(this string str, StringSplitOptions options)
        {
            return Split(str, Environment.NewLine, options);
        }

        //
        // 摘要:
        //     Converts PascalCase string to camelCase string.
        //
        // 参数:
        //   str:
        //     String to convert
        //
        //   useCurrentCulture:
        //     set true to use current culture. Otherwise, invariant culture will be used.
        //
        //   handleAbbreviations:
        //     set true to if you want to convert 'XYZ' to 'xyz'.
        //
        // 返回结果:
        //     camelCase of the string
        public static string ToCamelCase(this string str, bool useCurrentCulture = false, bool handleAbbreviations = false)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            if (str.Length == 1)
            {
                if (!useCurrentCulture)
                {
                    return str.ToLowerInvariant();
                }

                return str.ToLower();
            }

            if (handleAbbreviations && IsAllUpperCase(str))
            {
                if (!useCurrentCulture)
                {
                    return str.ToLowerInvariant();
                }

                return str.ToLower();
            }

            return (useCurrentCulture ? char.ToLower(str[0]) : char.ToLowerInvariant(str[0])) + str.Substring(1);
        }

        //
        // 摘要:
        //     Converts given PascalCase/camelCase string to sentence (by splitting words by
        //     space). Example: "ThisIsSampleSentence" is converted to "This is a sample sentence".
        //
        // 参数:
        //   str:
        //     String to convert.
        //
        //   useCurrentCulture:
        //     set true to use current culture. Otherwise, invariant culture will be used.
        public static string ToSentenceCase(this string str, bool useCurrentCulture = false)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            if (!useCurrentCulture)
            {
                return Regex.Replace(str, "[a-z][A-Z]", (Match m) => m.Value[0] + " " + char.ToLowerInvariant(m.Value[1]));
            }

            return Regex.Replace(str, "[a-z][A-Z]", (Match m) => m.Value[0] + " " + char.ToLower(m.Value[1]));
        }

        //
        // 摘要:
        //     Converts given PascalCase/camelCase string to kebab-case.
        //
        // 参数:
        //   str:
        //     String to convert.
        //
        //   useCurrentCulture:
        //     set true to use current culture. Otherwise, invariant culture will be used.
        public static string ToKebabCase(this string str, bool useCurrentCulture = false)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            str = str.ToCamelCase();
            if (!useCurrentCulture)
            {
                return Regex.Replace(str, "[a-z][A-Z]", (Match m) => m.Value[0] + "-" + char.ToLowerInvariant(m.Value[1]));
            }

            return Regex.Replace(str, "[a-z][A-Z]", (Match m) => m.Value[0] + "-" + char.ToLower(m.Value[1]));
        }

        //
        // 摘要:
        //     Converts given PascalCase/camelCase string to snake case. Example: "ThisIsSampleSentence"
        //     is converted to "this_is_a_sample_sentence". https://github.com/npgsql/npgsql/blob/dev/src/Npgsql/NameTranslation/NpgsqlSnakeCaseNameTranslator.cs#L51
        //
        // 参数:
        //   str:
        //     String to convert.
        public static string ToSnakeCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            StringBuilder stringBuilder = new StringBuilder(str.Length + Math.Min(2, str.Length / 5));
            UnicodeCategory? unicodeCategory = null;
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (c == '_')
                {
                    stringBuilder.Append('_');
                    unicodeCategory = null;
                    continue;
                }

                UnicodeCategory unicodeCategory2 = char.GetUnicodeCategory(c);
                switch (unicodeCategory2)
                {
                    case UnicodeCategory.UppercaseLetter:
                    case UnicodeCategory.TitlecaseLetter:
                        if (unicodeCategory == UnicodeCategory.SpaceSeparator || unicodeCategory == UnicodeCategory.LowercaseLetter || (unicodeCategory != UnicodeCategory.DecimalDigitNumber && unicodeCategory.HasValue && i > 0 && i + 1 < str.Length && char.IsLower(str[i + 1])))
                        {
                            stringBuilder.Append('_');
                        }

                        c = char.ToLower(c);
                        break;
                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.DecimalDigitNumber:
                        if (unicodeCategory == UnicodeCategory.SpaceSeparator)
                        {
                            stringBuilder.Append('_');
                        }

                        break;
                    default:
                        if (unicodeCategory.HasValue)
                        {
                            unicodeCategory = UnicodeCategory.SpaceSeparator;
                        }

                        continue;
                }

                stringBuilder.Append(c);
                unicodeCategory = unicodeCategory2;
            }

            return stringBuilder.ToString();
        }

        //
        // 摘要:
        //     Converts string to enum value.
        //
        // 参数:
        //   value:
        //     String value to convert
        //
        // 类型参数:
        //   T:
        //     Type of enum
        //
        // 返回结果:
        //     Returns enum object
        public static T ToEnum<T>(this string value) where T : struct
        {
            Check.NotNull<string>(value, "value");
            return (T)Enum.Parse(typeof(T), value);
        }

        //
        // 摘要:
        //     Converts string to enum value.
        //
        // 参数:
        //   value:
        //     String value to convert
        //
        //   ignoreCase:
        //     Ignore case
        //
        // 类型参数:
        //   T:
        //     Type of enum
        //
        // 返回结果:
        //     Returns enum object
        public static T ToEnum<T>(this string value, bool ignoreCase) where T : struct
        {
            Check.NotNull<string>(value, "value");
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        public static string ToMd5(this string str)
        {
            using (MD5 mD = MD5.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                byte[] array = mD.ComputeHash(bytes);
                StringBuilder stringBuilder = new StringBuilder();
                byte[] array2 = array;
                foreach (byte b in array2)
                {
                    stringBuilder.Append(b.ToString("X2"));
                }

                return stringBuilder.ToString();
            }
        }

        //
        // 摘要:
        //     Converts camelCase string to PascalCase string.
        //
        // 参数:
        //   str:
        //     String to convert
        //
        //   useCurrentCulture:
        //     set true to use current culture. Otherwise, invariant culture will be used.
        //
        // 返回结果:
        //     PascalCase of the string
        public static string ToPascalCase(this string str, bool useCurrentCulture = false)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            if (str.Length == 1)
            {
                if (!useCurrentCulture)
                {
                    return str.ToUpperInvariant();
                }

                return str.ToUpper();
            }

            return (useCurrentCulture ? char.ToUpper(str[0]) : char.ToUpperInvariant(str[0])) + str.Substring(1);
        }

        //
        // 摘要:
        //     Gets a substring of a string from beginning of the string if it exceeds maximum
        //     length.
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     Thrown if str is null
        public static string Truncate(this string str, int maxLength)
        {
            if (str == null)
            {
                return null;
            }

            if (str.Length <= maxLength)
            {
                return str;
            }

            return str.Left(maxLength);
        }

        //
        // 摘要:
        //     Gets a substring of a string from Ending of the string if it exceeds maximum
        //     length.
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     Thrown if str is null
        public static string TruncateFromBeginning(this string str, int maxLength)
        {
            if (str == null)
            {
                return null;
            }

            if (str.Length <= maxLength)
            {
                return str;
            }

            return str.Right(maxLength);
        }

        //
        // 摘要:
        //     Gets a substring of a string from beginning of the string if it exceeds maximum
        //     length. It adds a "..." postfix to end of the string if it's truncated. Returning
        //     string can not be longer than maxLength.
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     Thrown if str is null
        public static string TruncateWithPostfix(this string str, int maxLength)
        {
            return str.TruncateWithPostfix(maxLength, "...");
        }

        //
        // 摘要:
        //     Gets a substring of a string from beginning of the string if it exceeds maximum
        //     length. It adds given postfix to end of the string if it's truncated. Returning
        //     string can not be longer than maxLength.
        //
        // 异常:
        //   T:System.ArgumentNullException:
        //     Thrown if str is null
        public static string TruncateWithPostfix(this string str, int maxLength, string postfix)
        {
            if (str == null)
            {
                return null;
            }

            if (str == string.Empty || maxLength == 0)
            {
                return string.Empty;
            }

            if (str.Length <= maxLength)
            {
                return str;
            }

            if (maxLength <= postfix.Length)
            {
                return postfix.Left(maxLength);
            }

            return str.Left(maxLength - postfix.Length) + postfix;
        }

        //
        // 摘要:
        //     Converts given string to a byte array using System.Text.Encoding.UTF8 encoding.
        public static byte[] GetBytes(this string str)
        {
            return str.GetBytes(Encoding.UTF8);
        }

        //
        // 摘要:
        //     Converts given string to a byte array using the given encoding
        public static byte[] GetBytes(this string str, Encoding encoding)
        {
            Check.NotNull<string>(str, "str");
            Check.NotNull(encoding, "encoding");
            return encoding.GetBytes(str);
        }

        private static bool IsAllUpperCase(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsLetter(input[i]) && !char.IsUpper(input[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
