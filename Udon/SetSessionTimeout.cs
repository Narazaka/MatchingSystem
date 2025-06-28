using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Narazaka.VRChat.MatchingSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SetSessionTimeout : UdonSharpBehaviour
    {
        [SerializeField] MatchingTimingManager MatchingTimingManager;
        [SerializeField] float SessionTimeout = 300f;
        [SerializeField] GameObject ActiveOnValueMatched;

        void OnEnable()
        {
            MatchingTimingManager.AddEventListenter(GetComponent<UdonBehaviour>());
            _OnSessionTimeoutChanged();
        }

        public override void Interact()
        {
            _SetSessionTimeout();
        }

        public void _SetSessionTimeout()
        {
            MatchingTimingManager._SetSessionTimeout(SessionTimeout);
        }

        /// <summary>
        /// by MatchingTimingManager local event
        /// </summary>
        public void _OnSessionTimeoutChanged()
        {
            if (ActiveOnValueMatched != null) ActiveOnValueMatched.SetActive(MatchingTimingManager.SessionTimeout == SessionTimeout);
        }
    }
}
