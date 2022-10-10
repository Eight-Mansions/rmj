using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rmg_generate_audio_subtitles
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Subtitle
    {
        public int id { get; set; }
        public int logicalId { get; set; }
        public string original { get; set; }
        public string audioPath { get; set; }
        public string imagePath { get; set; }
        public string translated { get; set; }
        public string notes { get; set; }
        public string translatedSuggestion { get; set; }
        public string notesSuggestion { get; set; }
        public string state { get; set; }
        public int version { get; set; }
    }

    public class Subtitles
    {
        public List<Subtitle> data { get; set; }
        public string status { get; set; }
    }



    class Program
    {

        static int GetWidth(string line)
        {
            int cur_len = 0;
            for (int i = 0; i < line.Length; i++)
            {
                cur_len += 0x08; // Hard coded for now

            }

            return cur_len;
        }

        static string Format(string block, int max_len, Dictionary<string, int> widths)
        {
            if (block.Contains("A constant"))
            {
                int boopme = 0;
            }
            //block = block.Replace("\\n", "\r");
            string[] lines = block.Split(new string[] { "\\r" }, StringSplitOptions.None);


            string formattedBlock = "";
            string formattedLine = "";

            int curWidth = 0;
            //foreach (string line in lines)
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (line.IndexOf("//") != 0)
                {
                    int lineIdx = 0;
                    line = line.Replace("\\n", "\r");
                    while (true)
                    {
                        if (lineIdx == line.Length)
                            break;

                        if (line[lineIdx] == '<' && lineIdx + 1 < line.Length && line[lineIdx + 1] == '$')
                        {
                            while (true)
                            {
                                formattedLine += line[lineIdx++];
                                if (line[lineIdx - 1] == '>')
                                {
                                    break;
                                }
                            }
                        }
                        else if (line[lineIdx] == '\r')
                        {
                            formattedBlock += formattedLine + "\n";
                            formattedLine = "";
                            curWidth = 0;
                            lineIdx++;
                        }
                        else
                        {
                            formattedLine += line[lineIdx++];
                        }

                        if (GetWidth(formattedLine) > max_len)
                        {
                            int formattedLineLength = formattedLine.Length - 1;
                            while (true)
                            {
                                formattedLineLength--;
                                if (formattedLineLength == 0)
                                {
                                    formattedBlock += formattedLine + "\n";
                                    formattedLine = ""; //It's too big to fit ah well....
                                    curWidth = 0;
                                    break;
                                }

                                if ((curWidth + GetWidth(formattedLine.Substring(0, formattedLineLength))) < max_len && formattedLine[formattedLineLength] == ' ')
                                {
                                    formattedBlock += formattedLine.Substring(0, formattedLineLength) + "\n";
                                    formattedLine = formattedLine.Substring(formattedLineLength + 1);
                                    curWidth = 0;
                                    break;
                                }
                            }
                        }
                    }

                    curWidth += GetWidth(formattedLine);
                    formattedBlock += formattedLine + "\n";
                    formattedLine = "";
                }
                else
                {
                    formattedBlock += line + "\n";
                }
            }

            return formattedBlock.Trim();
        }

        static int[] GetEncoding(string line, string mapping)
        {
            List<int> mappings = new List<int>();
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '\n')
                {
                    mappings.Add(0x7F);
                }
                else if (line[i] == '<' && line[i + 1] == '$')
                {
                    int val = Int32.Parse(line[i + 2].ToString() + line[i + 3].ToString(), System.Globalization.NumberStyles.HexNumber);
                    i += 4;

                    mappings.Add(val);
                }
                else
                {
                    mappings.Add(mapping.IndexOf(line[i]) + 1);
                }
            }
            return mappings.ToArray();
        }

        static int sdbmHash(string audioname)
        {
            int hash = 0;
            int i = 0;

            for (; audioname[i] != 0; i++)
            {
                hash = audioname[i] + (hash << 6) + (hash << 16) - hash;
            }
            return hash & 0xFFFF;
        }

        static void Main(string[] args)
        {
            string audioSubsPath = args[0]; //"trans\\audio_subs\\rmj_audio.txt";
            string mappingFile = args[1];
            string audioSubsDisc = args[2]; // "disc1";

            Subtitles all = JsonConvert.DeserializeObject<Subtitles>(File.ReadAllText(audioSubsPath));
            List<Subtitle> subs = all.data.Where(x => !String.IsNullOrEmpty(x.translated)).ToList();

            string mappingfilename = mappingFile;
            string mapping = File.ReadAllText(mappingfilename).Replace("\r", "").Replace("\n", "");

            string generatedAudioFilename = (audioSubsDisc == "disc1") ? "generated_audio_1.cpp" : "generated_audio_2.cpp";

            StreamWriter generated = new StreamWriter("code\\rmj\\subtitle\\" + generatedAudioFilename, false, Encoding.GetEncoding("SJIS"));
            generated.WriteLine("#include \"generated.h\"");
            generated.WriteLine("");

            int partIdx = 0;
            int subIdx = 0;
            List<int> collisions = new List<int>();
            List<string> collected = new List<string>();
            foreach (Subtitle sub in subs)
            {
                if (sub.original.ToLower().Contains(audioSubsDisc))
                {
                    string audioPath = sub.audioPath;
                    string audioId = sub.audioPath.Substring(sub.audioPath.LastIndexOf("_") + 1).Replace(".wav", "");
                    int hash = sdbmHash(audioId + "\0");
                    if (collisions.Contains(hash))
                    {
                        int boopme = 0;
                    }
                    else
                    {
                        collisions.Add(hash);
                    }

                    List<string> partdata = new List<string>();

                    string translated = "";
                    string notes = "";
                    if (sub.translated.Contains("disc"))
                    {
                        translated = subs.Where(x => x.audioPath == sub.translated.Replace("@", "")).Select(x => x.translated).FirstOrDefault();
                        notes = subs.Where(x => x.audioPath == sub.translated.Replace("@", "")).Select(x => x.notes).FirstOrDefault();
                    }
                    else
                    {
                        translated = sub.translated;
                        notes = sub.notes;
                    }

                    if (!String.IsNullOrEmpty(translated))
                    {

                         List<string> subLines = translated.Split(new char[] { '\n' }).ToList();
                        subLines.Add(" ");
                        List<string> timings = new List<string> { "1" };
                        timings.AddRange(notes.Replace("0,1\n", "").Split(new char[] { '\n' }));

                        for (int i = 0; i < subLines.Count; i++)
                        {
                            int y = 432;
                            string subLine = subLines[i].Replace("…", "...");

                            //string[] formatted = Format(subLine, 288, null).Split(new char[] { '\n' });
                            string line = Format(subLine, 288, null);
                            if (String.IsNullOrEmpty(line))
                            {
                                line = " ";
                            }

                            int centerX = -1;
                            string centered = "";
                            foreach (string part in line.Split(new char[] { '\n' }))
                            {
                                if (!String.IsNullOrEmpty(centered))
                                {
                                    centered += "\n";
                                }

                                int textWidth = GetWidth(part);

                                int totalPadding = ((320 >> 1) - (textWidth >> 1));
                                if (centerX == -1)
                                {
                                    centerX = totalPadding;
                                }
                                else
                                {
                                    centered += "<$" + totalPadding.ToString("X2") + ">";
                                }

                                centered += part;
                            }

                            string timing = timings[i];

                            // foreach(string line in formatted)
                            {

                                int[] encoding = GetEncoding(centered, mapping);

                                generated.WriteLine("//" + audioPath + " | " + line.Replace("\n", "\\n"));
                                generated.WriteLine(String.Format("const u8 partdata_{0}[] = {{{1}}};", partIdx, String.Join(", ", encoding)));
                                generated.WriteLine("");

                                partdata.Add(String.Format("{{(const char*)partdata_{0}, {1}, {2}, {3}, {4}}},", partIdx, encoding.Length, timing, centerX, y));

                                int newLineCount = encoding.Where(x => x == 0xFF).Count();
                                newLineCount += 1;
                                y += newLineCount * 12;
                                partIdx++;

                            }
                        }
                    }

                    generated.WriteLine(String.Format("const subtitle_part sub{0}_parts[] = {{", subIdx));
                    foreach (string parts in partdata)
                    {
                        generated.WriteLine("\t" + parts);
                    }
                    generated.WriteLine("};");
                    generated.WriteLine("");

                    collected.Add(String.Format("{{{0}, {1}, sub{2}_parts}},", hash, partdata.Count, subIdx));

                    subIdx++;
                }
            }
            generated.WriteLine("const u32 subsCount = {0};", subIdx);
            generated.WriteLine("const subtitle subs[] = {");

            foreach (string parts in collected)
            {
                generated.WriteLine("\t" + parts);
            }

            generated.WriteLine("};");

            generated.Close();
        }
    }
}
