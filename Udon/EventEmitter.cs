using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Narazaka.VRChat.MatchingSystem
{
    public abstract class EventEmitter : UdonSharpBehaviour
    {
        UdonBehaviour[] _eventListeners = new UdonBehaviour[0];

        public void AddEventListenter(UdonBehaviour listener)
        {
            if (listener == null) return;
            if (_eventListeners == null || _eventListeners.Length == 0)
            {
                _eventListeners = new UdonBehaviour[] { listener };
                return;
            }
            if (Array.IndexOf(_eventListeners, listener) >= 0) return; // already added
            var newListeners = new UdonBehaviour[_eventListeners.Length + 1];
            Array.Copy(_eventListeners, newListeners, _eventListeners.Length);
            newListeners[_eventListeners.Length] = listener;
            _eventListeners = newListeners;
        }

        public void RemoveEventListener(UdonBehaviour listener)
        {
            if (listener == null || _eventListeners == null || _eventListeners.Length == 0) return;
            var index = Array.IndexOf(_eventListeners, listener);
            if (index < 0) return; // not found
            if (_eventListeners.Length == 1)
            {
                _eventListeners = new UdonBehaviour[0];
                return;
            }
            var newListeners = new UdonBehaviour[_eventListeners.Length - 1];
            if (index > 0) Array.Copy(_eventListeners, 0, newListeners, 0, index);
            if (index < _eventListeners.Length - 1) Array.Copy(_eventListeners, index + 1, newListeners, index, _eventListeners.Length - index - 1);
            _eventListeners = newListeners;
        }

        protected void NotifyEvent(string name)
        {
            if (_eventListeners == null || _eventListeners.Length == 0) return;
            foreach (var listener in _eventListeners)
            {
                if (listener == null) continue;
                listener.SendCustomEvent(name);
            }
        }
    }
}
