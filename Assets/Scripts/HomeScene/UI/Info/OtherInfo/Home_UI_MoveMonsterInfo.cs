using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Home_UI_MoveMonsterInfo : Home_UI_Info<Home_Monster> {
        [SerializeField, Required]
        private Home_UI_InfoBtn discardBtn;

        protected override void LoadInfoFor(Home_Monster building) {
            base.LoadInfoFor(building);
            
            discardBtn.SetCallback(() => {
                Home_SceneManager.Ins.ConfirmMoveMonster(null);
            });
        }
    }
}