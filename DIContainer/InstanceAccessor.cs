using DIContainer.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer
{
    internal class InstanceAccessor
    {
        public Func<DependencyResolver, object> Factory { get; set; }

        //public object Instance { get; set; }

        public object CreateInstance(DependencyResolver dependencyResolver)
        {
            return Factory?.Invoke(dependencyResolver);
        }
        public InstanceAccessor() 
        {
            
        }
    }
}
