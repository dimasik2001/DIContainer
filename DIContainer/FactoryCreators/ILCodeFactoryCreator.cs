using DIContainer.Descriptors;
using DIContainer.FactoryCreators.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer.FactoryCreators
{
    public class ILCodeFactoryCreator : IFactoryCreator
    {

        public Func<DependencyResolver, object> Create(DependencyDescriptor descriptor, IEnumerable<Type> declaredTypes)
        {

            // Get the matching constructor
            var constructor = descriptor.RuntimeType.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => !x.GetParameters().Select(p => p.ParameterType).Except(declaredTypes).Any())
                .Single();

            // Create a dynamic method
            var dynamicMethod = new DynamicMethod(
                "CreateInstance",
                typeof(object),                       // Return type
                new[] { typeof(DependencyResolver) }, // Parameter types
                typeof(DependencyResolver).Module);   // Module to define the method

            var il = dynamicMethod.GetILGenerator();

            // Get the constructor parameters
            var parameters = constructor.GetParameters();

            // Push each constructor parameter onto the evaluation stack
            foreach (var parameter in parameters)
            {
                // Load the resolver (argument 0)
                il.Emit(OpCodes.Ldarg_0);

                // Push the parameter's type onto the stack
               

                // Call resolver.Resolve(type)
                il.Emit(OpCodes.Callvirt, typeof(DependencyResolver).GetMethod(nameof(DependencyResolver.Resolve), Array.Empty<Type>()).MakeGenericMethod(parameter.ParameterType));

                // If the parameter type is a value type, unbox it
                if (parameter.ParameterType.IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, parameter.ParameterType);
                }
            }

            // Call the constructor with the resolved parameters
            il.Emit(OpCodes.Newobj, constructor);

            // If the created object is a value type, box it to match the return type of object
            //if (descriptor.RuntimeType.IsValueType)
            //{
            //    il.Emit(OpCodes.Box, descriptor.RuntimeType);
            //}

            // Return the created object

            il.Emit(OpCodes.Ret);
            Console.WriteLine("Emitter invoked");

            // Compile the dynamic method into a delegate
            return (Func<DependencyResolver, object>)dynamicMethod.CreateDelegate(typeof(Func<DependencyResolver, object>));
        }
        public Func<DependencyResolver, object> CreateWithILGenerator(DependencyDescriptor descriptor, IEnumerable<Type> declaredTypes)
        {
            // Get the matching constructor
            var constructor = descriptor.RuntimeType.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => !x.GetParameters().Select(p => p.ParameterType).Except(declaredTypes).Any())
                .Single();

            // Create a dynamic method
            var dynamicMethod = new DynamicMethod(
                "CreateInstance",
                typeof(object),                       // Return type
                new[] { typeof(DependencyResolver) }, // Parameter types
                typeof(DependencyResolver).Module);   // Module to define the method

            var il = dynamicMethod.GetILGenerator();

            // Get the constructor parameters
            var parameters = constructor.GetParameters();

            // Push each constructor parameter onto the evaluation stack
            foreach (var parameter in parameters)
            {
                // Load the resolver (argument 0)
                il.Emit(OpCodes.Ldarg_0);

                // Get the generic method definition for Resolve<T>()
                var resolveGenericMethod = typeof(DependencyResolver)
                    .GetMethod(nameof(DependencyResolver.Resolve), Array.Empty<Type>())
                    ?.MakeGenericMethod(parameter.ParameterType);

                if (resolveGenericMethod == null)
                {
                    throw new InvalidOperationException($"Unable to find the generic method Resolve<T>() for parameter type {parameter.ParameterType}.");
                }

                // Call the generic Resolve<T>() method
                il.Emit(OpCodes.Callvirt, resolveGenericMethod);
            }

            // Call the constructor with the resolved parameters
            il.Emit(OpCodes.Newobj, constructor);

            // If the created object is a value type, box it to match the return type of object
            if (descriptor.RuntimeType.IsValueType)
            {
                il.Emit(OpCodes.Box, descriptor.RuntimeType);
            }

            // Return the created object
            il.Emit(OpCodes.Ret);
            // Compile the dynamic method into a delegate
            return (Func<DependencyResolver, object>)dynamicMethod.CreateDelegate(typeof(Func<DependencyResolver, object>));
        }

    }


}
