using System;

namespace PortChat
{
    public static class CrcCode
    {
        public static ushort GetCrc16UInt(byte[] data, int nLen)
        {
            int i;
            ushort crc = 65535;
            const ushort a001H = 40961;

            for (i = 0; i < nLen; i++)
            {
                var temp = Convert.ToUInt16(data[i]);
                //当前字节与CRC值异或，得到新的CRC
                crc ^= temp;
                int j;
                for (j =0 ; j < 8; j++)
                {
                    // CRC开始移位，除以2余数为零则无操作，余数为1则CRC与A001异或
                    if (crc%2 == 1)
                    {
                        crc /= 2;
                        crc ^= a001H;
                    }
                    else
                    {
                        crc /= 2;
                    }
                }

            }
            return crc;
        }
    }
    public class SendTask
    {
        public UInt32 nID;                      // 请求ID号，用于区别不同的请求
        public UInt32 nSendLen;                 // 发送数据的长度
        public UInt32 nRevLen;                  // 接收数据的长度
        public byte[] szSend = new byte[270];   // 发送的数据
        public byte[] szRev = new byte[270];    // 接收的数据

        private const ushort CRC_SEED = 0xFFFF;

        // 初始化寄存器读请求
        // ID 请求ID号， nStation 站地址，nAddr操作起始地址，nLen读取寄存器长度
        public void InitReadRegister(UInt32 ID, byte nStation, UInt32 nAddr, UInt32 nLen)
        {
            nID = ID;
            nSendLen = 8;
            nRevLen = 5 + nLen * 2;

            szSend[0] = nStation;
            szSend[1] = 3;
            szSend[2] = (byte)((nAddr & 0XFF00) >> 8);
            szSend[3] = (byte) (nAddr & 0XFF);
            szSend[4] = (byte) (nLen & 0XFF);
            szSend[5] = (byte) (nLen & 0XFF);
            ushort crc = CrcCode.GetCrc16UInt(szSend, 6); // 计算校验位
            szSend[7] = (byte)((crc & 0XFF00) >> 8);
            szSend[6] = (byte) (crc & 0XFF);
        }
    }
}
