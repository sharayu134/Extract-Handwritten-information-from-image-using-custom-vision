using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary13
{
    public class Word
    {
        public List<int> boundingBox { get; set; }
        public string text { get; set; }
        public string confidence { get; set; }
    }

    public class Line
    {
        public List<int> boundingBox { get; set; }
        public string text { get; set; }
        public List<Word> words { get; set; }
    }

    public class RecognitionResult
    {
        public List<Line> lines { get; set; }
    }

    public class RootObject
    {
        public string status { get; set; }
        public RecognitionResult recognitionResult { get; set; }
    }

}
