using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIbraryInteractionTest
{
    internal class Repository : BaseInjectable, IDisposable
    {
        public Repository(Context dbContext)
        {
            //Console.WriteLine("Repository constructor invoked");
        }

        public void Dispose()
        {
            //Console.WriteLine("Repository Disposed");
        }

        public string GetAll()
        {
            return "ALL";
        }
    }
}
