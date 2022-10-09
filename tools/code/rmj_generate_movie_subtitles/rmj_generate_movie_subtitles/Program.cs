using SubtitlesParser.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace rmj_generate_movie_subtitles
{
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

        static int[] GetEncoding(string line, string mapping)
        {
            List<int> mappings = new List<int>();
            for (int i = 0; i < line.Length; i++)
            {
                mappings.Add(mapping.IndexOf(line[i]));
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
            string movieSubsPath = args[0]; //"trans\\audio_subs\\rmj_audio.txt";
            string mappingFile = args[1];
            string audioSubsDisc = args[2]; // "disc1";

            // First let's rename all files
            foreach (string infFilename in Directory.GetFiles(movieSubsPath, "*.inf"))
            {
                string inf = (new FileInfo(infFilename)).Name.Split(".")[0];
                string[] infLines = File.ReadAllLines(infFilename);
                for (int i = 0; i < infLines.Length; i++)
                {

                    try
                    {
                        string properName = infLines[i].Split(" ")[3];
                        string assName = movieSubsPath + "\\" + inf + ".DAT[" + i + "].ass";
                        if (File.Exists(assName))
                            File.Move(assName, assName.Replace(".DAT[" + i + "]", "_" + properName));
                    }
                    catch { }; // Due ot the line ending in the inf file it'll break
                }
            }


            string mappingfilename = mappingFile;
            string mapping = File.ReadAllText(mappingfilename).Replace("\r", "").Replace("\n", "");

            //var something = new Opportunity.AssLoader.
            string generatedAudioFilename = (audioSubsDisc == "disc1") ? "generated_movie_1.cpp" : "generated_movie_2.cpp";

            StreamWriter generated = new StreamWriter("code\\rmj\\subtitle\\" + generatedAudioFilename, false);
            generated.WriteLine("#include \"generated.h\"");
            generated.WriteLine();

            int partIdx = 0;
            int subIdx = 0;
            int defaultX = 320;
            int defaultY = 144;
            List<string> subs = new List<string>();

            foreach (string assname in Directory.GetFiles(movieSubsPath, "*.ass"))
            {
                //string assname = "EV1A_1M07.ass";
                var parser = new SubtitlesParser.Classes.Parsers.SubParser();
                var lines = parser.ParseStream(File.OpenRead(assname));


                int indexOfUnderline = assname.LastIndexOf("_") + 1;
                int indexOfPeriod = assname.LastIndexOf(".");

                string strName = assname.Substring(indexOfUnderline, indexOfPeriod - indexOfUnderline);
                int hash = sdbmHash(strName + '\0');





                List<string> subLines = new List<string>();
                List<int> startTimes = new List<int>();
                List<int> endTimes = new List<int>();


                for (int i = 0; i < lines.Count; i++)
                {
                    SubtitleItem line = lines[i];
                    subLines.AddRange(line.PlaintextLines);
                    foreach (string plainLine in line.PlaintextLines)
                    {
                        startTimes.Add(line.StartTime);
                        endTimes.Add(line.EndTime);
                    }

                }

                List<string> partdata = new List<string>();
                int timing = 1;
                //foreach (string engLine in subLines)
                for (int i = 0; i < subLines.Count; i++)
                {
                    string engLine = subLines[i];
                    int startFrame = (int)(((float)15 / (float)1000) * startTimes[i]); // 15 frames per second
                    int endFrame = (int)(((float)15 / (float)1000) * endTimes[i]);

                    if (!String.IsNullOrEmpty(engLine))
                    {
                        //string[] formatted = Format(engLine, 200, null).Split(new char[] { '\n' });

                        //for (int z = 0; z < formatted.Length; z++)
                        //{
                        //    string centered = formatted[z];

                        //    int textWidth = GetWidth(centered);
                        //    int totalPadding = ((200 >> 1) - (textWidth >> 1));
                        //    totalPadding = (totalPadding + 4) / 5; // Round up to nearest divisible by 5 and divide by 5.  5 is chosen as our space is 5 pixels wide.

                        //    string padding = "";
                        //    for (int j = 0; j < totalPadding; j++)
                        //    {
                        //        padding += " ";
                        //    }
                        //    centered = padding + centered;

                        //    formatted[z] = centered;
                        //}

                        //string formattedLine = String.Join("\\n", formatted);

                        //int[] encoding = GetEncoding(formattedLine);


                        int textWidth = GetWidth(engLine);
                        while ((textWidth / 2) % 8 != 0)
                            textWidth++;

                        int totalPadding = ((320 >> 1) - (textWidth >> 1));
                        int x = defaultX + totalPadding;
                        int y = defaultY;

                        if (i != 0 && (startTimes[i - 1] <= startTimes[i] && startTimes[i] < endTimes[i - 1]))
                        {
                            y += 16;
                        }



                        //    totalPadding = (totalPadding + 4) / 5; // Round up to nearest divisible by 5 and divide by 5.  5 is chosen as our space is 5 pixels wide.

                        int[] encoding = GetEncoding(engLine, mapping);

                        generated.WriteLine("//" + assname.Replace(".ass", "") + " | " + engLine);
                        generated.WriteLine(String.Format("const u8 partdata_{0}[] = {{ {1} }};", partIdx, String.Join(", ", encoding)));
                        generated.WriteLine("");




                        partdata.Add(String.Format("{{(const char*)partdata_{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}}},", partIdx, encoding.Length, 255, startFrame, endFrame, x, y, x, y));

                        timing += 14;
                        partIdx++;
                    }
                }

                generated.WriteLine(String.Format("MovieSubtitlePart sub{0}_parts[] = {{", subIdx));
                foreach (string parts in partdata)
                {
                    generated.WriteLine("\t" + parts);
                }
                generated.WriteLine("};");
                generated.WriteLine("");

                subs.Add(String.Format("{{{0}, {1}, sub{2}_parts}},", hash, partdata.Count, subIdx));

                subIdx++;
            }


            generated.WriteLine("const u32 movieSubtitlesCount = {0};", subIdx);
            generated.WriteLine("MovieSubtitle movieSubtitles[] = {");

            foreach (string sub in subs)
            {
                generated.WriteLine("\t" + sub);
            }

            generated.WriteLine("};");

            generated.Close();


            //
            //if (line.Contains(".XA"))
            //{
            //    string xaName = line.Substring(0, line.IndexOf("["));
            //    string idx = line.Substring(line.IndexOf("[")).Replace("[", "").Replace("]", "");

            //    List<string> subLines = new List<string>();
            //    for (int j = i + 1; j < lines.Length; j++)
            //    {
            //        line = lines[j];
            //        if (line.Contains(".XA"))
            //        {
            //            i = j - 1;
            //            break;
            //        }
            //        else
            //        {
            //            subLines.Add(line);
            //        }
            //    }

            //    int hash = sdbmHash(strName + '\0');


            //    List<string> partdata = new List<string>();
            //    int timing = 1;
            //    foreach (string engLine in subLines)
            //    {
            //        if (!String.IsNullOrEmpty(engLine))
            //        {
            //            string[] formatted = Format(engLine, 200, null).Split(new char[] { '\n' });

            //            for (int z = 0; z < formatted.Length; z++)
            //            {
            //                string centered = formatted[z];

            //                int textWidth = GetWidth(centered);
            //                int totalPadding = ((200 >> 1) - (textWidth >> 1));
            //                totalPadding = (totalPadding + 4) / 5; // Round up to nearest divisible by 5 and divide by 5.  5 is chosen as our space is 5 pixels wide.

            //                string padding = "";
            //                for (int j = 0; j < totalPadding; j++)
            //                {
            //                    padding += " ";
            //                }
            //                centered = padding + centered;

            //                formatted[z] = centered;
            //            }

            //            string formattedLine = String.Join("\\n", formatted);

            //            int[] encoding = GetEncoding(formattedLine);

            //            generated.WriteLine("//" + xaName + " | " + idx + " | " + formattedLine);
            //            generated.WriteLine(String.Format("const u8 partdata_{0}[] = {{{1}}};", partIdx, String.Join(", ", encoding)));
            //            generated.WriteLine("");

            //            partdata.Add(String.Format("{{(const char*)partdata_{0}, {1}, {2}}},", partIdx, encoding.Length / 2, timing));

            //            timing += 14;
            //            partIdx++;
            //        }
            //    }

            //    generated.WriteLine(String.Format("const subtitle_part sub{0}_parts[] = {{", subIdx));
            //    foreach (string parts in partdata)
            //    {
            //        generated.WriteLine("\t" + parts);
            //    }
            //    generated.WriteLine("};");
            //    generated.WriteLine("");

            //    subs.Add(String.Format("{{{0}, {1}, {2}, sub{3}_parts}},", hash, idx, partdata.Count, subIdx));

            //    subIdx++;
            //}


            //////{1544405569, 0, 1, 1, 1, sub0_parts} /* cv/dan_1501.at3 */,
            ////subs.Add(String.Format("{{{0}, (const char*)subdata_{1}}}, //{2}", hash, subIdx, filename));


            //}


        }
    }
}
