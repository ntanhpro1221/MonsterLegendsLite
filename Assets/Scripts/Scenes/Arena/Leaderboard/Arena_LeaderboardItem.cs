using System;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class Arena_LeaderboardItem : MonoBehaviourExt {
        [Serializable]
        public class RankBasedElement {
            [SerializeField, Required, PreviewField]
            public Sprite background;

            [SerializeField, Required, PreviewField]
            public Sprite rankBg;
        }

        [SerializeField, Required]
        private Image background;

        [SerializeField, Required]
        private UI_ImageAspect rankBg;
        
        [SerializeField, Required]
        private RankBasedElement[] rankBasedElements;

        [SerializeField, Required]
        private TextMeshProUGUI rankTxt;

        [SerializeField, Required]
        private Image avatarImg;

        [SerializeField, Required]
        private TextMeshProUGUI nameTxt;

        [SerializeField, Required]
        private TextMeshProUGUI eloTxt;

        public void SetAllData(UserInsData user, int eloSortedId) {
            SetRank(eloSortedId);
            // SetAvatar(null);
            SetName(user.Name);
            SetElo(user.Elo);
        }

        public void SetRank(int eloSortedId) {
            rankTxt.text = (eloSortedId + 1).ToString();

            for (int i = 0; i < rankBasedElements.Length; ++i) {
                if (i != eloSortedId && i != rankBasedElements.Length - 1) continue;

                background.sprite = rankBasedElements[i].background;
                rankBg.SetSprite(rankBasedElements[i].rankBg);
                break;
            }
        }

        public void SetAvatar(Sprite avatar) {
            avatarImg.sprite = avatar;
        }

        public void SetName(string name) {
            nameTxt.text = name;
        }

        public void SetElo(int elo) {
            eloTxt.text = elo.ToString();
        }
    }
}