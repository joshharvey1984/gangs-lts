using System.Collections.Generic;
using System.Linq;
using Gangs.Abilities;
using Gangs.Abilities.Structs;
using Gangs.Calculators;
using Gangs.GameObjects;
using Gangs.Grid;
using Gangs.Managers;
using UnityEngine;

namespace Gangs.AI {
    public static class EnemyAI {
        public static void TakeTurn(Unit unit) {
            var knownEnemyUnits = GetKnownEnemyUnits();
            var moveAbility = unit.Abilities[0] as MoveAbility;
            var moveRange = moveAbility!.CalculateMoveRange();
            
            // find best move considering cover, distance, line of sight and remaining action points
            var bestMove = GetBestMove(unit, moveRange, knownEnemyUnits);
            if (bestMove == null) {
                GameManager.Instance.SquadTurn.EndUnitTurn();
                return;
            }
            if (bestMove == GameManager.Instance.GetSoldierTile(unit)) {
                GameManager.Instance.SquadTurn.EndUnitTurn();
                return;
            }
            moveAbility!.AddWaypoint(bestMove);
            moveAbility!.Execute();
        }

        private static Tile GetBestMove(Unit unit, List<MoveRange> moveRange, List<Unit> knownEnemyUnits) {
            var candidateMoves = new List<CandidateMove>();
            var moveAbility = unit.Abilities[0] as MoveAbility;
            var toHitCalculator = new ToHitCalculator();
            foreach (var move in moveRange) {
                foreach (var tile in move.Tiles) {
                    var candidateMove = new CandidateMove { Tile = tile, ExpectedDamageDifferential = 0 };
                    foreach (var enemyUnit in knownEnemyUnits) {
                        var enemyTile = GridManager.Instance.Grid.FindGridUnit(enemyUnit.GridUnit);
                        var toHitModifiers = moveAbility!.GetToHitModifiers(tile, enemyTile, unit.ActionPointsRemaining - move.ActionPoint);
                        var toHit = toHitCalculator.CalculateToHitChance(tile, enemyTile, unit, enemyUnit, toHitModifiers);
                        var toBeHitModifiers = moveAbility.GetToHitModifiers(enemyTile, tile);
                        var toBeHit = toHitCalculator.CalculateToHitChance(enemyTile, tile, enemyUnit, unit, toBeHitModifiers);
                        
                        var expectedDamageGiven = ((float)toHit / 100) * 10.0f;
                        var expectedDamageTaken = ((float)toBeHit / 100) * 10.0f;
                        var expectedDamageDifferential = expectedDamageGiven - expectedDamageTaken;
                        
                        //var healthWeight = 1.0f - (unit.GetCurrentHitPoints() + enemyUnit.GetCurrentHitPoints()) / (unit.GetAttribute(FighterAttribute.HitPoints) + enemyUnit.GetAttribute(FighterAttribute.HitPoints));
                        
                        candidateMove.ExpectedDamageDifferential += expectedDamageDifferential;
                    }
                    
                    if (unit.ActionPointsRemaining - move.ActionPoint > 0) {
                        candidateMove.ExpectedDamageDifferential += 5.0f;
                    }
                    
                    candidateMoves.Add(candidateMove);
                }
            }
            
            candidateMoves.ForEach(c => c.ExpectedDamageDifferential += Random.Range(-0.1f, 0.1f));
            candidateMoves = candidateMoves.OrderByDescending(c => c.ExpectedDamageDifferential).ToList();
            
            if (candidateMoves.Count == 0) return null;
            
            if (DebugManager.Instance.DebugMode) {
                for (var i = 0; i < 10; i++) {
                    Debug.Log($"Move: {candidateMoves[i].Tile.GridPosition}, Expected damage differential: {candidateMoves[i].ExpectedDamageDifferential}");
                    GridVisualManager.Instance.ColorTile(candidateMoves[i].Tile, Color.green);
                    GridVisualManager.Instance.NumberTile(candidateMoves[i].Tile, candidateMoves[i].ExpectedDamageDifferential);
                }
                
                // wait 3 seconds
                // var start = Time.realtimeSinceStartup;
                // while (Time.realtimeSinceStartup < start + 3) { }
                //
                // GridVisualManager.Instance.ClearAllTileColors();
                // GridVisualManager.Instance.DeleteAllTileNumbers();
            }
            
            return candidateMoves[0].Tile;
        }

        private struct CandidateMove {
            public Tile Tile;
            public float ExpectedDamageDifferential;
        }

        private static List<Unit> GetKnownEnemyUnits() {
            var squad = GameManager.Instance.SquadTurn;
            var knownUnits = new List<Unit>();
            foreach (var unit in squad.Units) {
                unit.GetEnemiesInLineOfSight().ForEach(u => knownUnits.Add(u));
            }
            
            return knownUnits;
        }

        private static Dictionary<Unit, CoverType> GetTileCoverFromEnemyUnits(Tile tile, List<Unit> knownEnemyUnits) {
            var cover = new Dictionary<Unit, CoverType>();
            foreach (var enemyUnit in knownEnemyUnits) {
                var enemyTile = GridManager.Instance.Grid.FindGridUnit(enemyUnit.GridUnit);
                var coverType = GridManager.Instance.CheckTileCover(tile, enemyTile);
                cover.Add(enemyUnit, coverType);
            }

            return cover;
        }
    }
}