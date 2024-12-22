using System;

namespace LIbraryInteractionTest
{
    public class Context : BaseInjectable, IDisposable
    {
        public Context()
        {
           // Console.WriteLine("Context constructor invoked");
        }

        public void Dispose()
        {
           // Console.WriteLine("Context disposed");
        }
    }
}