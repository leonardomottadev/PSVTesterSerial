using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSVTesterSerial
{
    public class SerialProtocol
    {
        public const int SampleRate = 44100; // Sample rate in Hz
        public const int MicrophoneSamples = 1024;
        public const int HeaderLength = 3;
        public const int ChecksumLength = 4;
        public const int TailLength = 3;
        public const int TxDataLength = 2;
        public const int RxDataLength = 6 + (2 * MicrophoneSamples);
        public const int ReceiveMessageSize = 2064; // HeaderLength + RxDataLength + ChecksumLength +TailLength;
        public const int TransmitMessageSize = 8; // HeaderLength + TxDataLength + TailLength
        public const int Tail = HeaderLength + RxDataLength + ChecksumLength;
        public const int Checksum = HeaderLength + RxDataLength;
        private int crc32Ok;
        private int crc32NomOk;

        public static bool ValidateMessage(byte[] message)
        {
            if (message == null || message.Length < ReceiveMessageSize)
            {
                return false;
            }

            if (message[0] == 0xFF && 
                message[1] == 0xFF && 
                message[2] == 0xFF &&
                message[Tail + 0] == 0x80 &&
                message[Tail + 1] == 0x80 &&
                message[Tail + 2] == 0x80)
            {
                var data = new byte[RxDataLength];
                Array.Copy(message, HeaderLength, data, 0, RxDataLength);

                var crc32buffer = Force.Crc32.Crc32Algorithm.Compute(data);

                var checksum = new byte[]
                {
                    message[Checksum + 0],
                    message[Checksum + 1],
                    message[Checksum + 2],
                    message[Checksum + 3]
                };

                var checksum32 = BitConverter.ToUInt32(checksum, 0);

                if(crc32buffer == checksum32)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
