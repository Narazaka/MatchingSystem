using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Narazaka.VRChat.MatchingSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    class ColliderOwnerTrigger : UdonSharpBehaviour
    {
        [SerializeField] MatchingManager MatchingManager;

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (Networking.IsOwner(MatchingManager.gameObject)) MatchingManager._Leave(player);
        }
    }
}
