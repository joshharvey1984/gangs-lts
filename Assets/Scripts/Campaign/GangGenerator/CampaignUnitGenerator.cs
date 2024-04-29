using System.Linq;
using Gangs.Core;
using Gangs.Data;
using UnityEngine;

namespace Gangs.Campaign.GangGenerator {
    public static class CampaignUnitGenerator {
        public static CampaignUnit GenerateUnit(Faction faction, int level) {
            var unitType = faction.Units[Random.Range(0, faction.Units.Count)];
            var unit = new CampaignUnit {
                Class = unitType.Name,
                Name = unitType.Name,
                Level = level,
                Attributes = unitType.BaseAttributes.Select(x => new UnitAttribute {
                    Type = x.UnitAttributeType,
                    BaseValue = x.BaseValue
                }).ToList()
            };

            var unitModifiers = unitType.GetModifiers(level);
            foreach (var modifier in unitModifiers) {
                if (modifier.Type == ModifierType.AttributeChange) {
                    var attribute = unit.Attributes.FirstOrDefault(x => x.Type == modifier.GetAttributeChange().AttributeType);
                    attribute!.Modifiers.Add(new UnitAttributeModifier {
                        Source = new UnitAttributeModifierSource {
                            Type = UnitAttributeModifierSourceType.Individual,
                            Name = $"Level {modifier.Level} {unitType.Name} Modifier"
                        },
                        Value = modifier.GetAttributeChange().Modifier
                    });
                }
                if (modifier.Type == ModifierType.NameChange) unit.Name = modifier.GetNameChange();
            }
            
            var factionModifiers = faction.AttributeModifiers;
            foreach (var modifier in factionModifiers) {
                var attribute = unit.Attributes.FirstOrDefault(x => x.Type == modifier.AttributeType);
                attribute!.Modifiers.Add(new UnitAttributeModifier {
                    Source = new UnitAttributeModifierSource {
                        Type = UnitAttributeModifierSourceType.Faction,
                        Name = $"{faction.Name} Modifier"
                    },
                    Value = modifier.Modifier
                });
            }
            
            return unit;
        }
    }
}