using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Narazaka.VRChat.MatchingSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class MatchingRoom : UdonSharpBehaviour
    {
        [SerializeField] internal string RoomName;
        [SerializeField] internal Transform[] SpawnPoints; // 2
        [SerializeField] internal Transform ControlPosition;
        [SerializeField] internal Transform InformationPosition;
        [SerializeField] internal Transform[] ResetTransforms;

        Vector3[] ResetPositions;
        Quaternion[] ResetRotations;

        void OnEnable()
        {
            ResetPositions = new Vector3[ResetTransforms.Length];
            ResetRotations = new Quaternion[ResetTransforms.Length];
            for (var i = 0; i < ResetTransforms.Length; i++)
            {
                ResetPositions[i] = ResetTransforms[i].position;
                ResetRotations[i] = ResetTransforms[i].rotation;
            }
        }

        public void ResetRoom(VRCPlayerApi owner)
        {
            for (var i = 0; i < ResetTransforms.Length; i++)
            {
                Networking.SetOwner(owner, ResetTransforms[i].gameObject);
                ResetTransforms[i].position = ResetPositions[i];
                ResetTransforms[i].rotation = ResetRotations[i];
            }
        }
    }
}
