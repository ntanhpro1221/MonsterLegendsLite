using System.Collections.Generic;
using System.Linq;
using MonsterLegendsLite.Data;
using NGDtuanh.MonsterLegendsLite;

namespace MonsterLegendsLite {
    public abstract class Battle_SkillSelector : MonoBehaviourExt {
        public delegate void OnSelected(bool isRecharge, Battle_Skill skill, IEnumerable<Battle_Monster> targets);

        protected IReadOnlyDictionary<string, Battle_Monster> Allies { get; private set; }
        protected IReadOnlyDictionary<string, Battle_Monster> Enemies { get; private set; }

        public virtual void Initialize(IReadOnlyDictionary<string, Battle_Monster> allies, IReadOnlyDictionary<string, Battle_Monster> enemies) {
            Allies  = allies;
            Enemies = enemies;
        }

        public abstract void SelectSkill(Battle_Monster monster, OnSelected onSelected);

        protected IEnumerable<Battle_Monster> IEAllMonster() {
            foreach (var monster in Allies.Values) yield return monster;
            foreach (var monster in Enemies.Values) yield return monster;
        }

        protected IEnumerable<Battle_Monster> GetTargets(Battle_Skill skill, Battle_Monster target = null) {
            var targets = (skill.skillData.Target.IsTargetEnemy() ? Enemies : Allies).Values.ToList();

            return skill.skillData.Target.IsSingle()
                ? IESingleMonster(target != null ? target : utils.RandomInside(targets))
                : targets;

            static IEnumerable<Battle_Monster> IESingleMonster(Battle_Monster monster) {
                yield return monster;
            }
        }
    }
}