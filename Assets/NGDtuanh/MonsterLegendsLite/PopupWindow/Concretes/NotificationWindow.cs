using UnityEngine.Events;

namespace NGDtuanh.MonsterLegendsLite {
    public class NotificationWindow : PopupWindow {
        public static NotificationWindow Show(
            string title
          , string content
          , UnityAction onDoneClose = null) {
            return PopupWindowPool.Ins.Show<NotificationWindow>(PopupWindowId.Notification, title, content, onDoneClose);
        }
    }
}