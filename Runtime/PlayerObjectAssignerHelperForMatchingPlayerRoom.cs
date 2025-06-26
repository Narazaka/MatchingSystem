using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using VRC.SDKBase;

[assembly: InternalsVisibleTo("Narazaka.VRChat.MatchingSystem.Editor")]

namespace Narazaka.VRChat.MatchingSystem.Runtime
{
    public class PlayerObjectAssignerHelperForMatchingPlayerRoom : MonoBehaviour, IEditorOnly
    {
        [SerializeField] internal MatchingManager Manager;
        [SerializeField] internal Teleporter Teleporter;
        [SerializeField] internal MatchingPlayer TemplateMatchingPlayer;
    }
}
