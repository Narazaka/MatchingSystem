using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Narazaka.VRChat.MatchingSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Teleporter : UdonSharpBehaviour
    {
        // TODO: yasashii teleport
        [SerializeField] Transform Control;
        [SerializeField] Transform Information;

        internal void Respawn()
        {
            Networking.LocalPlayer.Respawn();
        }

        internal void TeleportTo(MatchingRoom room, int spawnPointIndex)
        {
            var spawnPoint = room.SpawnPoints[spawnPointIndex];
            Networking.LocalPlayer.TeleportTo(spawnPoint.position, spawnPoint.rotation);
            Control.position = room.ControlPosition.position;
            Control.rotation = room.ControlPosition.rotation;
            if (Information == null) return;
            Information.position = room.InformationPosition.position;
            Information.rotation = room.InformationPosition.rotation;
        }
    }
}
