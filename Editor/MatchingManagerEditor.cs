using UnityEngine;
using UnityEditor;
using UdonSharpEditor;
using System.Linq;

namespace Narazaka.VRChat.MatchingSystem.Editor
{
    [CustomEditor(typeof(MatchingManager))]
    public class MatchingManagerEditor : UnityEditor.Editor
    {
        SerializedProperty SessionTimeout;
        SerializedProperty Assigner;
        SerializedProperty Rooms;

        void OnEnable()
        {
            SessionTimeout = serializedObject.FindProperty(nameof(MatchingManager.SessionTimeout));
            Assigner = serializedObject.FindProperty(nameof(MatchingManager.Assigner));
            Rooms = serializedObject.FindProperty(nameof(MatchingManager.Rooms));
        }

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            serializedObject.UpdateIfRequiredOrScript();
            EditorGUILayout.PropertyField(SessionTimeout);
            EditorGUILayout.PropertyField(Assigner);
            EditorGUILayout.PropertyField(Rooms, true);
            var matchingManager = (MatchingManager)target;
            var rooms = matchingManager.GetComponentsInChildren<MatchingRoom>();
            if (!matchingManager.Rooms.SequenceEqual(rooms))
            {
                Rooms.ClearArray();
                Rooms.arraySize = rooms.Length;
                for (var i = 0; i < rooms.Length; i++)
                {
                    Rooms.GetArrayElementAtIndex(i).objectReferenceValue = rooms[i];
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
