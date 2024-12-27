using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalScripts
{
    public struct DebugUtils
    {
        public static void Log(object message, bool onlyEditor = true)
        {
            if (onlyEditor)
            {
#if UNITY_EDITOR
                Debug.Log(message);
#endif
            }

            else Debug.Log(message);
        }

        public static void Log(object message, Object context, bool onlyEditor = true)
        {
            if (onlyEditor)
            {
#if UNITY_EDITOR
                Debug.Log(message, context);
#endif
            }

            else Debug.Log(message, context);
        }

        public static void LogWarning(object message, bool onlyEditor = true)
        {
            if (onlyEditor)
            {
#if UNITY_EDITOR
                Debug.LogWarning(message);
#endif
            }

            else Debug.LogWarning(message);
        }

        public static void LogWarning(object message, Object context, bool onlyEditor = true)
        {
            if (onlyEditor)
            {
#if UNITY_EDITOR
                Debug.LogWarning(message, context);
#endif
            }

            else Debug.LogWarning(message, context);
        }

        public static void LogError(object message, bool onlyEditor = true)
        {
            if (onlyEditor)
            {
#if UNITY_EDITOR
                Debug.LogError(message);
#endif
            }

            else Debug.LogError(message);
        }

        public static void LogError(object message, Object context, bool onlyEditor = true)
        {
            if (onlyEditor)
            {
#if UNITY_EDITOR
                Debug.LogError(message, context);
#endif
            }

            else Debug.LogError(message, context);
        }
    }
}
