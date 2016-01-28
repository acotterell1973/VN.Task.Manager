using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.PlatformAbstractions;

namespace VN.DependencyInjection
{
    public class DirectoryLoader : IAssemblyLoader
    {
        private readonly IAssemblyLoadContext _context;
        private readonly ILibraryManager _libraryManager;
        private readonly string _path;

        public DirectoryLoader(string path, IAssemblyLoadContext context, ILibraryManager libraryManager)
        {
            _path = path;
            _context = context;
            _libraryManager = libraryManager;
        }

        public Assembly Load(AssemblyName assemblyName)
        {
            return _context.LoadFile(Path.Combine(_path, assemblyName.Name + ".dll"));
        }

        public Assembly Load(string name)
        {
            return _context.Load(name);
        }
        public IntPtr LoadUnmanagedLibrary(string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AssemblyName> GetAssembliesReferencingThis(List<string> assemblyNames)
        {
            return _libraryManager.GetLibraries()
                .Where(library => assemblyNames.Contains(library.Name, StringComparer.InvariantCulture))
                .Select(library => new AssemblyName(library.Name));
        }

        public IEnumerable<AssemblyName> GetAssembliesReferencingThis(AssemblyName assemblyName)
        {
            return _libraryManager.GetReferencingLibraries(assemblyName.Name)
        
                .SelectMany(library => library.Assemblies);
        }
    }
}