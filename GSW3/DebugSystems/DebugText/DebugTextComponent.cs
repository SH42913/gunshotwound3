using System.Collections.Generic;
using Leopotam.Ecs;

namespace GSW3.DebugSystems.DebugText
{
#if DEBUG
    public class DebugTextComponent : IEcsAutoResetComponent
    {
        [EcsIgnoreNullCheck]
        public readonly Dictionary<string, string> DebugKeyToText = new Dictionary<string, string>();

        public void UpdateDebugText(string key, string text)
        {
            if (DebugKeyToText.ContainsKey(key))
            {
                DebugKeyToText[key] = text;
            }
            else
            {
                DebugKeyToText.Add(key, text);
            }
        }

        public void RemoveDebugText(string key)
        {
            if (DebugKeyToText.ContainsKey(key))
            {
                DebugKeyToText.Remove(key);
            }
        }

        public void Reset()
        {
            DebugKeyToText.Clear();
        }
    }
#endif
}