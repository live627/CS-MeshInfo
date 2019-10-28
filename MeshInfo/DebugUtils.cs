using UnityEngine;
using ColossalFramework.Plugins;

namespace MCSI
{
    public class DebugUtils
    {
        public const string modPrefix = "[MCSI] ";

        public static void Message(string message)
        {
            Log(message);
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, modPrefix + message);
        }

        public static void Warning(string message)
        {
            Debug.LogWarning(modPrefix + message);
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Warning, modPrefix + message);
        }

        public static void Log(string message)
        {
            Debug.Log(modPrefix + message);
        }
    }
}
