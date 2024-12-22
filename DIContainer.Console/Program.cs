using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using DIContainer;
using DIContainer.FactoryCreators;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using System.Reflection;

namespace LIbraryInteractionTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<BenchmarkTests>();
            var bt = new BenchmarkTests();
            bt.MSDI();
            Console.WriteLine("----------------");
            bt.MyDIIL();
        }

        public Expression Test()
        {
            Expression<Func<DependencyResolver, object>> exp = (resolver) => new Service2(resolver.Resolve<IService>(), resolver.Resolve<Repository>());
            return exp;
        }
        //public Expression Test()
        //{
        //    ParameterExpression parameterExpression = Expression.Parameter(typeof(DependencyResolver), "resolver");
        //    return Expression.Lambda<Func<DependencyResolver, object>>(Expression.New((ConstructorInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/), (IEnumerable<Expression>)new Expression[2]
        //    {
        //Expression.Call(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/), Array.Empty<Expression>()),
        //Expression.Call(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/), Array.Empty<Expression>())
        //    }), new ParameterExpression[1] { parameterExpression });
        //}

    }

    internal class Person
    {
    }

    [MemoryDiagnoser]
    public class BenchmarkTests
    {
        private readonly int[] _data;

       private readonly ServiceProvider _serviceProvider = new ServiceCollection()
                            .AddTransient<IService, Service>()
                            .AddScoped<Repository>()
                            .AddTransient<Service2>()
                            .AddSingleton<Context>()
                            .BuildServiceProvider();
        private readonly DependencyResolver _dependencyResolverReflection = new DependencyBuilder()
                            .AddTransient<IService, Service>()
                            .AddScoped<Repository>()
                            .AddTransient<Service2>()
                            .AddSingleton<Context>()
                            .Build();
        private readonly DependencyResolver _dependencyResolverExpression = new DependencyBuilder(new ExpressionFactoryCreator())
                            .AddTransient<IService, Service>()
                            .AddScoped<Repository>()
                            .AddTransient<Service2>()
                            .AddSingleton<Context>()
                            .Build();
        private readonly DependencyResolver _dependencyResolverIL = new DependencyBuilder(new ILCodeFactoryCreator())
                            .AddTransient<IService, Service>()
                            .AddScoped<Repository>()
                            .AddTransient<Service2>(r => new Service2(new Service(new Repository(new Context())), new Repository(new Context())))
                            .AddSingleton<Context>()
                            .Build();
      //  [Benchmark]
        public object TestMS()
        {
            return _serviceProvider.GetService<Service2>();
        }
       // [Benchmark]
        public object TestMyReflection()
        {
            return _dependencyResolverReflection.Resolve<Service2>();
        }
       // [Benchmark]
        public object TestMyExpression()
        {
            return _dependencyResolverExpression.Resolve<Service2>();
        }
        [Benchmark]
        public object TestMyIL()
        {
            return _dependencyResolverIL.Resolve<Service2>();
        }

        // [Benchmark]
        public void MSDI()
        {
            var serviceProvider = new ServiceCollection()
                            .AddTransient<IService, Service>()
                            .AddScoped<Repository>()
                            .AddTransient<Service2>()
                            .AddSingleton<Context>()
                            .BuildServiceProvider();
            using (var s = serviceProvider.CreateScope())
            {
                s.ServiceProvider.GetService<Service2>();
            }
            //Console.WriteLine();
            //Console.WriteLine("other scope:");
            using (var s = serviceProvider.CreateScope())
            {
                s.ServiceProvider.GetService<Repository>();
            }
        }

        //[Benchmark]
        public void MyDI()
        {
            var resolver = new DependencyBuilder()
                            .AddTransient<IService, Service>()
                            .AddScoped<Repository>()
                            .AddTransient<Service2>()
                            .AddSingleton<Context>()
                            .Build();
            // var service = resolver.CreateScope().Resolve<Repository>();
            using (var scope = resolver.CreateScope())
            {
                scope.Resolve<Service2>();
            }
            //Console.WriteLine(  );
            //Console.WriteLine("other scope:");
            using (var scope = resolver.CreateScope())
            {
                scope.Resolve<Repository>();
            }

        }
       // [Benchmark]
        public void MyDIExpr()
        {
            var resolver = new DependencyBuilder(new ExpressionFactoryCreator())
                            .AddTransient<IService, Service>()
                            .AddScoped<Repository>()
                            .AddTransient<Service2>()
                            .AddSingleton<Context>()
                            .Build();
            // var service = resolver.CreateScope().Resolve<Repository>();
            using (var scope = resolver.CreateScope())
            {
                scope.Resolve<Service2>();
            }
            //Console.WriteLine(  );
            //Console.WriteLine("other scope:");
            using (var scope = resolver.CreateScope())
            {
                scope.Resolve<Repository>();
            }

        } 
        public void MyDIIL()
        {
            var resolver = new DependencyBuilder(new ILCodeFactoryCreator())
                            .AddTransient<IService, Service>()
                            .AddScoped<Repository>()
                            .AddTransient<Service2>()
                            .AddSingleton<Context>()
                            .Build();
            // var service = resolver.CreateScope().Resolve<Repository>();
            using (var scope = resolver.CreateScope())
            {
                scope.Resolve<Service2>();
            }
            //Console.WriteLine(  );
            //Console.WriteLine("other scope:");
            using (var scope = resolver.CreateScope())
            {
                scope.Resolve<Repository>();
            }

        }
    }
}
