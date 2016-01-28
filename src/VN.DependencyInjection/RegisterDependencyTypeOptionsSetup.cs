using System.Collections.Generic;
using Microsoft.Extensions.OptionsModel;

namespace VN.DependencyInjection
{
    public class RegisterDependencyTypeOptionsSetup : ConfigureOptions<RegisterDependencyTypeOptions>
    {
        public RegisterDependencyTypeOptionsSetup() : base(ConfigureAssemblyPathOptions)
        {
        }

        /// <summary>
        /// Set the default options
        /// </summary>
        public static void ConfigureAssemblyPathOptions(RegisterDependencyTypeOptions options)
        {
            options.AssemblyPathLocation = string.Empty;
            options.InjectFromInterfaceName = string.Empty;
            options.TaskManifest = new List<string>();
        }
    }
}