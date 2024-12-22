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
    public class ExpressionFactoryCreator : IFactoryCreator
    {

        public Func<DependencyResolver, object> Create(DependencyDescriptor descriptor, IEnumerable<Type> declaredTypes)
        {
            var constructor = descriptor.RuntimeType.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => !x.GetParameters()
                .Select(p => p.ParameterType).Except(declaredTypes).Any())
                .Single();

            // Create the parameter for the resolver
            var resolverParameter = Expression.Parameter(typeof(DependencyResolver), "resolver");

            // Generate expressions for constructor parameters
            var constructorParameters = constructor.GetParameters();
            var parameterExpressions = constructorParameters
                .Select(param =>
                {
                    var resolveMethod = typeof(DependencyResolver).GetMethod(nameof(DependencyResolver.Resolve), Array.Empty<Type>())
                    .MakeGenericMethod(param.ParameterType);
                    return Expression.Call(resolverParameter, resolveMethod);
                })
                .ToArray();

            // Create constructor invocation expression
            var newExpression = Expression.New(constructor, parameterExpressions);

            // Lambda expression for Func<DependencyResolver, object>
            var lambda = Expression.Lambda<Func<DependencyResolver, object>>(newExpression, resolverParameter);
            // Compile the expression into a delegate
            return lambda.Compile();
        }
    }
}
