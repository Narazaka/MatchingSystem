using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Narazaka.VRChat.MatchingSystem.Editor
{
    [CustomEditor(typeof(Runtime.RoomsHelper))]
    public class RoomsHelperEditor : UnityEditor.Editor
    {
        bool occlusionEnabled = false;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUI.BeginChangeCheck();
            occlusionEnabled = EditorGUILayout.Toggle("Occlusion Enabled", occlusionEnabled);
            if (EditorGUI.EndChangeCheck())
            {
                ChangeOcclusion();
            }

            if (GUILayout.Button("Regenerate Rooms"))
            {
                GenerateRooms();
            }
        }

        void GenerateRooms()
        {
            var roomsHelper = (Runtime.RoomsHelper)target;
            var toDeletes = new List<GameObject>();
            // delete all children
            foreach (Transform child in roomsHelper.transform)
            {
                toDeletes.Add(child.gameObject);
            }
            foreach (var obj in toDeletes)
            {
                Undo.DestroyObjectImmediate(obj);
            }
            // generate rooms
            var count = 0;
            var rooms = new List<MatchingRoom>();
            for (int i = 0; i < roomsHelper.RoomSettings.Length; i++)
            {
                var roomSetting = roomsHelper.RoomSettings[i];
                for (int j = 0; j < roomSetting.RoomCount; j++)
                {
                    var room = PrefabUtility.InstantiatePrefab(roomSetting.RoomPrefab, roomsHelper.transform) as GameObject;
                    room.name = $"{roomSetting.RoomPrefab.name}_{j}";
                    roomsHelper.SetRoomTransforms(room.transform, count);
                    Undo.RegisterCreatedObjectUndo(room, "Create Room");
                    rooms.Add(room.GetComponent<MatchingRoom>());
                    count++;
                }
            }
            var manager = roomsHelper.GetComponent<MatchingManager>();
            var so = new SerializedObject(manager);
            so.UpdateIfRequiredOrScript();
            var roomsProp = so.FindProperty(nameof(MatchingManager.Rooms));
            roomsProp.arraySize = count;
            for (int i = 0; i < count; i++)
            {
                roomsProp.GetArrayElementAtIndex(i).objectReferenceValue = rooms[i];
            }
            so.ApplyModifiedProperties();
            ChangeOcclusion();
        }

        void ChangeOcclusion()
        {
            var roomsHelper = (Runtime.RoomsHelper)target;
            var rooms = roomsHelper.GetComponentsInChildren<MatchingRoom>();
            foreach (var room in rooms)
            {
                roomsHelper.SetOcclusionMeshVisible(room.transform, occlusionEnabled);
            }
        }
    }
}
