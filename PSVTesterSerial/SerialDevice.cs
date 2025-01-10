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

        private static SerialPort? SerialPort { get; set; }

        private SerialDevice()
        {
            SerialPort = null;
        }

        public static SerialDevice GetInstance()
        {
            lock (_lock)
            {
                _instance ??= new SerialDevice();
            }
            return _instance;
        }

        public static bool IsSerialAvailable()
        {
            return SerialPort != null && SerialPort.IsOpen;
        }

        public static async Task<bool> Start(string port)
        {
            if (string.IsNullOrWhiteSpace(port))
                return false;

            var availablePorts = SerialPort.GetPortNames().Distinct();

            if (!availablePorts.Contains(port) || !IsPortFunctional(port))
                return false;

            if (SerialPort == null)
            {
                SerialPort = new SerialPort
                {
                    PortName = port,
                    BaudRate = 500000,
                    ReadTimeout = 10,
                    Parity = Parity.None,
                    Encoding = Encoding.UTF8
           
                };
                SerialPort.ReadTimeout = 500;
                SerialPort.WriteTimeout = 500;
            }

            if (SerialPort.IsOpen)
                SerialPort.Close();

            try
            {
                SerialPort.Open();
                SerialPort.BaseStream.Flush();
                await Task.Delay(100).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error opening port {port}: {ex.Message}");
            }

            return true;
        }

        public static void Stop()
        {
            if (IsSerialAvailable())
            {
                SerialPort?.Close();
            }
        }

        private static bool IsPortFunctional(string port)
        {
            try
            {
                using var testPort = new SerialPort(port);
                testPort.Open();
                testPort.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void SendMessage()
        {
            var buffer = new byte[SerialProtocol.TransmitMessageSize];
            buffer[0] = 0xFF;
            buffer[1] = 0xFF;
            buffer[2] = 0xFF; 
            buffer[3] = 0;
            buffer[4] = 0;
            buffer[5] = 0x80;
            buffer[6] = 0x80;
            buffer[7] = 0x80;

            Console.WriteLine("Message Sent:");
            foreach (var b in buffer)
            {
                Console.Write($"{b} ");
            }
            Console.WriteLine();

            SerialPort?.Write(buffer, 0, buffer.Length);
        }

        public static void ReceiveMessage()
        {
            int read;
            const int dataSize = SerialProtocol.ReceiveMessageSize;
            byte[] buffer = new byte[dataSize];

            if(IsSerialAvailable())
            {
                if(SerialPort.BytesToRead >= dataSize)
                {
                    read = SerialPort.Read(buffer, 0, dataSize);
                }

                Console.WriteLine("Message Received: ");
                foreach (var b in buffer)
                {
                    Console.Write($"{b} ");
                }
                Console.WriteLine();

                SerialPort.BaseStream.Flush();
            }
        }
    }
}
