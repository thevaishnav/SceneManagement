using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Omnix.SceneManagement
{
    public class TaskQueue 
    {
        private Queue<Action> _queue = new Queue<Action>();
        private bool _taskInProgress = false;

        public void BeginTask([NotNull] Action task)
        {
            if (_taskInProgress)
            {
                _queue.Enqueue(task);
            }
            else
            {
                _taskInProgress = true;
                task.Invoke();
            }
        }
        
        public void TaskDone()
        {
            _taskInProgress = false;
            if (_queue.Count <= 0) return;

            _taskInProgress = true;
            _queue.Dequeue().Invoke();
        }
    }
}