using System;
using System.Collections.Generic;
using NGDtuanh.Types;
using Object = UnityEngine.Object;

namespace NGDtuanh.MonsterLegendsLite {
    public static class EventDispatcher {
        private static readonly EnumMap<EventId, List<(Object, Action)>> events = new();

        private static List<(Object author, Action callback)> GetEvent(EventId id) {
            return events[id] ??= new();
        }

        public static void RegisterEvent(EventId id, Action callback, Object author) {
            GetEvent(id).Add((author, callback));
        }

        public static void UnregisterEvent(EventId id, Action callback, Object author) {
            for (int i = GetEvent(id).Count - 1; i >= 0; --i) {
                if (GetEvent(id)[i].author != author) continue;
                if (GetEvent(id)[i].callback != callback) continue;
                GetEvent(id).RemoveAt(i);
                break;
            }
        }

        public static void PostEvent(EventId id) {
            for (int i = GetEvent(id).Count - 1; i >= 0; --i) {
                if (GetEvent(id)[i].author == null) {
                    GetEvent(id).RemoveAt(i);
                    continue;
                }
                GetEvent(id)[i].callback?.Invoke();
            }
        }
    }
}