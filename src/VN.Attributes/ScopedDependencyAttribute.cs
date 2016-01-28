using Microsoft.Extensions.DependencyInjection;

namespace VN.Attributes
{
    public class ScopedDependencyAttribute : DependencyAttribute
    {
        public ScopedDependencyAttribute() : base(ServiceLifetime.Transient)
        {

        }
    }
}
