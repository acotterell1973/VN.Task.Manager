using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.Runtime;
using VN.Task.Common;
using VN.Task.Manager.config;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;
using IApplicationEnvironment = Microsoft.Extensions.PlatformAbstractions.IApplicationEnvironment;
using IAssemblyLoadContext = Microsoft.Extensions.PlatformAbstractions.IAssemblyLoadContext;
using IAssemblyLoader = Microsoft.Extensions.PlatformAbstractions.IAssemblyLoader;
using IConfigurationRoot = Microsoft.Extensions.Configuration.IConfigurationRoot;
using IRuntimeEnvironment = Microsoft.Extensions.PlatformAbstractions.IRuntimeEnvironment;
using VN.Task.UPStracking;

namespace VN.Task.Manager
{
    public class Program
    {
        private const string logCategory = "VN.Task.Manager";
        private static int _windowWidth = 80;
        private static IRuntimeEnvironment _runtime;
        private static IApplicationEnvironment _application;
        private static IServiceManifest _serviceManifest;
        private static IOptions<TaskManagerConfigurationSettings> _options;
        private static IAssemblyLoadContext _loadContext;
        private static IConfigurationRoot _configuration;

        private static Startup _startup;
        private static ServiceCollection _serviceCollection;

        public static void Main(string[] args)
        {
            _application = PlatformServices.Default.Application;
            _runtime = PlatformServices.Default.Runtime;
            var configurationBuilder = new ConfigurationBuilder();
            _serviceCollection = new ServiceCollection();

            try
            {
                try
                {
                    _windowWidth = Console.WindowWidth;
                }
                catch (IOException)
                {
                    _windowWidth = 80;
                }

                // No arguments displays the usage text:
                if (args.Length == 0)
                {
                    DisplayUsageText();
                    Environment.ExitCode = -1;
                    return;
                }

                _startup = new Startup(_application);
                _startup.Configure(configurationBuilder, args);
                _startup.ConfigureServices(_serviceCollection);

                //get all instantiated task
                var registeredTaskInstances = _startup.GetServices<IScheduledTask>();

                //Validate all task codes
                var invalidTasks = registeredTaskInstances.Where(t => !t.TaskCode.IsIdentifier());
                if (invalidTasks.Any())
                {
                    foreach (var invalidTask in invalidTasks)
                    {
                        //log the invalid task somethere
                    }
                    Environment.ExitCode = -1;
                    return;
                }

                // Parse the command line arguments:
                Queue<string> argQueue = new Queue<string>(args);
                while (argQueue.Count > 0)
                {
                    string arg = argQueue.Dequeue();
                    if (arg.CaseInsensitiveEquals("/help"))
                    {
                        // If no arguments provided after /help, display normal usage text:
                        if (argQueue.Count == 0)
                        {
                            DisplayUsageText();
                            Environment.ExitCode = -1;
                            return;
                        }

                        // Assume the next argument is the task code and find the specific task:
                        string code = argQueue.Dequeue();

                        var specTask =
                            registeredTaskInstances.Where(task => task.TaskCode.CaseInsensitiveTrimmedEquals(code))
                                .SingleOrDefault();
                        if (specTask == null)
                        {
                            Console.Error.WriteLine("Could not find task for code \"{0}\"", code);
                            Environment.ExitCode = -1;
                            return;
                        }

                        // Display usage and scheduling information:
                        Console.WriteLine("Task Code:   \"{0}\"", specTask.TaskCode);
                        Console.WriteLine("Description: {0}", String.Join(
                            Environment.NewLine + "             ",
                            specTask.TaskName.WordWrap(_windowWidth - 14).ToArray()
                            ));
                        Console.WriteLine();
                        Console.WriteLine("Business Purpose:");
                        Console.WriteLine();
                        Console.WriteLine("  " + String.Join(
                            Environment.NewLine + "  ",
                            specTask.TaskDescription.WordWrap(_windowWidth - 3).ToArray()
                            ));
                        Console.WriteLine();
                        Console.WriteLine("Usage:");
                        Console.WriteLine();
                        Console.WriteLine(FormatArgumentDescriptors(specTask.ArgumentDescriptors, _windowWidth));
                        //Console.WriteLine();
                        Console.WriteLine("Scheduling:");
                        Console.WriteLine();
                        //    Console.WriteLine(FormatArgumentDescriptors(specTask.ScheduleTriggers, _windowWidth));
                        //Console.WriteLine();
                        return;
                    }
                    else if (arg.CaseInsensitiveEquals("/list"))
                    {
                        // Lists all available tasks:
                        Console.WriteLine("Available tasks:");
                        Console.WriteLine();
                        foreach (var task in registeredTaskInstances.OrderBy(t => t.TaskCode))
                        {
                            Console.WriteLine($"  Task Code:   \"{task.TaskCode}\"");
                            Console.WriteLine("  Description: {0}", string.Join(
                                Environment.NewLine + "               ",
                                task.TaskName.WordWrap(_windowWidth - 16).ToArray()
                                ));
                            Console.WriteLine();
                        }
                        return;
                    }
                    else if (arg.CaseInsensitiveEquals("/run"))
                    {


                    }
                }

            }
            catch (Exception exception)
            {

                Console.WriteLine(exception.Message);
                Console.ReadLine();
            }
            
        }




