using System.Linq;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Arena_Monster : MonoBehaviourExt {
        [field: SerializeField, Required]
        protected Arena_MonsterSharedData SharedData { get; private set; }
        
        public virtual void SetAllData(MonsterInsData insData) {
            SharedData.ContentRoot.SetActive(insData != null);
            if (insData == null) return;
            
            var defData        = DataManager.Ins.GameDef.Monsters[insData.Id];
            var locDefData     = DataManager.Ins.GameLocDef.Monsters[insData.Id];
            var rankLocData    = DataManager.Ins.GameLocDef.MonsterRanks[defData.Rank];
            var elementLocData = DataManager.Ins.GameLocDef.Elements;

            SetCustomName(defData.GetCustomNameIfPossible(insData));
            SetName(defData.Name);
            SetLevel(insData.Level);
            SetStats(defData.CalculateStats(insData));
            SetAvatar(locDefData.Avatar);
            SetRank(rankLocData.Icon);
            SetElements(defData.Elements.Select(i => elementLocData[i].ElementButton).ToArray());
        }

        public void SetCustomName(string customName) {
            SharedData.CustomNameTxt.text = customName;
        }

        public void SetName(string name) {
            SharedData.NameTxt.text = name;
        }

        public void SetLevel(int level) {
            SharedData.LevelTxt.text = level.ToString();
        }

        public void SetStats(MonsterStats<int> stats) {
            foreach (var (key, value) in SharedData.StatsTxt)
                if (value != null)
                    value.text = stats[key].ToString();
        }

        public void SetAvatar(Sprite avatar) {
            SharedData.AvatarImg.sprite = avatar;
        }

        public void SetRank(Sprite rank) {
            SharedData.RankImg.sprite = rank;
        }

        public void SetElements(params Sprite[] elementSprites) {
            SharedData.Elements.SetElements(elementSprites);
        }
    }
}