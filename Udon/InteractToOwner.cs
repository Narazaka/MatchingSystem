using UdonSharp;
using UnityEngine;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;

namespace Narazaka.VRChat.MatchingSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    class InteractToOwner : UdonSharpBehaviour
    {
        [SerializeField] MatchingManager MatchingManager;

        public override void Interact()
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(_Join), Networking.LocalPlayer.playerId);
        }

        [NetworkCallable]
        public void _Join(int playerId)
        {
            if (Networking.IsOwner(MatchingManager.gameObject)) MatchingManager.Join(VRCPlayerApi.GetPlayerById(playerId));
        }
    }
}
