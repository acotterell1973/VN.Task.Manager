using System.Collections.Generic;

namespace VN.DependencyInjection
{
    public class RegisterDependencyTypeOptions
    {
        public RegisterDependencyTypeOptions()
        {
            AssemblyPathLocation = string.Empty;
            InjectFromInterfaceName = string.Empty;
            TaskManifest = new List<string>();
        }

        public string AssemblyPathLocation { get; set; }
        public string InjectFromInterfaceName { get; set; }

        public List<string> TaskManifest { get; set; } 
    }
}