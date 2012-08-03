using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;


namespace HuntTheWumpus.Utilities
{
    public static class Parallel
    {
        private class WorkerThread<T>
        {
            ManualResetEvent rst;
            T obj;
            Action<T> act;
            public WorkerThread(T obj, Action<T> act, ManualResetEvent rst)
            {
                this.rst = rst;
                this.obj = obj;
                this.act = act;
            }
            public void Worker(Object context)
            {
                T obj = (T)context;
                act(obj);
                rst.Set();
            }
        }

        public static void PForEach<T>(this IEnumerable<T> list, Action<T> act)
        {
            if (list.Count() == 0) return;

            ThreadPool.SetMinThreads(2, 2);
            ThreadPool.SetMaxThreads(16, 16);
            
            var doneEvents = new List<ManualResetEvent>();
            //var workers = new List<WorkerThread<T>>();
            foreach (var e in list)
            {
                var rst = new ManualResetEvent(false);
                WorkerThread<T> wkr = new WorkerThread<T>(e, act, rst);
                ThreadPool.QueueUserWorkItem(new WaitCallback(wkr.Worker), e);

                //workers.Add(wkr);
                doneEvents.Add(rst);
            }
            //WaitHandle.WaitAll(doneEvents.ToArray());
            doneEvents.ForEach(e => e.WaitOne());
        }

    }
}
