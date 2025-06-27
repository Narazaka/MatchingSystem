using UnityEngine;
using UnityEditor;
using System.Linq;
using UdonSharpEditor;

namespace Narazaka.VRChat.MatchingSystem.Editor
{
    abstract class CommonPropertiesEditor : UnityEditor.Editor
    {
        SerializedProperty MatchingManager;

        void OnEnable()
        {
            MatchingManager = serializedObject.FindProperty("MatchingManager");
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
            if (MatchingManager.objectReferenceValue == null)
            {
                MatchingManager.objectReferenceValue = FindObjectsByType<MatchingManager>(FindObjectsInactive.Include, FindObjectsSortMode.None).FirstOrDefault();
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
