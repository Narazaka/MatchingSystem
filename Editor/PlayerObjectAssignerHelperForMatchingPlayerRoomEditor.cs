using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Narazaka.VRChat.MatchingSystem.Runtime;

namespace Narazaka.VRChat.MatchingSystem.Editor
{
    [CustomEditor(typeof(PlayerObjectAssignerHelperForMatchingPlayerRoom))]
    public class PlayerObjectAssignerHelperForMatchingPlayerRoomEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var helper = target as PlayerObjectAssignerHelperForMatchingPlayerRoom;
            if (helper == null) return;
            var playerRooms = helper.GetComponentsInChildren<MatchingPlayerRoom>();
            foreach (var playerRoom in playerRooms)
            {
                if (playerRoom.Manager != helper.Manager ||
                    playerRoom.Teleporter != helper.Teleporter ||
                    playerRoom.TemplateMatchingPlayer != helper.TemplateMatchingPlayer)
                {
                    var so = new SerializedObject(playerRoom);
                    so.FindProperty(nameof(playerRoom.Manager)).objectReferenceValue = helper.Manager;
                    so.FindProperty(nameof(playerRoom.Teleporter)).objectReferenceValue = helper.Teleporter;
                    so.FindProperty(nameof(playerRoom.TemplateMatchingPlayer)).objectReferenceValue = helper.TemplateMatchingPlayer;
                    so.ApplyModifiedProperties();
                }
            }
        }
    }
}
