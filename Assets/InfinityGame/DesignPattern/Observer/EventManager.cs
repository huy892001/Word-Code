using System;
using System.Collections.Generic;
using UnityEngine;

namespace InfinityGame.DesignPattern.Observer
{
    /// <summary>
    /// A type-safe, lightweight EventManager with priority support.
    /// Usage:
    /// 1. Define event struct: public struct PlayerDiedEvent { public int Level; }
    /// 2. Subscribe: EventManager.AddListener<PlayerDiedEvent>(OnPlayerDied);
    /// 3. Subscribe with priority: EventManager.AddListener<PlayerDiedEvent>(OnPlayerDied, priority: 10);
    /// 4. Publish: EventManager.Broadcast(new PlayerDiedEvent { Level = 5 });
    /// 
    /// Priority: số càng cao → gọi trước. Default = 0.
    /// </summary>
    public static class EventManager
    {
        private struct ListenerEntry
        {
            public Delegate Callback;
            public int Priority;
        }

        private static readonly Dictionary<Type, List<ListenerEntry>> _events = new();

        /// <summary>
        /// Đăng ký listener với priority. Priority cao hơn được gọi trước.
        /// </summary>
        public static void AddListener<T>(Action<T> listener, int priority = 0)
        {
            var type = typeof(T);
            if (!_events.TryGetValue(type, out var list))
            {
                list = new List<ListenerEntry>();
                _events[type] = list;
            }

            list.Add(new ListenerEntry { Callback = listener, Priority = priority });

            // Sort descending: priority cao → gọi trước
            list.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }

        public static void RemoveListener<T>(Action<T> listener)
        {
            var type = typeof(T);
            if (_events.TryGetValue(type, out var list))
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (list[i].Callback.Equals(listener))
                    {
                        list.RemoveAt(i);
                        break;
                    }
                }

                if (list.Count == 0)
                {
                    _events.Remove(type);
                }
            }
        }

        public static void Broadcast<T>(T eventData)
        {
            var type = typeof(T);
            if (_events.TryGetValue(type, out var list))
            {
                // Iterate trên bản copy để tránh lỗi nếu listener add/remove trong callback
                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    if (list[i].Callback is Action<T> callback)
                    {
                        try
                        {
                            callback.Invoke(eventData);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"[EventManager] Error invoking event {type.Name}: {ex}");
                            Debug.LogException(ex);
                        }
                    }
                }
            }
        }

        public static void ClearAll()
        {
            _events.Clear();
        }
    }
}