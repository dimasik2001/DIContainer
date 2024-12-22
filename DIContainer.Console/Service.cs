using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIbraryInteractionTest
{
    internal class Service : BaseInjectable, IService
    {
        public Service(Repository repository)
        {
           // Console.WriteLine("Service constructor invoked");

        }
        public string Handle()
        {
            return "Test";
        }
    }
}
