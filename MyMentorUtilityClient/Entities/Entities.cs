using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MyMentor
{
    public class Category
    {
        public string ObjectId { get; set; }
        public string Value { get; set; }
        public decimal MinPrice { get; set; }
    }
    public class SectionCellData
    {
        public int Index { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string Text { get; set; }
    }

    public class WordsJsonConverter : CustomCreationConverter<List<Word>>
    {
        public override List<Word> Create(Type objectType)
        {
            return new List<Word>();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var list = value as IList<Word>;
            if (list.Count() > 0)
            {
                base.WriteJson(writer, value, serializer);
            }
        }
    }

    [Serializable()]
    public abstract class BaseSection
    {

        [JsonProperty(PropertyName = "text", Order = 1)]
        [XmlAttribute("Content")]
        public virtual string Content
        {
            get
            {
                return m_content.Replace("[3]", "").Replace("[2]", "").Replace("[1]", "").Replace("[0]", "");
            }
            set
            {
                m_content = value;
            }
        }

        [JsonIgnore]
        [XmlIgnore]
        public TimeSpan StartTime
        {
            get
            {
                switch (this.GetType().ToString())
                {
                    case "MyMentor.Chapter":

                        var firstParagraph = ((Chapter)this).Paragraphs.FirstOrDefault();

                        if (firstParagraph != null)
                        {
                            m_startTime = firstParagraph.StartTime;
                        }
                        break;
                    case "MyMentor.Paragraph":

                        var firstSentense = ((Paragraph)this).Sentences.FirstOrDefault();

                        if (firstSentense != null)
                        {
                            m_startTime = firstSentense.StartTime;
                        }
                        break;
                    case "MyMentor.Sentence":
                        var firstSection = ((Sentence)this).Sections.FirstOrDefault();

                        if (firstSection != null)
                        {
                            m_startTime = firstSection.StartTime;
                        }
                        break;
                    case "MyMentor.Section":
                        var firstWord = ((Section)this).Words.FirstOrDefault();

                        if (firstWord != null)
                        {
                            m_startTime = firstWord.StartTime;
                        }
                        break;
                }

                return m_startTime;
            }
            set
            {
                m_startTime = value;
            }
        }


        private TimeSpan m_duration;
        private TimeSpan m_startTime;
        protected string m_content = string.Empty;

        [JsonIgnore]
        [XmlIgnore]
        public TimeSpan Duration
        {
            get
            {
                //last word...we have its duration manually
                switch (this.GetType().ToString())
                {
                    case "MyMentor.Chapter":
                        m_duration = new TimeSpan(((Chapter)this).Paragraphs.Sum(s => s.Duration.Ticks));
                        break;
                    case "MyMentor.Paragraph":
                        m_duration = new TimeSpan(((Paragraph)this).Sentences.Sum(p => p.Duration.Ticks));
                        break;
                    case "MyMentor.Sentence":
                        m_duration = new TimeSpan(((Sentence)this).Sections.Sum(p => p.Duration.Ticks));
                        break;
                    case "MyMentor.Section":
                        m_duration = new TimeSpan(((Section)this).Words.Sum(p => p.Duration.Ticks));
                        break;
                    case "MyMentor.Word":

                        var nextWord = ((Word)this).NextWord;
                        if (nextWord != null)
                        {
                            m_duration = nextWord.StartTime - ((Word)this).StartTime;

                            if (m_duration < TimeSpan.Zero)
                            {
                                m_duration = TimeSpan.Zero;
                            }
                        }

                        break;
                }

                return m_duration;
            }
            set
            {
                m_duration = value;
            }
        }


        [JsonIgnore]
        [XmlIgnore]
        public TimeSpan EndTime
        {
            get
            {
                return m_startTime.Add(m_duration);
            }
        }

        [JsonProperty(PropertyName = "index", Order = 1)]
        public int Index { get; set; }

        [JsonProperty(PropertyName = "charIndex", Order = 2)]
        public int CharIndex { get; set; }

        [JsonProperty(PropertyName = "length", Order = 3)]
        public virtual int Length
        {
            get
            {
                return Content.Length;
            }

        }

        [JsonIgnore]
        public int RealCharIndex { get; set; }

        [JsonProperty(PropertyName = "audioStart", Order = 4)]
        [XmlIgnore]
        public string StartTimeText
        {
            get
            {
                return this.StartTime.ToString(@"hh\:mm\:ss\.fff");
            }
        }

        [JsonProperty(PropertyName = "audioEnd", Order = 6)]
        [XmlIgnore]
        public string EndTimeText
        {
            get
            {
                return this.EndTime.ToString(@"hh\:mm\:ss\.fff");
            }
        }

        [JsonProperty(PropertyName = "audioDuration", Order = 5)]
        [XmlIgnore]
        public string DurationText
        {
            get
            {
                return this.Duration.ToString(@"hh\:mm\:ss\.fff");
            }
        }

        [JsonIgnore]
        [XmlAttribute("StartTime")]
        public long XmlStartTime
        {
            get { return StartTime.Ticks; }
            set { StartTime = new TimeSpan(value); }
        }

        [JsonIgnore]
        [XmlAttribute("Duration")]
        public long XmlDuration
        {
            get { return Duration.Ticks; }
            set { Duration = new TimeSpan(value); }
        }
    }


    [Serializable()]
    public class Word : BaseSection
    {
        [JsonIgnore]
        public short GraphicItemUnique { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Word PreviousWord { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Word NextWord { get; set; }
    }

    [Serializable()]
    public class Section : BaseSection, ICloneable
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }


        [JsonProperty(PropertyName = "words", Order = 8, NullValueHandling = NullValueHandling.Ignore)]
        [XmlArrayItem("Words")]
        public virtual List<Word> Words
        {
            get;
            set;
        }


    }

    [Serializable()]
    public class Sentence : BaseSection, ICloneable
    {
        [JsonProperty(PropertyName = "sections", Order = 8)]
        [XmlArrayItem("Sections")]
        public List<Section> Sections { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    [Serializable()]
    public class Chapter : BaseSection, ICloneable
    {
        [JsonProperty(PropertyName = "paragraphs", Order = 8)]
        [XmlArrayItem("Paragraphs")]
        public List<Paragraph> Paragraphs { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        [JsonIgnore]
        [XmlIgnore]
        public Word FirstWord
        {
            get;
            set;
        }

        [JsonIgnore]
        [XmlIgnore]
        public Word LastWord
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "length", Order = 3)]
        public override int Length
        {
            get
            {
                return this.Content.Length;
            }

        }

    }

    [Serializable()]
    public class Paragraph : BaseSection, ICloneable
    {
        [JsonProperty(PropertyName = "sentences", Order = 8)]
        [XmlArrayItem("Sentences")]
        public List<Sentence> Sentences { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public enum AnchorType
    {
        Paragraph,
        Sentence,
        Section,
        Word,
        None
    }

    public enum AnchorDirection
    {
        Open,
        Close
    }
}
