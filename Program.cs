using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace loaddump
{
    public class Program
    {

        static string work_folder = @"C:\Users\Admin\Desktop\WORK\DATA\";
        static string source_file = @"sim";
        static string destinaton_file = @"sim.json";

        static Encoding StringEncoding_CPlusPlus = Encoding.ASCII;

        #region hex converting
        static byte HexCifer(char Cifer)
        {
            switch (Cifer)
            {
                case '0': return 0;
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                case 'a': return 10;
                case 'A': return 10;
                case 'b': return 11;
                case 'B': return 11;
                case 'c': return 12;
                case 'C': return 12;
                case 'd': return 13;
                case 'D': return 13;
                case 'e': return 14;
                case 'E': return 14;
                case 'f': return 15;
                case 'F': return 15;
                default:
                    throw new Exception("Wrong hex cifer = " + Cifer);
                    //return 0xFF;
            }
        }
        static char HexDigit(byte Digit)
        {
            Digit &= 0x0F;
            switch (Digit)
            {
                case 0x00: return '0';
                case 0x01: return '1';
                case 0x02: return '2';
                case 0x03: return '3';
                case 0x04: return '4';
                case 0x05: return '5';
                case 0x06: return '6';
                case 0x07: return '7';
                case 0x08: return '8';
                case 0x09: return '9';
                case 0x0A: return 'A';
                case 0x0B: return 'B';
                case 0x0C: return 'C';
                case 0x0D: return 'D';
                case 0x0E: return 'E';
                case 0x0F: return 'F';
                default:
                    throw new Exception("unreachable code");
            }
        }
        static string NumberToHexString(byte Value)
        {
            byte data = 0;
            string result = "";
            for (int i = 0; i < 2 * sizeof(byte); i++)
            {
                data = (byte)(Value & 0x0F);
                result = result.Insert(0, new string(new char[] { HexDigit(data) }));
                Value >>= 4;
            }
            return result;
        }
        static string NumberToHexString(ushort Value)
        {
            byte data = 0;
            string result = "";
            for (int i = 0; i < 2 * sizeof(ushort); i++)
            {
                data = (byte)(Value & 0x0F);
                result = result.Insert(0, new string(new char[] { HexDigit(data) }));
                Value >>= 4;
            }
            return result;
        }
        static string NumberToHexString(uint Value)
        {
            byte data = 0;
            string result = "";
            for (int i = 0; i < 2 * sizeof(uint); i++)
            {
                data = (byte)(Value & 0x0F);
                result = result.Insert(0, new string(new char[] { HexDigit(data) }));
                Value >>= 4;
            }
            return result;
        }
        static string NumberToHexString(ulong Value)
        {
            byte data = 0;
            string result = "";
            for (int i = 0; i < 2 * sizeof(ulong); i++)
            {
                data = (byte)(Value & 0x0F);
                result = result.Insert(0, new string(new char[] { HexDigit(data) }));
                Value >>= 4;
            }
            return result;
        }
        static ulong HexStringToNumber(string Value)
        {
            byte data = 0;
            ulong result = 0;
            for (int i = 0; i < 2 * sizeof(ulong); i++)
            {
                if (Value.Count() == 0) break;
                data = HexCifer(Value[0]);
                Value = Value.Remove(0, 1);
                result <<= 4;
                result |= data;
            }
            return result;
        }
        #endregion
        #region JSON hexadecimal converters
        public class UINT8_JsonConverter : JsonConverter<byte>
        {
            public override byte ReadJson(JsonReader reader, Type objectType, byte existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                string s = (string)reader.Value;
                return (byte)(HexStringToNumber(s) & byte.MaxValue);
            }
            public override void WriteJson(JsonWriter writer, byte value, JsonSerializer serializer)
            {
                writer.WriteValue(NumberToHexString(value));
            }
        }
        public class UINT16_JsonConverter : JsonConverter<ushort>
        {
            public override ushort ReadJson(JsonReader reader, Type objectType, ushort existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                string s = (string)reader.Value;
                return (ushort)(HexStringToNumber(s) & ushort.MaxValue);
            }
            public override void WriteJson(JsonWriter writer, ushort value, JsonSerializer serializer)
            {
                writer.WriteValue(NumberToHexString(value));
            }
        }
        public class UINT32_JsonConverter : JsonConverter<uint>
        {
            public override uint ReadJson(JsonReader reader, Type objectType, uint existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                string s = (string)reader.Value;
                return (uint)(HexStringToNumber(s) & uint.MaxValue);
            }
            public override void WriteJson(JsonWriter writer, uint value, JsonSerializer serializer)
            {
                writer.WriteValue(NumberToHexString(value));
            }
        }
        public class UINT64_JsonConverter : JsonConverter<ulong>
        {
            public override ulong ReadJson(JsonReader reader, Type objectType, ulong existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                string s = (string)reader.Value;
                return (ulong)(HexStringToNumber(s) & ulong.MaxValue);
            }
            public override void WriteJson(JsonWriter writer, ulong value, JsonSerializer serializer)
            {
                writer.WriteValue(NumberToHexString(value));
            }
        }
        #endregion

        static IEnumerable<byte> ParseHex(string S)
        {
            bool even = true;
            byte data = 0;
            var chars = S.ToUpper().Reverse().ToArray();
            for (int i = 0; i < chars.Count(); i++)
            {
                even = !even;
                if (even)
                {
                    data = (byte)((HexCifer(chars[i]) << 4) | data);
                    yield return data;
                }
                else data = HexCifer(chars[i]);
            }
            yield break;
        }
        static byte[] DeadBeef = ParseHex("1011121314151617AAAAAAAAEEEEEEEEDEADBEEFDEADBEEF").ToArray();

        /// <summary>
        /// Determines if bytes sequence contains subsequence in specified position.
        /// </summary>
        /// <param name="Source">Bytes sequence.</param>
        /// <param name="Index">Position.</param>
        /// <param name="TheStart">Subsequence</param>
        /// <returns>True if contains.</returns>
        static bool StartsWith(ref byte[] Source, int Index, byte[] TheStart)
        {
            if (Index + TheStart.Count() > Source.Count()) return false;
            for (int i = 0; i < TheStart.Count(); i++)
                if (Source[Index + i] != TheStart[i]) return false;
            return true;
        }


        #region bytes moving
        static bool BOOL(ref byte[] Source, ref int Index)
        {
            return UINT8(ref Source, ref Index) != 0;
        }
        static uint UINT(ref byte[] Source, ref int Index)
        {
            return UINT32(ref Source, ref Index);
        }
        static byte UINT8(ref byte[] Source, ref int Index)
        {
            byte result = Source[Index];
            Index++;
            return result;
        }
        static ushort UINT16(ref byte[] Source, ref int Index)
        {
            ushort result = BitConverter.ToUInt16(Source, Index);
            Index += 2;
            return result;
        }
        static uint UINT32(ref byte[] Source, ref int Index)
        {
            uint result = BitConverter.ToUInt32(Source, Index);
            Index += 4;
            return result;
        }
        static ulong UINT64(ref byte[] Source, ref int Index)
        {
            ulong result = BitConverter.ToUInt64(Source, Index);
            Index += 8;
            return result;
        }
        static byte[] BYTES(ref byte[] Source, ref int Index, int Count)
        {
            var result = new byte[Count];
            for (int i = 0; i < Count; i++)
            {
                result[i] = Source[Index];
                Index++;
            }
            return result;
        }
        static string STRING(ref byte[] Source, ref int Index, int Length)
        {
            var result = new byte[Length];
            for (int i = 0; i < Length; i++)
            {
                result[i] = Source[Index];
                Index++;
            }
            return StringEncoding_CPlusPlus.GetString(result);
        }
        #endregion

        #region MAIN STRUCTURES
        const int CountOfALU = 6;
        public struct SrcOpt
        {
            public byte type;
            public byte src;

            public SrcOpt(ref byte[] Source, ref int Index)
            {
                type = UINT8(ref Source, ref Index);
                src = UINT8(ref Source, ref Index);
            }
        }
        public struct OperationDescr
        {
            public SrcOpt src1;
            public SrcOpt src2;
            public SrcOpt src3;
            public SrcOpt dst;
            public string name;

            public OperationDescr(ref byte[] Source, ref int Index)
            {
                src1 = new SrcOpt(ref Source, ref Index);
                src2 = new SrcOpt(ref Source, ref Index);
                src3 = new SrcOpt(ref Source, ref Index);
                dst = new SrcOpt(ref Source, ref Index);
                ulong _length = UINT64(ref Source, ref Index);
                name = STRING(ref Source, ref Index, (int)_length);
            }
        }
        public struct OperationVectorFlags
        {
            public bool have_mova;
            public bool have_store_ch2;
            public bool have_store_ch5;
            public bool have_call;
            public bool have_load_store;
            public bool have_ldrd_ch0;
            public bool have_ldrd_ch2;
            public bool have_ldrd_ch3;
            public bool have_ldrd_ch5;
            public bool have_strd_ch2;
            public bool have_strd_ch5;
            public bool have_puttst;
            public bool have_puttsd;
            public bool have_ct_branch_conflict_possibility;
            public bool have_fpsr_c0_change_in_ch4;
            public bool have_ct;
            public bool have_ldisp;

            public OperationVectorFlags(ref byte[] Source, ref int Index)
            {
                have_mova = BOOL(ref Source, ref Index);
                have_store_ch2 = BOOL(ref Source, ref Index);
                have_store_ch5 = BOOL(ref Source, ref Index);
                have_call = BOOL(ref Source, ref Index);
                have_load_store = BOOL(ref Source, ref Index);
                have_ldrd_ch0 = BOOL(ref Source, ref Index);
                have_ldrd_ch2 = BOOL(ref Source, ref Index);
                have_ldrd_ch3 = BOOL(ref Source, ref Index);
                have_ldrd_ch5 = BOOL(ref Source, ref Index);
                have_strd_ch2 = BOOL(ref Source, ref Index);
                have_strd_ch5 = BOOL(ref Source, ref Index);
                have_puttst = BOOL(ref Source, ref Index);
                have_puttsd = BOOL(ref Source, ref Index);
                have_ct_branch_conflict_possibility = BOOL(ref Source, ref Index);
                have_fpsr_c0_change_in_ch4 = BOOL(ref Source, ref Index);
                have_ct = BOOL(ref Source, ref Index);
                have_ldisp = BOOL(ref Source, ref Index);
            }
        }
        public struct Notation
        {
            public uint _lng;
            public ulong _phys1;
            public ulong _phys2;
            public OperationDescr[] _operations;
            public ulong count;
            public OperationVectorFlags _operation_vector_flags;

            public Notation(ref byte[] Source, ref int Index)
            {
                var Offset = Index;

                if (StartsWith(ref Source, Index, DeadBeef)) Index += DeadBeef.Count();
                else;

                _lng = UINT(ref Source, ref Index);
                _phys1 = UINT64(ref Source, ref Index);
                _phys2 = UINT64(ref Source, ref Index);
                if (_phys2 != 0xFFFFFFFFFFFFFFFF) ;
                uint _operation_descr_count = UINT32(ref Source, ref Index);
                _operations = new OperationDescr[_operation_descr_count];
                for (int i = 0; i < _operations.Count(); i++)
                    _operations[i] = new OperationDescr(ref Source, ref Index);
                count = UINT64(ref Source, ref Index);
                _operation_vector_flags = new OperationVectorFlags(ref Source, ref Index);
            }
        }
        #endregion

        static IEnumerable<Notation> Read_Notations(byte[] Source)
        {
            int Count_1 = Source.Count() - 1;
            int Index = 0;
            while (Index < Count_1)
                yield return new Notation(ref Source, ref Index);
            yield break;
        }

        /// <summary>
        /// Extracts continuous sections of commands.
        /// </summary>
        /// <param name="Source">Collection of commands.</param>
        /// <param name="CutAfterControlTransfer">TRUE if any control transfer command ends commands section.</param>
        /// <returns>Collection of commands sections.</returns>
        static IEnumerable<IEnumerable<Notation>> Chain(IEnumerable<Notation> Source, bool CutAfterControlTransfer = true)
        {
            var _elements = Source.OrderBy(x => x._phys1).ToArray();
            var _chained = new List<Notation>();
            ulong _offset = 0;
            for (int i = 0; i < _elements.Length; i++)
            {
                if (CutAfterControlTransfer) if ((_chained.Count > 0) && (_chained.Last()._operations.Select(y => y.name).Intersect(new string[]
                {
                    "CT",
                    "RBRANCH",
                    "IBRANCH"
                }).Count() > 0))
                    {
                        yield return _chained.AsEnumerable();
                        _chained.Clear();
                    }
                var x = _elements[i];
                if (_chained.Count == 0) _chained.Add(x);
                else if (x._phys1 == _offset) _chained.Add(x);
                else
                {
                    yield return _chained.AsEnumerable();
                    _chained.Clear();
                }
                _offset = x._phys1 + x._lng;
            }
            if (_chained.Count > 0) yield return _chained.AsEnumerable();
            _chained.Clear();
            yield break;
        }

        /// <summary>
        /// Commands equality comparer.
        /// </summary>
        public class EqualityComparer_Notation_OperationsNames : EqualityComparer<Notation>
        {
            public override bool Equals(Notation TheNotation1, Notation TheNotation2)
            {
                //var names1 = TheNotation1._operations.Select(x => x.name).ToList();
                //var names2 = TheNotation2._operations.Select(x => x.name).ToList();
                //int count = 0;
                //if (names1.Count != names2.Count) return false;
                //else count = names1.Count;
                //for (int i = 0; i < count; i++)
                //    if (names1[i] != names2[i]) return false;
                //return true;
                var s1 = TheNotation1._operations.Aggregate("", (s, x) => s + x.name);
                var s2 = TheNotation2._operations.Aggregate("", (s, x) => s + x.name);
                return s1 == s2;
            }
            public override int GetHashCode(Notation TheNotation)
            {
                //throw new NotImplementedException();
                return TheNotation._operations.Aggregate("", (s, x) => s + x.name).GetHashCode();
            }
        }

        /// <summary>
        /// Description of command statistics.
        /// </summary>
        public struct Statistics_Notation
        {
            /// <summary>
            /// The command.
            /// </summary>
            public Notation TheCommand;
            /// <summary>
            /// How many times the command appears in program.
            /// </summary>
            public ulong Appeared;
            /// <summary>
            /// How many times the command was executed.
            /// </summary>
            public ulong Executed;
        }



        static void Main(string[] args)
        {
            { }//loading
            var _notations = Read_Notations(File.ReadAllBytes(Path.Combine(work_folder, source_file))).ToList();

            { }//sections extracting

            var _sections = Chain(_notations, true).Select(x => x.ToList()).ToList();

            { }//miscellaneous

            //var _max_count = _sections.Max(x => x.Count);
            //var _hot_code = _sections.OrderByDescending(x => x.Max(y => y.count)).ToList();
            //var _low_count = _hot_code.Where(x => (int)x.Max(y => y.count) > (_max_count / 2)).ToList();

            { }//collecting statistics

            var _total_executed = _notations.Aggregate<Notation, ulong>(0, (s, x) => s + x.count);

            var _commands_appearing = new Dictionary<Notation, Statistics_Notation>(new EqualityComparer_Notation_OperationsNames());
            foreach (var _notation in _notations)
            {
                if (_commands_appearing.ContainsKey(_notation))
                {
                    var _value = _commands_appearing[_notation];
                    _commands_appearing[_notation] = new Statistics_Notation()
                    {
                        TheCommand = _value.TheCommand,
                        Appeared = _value.Appeared + 1,
                        Executed = _value.Executed + _notation.count
                    };
                }
                else _commands_appearing.Add(_notation, new Statistics_Notation() { TheCommand = _notation, Appeared = 1, Executed = _notation.count });
            }

            { }//filtering and sorting

            _commands_appearing = _commands_appearing.Where(x => x.Value.TheCommand._operations.Count() > 1).OrderByDescending(x => x.Value.Executed).ToDictionary(x => x.Key, x => x.Value);
            var _statistics = _commands_appearing.Select(x => x.Value).ToList();

            { }

            if (false)
            {
                var _json = JsonConvert.SerializeObject(_notations.Take(50), Formatting.Indented,
                    new UINT8_JsonConverter(),
                    new UINT16_JsonConverter(),
                    new UINT32_JsonConverter(),
                    new UINT64_JsonConverter());
                File.WriteAllText(Path.Combine(work_folder, destinaton_file), _json);
            }
        }
    }
}
