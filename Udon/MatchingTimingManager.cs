using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Narazaka.VRChat.MatchingSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class MatchingTimingManager : EventEmitter
    {
        [SerializeField] MatchingManager MatchingManager;
        [SerializeField, UdonSynced, FieldChangeCallback(nameof(SessionTimeout))] float _sessionTimeout = 300f; // 5min
        public float SessionTimeout
        {
            get => _sessionTimeout;
            private set
            {
                if (_sessionTimeout == value) return;
                _sessionTimeout = value;
                NotifyEvent("_OnSessionTimeoutChanged");
            }
        }
        [SerializeField] float SessionChangeInterval = 2.5f;
        [SerializeField] public float CheckInterval = 0.5f;

        public void _SetSessionTimeout(float timeout)
        {
            if (Networking.IsOwner(gameObject))
            {
                SessionTimeout = timeout;
                RequestSerialization();
            }
        }

        public int DisplayRemainTime
        {
            get
            {
                var remainTime = RemainTime;
                if (remainTime >= SessionTimeoutWithChangeInterval - SessionChangeInterval / 2) return 0;
                return Mathf.CeilToInt(Mathf.Clamp(remainTime, 0f, SessionTimeout));
            }
        }
        float RemainTime => SessionTimeoutWithChangeInterval - PastTime;
        float PastTime => (float)((Networking.GetNetworkDateTime() - MatchingManager.SessionStartTime).TotalSeconds);

        public float SessionTimeoutWithChangeInterval => SessionTimeout + SessionChangeInterval;

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (player.isLocal)
            {
                _CheckInterval();
            }
        }

        public void _CheckInterval()
        {
            _Check();
            SendCustomEventDelayedSeconds(nameof(_CheckInterval), CheckInterval);
        }

        public void _Check()
        {
            if (!Networking.IsOwner(MatchingManager.gameObject) || MatchingManager.SessionId == -1) return;
            if (RemainTime <= 0)
            {
                MatchingManager._InitializeSession();
            }
        }
    }
}
