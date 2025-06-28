using UnityEngine;
using UnityEditor;
using Narazaka.VRChat.MatchingSystem.Runtime;
using UdonSharp;

namespace Narazaka.VRChat.MatchingSystem.Editor
{
    abstract class CommonPropertiesChildrenHelperEditor<T> : CommonPropertiesEditor where T : UdonSharpBehaviour
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            SetupCommonPropertiesToChildren();
        }

        protected void SetupCommonPropertiesToChildren()
        {
            var children = (target as CommonPropertiesChildrenHelper<T>).GetComponentsInChildren<T>(true);
            var matchingManager = MatchingManager == null ? null : MatchingManager.objectReferenceValue;
            var matchingTimingManager = MatchingTimingManager == null ? null : MatchingTimingManager.objectReferenceValue;
            foreach (var child in children)
            {
                var so = new SerializedObject(child);
                so.UpdateIfRequiredOrScript();
                var childMatchingManager = so.FindProperty("MatchingManager");
                if (childMatchingManager != null && childMatchingManager.objectReferenceValue == null)
                {
                    childMatchingManager.objectReferenceValue = matchingManager;
                }
                var childMatchingTimingManager = so.FindProperty("MatchingTimingManager");
                if (childMatchingTimingManager != null && childMatchingTimingManager.objectReferenceValue == null)
                {
                    childMatchingTimingManager.objectReferenceValue = matchingTimingManager;
                }
                so.ApplyModifiedProperties();
            }
        }
    }
}