        private static string FormatArgumentDescriptors(IEnumerable<ArgumentDescriptor> args, int columns)
        {
            StringBuilder sb = new StringBuilder();

            // 2/7ths, pulled directly out of my...
            int descstart = columns * 2 / 7;

            foreach (var arg in args)
            {
                // Write the argument's usage format:
                sb.Append("  ");
                sb.Append(arg.Argument);
                var lineLength = 2 + arg.Argument.Length;

                if (!arg.PostArguments.IsNullOrEmpty())
                {
                    lineLength += 1 + arg.PostArguments.Length;
                    sb.Append(" ");

                    sb.Append(arg.PostArguments);
                }

                if (!arg.Description.IsNullOrEmpty())
                {
                    // Align up to where descriptions should start:
                    if (lineLength > descstart)
                    {
                        sb.AppendLine();
                        sb.Append(new string(' ', descstart));
                    }
                    else
                    {
                        sb.Append(new string(' ', descstart - lineLength));
                    }

                    // Write out the description word-wrapped to the end of the window:
                    sb.AppendLine(String.Join(
                        Environment.NewLine + new string(' ', descstart),
                        arg.Description.WordWrap(columns - descstart - 1).ToArray()
                    ));
                }
                else
                {
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }
        private static void DisplayUsageText()
        {
            Console.WriteLine("ApplicationName: {0} {1}", _application.ApplicationName, _application.ApplicationVersion);
            Console.WriteLine("ApplicationBasePath: {0}", _application.ApplicationBasePath);
            Console.WriteLine("Framework: {0}", _application.RuntimeFramework.FullName);
            Console.WriteLine("_runtime: {0} {1} {2}", _runtime.RuntimeType, _runtime.RuntimeArchitecture, _runtime.RuntimeVersion);
            Console.WriteLine("System: {0} {1}", _runtime.OperatingSystem, _runtime.OperatingSystemVersion);


            Console.WriteLine("Usage:");
            Console.WriteLine();
            IEnumerable<ArgumentDescriptor> args = new ArgumentDescriptor[] {
                new ArgumentDescriptor() { Argument = "VN.Task.Manager.exe", PostArguments = "<command>" }
            };

            Console.WriteLine(FormatArgumentDescriptors(args, _windowWidth));
            Console.WriteLine("Commands:");
            Console.WriteLine();
            args = new ArgumentDescriptor[] {
                new ArgumentDescriptor() { Argument = "/list", Description = "Displays a list of tasks available." },
                new ArgumentDescriptor() { Argument = "/help", Description = "Displays this help text." },
                new ArgumentDescriptor() { Argument = "/help", PostArguments = "<task code>", Description = "Displays help text for the specific task including arguments usage and scheduling details." },
                new ArgumentDescriptor() { Argument = "/run", PostArguments = "<task code> [arguments]", Description = "Runs a specific task given its identifier code with optional arguments for the task." },
            };
            Console.WriteLine(FormatArgumentDescriptors(args, _windowWidth));
            Console.WriteLine("Purpose:");
            Console.WriteLine();
            Console.WriteLine("  " + String.Join(
                Environment.NewLine + "  ",
                ("This tool is intended to be used as a Windows Scheduled Task. The scheduled " +
                "task command line should use a /run command with a specific task code to run. " +
                "The other commands are intended for off-line usage to help the administrator " +
                "configure the scheduled tasks.").WordWrap(_windowWidth - 3).ToArray()
            ));
            Console.WriteLine();
        }
    }


}
