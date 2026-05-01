using System;
using System.Buffers;
using System.Collections.Generic;
using NGDtuanh.Types;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NGDtuanh.MonsterLegendsLite {
    public static class EventDispatcher {
        private readonly struct EventItem {
            public readonly Object Author;
            public readonly Action Callback;
            public readonly int Priority;

            public EventItem(
                Object author
              , Action callback
              , int priority) {
                Author   = author;
                Callback = callback;
                Priority = priority;
            }
        }

        private static readonly EnumMap<EventId, List<EventItem>> events = new();

        private static List<EventItem> GetEvent(EventId id) {
            return events[id] ??= new();
        }

        /// <param name="author">Only invoke <see cref="callback"/> if <see cref="author"/> is still valid.</param>
        /// <param name="priority">Higher <see cref="priority"/> executes first.</param>
        public static void RegisterEvent(EventId id, Action callback, Object author, int priority = 0) {
            var targetEvents = GetEvent(id);
            var newEvent     = new EventItem(author, callback, priority);

            int i = 0;
            while (i < targetEvents.Count && targetEvents[i].Priority > newEvent.Priority) ++i;
            targetEvents.Insert(i, newEvent);
        }

        public static void UnregisterEvent(EventId id, Action callback, Object author) {
            var targetEvents = GetEvent(id);

            for (int i = 0; i < targetEvents.Count; ++i) {
                if (targetEvents[i].Author != author) continue;
                if (targetEvents[i].Callback != callback) continue;
                targetEvents.RemoveAt(i);
                return;
            }
        }

        public static void PostEvent(EventId id) {
            var targetEvents = GetEvent(id);
            targetEvents.RemoveAll(static i => i.Author == null);

            var eventCount     = targetEvents.Count;
            var snapshotEvents = ArrayPool<EventItem>.Shared.Rent(eventCount);
            targetEvents.CopyTo(snapshotEvents);

            for (int i = 0; i < eventCount; ++i)
            // @formatter:off
                try { snapshotEvents[i].Callback?.Invoke(); } 
                catch (Exception e) { Debug.LogException(e); }
            // @formatter:on

            ArrayPool<EventItem>.Shared.Return(snapshotEvents, clearArray: true);
        }
    }
}