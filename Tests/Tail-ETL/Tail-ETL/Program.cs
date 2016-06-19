using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace Tail_ETL
{
    class Program
    {
        public struct InsertsTypeMask
        {
            public ulong LowPart;

            public ulong HighPart;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct ETLCACHE_RECORD
        {
            [FieldOffset(0)]
            public uint EntryLength;

            [FieldOffset(4)]
            public uint Signature;

            [FieldOffset(8)]
            public uint AgentId;

            [FieldOffset(12)]
            public long Timestamp;

            [FieldOffset(20)]
            public uint ProcessId;

            [FieldOffset(24)]
            public uint ThreadId;

            [FieldOffset(28)]
            public uint MessageSequence;

            [FieldOffset(32)]
            public ushort Level;

            [FieldOffset(34)]
            public ushort MessageNumber;

            [FieldOffset(36)]
            public Guid MessageGuid;

            [FieldOffset(52)]
            public uint CorrelationId;

            [FieldOffset(56)]
            public uint Reserved;

            [FieldOffset(60)]
            public InsertsTypeMask InsertsTypeMask;

            [FieldOffset(76)]
            public ushort ComponentNameLength;

            [FieldOffset(78)]
            public ushort NumberOfInserts;
        }

        public class ETLFile
        {
            public string cacheFile { get; set; }
            public long ETLpos { get; set; }
        }

        static unsafe ETLFile ReadETL(string cacheFile, long ETLpos)
        {
            FileStream fileStream = null;
            long ticks = DateTime.MinValue.Ticks;
            long num = DateTime.MaxValue.Ticks;
            ushort num1 = 0;
            List<uint> nums = new List<uint>();

            fileStream = new FileStream(cacheFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            using (BinaryReader binaryReader = new BinaryReader(fileStream))
            {
                binaryReader.BaseStream.Position = ETLpos;
                do
                {
                    if ((binaryReader.BaseStream.Position + (long)32) > binaryReader.BaseStream.Length)
                    {

                        var result = new ETLFile
                        {
                            cacheFile = cacheFile,
                            ETLpos = binaryReader.BaseStream.Position
                        };
                        return result;
                    }
                        long position = binaryReader.BaseStream.Position;
                    int num2 = binaryReader.ReadInt32();

                    //Check if full message was written
                    if ((binaryReader.BaseStream.Position + num2) > binaryReader.BaseStream.Length)
                    {

                        var result = new ETLFile
                        {
                            cacheFile = cacheFile,
                            ETLpos = binaryReader.BaseStream.Position-4
                        };
                        return result;
                    }
                    //


                    byte[] numArray = new byte[num2];
                    binaryReader.BaseStream.Seek((long)-4, SeekOrigin.Current);
                    binaryReader.Read(numArray, 0, num2);
                    try
                    {
                        fixed (byte* numPointer = &numArray[0])
                        {
                            ETLCACHE_RECORD* eTLCACHERECORDPointer = (ETLCACHE_RECORD*)numPointer;
                            byte* numPointer1 = (byte*)(eTLCACHERECORDPointer + sizeof(ETLCACHE_RECORD));
                            //string stringAnsi = Marshal.PtrToStringAnsi((IntPtr)numPointer1, (int)(*eTLCACHERECORDPointer).ComponentNameLength);
                            //long timestamp = (*eTLCACHERECORDPointer).Timestamp;
                            //ushort level = (*eTLCACHERECORDPointer).Level;
                            //uint correlationId = (*eTLCACHERECORDPointer).CorrelationId;

                            int tstart = 80 + (*eTLCACHERECORDPointer).ComponentNameLength + 15;
                            if (tstart < num2)
                            {
                                int tcount = num2 - tstart;
                                var SipTrace = System.Text.Encoding.Default.GetString(numArray, tstart, tcount);
                                Console.WriteLine(SipTrace);
                                Console.WriteLine("-----------------------------------------------------------------------------");
                            }
                            else { }
 
                        }
                    }
                    finally
                    {
                        //numPointer = null;
                    }
                    binaryReader.BaseStream.Position = position + (long)num2;
                } while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length);
                var result1 = new ETLFile
                {
                    cacheFile = cacheFile,
                    ETLpos = binaryReader.BaseStream.Position
                };
                return result1;

            }

        }

        static void Main(string[] args)
        {
            var ETL = new ETLFile();
            ETL.ETLpos = 0;
            ETL.cacheFile = System.Configuration.ConfigurationManager.AppSettings["ETLfile"];
            while (true)
            {
                ETL = ReadETL("test.cache", ETL.ETLpos);
                Console.WriteLine("Read exit at ETL Position: " + ETL.ETLpos);
                System.Threading.Thread.Sleep(500);
                //Console.ReadLine();
            }
           
        }
    }
}
