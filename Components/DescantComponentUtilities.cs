using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DescantComponents
{
    public static class DescantComponentUtilities
    {
        #region Component Type Lists
        
        /// <summary>
        /// Gets the list of all class types that inherit from DescantComponent
        /// </summary>
        public static List<Type> GetComponentTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(domainAssembly => domainAssembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(DescantComponent)))
                .ToList();
        }
        
        /// <summary>
        /// Trims a class name down to its base name (i.e. not including namespaces/parents)
        /// </summary>
        /// <param name="type">The type with the name to be trimmed</param>
        public static string GetTrimmedTypeName(Type type)
        {
            var temp = type.ToString();
            return temp.Substring(temp.LastIndexOf('.') + 1);
        }

        /// <summary>
        /// Gets the list of trimmed names of all class types that inherit from DescantComponent
        /// </summary>
        /// <param name="types">The types with names to be trimmed</param>
        /// <returns></returns>
        public static List<string> GetTrimmedComponentTypes(List<Type> types)
        {
            return types
                .Select(GetTrimmedTypeName)
                .ToList();
        }
        
        #endregion
        
        #region Component Attributes

        /// <summary>
        /// Gets the maximum number of Components of some type that can be added to a DescantNode
        /// </summary>
        /// <param name="componentName">The name of the Component being checked</param>
        /// <returns>(-1 if no maximum has been indicated)</returns>
        public static float GetComponentMaximum(string componentName)
        {
            foreach (var i in GetComponentTypes())
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

        /// <summary>
        /// Gets the value of a Component's parameter, making sure to first parse it accordingly
        /// </summary>
        /// <param name="value">The value of the parameter</param>
        /// <param name="type">The type of the parameter</param>
        public static object GetComponentParameterValue(string value, Type type)
        {
            if (type == typeof(int)) return value.Trim().Equals("") ? 0 : int.Parse(value);
            if (type == typeof(float)) return value.Trim().Equals("") ? 0f : float.Parse(value);
            return value;
        }
        
        #endregion
        
        #region Method Invokation
        
        /// <summary>
        /// Calls a method from somewhere in the scene
        /// </summary>
        /// <param name="component">The Component source that this call is coming from</param>
        /// <param name="script">The script that the method belongs to</param>
        /// <param name="scriptName">The name of the script that the method belongs to</param>
        /// <param name="methodName">The name of the method being called</param>
        /// <param name="parameter">The parameter being accepted by the method (can be empty)</param>
        /// <returns>Whether the method was successfully called</returns>
        static bool InvokeMethod(
            DescantComponent component,
            MonoBehaviour script,
            string scriptName,
            string methodName,
            string parameter)
        {
            if (scriptName == "" || methodName == "") return false; // Stopping if the script or method names are false
            
            if (GetTrimmedTypeName(script.GetType()) == scriptName)
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

                    var methodParameters = method.GetParameters(); // The ParameterInfo for the method

                    if (methodParameters.Length > 1)
                    {
                        DescantUtilities.ErrorMessage(
                            component.GetType(),
                            "Method '" + methodName + "' on script '" + scriptName + "' has more than one parameter!"
                        );
                        
                        return false;
                    }

                    List<object> parameters = new List<object>(); // The parameters that are going to be supplied

                    foreach (var j in methodParameters)
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

        /// <summary>
        /// Calls a method from somewhere in the scene
        /// </summary>
        /// <param name="component">The Component source that this call is coming from</param>
        /// <param name="objectTag">
        /// The tag of the GameObject that the script is attached to
        /// (if empty, the first found script in the scene with the given name is called instead
        /// </param>
        /// <param name="scriptName">The name of the script that the method belongs to (if empty, no method is called)</param>
        /// <param name="methodName">The name of the method being called (if empty, no method is called)</param>
        /// <param name="parameter">The parameter being accepted by the method (can be empty)</param>
        /// <returns>Whether the method was successfully called</returns>
        public static bool InvokeFromObjectOrScript(
            DescantComponent component,
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
        
        /// <summary>
        /// Method to be called by Components if InvokeFromObjectOrScript returns false
        /// </summary>
        /// <param name="component">The Component source that this call is coming from</param>
        /// <param name="scriptName">The name of the script that the method belongs to</param>
        /// <param name="methodName">The name of the method being called</param>
        public static void MissingMethodError(DescantComponent component, string scriptName, string methodName)
        {
            DescantUtilities.ErrorMessage(
                component.GetType(),
                "Unable to find method '" + methodName + "' on script '" + scriptName + "'!"
            );
        }
        
        #endregion

        /// <summary>
        /// Gets an actor with a given name from the a list of actors
        /// </summary>
        /// <param name="component">The Component source that this call is coming from</param>
        /// <param name="actors">The current list of DescantActors</param>
        /// <param name="name">The name of the DescantActor being searched for</param>
        public static DescantActor GetActor(DescantComponent component, List<DescantActor> actors, string name)
        {
            foreach (var i in actors)
                if (i.name.Equals(name.Trim()))
                    return i;
            
            DescantUtilities.ErrorMessage(
                component.GetType(),
                "Unable to find actor '" + name + "'!"
            );
            
            return null;
        }
        
        /// <summary>
        /// Parses a string into an enum
        /// </summary>
        /// <param name="value">The string value to parse</param>
        /// <typeparam name="T">The enum type we are checking</typeparam>
        /// <returns>The value that the string corresponds to</returns>
        public static T ParseEnum<T>(string value) where T : Enum
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// Method to copy all values from one DescantActor to another
        /// (simply assigning SerializedObjects breaks the link to the object in the project files)
        /// </summary>
        /// <param name="data">The actor to be assigned to</param>
        /// <param name="actor">The actor to be assigned from</param>
        public static void AssignActor(DescantActor data, DescantActor actor)
        {
            data.StatisticKeys = actor.StatisticKeys;
            data.StatisticValues = actor.StatisticValues;

            data.Topics = actor.Topics;

            data.RelationshipKeys = actor.RelationshipKeys;
            data.RelationshipValues = actor.RelationshipValues;

            data.DialogueAttempts = actor.DialogueAttempts;
        }
    }
}