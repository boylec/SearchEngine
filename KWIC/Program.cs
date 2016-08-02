using SearchEngine.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KWIC
{
    internal class Program
    {
        private static readonly SearchEngineContextFactory DbContextFactory = new SearchEngineContextFactory();
        public static SearchEngineEntities DbContext;
        public static void Main(string[] args)
        {
            DbContext = DbContextFactory.Create();

            var filePath = "";

            while (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                Console.WriteLine("Enter file name to parse and input into database:");
                filePath = Console.ReadLine();
            }

            LineStore.Init();
            IndexStore.Init();
            Input.Init(filePath);
            CircularShift.Init();
            while (!Input.EndOfFileReached){
                CircularShift.Shift(Input.GetLine());
            }
            Alphabetizer.Sort();
            Output.SaveOutput();
            Console.ReadKey();
        }

        public class Word
        {
            private readonly string _store;
            public Word(string input) { _store = input; }
            public string GetWord() { return _store; }
        }

        public class Line
        {
            private readonly string _url;
            private readonly Word[] _words;
            public Line(Word[] input, string url) {
                _words = input;
                _url = url;
            }
            public Word[] GetLine() { return _words; }
            public string GetUrl()
            {
                return _url;
            }
        }

        public static class LineStore
        {
            private static Line[] _lines;

            public static void Init()
            {
                _lines = new Line[0];
            }

            public static Line GetLine(int index)
            {
                return _lines[index];
            }

            public static int AddLine(Line line)
            {
                var temp = new Line[_lines.Length + 1];
                for (var i = 0; i < _lines.Length; i++)
                    temp[i] = _lines[i];
                temp[_lines.Length] = line;
                _lines = temp;
                return _lines.Length - 1;
            }

            public static string GetString(Index i)
            {
                var output = "";
                var words = _lines[i.LineNumber].GetLine();
                for (var j = i.Start; j < words.Length; j++)
                    output += words[j].GetWord() + ' ';
                for (var j = 0; j < i.Start; j++)
                    output += words[j].GetWord() + ' ';
                return output.Trim();
            }

            public static string GetUrl(Index i)
            {
                return _lines[i.LineNumber].GetUrl();
            }
        }

        public static class IndexStore
        {
            public static Index[] Indices;

            public static void Init()
            {
                Indices = new Index[0];
            }

            public static int AddIndex(Index index)
            {
                var temp = new Index[Indices.Length + 1];
                for (var i = 0; i < Indices.Length; i++)
                    temp[i] = Indices[i];
                temp[Indices.Length] = index;
                Indices = temp;
                return Indices.Length - 1;
            }
        }

        public class Index : IComparable
        {
            public int Start;
            public int LineNumber;
            //private readonly IndexStore _parent;
            public Index(int strt, int lin)
            {
                Start = strt;
                LineNumber = lin;
            }

            public int CompareTo(object obj)
            {
                var myString = LineStore.GetString(this);
                var oString = LineStore.GetString((Index)obj);
                return string.Compare(myString, oString, StringComparison.Ordinal);
            }
        }

        public static class Input
        {
            private static StreamReader _reader;
            private static string _input;

            public static bool EndOfFileReached => _reader.EndOfStream;

            public static void Init(string filePath)
            {
                _reader = new StreamReader(filePath);
                _input = "";
            }

            private static Word[] GetWords(out string url)
            {
                url = null;

                if ((_input = _reader.ReadLine()) == null)
                {
                    return null;
                }

                var parsed = LineParser.Parse(_input).ToList();

                url = parsed[0];

                parsed.RemoveAt(0);

                var tokens = parsed.ToArray();

                var words = new Word[tokens.Length];

                for (var i = 0; i < tokens.Length; i++)
                    words[i] = new Word(tokens[i]);
                return words;
            }

            public static int GetLine()
            {
                string url;
                var words = GetWords(out url);
                return LineStore.AddLine(new Line(words,url));
            }
        }

        public static class CircularShift
        {
            public static void Init()
            {
                
            }

            public static int Shift(int line)
            {
                var tempLine = LineStore.GetLine(line);
                var words = tempLine.GetLine();
                for (var i = 0; i < words.Length; i++)
                {
                    IndexStore.AddIndex(new Index(i, line));
                }
                return words.Length;
            }
        }

        private static class Alphabetizer
        {
            public static void Sort()
            {
                MergerSort.MergeSort(IndexStore.Indices);
            }
        }

        public static class Output
        {
            public static void SaveOutput()
            {
                foreach (var index in IndexStore.Indices)
                {

                    var line = LineStore.GetLine(index.LineNumber);
                    var words = line.GetLine().Select(x => x.GetWord());
                    var url = line.GetUrl();

                    

                    foreach (var word in words)
                    {
                        var wordEntry = DbContext.Words.SingleOrDefault(x => x.WordString == word) ??
                                        DbContext.Words.Add(new SearchEngine.Data.Word()
                        {
                            WordString = word
                        });

                        var urlEntry = DbContext.Urls.SingleOrDefault(x => x.UrlString == url) ??
                                       DbContext.Urls.Add(new Url()
                        {
                            UrlString = url
                        });

                        var association =
                            DbContext.WordUrlAssociations.SingleOrDefault(
                                x => x.Url.UrlString == url && x.Word.WordString == word) ??

                            DbContext.WordUrlAssociations.Add(new WordUrlAssociation()
                                {
                                    Url = urlEntry,
                                    Word = wordEntry
                                });
                        
                        DbContext.SaveChanges();

                        Console.WriteLine($"Association saved. URL: {association.Url.UrlString}, Word: {association.Word.WordString}");

                        //
                    }
                }
            }
        }

        public static class LineParser
        {
            public static string[] Parse(string input)
            {
                var parsed = input.Split(',');
                return parsed;
            }
        }
    }
}
