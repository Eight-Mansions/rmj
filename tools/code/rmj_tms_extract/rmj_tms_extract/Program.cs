using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rmj_tms_extract
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = "TITLE.TMS";
            BinaryReader bin = new BinaryReader(File.OpenRead(filename));

            uint cnt = bin.ReadUInt32();

            List<uint> timPos = new List<uint>();
            for(int i = 0; i < cnt; i++)
            {
                timPos.Add(bin.ReadUInt32());
            }

            uint offset = (uint)bin.BaseStream.Position;


            string outDir = filename.Replace(".TMS", "_TMS");
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            for(int i = 0; i < cnt; i++)
            {
                bin.BaseStream.Seek(timPos[i] + offset, SeekOrigin.Begin);

                int length = (int)((i + 1 < timPos.Count) ? timPos[i + 1] - timPos[i] : bin.BaseStream.Length - timPos[i]);

                string outFilename = outDir + "\\" + i.ToString("D2") + ".tim";
                if (File.Exists(outFilename))
                    File.Delete(outFilename);

                File.WriteAllBytes(outFilename, bin.ReadBytes(length));
    
            }

            bin.Close();
        }
    }
}
