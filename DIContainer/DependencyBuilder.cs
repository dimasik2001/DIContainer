using DIContainer.Descriptors;
using DIContainer.FactoryCreators;
using DIContainer.FactoryCreators.Abstractions;

namespace DIContainer;

public class DependencyBuilder
{
    
    private Dictionary<Type, DependencyDescriptor> _dependencyDescriptors = new();
    public Dictionary<Type, Func<DependencyResolver, object>> _factories = new();
    public Dictionary<Type, object> _singletonInstances = new();
    private readonly IFactoryCreator _factoryCreator;

    public DependencyBuilder() : this(new ReflectionFactoryCreator())
    {
    }
    public DependencyBuilder(IFactoryCreator factoryCreator)
    {
        _factoryCreator = factoryCreator;
    }

    public DependencyBuilder AddTransient<TDecl, TImpl>() where TImpl : TDecl
    => AddTransient(typeof(TDecl), typeof(TImpl));
    public DependencyBuilder AddSingleton<TDecl, TImpl>() where TImpl : TDecl
   => AddSingleton(typeof(TDecl), typeof(TImpl));
    public DependencyBuilder AddScoped<TDecl, TImpl>() where TImpl : TDecl
   => AddScoped(typeof(TDecl), typeof(TImpl));

    public DependencyBuilder AddTransient<TDecl, TImpl>(Func<DependencyResolver, TImpl> factory) where TImpl : TDecl
   => Add(typeof(TDecl), typeof(TImpl), LifeTime.Transient, (resolver) => factory.Invoke(resolver));

    public DependencyBuilder AddSingleton<TDecl, TImpl>(Func<DependencyResolver, TImpl> factory) where TImpl : TDecl
   => Add(typeof(TDecl), typeof(TImpl), LifeTime.Singleton, (resolver) => factory.Invoke(resolver));
    public DependencyBuilder AddScoped<TDecl, TImpl>(Func<DependencyResolver, TImpl> factory) where TImpl : TDecl
   => Add(typeof(TDecl), typeof(TImpl), LifeTime.Scoped, (resolver) => factory.Invoke(resolver));

    public DependencyBuilder AddTransient<TImpl>()
    => AddTransient(typeof(TImpl), typeof(TImpl));
    public DependencyBuilder AddSingleton<TImpl>()
   => AddSingleton(typeof(TImpl), typeof(TImpl));
    public DependencyBuilder AddScoped<TImpl>()
   => AddScoped(typeof(TImpl), typeof(TImpl));

    public DependencyBuilder AddTransient<TImpl>(Func<DependencyResolver, TImpl> factory)
    => Add(typeof(TImpl), typeof(TImpl), LifeTime.Transient, (resolver) => factory.Invoke(resolver));
    public DependencyBuilder AddSingleton<TImpl>(Func<DependencyResolver, TImpl> factory)
    => Add(typeof(TImpl), typeof(TImpl), LifeTime.Singleton, (resolver) => factory.Invoke(resolver));
    public DependencyBuilder AddScoped<TImpl>(Func<DependencyResolver, TImpl> factory)
    => Add(typeof(TImpl), typeof(TImpl), LifeTime.Scoped, (resolver) => factory.Invoke(resolver));

    public DependencyBuilder AddSingleton<TImpl>(TImpl instance)
   => Add(typeof(TImpl), typeof(TImpl), LifeTime.Singleton, null, instance);

    public DependencyBuilder AddTransient(Type declType, Type implType) => Add(declType, implType, LifeTime.Transient);

    public DependencyBuilder AddSingleton(Type declType, Type implType) => Add(declType, implType, LifeTime.Singleton);
    public DependencyBuilder AddScoped(Type declType, Type implType) => Add(declType, implType, LifeTime.Scoped);


    private DependencyBuilder Add(Type declType, Type implType, LifeTime lifeTime, Func<DependencyResolver, object> factory = null, object instance = null)
    {
        var descriptor = new DependencyDescriptor(declType, implType, lifeTime);
        _dependencyDescriptors.Add(declType, descriptor);
        if (factory != null)
        {
            _factories.Add(declType, factory);
        }
        if (lifeTime == LifeTime.Singleton && instance != null)
        {
            _singletonInstances.Add(declType, instance);
        }
        return this;
    }

    public DependencyResolver Build()
    {
        return new DependencyResolver(_factoryCreator, _dependencyDescriptors, _factories, _singletonInstances);
    }
}

