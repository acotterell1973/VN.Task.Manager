using System;
using System.Collections.Generic;
using VN.Attributes;
using VN.Task.Common;

namespace VN.Task.UPStracking
{

    [ScopedDependency(ServiceType = typeof (IScheduledTask))]
    public sealed class UpsTracking : BaseSingleThreadedTask
    {
        private const string taskCode = "x";
          
        public UpsTracking() : base(taskCode)
        {
        }

        public override string TaskCode => "TASK_CODE";
        public override string TaskName => "stufhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhkkkkkkkkkkkkkkkkkkf";
        public override string TaskDescription => "stuffjjjjjjjjjjjjjjjjjhhhhhhhhhhhhhh";

        public override bool Run()
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<ArgumentDescriptor> ArgumentDescriptors => new[]
        {
            new ArgumentDescriptor
            {
                Argument="/filename",
                PostArguments="<filename>",
                Description= @" The csv file to parse for batch registrations."
            }
        };

        public override bool ParseArguments(string[] args)
        {
            var argQueue = new Queue<string>(args);
            while (argQueue.Count > 0)
            {
                var arg = argQueue.Dequeue();
                if (!arg.Contains("/filename")) continue;
                if (argQueue.Count == 0)
                {
                    Log("/filename argument expects a filename.csv value");
                    return false;
                }
                var argVal = argQueue.Dequeue();
                   
                return true;
            }

            return false;


        }
    }
}