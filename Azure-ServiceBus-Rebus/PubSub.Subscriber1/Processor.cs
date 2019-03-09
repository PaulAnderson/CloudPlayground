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
                    Console.WriteLine($"Added dequeued from internal queue. Queue length:{_tasks.Count}");
                    currentTask.Start();
                    Console.WriteLine($"Sleeping for 1 sec");

                    Thread.Sleep(1000);
                    Console.WriteLine($"Awake.");

                }
            }
        }
        
        public Task AddTask(Task task)
        {
            lock (_tasks)
            {
                _tasks.Enqueue(task);
            }
            Console.WriteLine($"Added task to internal queue. Queue length:{_tasks.Count}");

            return task;
        }
    }
}
