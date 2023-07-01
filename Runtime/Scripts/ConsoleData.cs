using UnityEngine;
using UnityEngine.Serialization;

namespace CompositeConsole
{
    public class ConsoleData : ScriptableObject
    {
        [FormerlySerializedAs("ShowConsoleOnError")] public bool ShowLogsOnError;
    }
}