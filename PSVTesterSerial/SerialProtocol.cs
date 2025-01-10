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
        private int crc32Ok;
        private int crc32NomOk;
    }
}
