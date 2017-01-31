using System.Linq;
using HugsLib;
using HugsLib.Source.Detour;
using HugsLib.Utils;
using RimWorld;
using UnityEngine;
using Verse;

namespace Zombies
{
    public class ZombiesMod : ModBase
    {
        class BiteChecker : Pawn
        {
            [DetourMethod(typeof(Pawn), "PostApplyDamage")]
            public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
            {
                if (dinfo.Instigator is ZombiePawn && skills!=null ) {
                    var meele = skills.GetSkill(SkillDefOf.Melee);
                    if (meele!=null && Random.Range(0.0f, 100f) < ZombiesDefOf.ZombiesSettings.InfectChanceOnBite-meele.Level)
                    {
                        Infect(dinfo);
                    }
                }
            }

            private void Infect(DamageInfo dinfo)
            {
                BodyPartRecord part = health.hediffSet.GetRandomNotMissingPart(dinfo.Def, dinfo.Height, BodyPartDepth.Outside);
                if (part.def.IsSkinCovered(part, health.hediffSet))
                {
                    bool infected = Enumerable.Any(health.hediffSet.hediffs, hediff => hediff.Part==part && hediff.def == ZVirus);
                    if (!infected)
                    {
                        health.AddHediff(ZVirus, part, new DamageInfo?());
                        if (IsColonist)
                        {
                            Messages.Message(string.Format("ColonistInfected".Translate(), Name), MessageSound.SeriousAlert);
                        }
                    }
                }
            }

        }
        private int _ticksSinceLastSpawn;
        public static HediffDef ZVirus = DefDatabase<HediffDef>.GetNamed("ZombieInfection", true);
        private readonly ZombieFactory _factory = new ZombieFactory();
        private int _ticksUntilNextZombieRaid;
        public override string ModIdentifier
        {
            get { return "Zombies"; }
        }

        public override void WorldLoaded()
        {
            Logger.Message("Zombies loaed");
            _ticksUntilNextZombieRaid = GenerateTicksUntilNextRaid(true);
        }

        public override void Tick(int currentTick)
        {
            HandleReanimation();
            HandleZombieRaid();
        }

        private float GetChallengeModifier()
        {
            float num = 2f - Find.Storyteller.difficulty.threatScale;
            if (num < 0.699999988079071)
                num = 0.7f;
            return num;
        }

        private int GenerateTicksUntilNextRaid(bool first=false)
        {
            float challengeModifier = GetChallengeModifier();
            int num = Random.Range((int) ( ZombiesDefOf.ZombiesSettings.MinRaidTicksBase * challengeModifier), (int) (ZombiesDefOf.ZombiesSettings.MaxRaidTicksBase * challengeModifier));
            if (first && num < ZombiesDefOf.ZombiesSettings.MinTicksBeforeFirstRaid)
                num = ZombiesDefOf.ZombiesSettings.MinTicksBeforeFirstRaid;
            return num;
        }

        private void HandleZombieRaid()
        {
            _ticksUntilNextZombieRaid--;
            if (_ticksUntilNextZombieRaid <= 0)
            {
                _ticksUntilNextZombieRaid = GenerateTicksUntilNextRaid();

                IncidentParms param = new IncidentParms
                {
                    target=Find.AnyPlayerHomeMap

                };
                IncidentDef.Named("ZombieRaid").Worker.TryExecute(param);
            }
        }
        private void HandleReanimation()
        {
            _ticksSinceLastSpawn++;
            if (_ticksSinceLastSpawn <= 25)
                return;
            _ticksSinceLastSpawn = 0;
            var corpses = Find.AnyPlayerHomeMap.listerThings.ThingsInGroup(ThingRequestGroup.Corpse);
            if (corpses == null)
                return;
            for (var index = 0; index < corpses.Count; index++)
            {
                var thing = corpses[index];
                var corpse = (Corpse) thing;
                bool infected = Enumerable.Any(corpse.InnerPawn.health.hediffSet.hediffs,
                    hediff => hediff.def == ZVirus);
                if (infected)
                {
                    if (corpse.Age > ZombiesDefOf.ZombiesSettings.MinTicksBeforeReanimate && Random.Range(0f, 100f) <
                        ZombiesDefOf.ZombiesSettings.ChancePerTickToReanimate)
                    {
                        CompRottable comp = corpse.GetComp<CompRottable>();
                        if (comp == null || comp.Stage != RotStage.Dessicated)
                        {
                            Logger.Message("Reanimate");
                            _factory.ReanimateDeath(corpse);
                        }
                    }
//                    else if (MapCondition_Zombies.isAirborne && Find.TickManager.TicksGame - this.airborneTick >= 24000 && (corpse.Age > 12000 && (double) Random.value > 0.975000023841858))
//                    {
//                        CompRottable comp = corpse.GetComp<CompRottable>();
//                        if (comp == null || comp.Stage != RotStage.Dessicated)
//                            ZombieMod_Utility.ZombieFactory(corpse);
//                    }
                }
            }
        }
    }
}