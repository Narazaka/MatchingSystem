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
        [SerializeField] MatchingTimingManager MatchingTimingManager;
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

        public void _ToggleReserveLeave()
        {
            MatchingPlayer._ToggleReserveLeave();
        }

        public void _ToggleReserveRemain()
        {
            MatchingPlayer._ToggleReserveRemain();
        }

        void Update()
        {
            var remain = MatchingTimingManager.DisplayRemainTime;
            var minutes = Mathf.FloorToInt(remain / 60f);
            var seconds = Mathf.FloorToInt(remain % 60f);
            TimeText.text = $"{minutes:00}:{seconds:00}";
            RemainButton.color = MatchingPlayer.ReserveRemain ? Color.green : Color.white;
            LeaveButton.color = MatchingPlayer.ReserveLeave ? Color.red : Color.white;
        }
    }
}
