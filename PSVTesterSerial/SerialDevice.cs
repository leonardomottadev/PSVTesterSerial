using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSVTesterSerial
{
    public sealed class SerialDevice
    {
        private static SerialDevice? _instance;
        private static readonly Lock _lock = new();
        private static readonly Lock serialLock = new();

        private static SerialPort? SerialPort { get; set; }

        private SerialDevice() { }

        public static SerialDevice GetInstance()
        {
            lock (_lock)
            {
                _instance ??= new SerialDevice();
            }
            return _instance;
        }

        public static bool IsAvailable()
        {
            return SerialPort != null && SerialPort.IsOpen;
        }

        public static Task<bool> Start(string port)
        {
            List<string> AvailablePorts = new List<string>(SerialPort.GetPortNames().Distinct());

            if(AvailablePorts != null && AvailablePorts.Contains(port))
            {
                lock (serialLock) 
                {
                    SerialPort = new SerialPort(port, 500000)
                    {
                        ReadTimeout = 10
                    };
                    SerialPort.Open();
                    SerialPort.BaseStream.Flush();
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public static void Stop()
        {
            if (IsAvailable()) 
            {
                SerialPort?.Close();
            }
        }
    }
}
