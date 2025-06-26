using JetBrains.Annotations;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Narazaka.VRChat.MatchingSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ControlUI : UdonSharpBehaviour
    {
        [SerializeField] MatchingManager MatchingManager;
        [SerializeField] MatchingPlayer TemplateMatchingPlayer;
        [SerializeField] TextMeshProUGUI TimeText;
        [SerializeField] Image RemainButton;
        [SerializeField] Image LeaveButton;

        MatchingPlayer _matchingPlayer;
        MatchingPlayer MatchingPlayer
        {
            get
            {
                if (_matchingPlayer != null) return _matchingPlayer;
                _matchingPlayer = (MatchingPlayer)Networking.FindComponentInPlayerObjects(Networking.LocalPlayer, TemplateMatchingPlayer);
                return _matchingPlayer;
            }
        }

        /// <summary>
        /// by ui
        /// </summary>
        public void _ToggleReserveLeave()
        {
            MatchingPlayer._ToggleReserveLeave();
        }

        /// <summary>
        /// by ui
        /// </summary>
        public void _ToggleReserveRemain()
        {
            MatchingPlayer._ToggleReserveRemain();
        }

        void Update()
        {
            var now = Networking.GetNetworkDateTime();
            var past = now - MatchingManager.SessionStartTime;
            var remain = MatchingManager.SessionTimeout - (float)past.TotalSeconds;
            if (remain <= 0.5f)
            {
                TryInitializeSession();
            }
            if (remain < 0f)
            {
                remain = 0f;
            }
            var minutes = Mathf.FloorToInt(remain / 60f);
            var seconds = Mathf.FloorToInt(remain % 60f);
            TimeText.text = $"{minutes:00}:{seconds:00}";
            RemainButton.color = MatchingPlayer.ReserveLeave ? Color.red : Color.white;
            LeaveButton.color = MatchingPlayer.ReserveRemain ? Color.green : Color.white;
        }

        void TryInitializeSession()
        {
            if (Networking.IsOwner(MatchingManager.gameObject))
            {
                MatchingManager._TryInitializeSession();
            }
        }
    }
}
