using System;
using System.Collections.Generic;
using VN.Attributes;
using VN.Task.Common;

namespace VN.Task.UPStracking
{

    [ScopedDependency(ServiceType = typeof (IScheduledTask))]
    public sealed class UpsTracking : BaseSingleThreadedTask
    {
        private const string TracingCategory = "VN.Task.UPStracking";

        public UpsTracking() : base(TracingCategory)
        {
        }


        public override string TaskCode => "VN_SHIPPING_UPSTRACKING";
        public override string TaskName => "VendorNet Shpipping Ups Tracking Scheduled Task";
        public override string TaskDescription => "Calls out to UPS and downloads the tracking numbers for the givin shipped items.";

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