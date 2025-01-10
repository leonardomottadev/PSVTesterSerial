using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSVTesterSerial
{
    public sealed class MainRoutine : IDisposable
    {
        private static MainRoutine? _instance;
        private static readonly Lock _lock = new();
        private readonly TaskManager<bool> mainRoutineTaskManager;
        private const string PORT = "COM3";

        private MainRoutine()
        {
            mainRoutineTaskManager = new TaskManager<bool>(MainRoutineTask);
        }

        public static MainRoutine GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new MainRoutine();
                }
            }
            return _instance;
        }

        public static void ReleaseIntance()
        {
            _instance = null;
        }

        public void Dispose()
        {

        }

        public void StartRoutine()
        {
            mainRoutineTaskManager.StartAsync();
        }

        public void StopRoutine()
        {
            mainRoutineTaskManager.Cancel();
        }

        private async Task<bool> MainRoutineTask(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (SerialDevice.IsAvailable())
                    {
                        Console.WriteLine($"{PORT} is open");
                    }
                    else if (await SerialDevice.Start(PORT))
                    {
                        Console.WriteLine("Starting device...");
                    }
                    else
                    {
                        Console.WriteLine("Awaiting...");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                await Task.Delay(1000, cancellationToken);
            }
            return false;
        }
    }
}
