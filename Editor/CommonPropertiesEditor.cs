using UnityEngine;
using UnityEditor;
using System.Linq;
using UdonSharpEditor;

namespace Narazaka.VRChat.MatchingSystem.Editor
{
    abstract class CommonPropertiesEditor : UnityEditor.Editor
    {
        SerializedProperty MatchingManager;
        SerializedProperty MatchingTimingManager;

        void OnEnable()
        {
            MatchingManager = serializedObject.FindProperty("MatchingManager");
            MatchingTimingManager = serializedObject.FindProperty("MatchingTimingManager");
        }

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            base.OnInspectorGUI();
            SetupCommonProperties();
        }

        protected void SetupCommonProperties()
        {
            serializedObject.UpdateIfRequiredOrScript();
            if (MatchingManager != null && MatchingManager.objectReferenceValue == null)
            {
                MatchingManager.objectReferenceValue = FindObjectsByType<MatchingManager>(FindObjectsInactive.Include, FindObjectsSortMode.None).FirstOrDefault();
                serializedObject.ApplyModifiedProperties();
            }
            if (MatchingTimingManager != null && MatchingTimingManager.objectReferenceValue == null)
            {
                MatchingTimingManager.objectReferenceValue = FindObjectsByType<MatchingTimingManager>(FindObjectsInactive.Include, FindObjectsSortMode.None).FirstOrDefault();
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
