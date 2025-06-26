using UnityEngine;
using VRC.SDKBase;

namespace Narazaka.VRChat.MatchingSystem
{
    public class Logger
    {
        public static void Log(string cls, string subject, string message = null)
        {
            Debug.Log($"{Format(cls, subject)} {message}");
        }

        public static void Log(string cls, string subject, VRCPlayerApi player, string message = null)
        {
            Debug.Log($"{Format(cls, subject)}<[{player.playerId}]{(player.isLocal ? "(Local)" : "")}{player.displayName}> {message}");
        }

        static string Format(string cls, string subject)
        {
            return $"[MatchingSystem][{cls}.{subject}]";
        }
    }
}
