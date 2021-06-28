using System.Threading;

namespace FN_Engine
{
    public static class Threader
    {
        public static void Invoke(ThreadStart Method, uint MilliSeconds)
        {
            Thread thread = new Thread(ExecuteThread);
            thread.IsBackground = true;
            thread.Start();

            void ExecuteThread()
            {
                Thread.Sleep((int)MilliSeconds);
                Method.Invoke();
            }
        }
    }
}
