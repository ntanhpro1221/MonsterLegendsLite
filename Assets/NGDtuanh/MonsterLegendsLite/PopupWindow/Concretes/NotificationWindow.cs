using System;

namespace NGDtuanh.MonsterLegendsLite {
    public class NotificationWindow : PopupWindow {
        public new static NotificationWindow Show(string title, string content, Action onClose = null) {
            return (NotificationWindow)PopupWindow.Show(title, content, onClose);
        }
    }
}