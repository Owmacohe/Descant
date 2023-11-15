using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

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

        public static string GetTrimmedNodeComponentType(Type type)
        {
            return type.ToString().Substring(18);
        }

        public static List<string> TrimmedNodeComponentTypes(List<Type> types)
        {
            return types
                .Select(GetTrimmedNodeComponentType)
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

        public static object GetComponentParameterValue(string value, Type type)
        {
            object temp = type != typeof(string)
                ? float.Parse(value)
                : value;

            return type == typeof(int)
                ? int.Parse(value)
                : temp;
        }
        
        public static bool InvokeMethod(MonoBehaviour script, string scriptName, string methodName, string parameter)
        {
            string fullType = script.GetType().ToString();
            string type = fullType.Substring(fullType.LastIndexOf('.') + 1);

            if (type == scriptName)
            {
                try
                {
                    var method = script.GetType().GetMethod(methodName);

                    if (method == null)
                    {
                        Debug.Log("<b>EventScript:</b> Unable to find the given method!");
                        return false;
                    }

                    List<object> parameters = new List<object>();

                    foreach (var j in method.GetParameters())
                        parameters.Add(DescantComponentUtilities.GetComponentParameterValue(
                            parameter,
                            j.ParameterType
                        ));

                    method.Invoke(script, parameters.ToArray());
                }
                catch (AmbiguousMatchException e)
                {
                    Debug.Log("<b>EventScript:</b> Ambiguous method name!");
                }

                return true;
            }

            return false;
        }
    }
}