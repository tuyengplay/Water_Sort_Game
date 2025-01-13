namespace UnityEngine
{
    public static class EDebug
    {
        public static void LogError(string _log)
        {
            Debug.LogError($"<color=#FF0000><b>[Tuyen] ► </b></color>{_log}");
        }

        public static void LogWarning(string _log)
        {
            Debug.LogWarning($"<color=#FFFF00><b>[Tuyen] ► </b></color>{_log}");
        }

        public static void Log(string _log)
        {
            Debug.Log($"<color=#00FFFF><b>[Tuyen] ► </b></color>{_log}");
        }
    }
}