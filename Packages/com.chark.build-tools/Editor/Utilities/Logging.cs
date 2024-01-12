using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CHARK.BuildTools.Editor.Utilities
{
    /// <summary>
    /// Logging utilities to wrap Unity logging and add some spice.
    /// </summary>
    internal static class Logging
    {
        internal static void LogDebug(string message, Type owner)
        {
            if (Application.isBatchMode)
            {
                Debug.Log($"[{owner.Name}] {message}");
            }
            else
            {
                Debug.Log($"[<b><color=cyan>{owner.Name}</color></b>] {message}");
            }
        }

        internal static void LogDebug(string message, Object owner)
        {
            var name = string.IsNullOrWhiteSpace(owner.name)
                ? owner.GetType().Name
                : owner.name;

            if (Application.isBatchMode)
            {
                Debug.Log($"[{name}] {message}", owner);
            }
            else
            {
                Debug.Log($"[<b><color=cyan>{name}</color></b>] {message}", owner);
            }
        }

        internal static void LogWarning(string message, Type owner)
        {
            if (Application.isBatchMode)
            {
                Debug.LogWarning($"[{owner.Name}] {message}");
            }
            else
            {
                Debug.LogWarning($"[<b><color=yellow>{owner.Name}</color></b>] {message}");
            }
        }

        internal static void LogWarning(string message, Object owner)
        {
            var name = string.IsNullOrWhiteSpace(owner.name)
                ? owner.GetType().Name
                : owner.name;

            if (Application.isBatchMode)
            {
                Debug.LogWarning($"[{name}] {message}", owner);
            }
            else
            {
                Debug.LogWarning($"[<b><color=yellow>{name}</color></b>] {message}", owner);
            }
        }

        internal static void LogError(string message, Type owner)
        {
            if (Application.isBatchMode)
            {
                Debug.LogError($"[{owner.Name}] {message}");
            }
            else
            {
                Debug.LogError($"[<b><color=red>{owner.Name}</color></b>] {message}");
            }
        }

        internal static void LogError(string message, Object owner)
        {
            var name = string.IsNullOrWhiteSpace(owner.name)
                ? owner.GetType().Name
                : owner.name;

            if (Application.isBatchMode)
            {
                Debug.LogError($"[{name}] {message}", owner);
            }
            else
            {
                Debug.LogError($"[<b><color=red>{name}</color></b>] {message}", owner);
            }
        }

        internal static void LogException(Exception exception, Type owner)
        {
            Debug.LogException(exception);
        }

        internal static void LogException(Exception exception, Object owner)
        {
            Debug.LogException(exception, owner);
        }
    }
}
