using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class RuntimeSerializedObjectSaver : MonoBehaviour
{
    // A queue of SerializedObjects that need to be saved
    Queue<Object> objectsToSave = new Queue<Object>();
    
    /// <summary>
    /// Quick method to add an object to the saving queue
    /// (the queue is used instead of directly saving so that the objects can be saved even during runtime)
    /// </summary>
    /// <param name="obj">The SerializedObject to be saved</param>
    public void AddObjectToQueue(Object obj)
    {
        objectsToSave.Enqueue(obj);
    }

    #if UNITY_EDITOR
    void Update()
    {
        // Saving the SerializedObjects
        if (objectsToSave.Count > 0) DescantUtilities.SaveSerializedObject(objectsToSave.Dequeue());
    }
    #endif
}