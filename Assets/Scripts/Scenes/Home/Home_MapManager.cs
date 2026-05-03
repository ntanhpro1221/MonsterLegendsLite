using System.Collections.Generic;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;
using NGDtuanh.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace MonsterLegendsLite {
    public class Home_MapManager : SceneSingleton<Home_MapManager> {
        [SerializeField, Required]
        private Tilemap tileMap;

        [SerializeField, Required]
        private TilemapRenderer tileRenderer;

        private readonly Dictionary<Vector2Int, Home_Building> usedTiles = new();

        protected override void Initialize() {
            base.Initialize();
            
            SetVisibleGrid(false);
            EventDispatcher.RegisterEvent(EventId.HomeMapChanged, OnMapChanged, this);
            OnMapChanged();
        }

        private void OnDestroy() {
            EventDispatcher.UnregisterEvent(EventId.HomeMapChanged, OnMapChanged, this);
        }

        private void OnMapChanged() {
            usedTiles.Clear();
            foreach (var building in Home_SceneManager.Ins.IEBuildings())
            foreach (var point in IEAllCell(building.InsDataWeak.Position, DataManager.Ins.GetBuildingDefData(building.InsDataWeak).Size))
                usedTiles.Add(point, building);
        }

        public bool IsPlaceable(Home_Building building) {
            var pos  = GetNearestTilePos(building.TF.position);
            var size = DataManager.Ins.GetBuildingDefData(building.InsDataWeak).Size;

            foreach (var point in IEAllCell(pos, size)) {
                if (!tileMap.HasTile((Vector3Int)point)) return false;
                if (usedTiles.TryGetValue(point, out var owner) && owner != building) return false;
            }

            return true;
        }

        private IEnumerable<Vector2Int> IEAllCell(Vector2Int pos, Vector2Int size) {
            for (int x = pos.x + size.x - 1; x >= pos.x; --x)
            for (int y = pos.y + size.y - 1; y >= pos.y; --y)
                yield return new Vector2Int(x, y);
        }

        public void SetVisibleGrid(bool isOn) {
            tileRenderer.enabled = isOn;
        }

        public Vector2 GetWorldPos(Vector2Int tilePos) {
            return tileMap.GetCellCenterWorld(new Vector3Int(tilePos.x, tilePos.y, 0));
        }

        public Vector2Int GetNearestTilePos(Vector2 worldPos) {
            return (Vector2Int)tileMap.WorldToCell(worldPos);
        }

        public Vector2 RandomPointInHabitat(Vector2Int pos, Vector2Int size) {
            return tileMap.LocalToWorld(tileMap.CellToLocalInterpolated(new Vector3(
                pos.x + Random.Range(0, size.x)
              , pos.y + Random.Range(0, size.y)
              , 0)));
        }
    }
}