using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSVTesterSerial
{
    public class TaskManager<T>
    {
        private readonly Func<CancellationToken, Task<T>> _taskFunction;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Task<T> _task = null!;

        public TaskManager(Func<CancellationToken, Task<T>> taskFunction)
        {
            _taskFunction = taskFunction ?? throw new ArgumentNullException(nameof(taskFunction));
        }

        public Task<T> StartAsync()
        {
            if (IsRunning())
            {
                throw new InvalidOperationException("Task is already running.");
            }
            _cancellationTokenSource = new CancellationTokenSource();
            _task = ExecuteAsync(_cancellationTokenSource.Token);
            return _task;
        }

        public bool IsRunning()
        {
            return _task != null && !_task.IsCompleted;
        }

        public void Cancel()
        {
            _cancellationTokenSource?.Cancel();
        }

        private async Task<T> ExecuteAsync(CancellationToken cancellationToken)
        {
            return await _taskFunction(cancellationToken);
        }
    }
}
