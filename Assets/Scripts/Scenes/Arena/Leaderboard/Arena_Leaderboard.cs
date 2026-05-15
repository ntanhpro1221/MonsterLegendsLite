using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        private Task rebuildTask;

        private readonly List<Arena_LeaderboardItem> items = new();

        public void Initialize() {
            for (int i = 0; i < maxLength; ++i) items.Add(Instantiate(prefabItem, itemRoot));
            LayoutRebuilder.ForceRebuildLayoutImmediate(itemRoot);

            TriggerRebuild();
        }

        public void TriggerRebuild() {
            if (rebuildTask != null) return;

            rebuildTask = RebuildAsync();
        }

        private async Task RebuildAsync() {
            ToggleLoadingState(isLoading: true);

            try {
                var allUsers = await DataManager.Ins.GetAllUsersAsync();

                if (gameObject == null) return;

                var eloSortedUsers = allUsers.Values.OrderByDescending(static i => i.Elo).ToList();
                var userData       = DataManager.Ins.User;

                for (int i = 0; i < maxLength; ++i) {
                    bool validId = i < eloSortedUsers.Count;
                    items[i].gameObject.SetActive(validId);
                    if (validId) items[i].SetAllData(eloSortedUsers[i], i);
                }

                userItem.SetAllData(userData, eloSortedUsers.IndexOf(userData));
            } catch (Exception e) {
                utils.LogExceptionWithWindow(e);
            } finally {
                ToggleLoadingState(isLoading: false);

                rebuildTask = null;
            }
        }

        private void ToggleLoadingState(bool isLoading) {
            if (isLoading) LoadingIcon.Ins.Show(blockInteract: false);
            else LoadingIcon.Ins.Hide();

            itemRoot.gameObject.SetActive(!isLoading);
        }
    }
}