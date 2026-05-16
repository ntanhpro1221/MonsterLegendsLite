using System;
using MonsterLegendsLite.Data;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace MonsterLegendsLite {
    public class Home_BreedingPlace : Home_Building<BreedingPlaceInsData> {
        public enum State {
            Normal
          , Breeding
          , DoneBreed
        }

        [ShowInInspector, ReadOnly, PropertyOrder(-99)]
        public State CurState { get; private set; }

        [SerializeField, Required]
        private Home_ProgressBar progressBar;
        
        public Home_Monster HatchSample { get; private set; }

        public event UnityAction<State> onStateChanged;

        protected override void Initialize(BreedingPlaceInsData insData, bool isBuySample) {
            base.Initialize(insData, isBuySample);

            progressBar.Hide();

            if (!isBuySample) {
                ChangeState(
                    insData.CurBreeding == null
                        ? State.Normal
                        : IsDoneBreed(out _)
                            ? State.DoneBreed
                            : State.Breeding);
            }
        }

        public void ChangeState(State newState) {
            if (CurState == newState) return;
            CurState = newState;
            
            switch (newState) {
                case State.Normal:
                    progressBar.Hide();
                    HatchSample = null;
                    break;

                case State.Breeding:
                    progressBar.Show();
                    HatchSample = null;
                    break;

                case State.DoneBreed:
                    progressBar.Hide();
                    HatchSample = Instantiate(DataManager.Ins.GameLocDef.Monsters[InsData.CurBreeding.Output.Monster].PrefabHomeScene);
                    HatchSample.Initialize(new MonsterInsData(InsData.CurBreeding.Output.Monster), Home_Monster.Type.HatchSample);
                    break;

                default: throw new ArgumentOutOfRangeException();
            }
            
            onStateChanged?.Invoke(newState);
        }

        private void Update() {
            if (CurState is State.Breeding) {
                if (IsDoneBreed(out var elapsed)) ChangeState(State.DoneBreed);
                else progressBar.SetProgress_Time(InsData.CurBreeding.Output.Config.Duration, elapsed);
            }
        }

        public bool IsDoneBreed(out long elapsed) {
            elapsed = SerTimestamp.DeltaSeconds(InsData.CurBreeding.StartedAt, SerTimestamp.Now());
            return elapsed >= InsData.CurBreeding.Output.Config.Duration;
        }

        protected override void UpdateData_BuyBuilding(Vector2Int pos, out int cost, out string insId) {
            DataManager.Ins.UpdateData_BuyBreedingPlace(InsData.Id, pos, out cost, out insId);
        }

        protected override Home_Building GetBuildingFromInsId(string insId) {
            return Home_SceneManager.Ins.BreedingPlaces[insId];
        }
    }
}