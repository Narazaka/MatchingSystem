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
            Debug.Log($"{Format(cls, subject)}[{Player(player)}] {message}");
        }

        public static string Player(VRCPlayerApi player)
        {
            if (player == null)
            {
                return "(null player)";
            }
            return $"|{player.playerId}|{player.displayName}{(player.isLocal ? "(Local)" : "")}";
        }

        static string Format(string cls, string subject)
        {
            return $"[MatchingSystem][{cls}.{subject}]";
        }
    }
}
