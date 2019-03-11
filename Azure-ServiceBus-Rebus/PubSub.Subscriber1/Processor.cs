using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PubSub.Subscriber1
{
    class Processor
    {
        private Queue<Task> _tasks;
        private bool _running = true;

        public Processor()
        {
            _tasks = new Queue<Task>();

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                ProcessTasks();
            }).Start();
        }

        private void ProcessTasks()
        {
            while (_running)
            {
                Task currentTask = null;
                lock (_tasks)
                {
                    if (_tasks.Any())
                    {
                        currentTask = _tasks.Dequeue();
                    }
                }
                if (currentTask != null)
                {
                    Console.WriteLine($"Processor - Dequeued from internal queue. Queue length:{_tasks.Count}");
                    Console.WriteLine($"Processor - Starting task ");
                    currentTask.Start();
                    currentTask.Wait();
                    Console.WriteLine($"Processor - Task complete. Sleeping for 1 second");
                    Thread.Sleep(1000);
                    Console.WriteLine($"Processor - Awake.");
                } else
                {
                    Thread.Sleep(100);
                }
            }
        }
        
        public Task AddTask(Task task)
        {
            lock (_tasks)
            {
                _tasks.Enqueue(task);
            }
            Console.WriteLine($"Processor - Added task to internal queue. Queue length:{_tasks.Count}");

            return task;
        }
    }
}
