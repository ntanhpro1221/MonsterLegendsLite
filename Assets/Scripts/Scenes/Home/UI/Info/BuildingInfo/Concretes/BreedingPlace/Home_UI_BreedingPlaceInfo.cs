using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class Home_UI_BreedingPlaceInfo : Home_UI_BuildingInfo {
        [SerializeField, Required]
        private BreedWindow prefabBreedWindow;
        
        [SerializeField, Required]
        private HatchWindow prefabHatchWindow;

        [SerializeField, Required]
        private EnumMap<Home_BreedingPlace.State, Home_UI_InfoBtn> buttons;

        protected override void LoadInfoFor(Home_Building building) {
            base.LoadInfoFor(building);
            
            var breedingPlace = building.To<Home_BreedingPlace>();
            
            buttons[Home_BreedingPlace.State.Normal].SetCallback(() => BreedWindow.Show(
                prefab: prefabBreedWindow
              , place: breedingPlace));
            
            var breedingBtn = buttons[Home_BreedingPlace.State.Breeding];
            breedingBtn.SetCallback(null);
            breedingBtn.SetUpdate(() => {
                var elapsed = SerTimestamp.DeltaSeconds(breedingPlace.InsData.CurBreeding.StartedAt, SerTimestamp.Now());
                var total   = breedingPlace.InsData.CurBreeding.Output.Config.Duration;

                breedingBtn.SetInfo(utils.ToStr_TimeAmount(total - elapsed));
            });
            
            buttons[Home_BreedingPlace.State.DoneBreed].SetCallback(() => HatchWindow.Show(
                prefab: prefabHatchWindow
              , place: breedingPlace));
            
            UpdateState(breedingPlace.CurState);
            breedingPlace.onStateChanged += UpdateState;
        }

        public override void UnloadInfo() {
            CurTarget.To<Home_BreedingPlace>().onStateChanged += UpdateState;

            base.UnloadInfo();
        }

        private void UpdateState(Home_BreedingPlace.State state) {
            foreach (var (key, button) in buttons) button.gameObject.SetActive(key == state);
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(RectTF);
        }
    }
}