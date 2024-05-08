using System.Collections.Generic;
using System.Linq;
using Gangs.Abilities;
using Gangs.Abilities.Structs;
using Gangs.Grid;
using UnityEngine;

namespace Gangs.Battle.UI
{
    public class MoveRangeLine : MonoBehaviour {
        public float lineWidth = 0.05f;
        public float yOffset = 0.05f;
        public float emissionIntensity = 0.5f;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        public void DrawMovementRangeBoundary(List<TargetTiles> movementRangeTiles) {
            foreach (var movementRangeTile in movementRangeTiles) {
                var excludeTiles = movementRangeTiles.Take(movementRangeTile.Cost).ToList();
                var mergedExcludeTiles = new List<Tile>();
                foreach (var excludeTile in excludeTiles) {
                    mergedExcludeTiles.AddRange(excludeTile.Tiles);
                }
                DrawRangeLines(movementRangeTile.Tiles, movementRangeTile.Cost, mergedExcludeTiles);
            }
        }
        
        public void DestroyAllLines() {
            var lineRenderers = GetComponentsInChildren<LineRenderer>();
            foreach (var lineRenderer in lineRenderers) {
                Destroy(lineRenderer.gameObject);
            }
        }

        private void DrawRangeLines(List<Tile> rangeTiles, int index, List<Tile> excludeTiles = null) {
            foreach (var tile in rangeTiles) {
                Vector2Int[] directions = {
                    Vector2Int.up, Vector2Int.right,
                    Vector2Int.down, Vector2Int.left
                };

                var tileWorldPos = new Vector3(tile.GridPosition.X, tile.GridPosition.Y + yOffset, tile.GridPosition.Z);

                foreach (var dir in directions) {
                    var neighborPos = new GridPosition(tile.GridPosition.X + dir.x, tile.GridPosition.Y, tile.GridPosition.Z + dir.y);
                    if (!rangeTiles.Exists(t => t.GridPosition == neighborPos) &&
                        (excludeTiles == null || !excludeTiles.Exists(t => t.GridPosition == neighborPos)))
                    {
                        var startPoint = tileWorldPos + new Vector3(dir.x * 0.5f, 0, dir.y * 0.5f);
                        Vector3 endPoint;

                        if (dir == Vector2Int.up || dir == Vector2Int.down)
                        {
                            endPoint = startPoint + new Vector3(0.5f + (lineWidth / 2), 0, 0);
                            startPoint -= new Vector3(0.5f + (lineWidth / 2), 0, 0);
                        }
                        else
                        {
                            endPoint = startPoint + new Vector3(0, 0, 0.5f + (lineWidth / 2));
                            startPoint -= new Vector3(0, 0, 0.5f + (lineWidth / 2));
                        }

                        CreateLineSegment(startPoint, endPoint, GetLineColor(index), -index);
                    }
                }
            }
        }

        private void CreateLineSegment(Vector3 startPoint, Vector3 endPoint, Color lineColor, int sortingOrder) {
            var lineObject = new GameObject("MovementRangeBoundarySegment");
            lineObject.transform.SetParent(transform);

            var lineRenderer = lineObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Particles/Standard Unlit")) {
                color = lineColor
            };
            lineRenderer.material.EnableKeyword("_EMISSION");
            lineRenderer.material.SetColor(EmissionColor, lineColor * emissionIntensity);
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startPoint);
            lineRenderer.SetPosition(1, endPoint);
            lineRenderer.receiveShadows = false;
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    
            lineRenderer.sortingOrder = sortingOrder;  // Add this line
        }
        
        private static Color GetLineColor(int index) {
            var colors = new Dictionary<int, Color> {
                {0, new Color(128 / 255f, 210 / 255f, 196 / 255f, 1f)},
                {1, new Color(210 / 255f, 128 / 255f, 196 / 255f, 1f)},
                {2, new Color(255 / 255f, 193 / 255f, 7 / 255f, 1f)},
                {3, new Color(204 / 255f, 255 / 255f, 144 / 255f, 1f)},
                {4, new Color(255 / 255f, 138 / 255f, 101 / 255f, 1f)},
            };

            return colors.TryGetValue(index, out var color) ? color : Color.white;
        }
    }
}
