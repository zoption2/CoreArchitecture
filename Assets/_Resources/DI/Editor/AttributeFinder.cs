using System;
using System.Linq;
using System.Collections.Generic;
using Zenject;


public static class AttributeFinder
{
    public static IEnumerable<Type> FindClassesWithContextInstallerAttribute()
    {
        var typesWithAttribute = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from type in assembly.GetTypes()
                                 where type.Equals(typeof(MonoInstaller))
                                 let attributes = type.GetCustomAttributes(typeof(ContextInstallerAttribute), true)
                                 where attributes != null && attributes.Length > 0
                                 select type;

        /*var assemblies = AppDomain.CurrentDomain.GetAssemblies();

var types = assemblies.SelectMany(assembly => assembly.GetTypes())
    .Where(type => typeof(MonoInstaller).IsAssignableFrom(type)) // Check if type is assignable from MonoInstaller
    .Where(type => type.GetCustomAttributes(typeof(ContextInstallerAttribute), true).Length > 0) // Check if attributes exist
    .Select(type => type);
        */

        return typesWithAttribute;
    }
}
