using System;
using System.Linq;
using UnityEngine;
using VRC.SDKBase;

namespace Narazaka.VRChat.MatchingSystem.Runtime
{
    internal class RoomsHelper : MonoBehaviour, IEditorOnly
    {
        [SerializeField] internal RoomSetting[] RoomSettings;
        [SerializeField] internal Vector3 Bounds = new Vector3(20, 20, 20);
        [SerializeField] internal float BoundWallThickness = 2;
        [SerializeField] internal int RoomColCount = 10;
        [SerializeField] internal bool Centering = true;

        internal void SetRoomTransforms(Transform room, int index)
        {
            room.transform.localPosition = RoomPosition(index);
            room.transform.localRotation = Quaternion.identity;
            room.transform.localScale = Vector3.one;

            // bounds
            var top = room.transform.Find("System/Bounds/Top");
            var bottom = room.transform.Find("System/Bounds/Bottom");
            var left = room.transform.Find("System/Bounds/Left");
            var right = room.transform.Find("System/Bounds/Right");
            var front = room.transform.Find("System/Bounds/Front");
            var back = room.transform.Find("System/Bounds/Back");
            top.localScale = bottom.localScale = new Vector3(Bounds.x, BoundWallThickness, Bounds.z);
            left.localScale = right.localScale = new Vector3(BoundWallThickness, Bounds.y, Bounds.z);
            front.localScale = back.localScale = new Vector3(Bounds.x, Bounds.y, BoundWallThickness);
            top.localPosition = new Vector3(0, Bounds.y / 2, 0);
            bottom.localPosition = new Vector3(0, -Bounds.y / 2, 0);
            left.localPosition = new Vector3(-Bounds.x / 2, 0, 0);
            right.localPosition = new Vector3(Bounds.x / 2, 0, 0);
            front.localPosition = new Vector3(0, 0, -Bounds.z / 2);
            back.localPosition = new Vector3(0, 0, Bounds.z / 2);
            top.localRotation = bottom.localRotation = left.localRotation = right.localRotation = front.localRotation = back.localRotation = Quaternion.identity;
        }

        Vector3 RoomPosition(int index)
        {
            var totalCount = RoomSettings.Sum(setting => setting.RoomCount);
            var colCount = Mathf.Min(totalCount, RoomColCount);
            var rowCount = Mathf.CeilToInt((float)totalCount / RoomColCount);
            int col = index % RoomColCount;
            int row = index / RoomColCount;
            var pos = new Vector3(col * Bounds.x, 0, row * Bounds.z);
            if (Centering)
            {
                var center = new Vector3(-(colCount - 1) / 2f * Bounds.x, 0, -(rowCount - 1) / 2f * Bounds.z);
                pos += center;
            }
            return pos;
        }

        internal void SetOcclusionMeshVisible(Transform room, bool visible)
        {
            var top = room.transform.Find("System/Bounds/Top");
            var bottom = room.transform.Find("System/Bounds/Bottom");
            var left = room.transform.Find("System/Bounds/Left");
            var right = room.transform.Find("System/Bounds/Right");
            var front = room.transform.Find("System/Bounds/Front");
            var back = room.transform.Find("System/Bounds/Back");
            top.GetComponent<MeshRenderer>().enabled = visible;
            bottom.GetComponent<MeshRenderer>().enabled = visible;
            left.GetComponent<MeshRenderer>().enabled = visible;
            right.GetComponent<MeshRenderer>().enabled = visible;
            front.GetComponent<MeshRenderer>().enabled = visible;
            back.GetComponent<MeshRenderer>().enabled = visible;
        }

        [Serializable]
        internal class RoomSetting
        {
            [SerializeField] internal GameObject RoomPrefab;
            [SerializeField] internal int RoomCount = 10;
        }
    }
}
