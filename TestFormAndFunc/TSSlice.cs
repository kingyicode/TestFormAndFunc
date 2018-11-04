using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFormAndFunc
{
    class TSSlice
    {
        public class TSHead
        {
            protected Byte sync_byte;//8bit
            protected Byte transport_error_indicator;//1 bit
            protected Byte payload_unit_start_indicator;//1 bit
            protected Byte transport_priority;//1 bit
            protected UInt16 pid;//	13bit
            protected Byte transport_scrambling_control;//	2 bit
            protected Byte adaptation_field_control;//	2bit
            protected Byte continuity_counter;//	4bit

            public Byte Sync_yte
            {
                get { return sync_byte; }
                set { sync_byte = value; }
            }

            public Byte Transport_error_indicator
            {
                get { return transport_error_indicator; }
                set { transport_error_indicator = value; }
            }

            public Byte Payload_unit_start_indicator
            {
                get { return payload_unit_start_indicator; }
                set { payload_unit_start_indicator = value; }
            }

            public Byte Transport_priority
            {
                get { return transport_priority; }
                set { transport_priority = value; }
            }

            public UInt16 Pid
            {
                get { return pid; }
                set { pid = value; }
            }

            public Byte Transport_scrambling_control
            {
                get { return transport_scrambling_control; }
                set { transport_scrambling_control = value; }
            }

            public Byte Adaptation_field_control
            {
                get { return adaptation_field_control; }
                set { adaptation_field_control = value; }
            }

            public Byte Continuity_counter
            {
                get { return continuity_counter;  }
                set { continuity_counter = value; }
            }


            public void Set(Byte[] value)
            {
                if (value == null || value.Length < 4)
                    return;

                this.Sync_yte = value[0];
                this.Transport_error_indicator = (byte)( (value[1] >> 7) & 0x01);
                this.Payload_unit_start_indicator = (byte)((value[1] >> 6) & 0x01);
                this.Transport_priority = (byte)((value[1] >> 5) & 0x01);
                this.Pid = (ushort)( ((value[1] & 0x1f) << 8) | value[2]);
                this.Transport_scrambling_control = (byte)((value[1] >> 6) & 0x03);
                this.Adaptation_field_control = (byte)((value[1] >> 4) & 0x03);
                this.Continuity_counter = (byte)((value[1]) & 0x0f);
            }
        }

        public class TSAdaption
        {
            protected Byte adaptation_field_length;//1 byte
            protected Byte flag;//1 byte
            protected Byte[] pcr = new Byte[5];//5 byte
            protected Byte[] stuffing_bytes;

            public Byte Adaptation_field_length
            {
                get { return adaptation_field_length; }
                set { adaptation_field_length = value; }
            }

            public Byte Flag
            {
                get { return flag; }
                set { flag = value;  }
            }

            public Byte[] PCR
            {
                get { return pcr; }
                set {
                    if (value != null && value.Length >= 5)
                    {
                        Array.Copy(value, pcr, 5);
                    }
                }
            }

            public Byte[] Stuffing_bytes
            {
                get { return stuffing_bytes; }
                set { stuffing_bytes = value; }
            }

            public void Set(Byte[] value)
            {
                if (value == null || value.Length < 7)
                    return;


                this.Adaptation_field_length = value[0];
                this.Flag = value[1];
                value.CopyTo(pcr, 5);
                int len = this.adaptation_field_length - 7;
                if (len > 0)
                {
                    stuffing_bytes = new byte[len];
                    value.CopyTo(stuffing_bytes, 7);
                }

            }
        }

        public class ProgramInfo
        {
            protected UInt16 program_number;//	16b 节目号为0x0000时表示这是NIT，节目号为0x0001时,表示这是PMT
            protected Byte reserved;//	3b 固定为111
            protected UInt16 pid;//	13b 节目号对应内容的PID值

            public UInt16 Program_number
            {
                get { return program_number;  }
                set { program_number = value; }
            }

            public Byte Reserved
            {
                get { return reserved; }
                set { reserved = value; }
            }

            public UInt16 PID
            {
                get { return pid; }
                set { pid = value; }
            }

            public void Set(Byte[] value)
            {
                if (value == null || value.Length < 4)
                    return;

                this.program_number = (ushort)( (value[0] << 8) | value[1] );
                this.reserved = (byte)( (value[2] >> 5) & 0x07 );
                this.pid = (ushort)( ((value[3] & 0x1f) << 8) | value[4] );
            }
        }

        public class PAT
        {  
            protected Byte table_id;//	8b PAT表固定为0x00
            protected Byte section_syntax_indicator;//	1b 固定为1
            protected Byte zero;//	1b 固定为0
            protected Byte reservedA;//	2b 固定为11
            protected UInt16 section_length;//	12b 后面数据的长度
            protected UInt16 transport_stream_id;//	16b 传输流ID，固定为0x0001
            protected Byte reservedB;//	2b 固定为11
            protected Byte version_number;//	5b 版本号，固定为00000，如果PAT有变化则版本号加1
            protected Byte current_next_indicator;//	1b 固定为1，表示这个PAT表可以用，如果为0则要等待下一个PAT表
            protected Byte section_number;//	8b 固定为0x00
            protected Byte last_section_number;//	8b 固定为0x00
            //开始循环
            ProgramInfo[] program_info;
            //结束循环
            protected UInt32 crc32;//	32b 前面数据的CRC32校验码

            public void Set(Byte[] value)
            {
                if (value == null || value.Length < 8)
                    return;

                UInt16 len = (ushort)( ((value[1] & 0x0f) << 8) | value[2]);
                int program_len = len - 5 - 4;
                int program_count = program_len / 4;
                if ((len < 9) || ((program_len % 4) != 0))
                    return;

                this.table_id = value[0];
                this.section_syntax_indicator = (byte)( (value[1] >> 7) & 0x01 );
                this.zero = (byte)((value[1] >> 6) & 0x01);
                this.reservedA = (byte)( (value[1] >> 4) & 0x03 );
                this.section_length = len;
                this.transport_stream_id = (ushort)( (value[3] << 8) | value[4] );
                this.reservedB = (byte)((value[5] >> 6) & 0x03);
                this.version_number = (byte)((value[5] >> 1) & 0x1f);
                this.current_next_indicator = (byte)((value[5]) & 0x01);

                this.section_number = value[6];
                this.last_section_number = value[7];

                this.program_info = new ProgramInfo[program_count];
                Byte[] buf = new byte[4];
                int index = 8;
                for(int i = 0; i < program_count; i++)
                {
                    try
                    {
                        Array.ConstrainedCopy(value, index, buf, 0, 4);
                        this.program_info[i].Set(buf);

                        index += 4;
                    }
                    catch(Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }
                }

                this.crc32 = (uint)( (value[index] << 24) | (value[index+1] << 16) | (value[index+2] << 8) | value[index+3] );

            }
            
        }
         
    }
}
