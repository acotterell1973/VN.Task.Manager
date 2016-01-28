using System.Collections.Generic;

namespace VN.Task.Manager.config
{
    public interface ITaskManagerConfigurationSettings
    {
        string TaskLib { get; set; }
        string InterfaceType { get; set; }

        string ExternalAssemblyPath { get; set; }

        List<string> TaskManifest { get; set; } 

    }
}