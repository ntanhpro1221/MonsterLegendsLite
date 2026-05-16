using MonsterLegendsLite.Data;
using NGDtuanh.Types;

namespace MonsterLegendsLite {
    public class Home_BootData : Singleton<Home_BootData> {
        public MonsterInsData MoveMonster { get; private set; }
        public int? FloatingGold { get; private set; }
        public ElementId? BuyHabitat { get; private set; }
        public FarmId? BuyFarm { get; private set; }
        public BreedingPlaceId? BuyBreedingPlace { get; private set; }
        public MonsterId? BuyMonster { get; private set; }

        public void SetData_MoveMonster(MonsterInsData monster) {
            MoveMonster = monster;
        }

        public void SetData_FloatingGold(int gold) {
            FloatingGold = gold;
        }

        public void SetData_BuyHabitat(ElementId habitat) {
            BuyHabitat = habitat;
        }

        public void SetData_BuyFarm(FarmId farm) {
            BuyFarm = farm;
        }

        public void SetData_BuyBreedingPlace(BreedingPlaceId breedingPlace) {
            BuyBreedingPlace = breedingPlace;
        }

        public void SetData_BuyMonster(MonsterId monster) {
            BuyMonster = monster;
        }
    }
}