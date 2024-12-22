using DIContainer.Descriptors;
using DIContainer.FactoryCreators;
using DIContainer.FactoryCreators.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DIContainer;
public class DependencyResolver : IDisposable
{
    private readonly IDictionary<Type, DependencyDescriptor> _descriptors;
    private readonly Dictionary<Type, Func<DependencyResolver, object>> _factories;
    private readonly IFactoryCreator _factoryCreator;
    private readonly Dictionary<Type, object> _singletonInstances;
    private readonly Dictionary<Type, object> _scopedInstances = new();
    internal DependencyResolver(IDictionary<Type, DependencyDescriptor> descriptors) :
        this(new ReflectionFactoryCreator(), descriptors, new Dictionary<Type,Func<DependencyResolver, object>>(), new Dictionary<Type, object>())
    { }

    
    internal DependencyResolver(IFactoryCreator factoryCreator, IDictionary<Type, DependencyDescriptor> descriptors,
        Dictionary<Type, Func<DependencyResolver, object>> factories,
        Dictionary<Type, object> singletonInstances)    
    {
        _descriptors = descriptors;
        _factories = factories;
        _singletonInstances = singletonInstances;
        _factoryCreator = factoryCreator;
    }
    public DependencyResolver CreateScope()
    {
        return new DependencyResolver(_factoryCreator, _descriptors, _factories, _singletonInstances);
    }

   

    public void Dispose()
    {
        foreach (var disposable in _scopedInstances.Values.OfType<IDisposable>())
        {
            disposable.Dispose();
        }
    }

    public TInstance Resolve<TInstance>()
    {
        return (TInstance)Resolve(typeof(TInstance));
    }

    public object Resolve(Type declaredType)
    {
        var descriptor = _descriptors[declaredType];
        
        var cachedInstance = GetInstance(declaredType, descriptor.LifeTime);
        if (!_factories.ContainsKey(declaredType) && cachedInstance is null)
        {
            _factories[declaredType] = CreateFactory(descriptor, _descriptors.Values.Select(x => x.DeclaredType));
        }

        if (cachedInstance is not null)
        {
            return cachedInstance;
        }
        var instance = _factories[declaredType].Invoke(this);

        SetInstance(declaredType, descriptor.LifeTime, instance);

        return instance;

    }

    private object? GetInstance(Type declaredType, LifeTime lifeTime)
    {
        return lifeTime switch
        {
            LifeTime.Scoped => _scopedInstances.GetValueOrDefault(declaredType),
            LifeTime.Singleton => _singletonInstances.GetValueOrDefault(declaredType),
            _ => null
        };
    }
    private void SetInstance(Type declaredType, LifeTime lifeTime, object instance)
    {
        _ = lifeTime switch
        {
            LifeTime.Scoped => _scopedInstances[declaredType] = instance,
            LifeTime.Singleton => _singletonInstances[declaredType] = instance,
            _ => null
        };
    }

    private Func<DependencyResolver, object> CreateFactory(DependencyDescriptor descriptor, IEnumerable<Type> declaredTypes)
    {
        return _factoryCreator.Create(descriptor, declaredTypes);
    }
}
