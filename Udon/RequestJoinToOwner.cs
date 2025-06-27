using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;

namespace Narazaka.VRChat.MatchingSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class RequestJoinToOwner : UdonSharpBehaviour
    {
        [SerializeField] MatchingManager MatchingManager;

        public override void Interact()
        {
            _RequestJoin();
        }

        [PublicAPI]
        public void _RequestJoin() =>
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(_Join), Networking.LocalPlayer.playerId);

        [NetworkCallable]
        public void _Join(int playerId)
        {
            if (Networking.IsOwner(MatchingManager.gameObject)) MatchingManager._Join(VRCPlayerApi.GetPlayerById(playerId));
        }
    }
}
