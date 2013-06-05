using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyMentorUtilityClient
{
    public static class ExtensionMethods
    {
        public static string RemovePunctation(this string value)
        {
            char[] chars = value.ToCharArray();
            StringBuilder sb = new StringBuilder();

            foreach (char c in chars)
            {
                if ((int)c >= 1488 && (int)c <= 1514)
                {
                    sb = sb.Append(c);
                }
            }

            return sb.ToString();
        }


        public static IEnumerable<Word> FlattenWords(this IEnumerable<Paragraph> paragraphs)
        {
            return paragraphs.SelectMany(p => p.Words)
                .Concat(paragraphs.SelectMany(p => p.Sentences).SelectMany(s => s.Words))
                .Concat(paragraphs.SelectMany(p => p.Sentences).SelectMany(s => s.Sections).SelectMany(se => se.Words))
                .OrderBy(w => w.Index);
        }

        public static Word SeekForWord(this IEnumerable<Paragraph> paragraphs, int wordCharIndex)
        {
            Word theOne = null;

            foreach (Paragraph paragraph in paragraphs)
            {
                theOne = paragraph.Words.Where(w => w.CharIndex == wordCharIndex).SingleOrDefault();

                if (theOne != null)
                {
                    break;
                }
                else
                {
                    foreach (Sentence sentense in paragraph.Sentences)
                    {
                        theOne = sentense.Words.Where(w => w.CharIndex == wordCharIndex).SingleOrDefault();

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
