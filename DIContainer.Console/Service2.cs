using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIbraryInteractionTest
{
    internal class Service2 : BaseInjectable
    {
        public Service2(IService service, Repository repository)
        {
           // Console.WriteLine("Service2 constructor invoked");
        }
    }
}
