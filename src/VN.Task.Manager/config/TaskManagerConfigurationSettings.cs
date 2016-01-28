using System.Collections.Generic;

namespace VN.Task.Manager.config
{
    public class TaskManagerConfigurationSettings : ITaskManagerConfigurationSettings
    {
        public string TaskLib { get; set; }
        public string InterfaceType { get; set; }
        public string ExternalAssemblyPath { get; set; }
        public List<string> TaskManifest { get; set; }
    }
}
