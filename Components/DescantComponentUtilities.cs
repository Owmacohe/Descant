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
        
        public static bool InvokeMethod(
            DescantNodeComponent component,
            MonoBehaviour script,
            string scriptName,
            string methodName,
            string parameter)
        {
            if (scriptName == "" || methodName == "") return false;
            
            string fullType = script.GetType().ToString();
            string type = fullType.Substring(fullType.LastIndexOf('.') + 1);

            if (type == scriptName)
            {
                try
                {
                    var method = script.GetType().GetMethod(methodName);

                    if (method == null)
                    {
                        DescantUtilities.ErrorMessage(
                            component.GetType(),
                            "Unable to find the method '" + methodName + "' on script '" + scriptName + "'!"
                        );
                        
                        return false;
                    }

                    if (method.GetParameters().Length == 0)
                    {
                        DescantUtilities.ErrorMessage(
                            component.GetType(),
                            "Method '" + methodName + "' on script '" + scriptName + "' has more than one parameter!"
                        );
                        
                        return false;
                    }

                    List<object> parameters = new List<object>();

                    foreach (var j in method.GetParameters())
                        parameters.Add(GetComponentParameterValue(
                            parameter,
                            j.ParameterType
                        ));

                    method.Invoke(script, parameters.ToArray());
                }
                catch (AmbiguousMatchException)
                {
                    DescantUtilities.ErrorMessage(
                        component.GetType(),
                        "Ambiguous name for method '" + methodName + "' on script '" + scriptName + "'!"
                    );
                }

                return true;
            }

            return false;
        }

        public static bool InvokeFromObjectOrScript(
            DescantNodeComponent component,
            string objectTag,
            string scriptName,
            string methodName,
            string parameter)
        {
            if (objectTag == "")
            {
                foreach (var i in GameObject.FindObjectsOfType<MonoBehaviour>())
                    if (InvokeMethod(component, i, scriptName, methodName, parameter))
                        return true;   
            }
            else
            {
                foreach (var i in GameObject.FindWithTag(objectTag).GetComponents<MonoBehaviour>())
                    if (InvokeMethod(component, i, scriptName, methodName, parameter))
                        return true;
            }

            return false;
        }

        public static void MissingMethodError(DescantNodeComponent component, string scriptName, string methodName)
        {
            DescantUtilities.ErrorMessage(
                component.GetType(),
                "Unable to find method '" + methodName + "' on script '" + scriptName + "'!"
            );
        }

        public static DescantActor GetActor(DescantNodeComponent component, List<DescantActor> actors, string name)
        {
            foreach (var i in actors)
                if (i.Name.Equals(name.Trim()))
                    return i;
            
            DescantUtilities.ErrorMessage(
                component.GetType(),
                "Unable to find actor '" + name + "'!"
            );
            
            return null;
        }
        
        public static T ParseEnum<T>(string value) where T : Enum
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}