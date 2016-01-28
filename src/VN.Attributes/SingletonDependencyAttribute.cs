using Microsoft.Extensions.DependencyInjection;

namespace VN.Attributes
{
    public class SingletonDependencyAttribute : DependencyAttribute
    {
        public SingletonDependencyAttribute() : base(ServiceLifetime.Transient)
        {

        }
    }
}
