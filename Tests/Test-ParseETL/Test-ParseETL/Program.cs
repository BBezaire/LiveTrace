using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace Test_ParseETL
{
    class Test
    {
     
    }

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

    public class EtlCacheHeader
    {
        public DateTime StartTime;

        public DateTime EndTime;

        public string AgentVersion = "1";

        public string[] ComponentNames
        {
            get;
            set;
        }

        public List<uint> CorrelationIds
        {
            get;
            set;
        }

        public ushort MostSevereLevel
        {
            get;
            set;
        }

        public EtlCacheHeader()
        {
        }
    }

       static unsafe void BuildCacheHeader(string cacheFile)
        {
            Test_ParseETL.Program.EtlCacheHeader etlCacheHeader = null;
            //if (File.Exists(cacheFile))
            //{
            //    long length = (new FileInfo(cacheFile)).Length;
            //    if (length == (long)0)
            //    {
            //        if (Tracing.traceProvider.Level >= 4 && (Tracing.traceProvider.Flags & 1) != 0)
            //        {
            //            WPP_25edaa5c3f39f667965e06ad27825958.WPP_ps(27, 10, (IntPtr)this.GetHashCode(), TraceProvider.MakeStringArg(cacheFile));
            //        }
            //        File.Delete(cacheFile);
            //        if (File.Exists(cacheHeaderFile))
            //        {
            //            File.Delete(cacheHeaderFile);
            //        }
            //        return null;
            //    }
            FileStream fileStream = null;
            long ticks = DateTime.MinValue.Ticks;
            long num = DateTime.MaxValue.Ticks;
            ushort num1 = 0;
            List<uint> nums = new List<uint>();

           
                fileStream = new FileStream(cacheFile, FileMode.Open, FileAccess.Read);
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    do
                    {
                        long position = binaryReader.BaseStream.Position;
                        int num2 = binaryReader.ReadInt32();
                        byte[] numArray = new byte[num2];
                        binaryReader.BaseStream.Seek((long)-4, SeekOrigin.Current);
                        binaryReader.Read(numArray, 0, num2);
                        try
                        {
                            fixed (byte* numPointer = &numArray[0])
                            {
                                ETLCACHE_RECORD* eTLCACHERECORDPointer = (ETLCACHE_RECORD*)numPointer;
                                byte* numPointer1 = (byte*)(eTLCACHERECORDPointer + sizeof(ETLCACHE_RECORD));
                                string stringAnsi = Marshal.PtrToStringAnsi((IntPtr)numPointer1, (int)(*eTLCACHERECORDPointer).ComponentNameLength);
                                long timestamp = (*eTLCACHERECORDPointer).Timestamp;
                                ushort level = (*eTLCACHERECORDPointer).Level;
                                uint correlationId = (*eTLCACHERECORDPointer).CorrelationId;
                                int count = num2 - (int)80;
                                var SipTrace = System.Text.Encoding.Default.GetString(numArray,80, count);
                                Console.WriteLine(SipTrace);
                                if (ticks == DateTime.MinValue.Ticks || timestamp < ticks)
                                {
                                    ticks = timestamp;
                                }
                                if (num == DateTime.MaxValue.Ticks || timestamp > num)
                                {
                                    num = timestamp;
                                }
                                if (level > 0 && (num1 == 0 || level < num1))
                                {
                                    num1 = level;
                                }
                                if (!nums.Contains(correlationId))
                                {
                                    nums.Add(correlationId);
                                }
                                //if (!strs.Contains<string>(stringAnsi, StringComparer.OrdinalIgnoreCase))
                                //{
                                //    strs.Add(stringAnsi);
                                //}
                            }
                        }
                        finally
                        {
                            //numPointer = null;
                        }
                        binaryReader.BaseStream.Position = position + (long)num2;
                    }
                    while (binaryReader.BaseStream.Position < 30000);
                    //while (binaryReader.BaseStream.Position < length);
                    //strs.Sort(StringComparer.OrdinalIgnoreCase);
                    //nums.Sort();
                    //EtlCacheHeader etlCacheHeader1 = new EtlCacheHeader()
                    //{
                    //    AgentVersion = "2.0",
                    //    StartTime = new DateTime(ticks),
                    //    EndTime = new DateTime(num),
                    //    MostSevereLevel = num1,
                    //    CorrelationIds = nums,
                    //    ComponentNames = strs.ToArray()
                    //};
                    //etlCacheHeader = etlCacheHeader1;
                    //if (!this.SerializeEtlCacheHeader(etlCacheHeader, cacheHeaderFile))
                    //{
                    //    if (Tracing.traceProvider.Level >= 2 && (Tracing.traceProvider.Flags & 1) != 0)
                    //    {
                    //        WPP_25edaa5c3f39f667965e06ad27825958.WPP_ps(27, 12, (IntPtr)this.GetHashCode(), TraceProvider.MakeStringArg(cacheFile));
                    //    }
                    //    etlCacheHeader = null;
                    //}
                    //else if (Tracing.traceProvider.Level >= 4 && (Tracing.traceProvider.Flags & 1) != 0)
                    //{
                    //    WPP_25edaa5c3f39f667965e06ad27825958.WPP_ps(27, 11, (IntPtr)this.GetHashCode(), TraceProvider.MakeStringArg(cacheHeaderFile));
                    //}
                }
            

            //return etlCacheHeader;
        }

    


        unsafe static void Main(string[] args)
        {
            BuildCacheHeader("CLS_WPP_06-17-2016-18-24-24.cache");
        }

    }  
    }

