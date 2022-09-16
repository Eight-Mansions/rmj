using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rmj_tms_build
{
    class Program
    {
        static void Main(string[] args)
        {
            string inDir = args[0]; //"TITLE_TMS";
            string outFile = inDir.Replace("_TMS", ".TMS");

            List<string> files = Directory.GetFiles(inDir).ToList();
            files.Sort();

            if (File.Exists(outFile))
                File.Delete(outFile);


            List<uint> insPos = new List<uint>();

            BinaryWriter bw = new BinaryWriter(File.OpenWrite(outFile));
            bw.BaseStream.Seek((files.Count() * 4) + 4, SeekOrigin.Begin);

            for(int i = 0; i < files.Count; i++)
            {
                insPos.Add((uint)bw.BaseStream.Position);
                bw.Write(File.ReadAllBytes(files[i]));
            }

            bw.Seek(0, SeekOrigin.Begin);
            bw.Write((uint)insPos.Count);
            
            for (int i = 0; i < insPos.Count; i++)
            {
                bw.Write((uint)(insPos[i] - ((files.Count() * 4) + 4)));
            }

            bw.Close();
        }
    }
}
