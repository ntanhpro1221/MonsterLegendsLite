using System.Collections.Generic;
using System.Linq;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MonsterLegendsLite {
    public class Home_SceneManager : SceneSingleton<Home_SceneManager> {
        [SerializeField, Required]
        private ChoosePlayModeWindow prefabChoosePlayModeWindow;
        
        [SerializeField, Required]
        private Button playBtn;
        
        [SerializeField, Required]
        private SettingsWindow prefabSettingsWindow;

        [SerializeField, Required]
        private Button settingsBtn;

        [SerializeField, Required]
        private Home_UI_UserLevel uiUserLevel;
        
        [SerializeField, Required]
        private Home_UI_InfoManager uiInfo;

        [SerializeField, Required]
        private Transform buildingRoot;

        private readonly Dictionary<string, Home_Habitat> habitats = new();
        private readonly Dictionary<string, Home_Farm> farms = new();
        private readonly Dictionary<string, Home_BreedingPlace> breedingPlaces = new();
        private readonly Dictionary<string, Home_Monster> monsters = new();

        public IReadOnlyDictionary<string, Home_Habitat> Habitats => habitats;
        public IReadOnlyDictionary<string, Home_Farm> Farms => farms;
        public IReadOnlyDictionary<string, Home_BreedingPlace> BreedingPlaces => breedingPlaces;
        public IReadOnlyDictionary<string, Home_Monster> Monsters => monsters;

        private Camera cam;
        public Camera Cam => cam != null ? cam : cam = Camera.main;

        private Home_Monster movingMonster;
        private Home_BreedingPlace hatchingBreedingPlace;

        protected override void Initialize() {
            base.Initialize();

            UtilFuncs.Ins.SetListener(playBtn, () => ChoosePlayModeWindow.Show(
                prefab: prefabChoosePlayModeWindow
              , navArenaScene: NavArenaScene
              , navAdventureScene: NavAdventureScene
            ));

            UtilFuncs.Ins.SetListener(settingsBtn, () => SettingsWindow.Show(
                prefab: prefabSettingsWindow
            ));
            
            uiUserLevel.Initialize();
            uiInfo.Initialize();
            
            RebuildBuildings();
            RebuildMonsters();
            
            EventDispatcher.RegisterEvent(EventId.UserBuildingListChanged, RebuildBuildings, this, 100);
            EventDispatcher.RegisterEvent(EventId.UserHabitatListChanged, RebuildHabitats, this, 100);
            EventDispatcher.RegisterEvent(EventId.UserFarmListChanged, RebuildFarms, this, 100);
            EventDispatcher.RegisterEvent(EventId.UserBreedingPlaceListChanged, RebuildBreedingPlaces, this, 100);
            EventDispatcher.RegisterEvent(EventId.UserMonsterListChanged, RebuildMonsters, this, 100);
            EventDispatcher.RegisterEvent(EventId.HomeMapChanged, SortAllBuildings, this);
        }

        private void OnDestroy() {
            EventDispatcher.UnregisterEvent(EventId.UserBuildingListChanged, RebuildBuildings, this);
            EventDispatcher.UnregisterEvent(EventId.UserHabitatListChanged, RebuildHabitats, this);
            EventDispatcher.UnregisterEvent(EventId.UserFarmListChanged, RebuildFarms, this);
            EventDispatcher.UnregisterEvent(EventId.UserBreedingPlaceListChanged, RebuildBreedingPlaces, this);
            EventDispatcher.UnregisterEvent(EventId.UserMonsterListChanged, RebuildMonsters, this);
            EventDispatcher.UnregisterEvent(EventId.HomeMapChanged, SortAllBuildings, this);
        }

        private void Start() {
            LoadBootDataThenDelete();
        }

        private void LoadBootDataThenDelete() {
            var bootData = Home_BootData.Ins;
            if (bootData == null) return;

            if (bootData.MoveMonster != null) StartMoveMonster(Monsters[bootData.MoveMonster.InsId]);
            
            if (bootData.FloatingGold != null) FloatingTextPool.Ins.ShowAtCenterScreen(FloatingTextId.GoldChange).SetTextChange(bootData.FloatingGold.Value);
            
            if (bootData.BuyHabitat != null) StartBuyHabitat(bootData.BuyHabitat.Value);
            
            if (bootData.BuyFarm != null) StartBuyFarm(bootData.BuyFarm.Value);
            
            if (bootData.BuyBreedingPlace != null) StartBuyBreedingPlace(bootData.BuyBreedingPlace.Value);
            
            if (bootData.BuyMonster != null) StartBuyMonster(bootData.BuyMonster.Value);
            
            Destroy(bootData.gameObject);

            static void StartBuyHabitat(ElementId id) {
                var ins = Instantiate(DataManager.Ins.GameLocDef.Habitats[id].PrefabHomeScene, Ins.buildingRoot);

                ins.Initialize(new HabitatInsData(id), isBuySample: true);
            }

            static void StartBuyFarm(FarmId id) {
                var ins = Instantiate(DataManager.Ins.GameLocDef.Farms[id].PrefabHomeScene, Ins.buildingRoot);

                ins.Initialize(new FarmInsData(id), isBuySample: true);
            }

            static void StartBuyBreedingPlace(BreedingPlaceId id) {
                var ins = Instantiate(DataManager.Ins.GameLocDef.BreedingPlaces[id].PrefabHomeScene, Ins.buildingRoot);

                ins.Initialize(new BreedingPlaceInsData(id), isBuySample: true);
            }

            static void StartBuyMonster(MonsterId id) {
                var ins = Instantiate(DataManager.Ins.GameLocDef.Monsters[id].PrefabHomeScene);

                ins.Initialize(new MonsterInsData(id), Home_Monster.Type.BuySample);

                Ins.StartMoveMonster(ins);
            }
        }

        private void RebuildBuildings() {
            RebuildFarms();
            RebuildHabitats();
            RebuildBreedingPlaces();
            
            SortAllBuildings();
        }
        
        private void RebuildFarms() {
            var map        = Home_MapManager.Ins;
            var gameLocDef = DataManager.Ins.GameLocDef;
            
            // Remove
            foreach (var (key, _) in farms.Where(static i =>
                i.Value == null
             || !DataManager.Ins.User.Farms.Contains(i.Value.InsDataWeak)).ToList())
                farms.Remove(key);

            // Add
            foreach (var farm in DataManager.Ins.User.Farms) {
                if (farms.ContainsKey(farm.InsId)) continue;
                
                var ins = Instantiate(gameLocDef.Farms[farm.Id].PrefabHomeScene, buildingRoot);
                farms.Add(farm.InsId, ins);

                ins.Initialize(farm, isBuySample: false);
                ins.TF.position = map.GetWorldPos(farm.Position);
            }
        }
        
        private void RebuildHabitats() {
            var map        = Home_MapManager.Ins;
            var gameLocDef = DataManager.Ins.GameLocDef;
            
            // Remove
            foreach (var (key, _) in habitats.Where(static i =>
                i.Value == null
             || !DataManager.Ins.User.Habitats.Contains(i.Value.InsData)).ToList())
                habitats.Remove(key);

            // Add
            foreach (var habitat in DataManager.Ins.User.Habitats) {
                if (habitats.ContainsKey(habitat.InsId)) continue;
                
                var ins = Instantiate(gameLocDef.Habitats[habitat.Id].PrefabHomeScene, buildingRoot);
                habitats.Add(habitat.InsId, ins);

                ins.Initialize(habitat, isBuySample: false);
                ins.TF.position = map.GetWorldPos(habitat.Position);
            }
        }

        private void RebuildBreedingPlaces() {
            var map        = Home_MapManager.Ins;
            var gameLocDef = DataManager.Ins.GameLocDef;

            // Remove
            foreach (var (key, _) in breedingPlaces.Where(static i =>
                i.Value == null
             || !DataManager.Ins.User.BreedingPlaces.Contains(i.Value.InsData)).ToList())
                breedingPlaces.Remove(key);

            // Add
            foreach (var breedingPlace in DataManager.Ins.User.BreedingPlaces) {
                if (breedingPlaces.ContainsKey(breedingPlace.InsId)) continue;

                var ins = Instantiate(gameLocDef.BreedingPlaces[breedingPlace.Id].PrefabHomeScene, buildingRoot);
                breedingPlaces.Add(breedingPlace.InsId, ins);

                ins.Initialize(breedingPlace, isBuySample: false);
                ins.TF.position = map.GetWorldPos(breedingPlace.Position);
            }
        }

        private void SortAllBuildings() {
            var order        = short.MinValue; // For visual order
            var zPos         = 1f;             // For click cast order
            var allBuildings = IEBuildings().ToList();
            var zPosDelta    = 1f / allBuildings.Count;

            allBuildings.Sort(static (a, b) => {
                var pA = a.InsDataWeak.Position;
                var pB = b.InsDataWeak.Position;
                var sA = DataManager.Ins.GetBuildingDefData(a.InsDataWeak).Size;
                var sB = DataManager.Ins.GetBuildingDefData(b.InsDataWeak).Size;

                bool aInFront = pA.x + sA.x <= pB.x || pA.y + sA.y <= pB.y;
                bool bInFront = pB.x + sB.x <= pA.x || pB.y + sB.y <= pA.y;

                if (aInFront && !bInFront) return 1;
                if (bInFront && !aInFront) return -1;

                return pB.x.CompareTo(pA.x);
            });

            foreach (var building in allBuildings) {
                building.SetIdleSortingOrder(order++);
                utils.SetPosition(building.TF, UtilFuncs.VecAxis.Z, zPos -= zPosDelta);
            }
        }

        private void RebuildMonsters() {
            var gameLocDef = DataManager.Ins.GameLocDef;
            
            // Remove
            foreach (var (key, _) in monsters.Where(static i =>
                i.Value == null
             || !DataManager.Ins.User.Monsters.Contains(i.Value.InsData)).ToList())
                monsters.Remove(key);

            // Add
            foreach (var monster in DataManager.Ins.User.Monsters) {
                if (monsters.ContainsKey(monster.InsId)) continue;
                
                var habitat = habitats[monster.Habitat];
                var ins     = Instantiate(gameLocDef.Monsters[monster.Id].PrefabHomeScene);
                monsters.Add(monster.InsId, ins);

                ins.Initialize(monster, Home_Monster.Type.Normal);
                habitat.AddMonster(ins);
            }
        }

        public void StartHatchMonster(Home_Monster target, Home_BreedingPlace fromBreedingPlace) {
            hatchingBreedingPlace = fromBreedingPlace;
            StartMoveMonster(target);
        }

        private void StartMoveMonster(Home_Monster target) {
            movingMonster = target;

            foreach (var habitat in habitats.Values) habitat.SetVisibleMoveMonsterArrow(habitat.IsCanAcceptNewMonster(target));
            uiInfo.ShowMoveMonsterInfo(target);
        }

        public void OnClicked_Building(Home_Building building, bool isActiveCollectBtn, out bool isClickCollectBtn) {
            isClickCollectBtn = false;

            if (movingMonster != null) {
                if (building is Home_Habitat habitat
                 && habitat.IsCanAcceptNewMonster(movingMonster))
                    ConfirmMoveMonster(habitat);
            } else if (isActiveCollectBtn) isClickCollectBtn = true;
            else uiInfo.ShowBuildingInfoFor(building, hideAllCurrentInfo: false);
        }

        public void ConfirmMoveMonster(Home_Habitat toHabitat) {
            if (toHabitat == null) movingMonster.OnMoveDiscarded(hatchingBreedingPlace);
            else movingMonster.OnMoveConfirmed(toHabitat, hatchingBreedingPlace);

            hatchingBreedingPlace = null;
            
            foreach (var habitat in habitats.Values) habitat.SetVisibleMoveMonsterArrow(false);
            movingMonster = null;
        }

        public void OnMove_Building(Home_Building building) {
            uiInfo.ShowMoveBuildingInfoFor(building);
        }

        public void OnClicked_Void() {
            uiInfo.TryHideCurBuildingInfo();
        }
        
        public void TryHideCurBuildingInfo() {
            uiInfo.TryHideCurBuildingInfo();
        }

        public void TryHideMoveBuildingInfo() {
            uiInfo.TryHideMoveBuildingInfo();
        }
        
        public void TryHideMoveMonsterInfo() {
            uiInfo.TryHideMoveMonsterInfo();
        }

        public void ForceShowBuildingInfo(Home_Building building) {
            uiInfo.ShowBuildingInfoFor(building, hideAllCurrentInfo: true);
        }

        public void ViewMonsterDetail(string insId) {
            MonsterDetail_BootData.InsAutoSpawn.SetData(monsters[insId].InsData);
            SceneManager.LoadScene("MonsterDetailScene");
        }

        public void NavToShopScene() {
            SceneManager.LoadScene("ShopScene");
        }

        public void NavArenaScene() {
            SceneManager.LoadScene("ArenaScene");
        }

        public void NavAdventureScene() {
            SceneManager.LoadScene("AdventureScene");
        }

        public IEnumerable<Home_Building> IEBuildings() {
            foreach (var item in habitats.Values) yield return item;
            foreach (var item in farms.Values) yield return item;
            foreach (var item in breedingPlaces.Values) yield return item;
        }
    }
}