using NGDtuanh.MonsterLegends;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MonsterLegendsLite {
    public class Home_MapManager : Singleton<Home_MapManager> {
        [SerializeField, Required]
        private Tilemap tilemap;

        public Vector2 GetWorldPos(Vector2Int tilePos) {
            return tilemap.CellToWorld(new Vector3Int(tilePos.x, tilePos.y, 0));
        }

        public Vector2 RandomPointInHabitat(Vector2Int pos, Vector2Int size) {
            return tilemap.LocalToWorld(tilemap.CellToLocalInterpolated(new Vector3(
                pos.x + Random.Range(0, size.x)
              , pos.y + Random.Range(0, size.y)
              , 0)));
        }
    }
}