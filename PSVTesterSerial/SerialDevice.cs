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

        public static bool IsAvailable()
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

            SerialPort ??= new SerialPort(port, 500000)
            {
                ReadTimeout = 10
            };

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
            if (IsAvailable())
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
    }
}
