using System.Reflection;

namespace million.infrastructure.Common.Persistence;

public class BsonClassMapRegister
{
    private static readonly HashSet<Assembly> RegisteredAssemblies = [];
    private static readonly Lock Lock = new();

    public static void RegisterFromAssembly(Assembly assembly)
    {
        lock (Lock)
        {
            if (!RegisteredAssemblies.Add(assembly)) return;
            
            var configTypes = assembly.GetTypes()
                .Where(t => t is { IsInterface: false, IsAbstract: false } 
                            && typeof(IBsonMapConfiguration).IsAssignableFrom(t))
                .ToList();
            
            foreach (var configType in configTypes)
            {
                 var config = (IBsonMapConfiguration)Activator.CreateInstance(configType)!;
                 config.Configure();
            }
            
        }
    }
}