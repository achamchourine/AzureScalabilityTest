using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureScalabilityTest
{
    public class ThreadController
    {
        private List<CancellationTokenSource> TokenSourceList = new List<CancellationTokenSource>() ;
        private CancellationTokenSource TokenSource;
        private List<Task> tasks;

        public void Add10Tasks(string ConnStr)
        {
            TokenSource = new CancellationTokenSource();
            TokenSourceList.Add(TokenSource);

            for (int i = 0; i<10; i++ )
            {
                AddTask(ConnStr);
                //break;
            }
        }

        public void AddTask(string ConnStr)
        {
            var token = TokenSource.Token;

            Emulator eml = new Emulator(ConnStr, token);

            Task task = Task.Factory.StartNew( ()=>{eml.Run();}, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            tasks.Add(task);
        }

        public int NumberOfTasks
        {
            get { return tasks.Count(i => !i.IsCompleted); }
        }

        public void CancelAllTasks()
        {
            TokenSourceList.ForEach(i=>i.Cancel());
            TokenSourceList.Clear();
            //TokenSource.Cancel();
        }

        public void Cancel10Tasks()
        {
            var ts = TokenSourceList[TokenSourceList.Count-1];
            ts.Cancel();

            TokenSourceList.RemoveAt(TokenSourceList.Count - 1);
        }

        public ThreadController()
        {
            tasks = new List<Task>();
        }
    }
}
