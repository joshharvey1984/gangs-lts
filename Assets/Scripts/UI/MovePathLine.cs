using System.Collections.Generic;
using System.Linq;
using Gangs.Grid;
using Gangs.Managers;
using UnityEngine;

namespace Gangs.UI {
    public class MovePathLine : MonoBehaviour {
        public float lineWidth = 0.1f;
        public float yOffset = 0.05f;
        public Color pathColor = Color.blue;
        
        [Tooltip("Controls the number of segments used to draw the curve.")]
        public int curveSegments = 10;
        
        [Tooltip("Controls how sharply the curve bends at the control points.")]
        public float tension = 0.8f;

        [Tooltip("Controls the smoothness of the curve between control points.")]
        public float continuity = 0.4f;

        [Tooltip("Controls whether the curve favors the control point before or after the current one.")]
        public float bias = 0;
        
        private List<GameObject> _path = new();
        private List<GameObject> _waypoints = new();

    
        public void ClearMovementPath() => _path.ForEach(Destroy);
        public void ClearAllLines() => _waypoints.Union(_path).ToList().ForEach(Destroy);
        public void ConvertMovementPathToWayPoint() {
            _waypoints.AddRange(_path);
            _path.Clear();
        }
        
        public void DrawMovementPath(List<Tile> path) {
            if (path == null || path.Count < 2) {
                return;
            }

            var points = new List<Vector3>();
            for (var i = 0; i < path.Count; i++) {
                var tile = path[i];
                var position = new Vector3(tile.GridPosition.X, tile.GridPosition.Y + yOffset, tile.GridPosition.Z);
                points.Add(position);
            }
            
            if (DebugManager.Instance.pathfindingMode == DebugManager.PathfindingMode.AStarWithLerpAndSmoothing)
                CreateCurvedLineSegments(points, pathColor);
            else {
                CreateCurvedLineSegments(points, pathColor, false);
            }
        }
        
        private void CreateCurvedLineSegments(List<Vector3> points, Color lineColor, bool curved = true, bool waypoint = false) {
            var lineObject = new GameObject();
            lineObject.transform.SetParent(transform);

            var lineRenderer = lineObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.material.color = lineColor;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;

            if (curved) {
                points.Insert(0, points[0]);
                points.Add(points[points.Count - 1]);

                var smoothedPoints = new List<Vector3>();
                for (var i = 0; i < points.Count - 3; i++) {
                    for (var j = 0; j <= curveSegments; j++) {
                        smoothedPoints.Add(CalculateTcbSpline(points[i], points[i + 1], points[i + 2], points[i + 3], j / (float)curveSegments));
                    }
                }

                lineRenderer.positionCount = smoothedPoints.Count;
                lineRenderer.SetPositions(smoothedPoints.ToArray());
            } else {
                lineRenderer.positionCount = points.Count;
                lineRenderer.SetPositions(points.ToArray());
            }

            lineRenderer.receiveShadows = false;
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            
            if (waypoint) {
                _waypoints.Add(lineObject);
            } else {
                _path.Add(lineObject);
            }
        }

        private Vector3 CalculateTcbSpline(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
            // compute the tangent at p1
            var m1 = (1 - tension) * (1 + bias) * (1 - continuity) * 0.5f * (p2 - p1) +
                         (1 - tension) * (1 - bias) * (1 + continuity) * 0.5f * (p1 - p0);

            // compute the tangent at p2
            var m2 = (1 - tension) * (1 - bias) * (1 - continuity) * 0.5f * (p2 - p1) +
                         (1 - tension) * (1 + bias) * (1 + continuity) * 0.5f * (p3 - p2);

            // compute the coefficients of the cubic polynomial
            var a = 2*p1 - 2*p2 + m1 + m2;
            var b = -3*p1 + 3*p2 - 2*m1 - m2;
            var c = m1;
            var d = p1;

            // evaluate the cubic polynomial
            return a * t * t * t + b * t * t + c * t + d;
        }
    }
}