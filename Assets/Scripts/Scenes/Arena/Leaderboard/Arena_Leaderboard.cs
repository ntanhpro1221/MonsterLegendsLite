using System.Collections.Generic;
using System.Linq;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class Arena_Leaderboard : MonoBehaviourExt {
        [SerializeField, Required]
        private Arena_LeaderboardItem prefabItem;

        [SerializeField, Required]
        private RectTransform itemRoot;


        [SerializeField, Required]
        private Arena_LeaderboardItem userItem;

        [SerializeField, Required]
        private int maxLength;
        
        private readonly List<Arena_LeaderboardItem> items = new();

        public void Initialize() {
            for (int i = 0; i < maxLength; ++i) items.Add(Instantiate(prefabItem, itemRoot));
            LayoutRebuilder.ForceRebuildLayoutImmediate(itemRoot);

            Rebuild();
        }

        public void Rebuild() {
            var eloSortedUsers = DataManager.Ins.GetUserListTest().OrderByDescending(i => i.Elo).ToList();
            var userData       = DataManager.Ins.UserInsData;

            for (int i = 0; i < maxLength; ++i) {
                bool validId = i < eloSortedUsers.Count;
                items[i].gameObject.SetActive(validId);
                if (validId) items[i].SetAllData(eloSortedUsers[i], i);
            } 

            userItem.SetAllData(userData, eloSortedUsers.IndexOf(userData));
        }
    }
}