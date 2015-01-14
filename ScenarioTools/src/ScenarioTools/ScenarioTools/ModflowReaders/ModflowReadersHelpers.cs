using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ScenarioTools.ModflowReaders
{
    public class ModflowReadersHelpers
    {
        public static void ReadModflowHeader(int precision, BinaryReader br, out int kstp, out int kper, out double totimd)
        {
            totimd = 0.0;
            kstp = br.ReadInt32();
            kper = br.ReadInt32();
            switch (precision)
            {
                case 1:
                    float pertim = br.ReadSingle();
                    float totim = br.ReadSingle();
                    totimd = Convert.ToDouble(totim);
                    break;
                case 2:
                    double pertimd = br.ReadDouble();
                    totimd = br.ReadDouble();
                    break;
            }
        }
        public static void ReadUcnHeader(int precision, BinaryReader br, out int kstp, out int kper, out int ntrans, out double totimd)
        {
            totimd = 0.0;
            ntrans = br.ReadInt32();
            kstp = br.ReadInt32();
            kper = br.ReadInt32();
            switch (precision)
            {
                case 1:
                    float totim = br.ReadSingle();
                    totimd = Convert.ToDouble(totim);
                    break;
                case 2:
                    totimd = br.ReadDouble();
                    break;
            }
        }
    }
}
