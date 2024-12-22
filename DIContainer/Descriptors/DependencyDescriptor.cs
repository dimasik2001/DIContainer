using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainer.Descriptors
{
    public enum LifeTime
    {
        Transient,
        Scoped,
        Singleton
    }
    public class DependencyDescriptor
    {
        public Type DeclaredType { get; }
        public Type RuntimeType { get; }
        public LifeTime LifeTime { get; }
        public DependencyDescriptor(Type declaredType, Type runtimeType, LifeTime lifeTime)
        {
            LifeTime = lifeTime;
            DeclaredType = declaredType;
            RuntimeType = runtimeType;
        }
        

    }
}
