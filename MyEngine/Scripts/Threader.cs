﻿using System.Threading;

namespace MyEngine
{
    public static class Threader
    {
        public static void Invoke(ThreadStart Method, uint MilliSeconds)
        {
            Thread thread = new Thread(ExecuteThread);
            thread.Start();
            thread.IsBackground = true;

            void ExecuteThread()
            {
                Thread.Sleep((int)MilliSeconds);
                Method.Invoke();
            }
        }
    }
}
