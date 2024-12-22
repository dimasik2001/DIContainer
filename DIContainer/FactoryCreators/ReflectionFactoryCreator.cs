using DIContainer.Descriptors;
using DIContainer.FactoryCreators.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer.FactoryCreators
{
    public class ReflectionFactoryCreator : IFactoryCreator
    {
        public Func<DependencyResolver, object> Create(DependencyDescriptor descriptor, IEnumerable<Type> declaredTypes)
        {
            var constructor = descriptor.RuntimeType.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                    .Where(x => !x.GetParameters().Select(p => p.ParameterType).Except(declaredTypes).Any()).Single();

            var parameters = constructor.GetParameters();

            Func<DependencyResolver, object> factory = (resolver) => constructor.Invoke(parameters.Select(x => resolver.Resolve(x.ParameterType)).ToArray());
            return factory;
        }

       

    }
}
