using System;
using System.Collections.Generic;
using NGDtuanh.Types;
using Object = UnityEngine.Object;

namespace NGDtuanh.MonsterLegendsLite {
    public static class EventDispatcher {
        private static readonly EnumMap<EventId, List<(Object, Action, int)>> events = new();

        private static List<(Object author, Action callback, int priority)> GetEvent(EventId id) {
            return events[id] ??= new();
        }

        public static void RegisterEvent(EventId id, Action callback, Object author, int priority = 0) {
            var targetEvents = GetEvent(id);
            targetEvents.Add((author, callback, priority));
            targetEvents.Sort(static (a, b) => a.priority.CompareTo(b.priority));
        }

        public static void UnregisterEvent(EventId id, Action callback, Object author) {
            var targetEvents = GetEvent(id);
            
            for (int i = targetEvents.Count - 1; i >= 0; --i) {
                if (targetEvents[i].author != author) continue;
                if (targetEvents[i].callback != callback) continue;
                targetEvents.RemoveAt(i);
                break;
            }
        }

        public static void PostEvent(EventId id) {
            var targetEvents = GetEvent(id);

            for (int i = targetEvents.Count - 1; i >= 0; --i)
                if (targetEvents[i].author == null)
                    targetEvents.RemoveAt(i);

            foreach (var targetEvent in targetEvents.ToArray())
                targetEvent.callback?.Invoke();
        }
    }
}