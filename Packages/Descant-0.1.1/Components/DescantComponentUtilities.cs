using System;
using System.Collections.Generic;
using System.Linq;

namespace DescantComponents
{
    public static class DescantComponentUtilities
    {
        public static List<Type> GetAllNodeComponentTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(domainAssembly => domainAssembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(DescantNodeComponent)))
                .ToList();
        }

        public static List<string> TrimmedNodeComponentTypes(List<Type> types)
        {
            return types
                .Select(type => type.ToString().Substring(27))
                .ToList();
        }

        public static float GetNodeComponentMaximum(string componentName)
        {
            List<Type> types = GetAllNodeComponentTypes();

            foreach (var i in types)
            {
                string typeName = i.ToString().Substring(i.ToString().LastIndexOf('.') + 1);
                
                if (typeName == componentName)
                {
                    return (((MaxQuantityAttribute) i.GetCustomAttributes(
                        typeof(MaxQuantityAttribute),
                        true
                    ).FirstOrDefault())!).Quantity;
                }
            }

            return -1;
        }
    }
}