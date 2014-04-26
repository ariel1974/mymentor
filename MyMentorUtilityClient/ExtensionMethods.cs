using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyMentor
{
    public static class ExtensionMethods
    {
        public static string GetHebrewPlaceholderText(this ComboBox combo)
        {
            if (combo.SelectedItem != null)
            {
                if (combo.SelectedItem is Category)
                {
                    return ((Category)combo.SelectedItem).HebrewValue;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetEnglishPlaceholderText(this ComboBox combo)
        {
            if (combo.SelectedItem != null)
            {
                if (combo.SelectedItem is Category)
                {
                    return ((Category)combo.SelectedItem).EnglishValue;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }


        public static void RemoveAnchors(this RichTextBox myRtb)
        {
            string[] anchors = { "[3]", "[2]", "[1]", "[0]" };
            myRtb.ReadOnly = false;
            foreach (var anchor in anchors)
            {
                int index = myRtb.Find(anchor, 0, RichTextBoxFinds.None);

                while (index >= 0)
                {
                    myRtb.SelectedText = "";
                    index = myRtb.Find(anchor, index + 1, RichTextBoxFinds.None);
                }
            }
            myRtb.ReadOnly = true;
        }

        public static string RemoveAnchors(this string value)
        {
            return value.Replace("[3]", string.Empty).Replace("[2]", string.Empty).Replace("[1]", string.Empty).Replace("[0]", string.Empty);
        }

        public static string ClearSpacesAndBreakLines(this string text)
        {
            return text.Replace(System.Environment.NewLine, string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Replace(" ", string.Empty);
        }

        public static string SpecialReplace(this string text, string placeholder, string with)
        {
            if (string.IsNullOrEmpty(with) || 
                with == "__")
            {
                //where the placeholder
                int index = text.IndexOf(placeholder);
                int top = index;

                if (index == -1)
                {
                    return text;
                }

                char[] arr = text.ToCharArray();
                char c;
                int start = 0;
                int end = text.Length - 1;

                //go reverse and delete the comma
                while(index >= 0 && index <= top)
                {
                    c = arr[index];

                    if (c == ',')
                    {
                        start = index;
                        break;
                    }
                    else if (c == '_')
                    {
                        start = index + 1;
                        break;
                    }
                    index = index - 1;
                }

                //go forward
                index = text.IndexOf(placeholder);

                while (index < end)
                {
                    c = arr[index];
                    if (c == ',')
                    {
                        end = index;
                        break;
                    }
                    else if (c == '_')
                    {
                        end = index - 1;
                        break;
                    }

                    index = index + 1;
                }

                return text.Remove(start, end - start + 1);
            }
            else
            {
                return text.Replace(placeholder, with);
            }
        }

        public static bool IsNikudExists(this string s)
        {
            String normalizedString = s.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();
            var result = false;

            for (int i = 0; i < normalizedString.Length; i++)
            {
                Char c = normalizedString[i];

                Debug.WriteLine(string.Format("{0}:{1} char:{2}", c.ToString(), CharUnicodeInfo.GetUnicodeCategory(c).ToString(), (int)c));

                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark
                    || ((int)c >= 1423 && (int)c <= (1423 + 27)))
                {
                    stringBuilder.Append(c);
                }
                else
                {
                    result = true;
                }
            }

            return result;
        }

        public static bool IsTeamimExists(this string s)
        {
            String normalizedString = s.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();
            var result = false;

            for (int i = 0; i < normalizedString.Length; i++)
            {
                Char c = normalizedString[i];

                Debug.WriteLine(string.Format("{0}:{1} char:{2}", c.ToString(), CharUnicodeInfo.GetUnicodeCategory(c).ToString(), (int)c));

                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark
                    || ((int)c < 1423 || (int)c > (1423 + 27)))
                {
                    stringBuilder.Append(c);
                }
                else
                {
                    result = true;
                }
            }

            return result;
        }

        public static String RemoveNikud(this String s)
        {
            String normalizedString = s.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                Char c = normalizedString[i];

                Debug.WriteLine(string.Format("{0}:{1} char:{2}", c.ToString(), CharUnicodeInfo.GetUnicodeCategory(c).ToString(), (int)c));

                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark
                    || ((int)c >= 1423 && (int)c <= (1423 + 31)))
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }

        public static String RemoveTeamim(this String s)
        {
            String normalizedString = s.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                Char c = normalizedString[i];

                Debug.WriteLine(string.Format("{0}:{1} char:{2}", c.ToString(), CharUnicodeInfo.GetUnicodeCategory(c).ToString(), (int)c));

                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark
                    || ((int)c < 1423 || (int)c > (1423 + 31)))
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }

        public static String RemoveRtfTeamim(this String s)
        {
            var result = s;

            //makaf meunach
            result = result.Replace("\\'cd", string.Empty);

            for (var i = 1423; i <= 1454; i++)
            {
                result = result.Replace(string.Format("\\u{0}?", i), string.Empty);
            }

            return result;
        }

        public static String RemoveRtfNikud(this String s)
        {
            var result = s;

            for (var i = 192; i < 211; i++)
            {
                if (i == 206 ||
                    i == 208) 
                    continue;

                result = result.Replace(string.Format("\\'{0}", i.ToString("X").ToLower()), string.Empty);
            }

            return result;
        } 

        public static Word SeekForWord(this IEnumerable<Paragraph> paragraphs, int wordCharIndex)
        {
            Word theOne = null;

            foreach (Paragraph paragraph in paragraphs)
            {
                theOne = null;// paragraph.Words.Where(w => w.CharIndex == wordCharIndex).SingleOrDefault();

                if (theOne != null)
                {
                    break;
                }
                else
                {
                    foreach (Sentence sentense in paragraph.Sentences)
                    {
                        theOne = null;// sentense.Words.Where(w => w.CharIndex == wordCharIndex).SingleOrDefault();

                        if (theOne != null)
                        {
                            break;
                        }
                        else
                        {
                            foreach (Section section in sentense.Sections)
                            {
                                theOne = section.Words.Where(w => w.CharIndex == wordCharIndex).SingleOrDefault();

                                if (theOne != null)
                                {
                                    break;
                                }
                                else
                                {
                                    return null;
                                }
                            }

                            if (theOne != null)
                            {
                                break;
                            }
                        }
                    }

                    if (theOne != null)
                    {
                        break;
                    }
                }
            }

            return theOne;
        }


        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(this T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        public static bool IsPartOfAnchor(this string val)
        {
            Regex reg = new Regex(@"[\{\}\[\]<>\(\)]");

            return reg.IsMatch(val);
        }

        public static string ToValidFileName(this string fileName)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(fileName, "");
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
    (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}
