using UnityEngine;
using UnityEditor;
using System.Linq;
using Narazaka.VRChat.MatchingSystem.Runtime;

namespace Narazaka.VRChat.MatchingSystem.Editor
{
    [CustomEditor(typeof(SetSessionTimeoutHelper))]
    class SetSessionTimeoutHelperEditor : CommonPropertiesChildrenHelperEditor<SetSessionTimeout>
    {
    }
}
