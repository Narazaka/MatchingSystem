using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Narazaka.VRChat.MatchingSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    class ActiveGraphic : UdonSharpBehaviour
    {
        [SerializeField] Graphic graphic;
        [SerializeField] Color Active;
        [SerializeField] Color Inactive;

        void OnEnable()
        {
            if (graphic == null) return;
            graphic.color = Active;
        }

        void OnDisable()
        {
            if (graphic == null) return;
            graphic.color = Inactive;
        }
    }
}
