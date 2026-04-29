using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonsterLegendsLite {
    public class Home_MapManager : SceneSingleton<Home_MapManager> {
        [SerializeField, Required]
        private Grid grid;

        public Vector2 GetWorldPos(Vector2Int tilePos) {
            return grid.GetCellCenterWorld(new Vector3Int(tilePos.x, tilePos.y, 0));
        }

        public Vector2 RandomPointInHabitat(Vector2Int pos, Vector2Int size) {
            return grid.LocalToWorld(grid.CellToLocalInterpolated(new Vector3(
                pos.x + Random.Range(0, size.x)
              , pos.y + Random.Range(0, size.y)
              , 0)));
        }
    }
}