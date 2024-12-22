using DIContainer.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer.FactoryCreators.Abstractions
{
    public interface IFactoryCreator
    {
        Func<DependencyResolver, object> Create(DependencyDescriptor descriptor, IEnumerable<Type> declaredTypes);
    }
}
