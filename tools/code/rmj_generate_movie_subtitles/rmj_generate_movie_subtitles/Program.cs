using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rmj_generate_movie_subtitles
{
    class Program
    {
        static int[] widths = new int[] {
        0x09, // A
		0x08, // B
		0x07, // C
		0x08, // D
		0x07, // E
		0x07, // F
		0x08, // G
		0x08, // H
		0x03, // I
		0x07, // J
		0x08, // K
		0x07, // L
		0x08, // M
		0x08, // N
		0x08, // O
		0x08, // P
		0x08, // Q
		0x08, // R
		0x07, // S
		0x08, // T
		0x07, // U
		0x08, // V
		0x09, // W
		0x07, // X
		0x07, // Y
		0x08, // Z
		0x07, // a
		0x07, // b
		0x07, // c
		0x07, // d
		0x07, // e
		0x05, // f
		0x07, // g
		0x07, // h
		0x03, // i
		0x04, // j
		0x07, // k
		0x04, // l
		0x07, // m
		0x07, // n
		0x07, // o
		0x07, // p
		0x07, // q
		0x06, // r
		0x07, // s
		0x05, // t
		0x07, // u
		0x07, // v
		0x07, // w
		0x07, // x
		0x07, // y
		0x07, // z
		0x04, // .
		0x04, // ,
		0x03, // !
		0x07, // ?
		0x06, // "
		0x04, // (
		0x05, // )
		0x04, // :
		0x04, // ;
		0x07, // ~
		0x03, // '
		0x06, // -
		0x07, // 0
		0x05, // 1
		0x08, // 2
		0x07, // 3
		0x08, // 4
		0x07, // 5
		0x07, // 6
		0x07, // 7
		0x08, // 8
		0x07, // 9
		0x04 //  
        };

        static int GetWidth(string line)
        {
            int cur_len = 0;
            for (int i = 0; i < line.Length; i++)
            {
                char letter = line[i];
                cur_len += widths[letter - 0x20];
            }

            return cur_len;
        }

        static string Format(string block, int max_len, Dictionary<string, int> widths)
        {
            //block = block.Replace("\\n", "\r");
            string[] lines = block.Split(new string[] { "\\n" }, StringSplitOptions.None);


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

        static int[] GetEncoding(string line)
        {
            List<int> mappings = new List<int>();
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '\\')
                {
                    mappings.Add(line[i]);
                    mappings.Add(line[i + 1]);
                    i++;
                }
                else
                {
                    uint mapped = (uint)line[i] + 0x827F;
                    mappings.Add((int)mapped >> 8);
                    mappings.Add((int)mapped & 0xFF);
                }
            }
            mappings.Add(0x0D);
            mappings.Add(0x0A);
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
            return hash;
        }

        static void Main(string[] args)
        {
            string path = "_subtitles.txt";
            string outC = "subtitles_1.c";
            string mappingfilename = "graphics\\font\\mapping.txt";

            var parser = new SubtitlesParser.Classes.Parsers.SubParser();
            using (var fileStream = File.OpenRead("EV1A.DAT0.ass"))
            {
                var items = parser.ParseStream(fileStream);
            }


            //string mapping = File.ReadAllText(mappingfilename).Replace("\r", "").Replace("\n", "");

            string[] lines = File.ReadAllLines(path, Encoding.GetEncoding("SJIS"));

            StreamWriter generated = new StreamWriter("generated.cpp", false, Encoding.GetEncoding("SJIS"));

            //generated.WriteLine("// DO NOT MODIFY. THIS FILE IS AUTO-GENERATED.");
            //generated.WriteLine("");
            //generated.WriteLine("#include \"generated.h\"");
            //generated.WriteLine("");

            int partIdx = 0;
            int subIdx = 0;

            List<int> collisions = new List<int>();
            List<string> subs = new List<string>();
            //foreach (string line in lines)
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (line.Contains(".XA"))
                {
                    string xaName = line.Substring(0, line.IndexOf("["));
                    string idx = line.Substring(line.IndexOf("[")).Replace("[", "").Replace("]", "");

                    List<string> subLines = new List<string>();
                    for (int j = i + 1; j < lines.Length; j++)
                    {
                        line = lines[j];
                        if (line.Contains(".XA"))
                        {
                            i = j - 1;
                            break;
                        }
                        else
                        {
                            subLines.Add(line);
                        }
                    }

                    int hash = sdbmHash(xaName + '\0');


                    List<string> partdata = new List<string>();
                    int timing = 1;
                    foreach (string engLine in subLines)
                    {
                        if (!String.IsNullOrEmpty(engLine))
                        {
                            string[] formatted = Format(engLine, 200, null).Split(new char[] { '\n' });

                            for (int z = 0; z < formatted.Length; z++)
                            {
                                string centered = formatted[z];

                                int textWidth = GetWidth(centered);
                                int totalPadding = ((200 >> 1) - (textWidth >> 1));
                                totalPadding = (totalPadding + 4) / 5; // Round up to nearest divisible by 5 and divide by 5.  5 is chosen as our space is 5 pixels wide.

                                string padding = "";
                                for (int j = 0; j < totalPadding; j++)
                                {
                                    padding += " ";
                                }
                                centered = padding + centered;

                                formatted[z] = centered;
                            }

                            string formattedLine = String.Join("\\n", formatted);

                            int[] encoding = GetEncoding(formattedLine);

                            generated.WriteLine("//" + xaName + " | " + idx + " | " + formattedLine);
                            generated.WriteLine(String.Format("const u8 partdata_{0}[] = {{{1}}};", partIdx, String.Join(", ", encoding)));
                            generated.WriteLine("");

                            partdata.Add(String.Format("{{(const char*)partdata_{0}, {1}, {2}}},", partIdx, encoding.Length / 2, timing));

                            timing += 14;
                            partIdx++;
                        }
                    }

                    generated.WriteLine(String.Format("const subtitle_part sub{0}_parts[] = {{", subIdx));
                    foreach (string parts in partdata)
                    {
                        generated.WriteLine("\t" + parts);
                    }
                    generated.WriteLine("};");
                    generated.WriteLine("");

                    subs.Add(String.Format("{{{0}, {1}, {2}, sub{3}_parts}},", hash, idx, partdata.Count, subIdx));

                    subIdx++;
                }


                ////{1544405569, 0, 1, 1, 1, sub0_parts} /* cv/dan_1501.at3 */,
                //subs.Add(String.Format("{{{0}, (const char*)subdata_{1}}}, //{2}", hash, subIdx, filename));


            }

            generated.WriteLine("const u32 subsCount = {0};", subIdx);
            generated.WriteLine("const subtitle subs[] = {");

            foreach (string sub in subs)
            {
                generated.WriteLine("\t" + sub);
            }

            generated.WriteLine("};");

            generated.Close();
        }
    }
}
