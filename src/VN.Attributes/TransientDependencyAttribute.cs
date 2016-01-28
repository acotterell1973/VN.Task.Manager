using Microsoft.Extensions.DependencyInjection;

namespace VN.Attributes
{
    public class TransientDependencyAttribute : DependencyAttribute
    {
        public TransientDependencyAttribute() : base(ServiceLifetime.Transient)
        {
        
        }
    }
}
