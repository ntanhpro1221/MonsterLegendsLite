using System;

namespace NGDtuanh.MonsterLegendsLite {
    public class NotificationWindow : PopupWindow {
        public static NotificationWindow Show(string title, string content, Action onClose = null) {
            return PopupWindowPool.Ins.Show<NotificationWindow>(PopupWindowId.Notification, title, content, onClose);
        }
    }
}