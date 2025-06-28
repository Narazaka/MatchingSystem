using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Narazaka.VRChat.MatchingSystem.Runtime
{
    public class SetSessionTimeoutHelper : CommonPropertiesChildrenHelper<SetSessionTimeout>, IEditorOnly
    {
        [SerializeField] MatchingTimingManager MatchingTimingManager;
    }
}
