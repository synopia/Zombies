using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;

namespace Zombies
{
    public class ZombieFactory
    {
        public Pawn ReanimateDeath(Corpse corpse)
        {
            var pawn = GenerateZombieFromSource(corpse.InnerPawn);
            pawn.IsRaiding = false;
            var position = corpse.Position;
            var building = corpse.StoringBuilding();
            ((Building_Storage) building)?.Notify_LostThing(corpse);
            GenSpawn.Spawn(pawn, position, corpse.Map);
            corpse.Destroy();
            return pawn;
        }

        private ZombiePawn GenerateZombieFromSource(Pawn sourcePawn)
        {
            var pawnKindDef = PawnKindDef.Named("Zombie");
            var zombieFaction = Find.FactionManager.FirstFactionOfDef(DefDatabase<FactionDef>.GetNamed("Zombies"));
            var pawn = (Pawn) ThingMaker.MakeThing(pawnKindDef.race);
            pawn.kindDef = pawnKindDef;
            pawn.SetFactionDirect(zombieFaction);
            PawnComponentsUtility.CreateInitialComponents(pawn);
            if (pawn.RaceProps.intelligence <= Intelligence.ToolUser)
                pawn.caller = new Pawn_CallTracker(pawn);
            pawn.gender = sourcePawn.gender;
            pawn.needs.SetInitialLevels();

            GenerateSkillsFromSource(sourcePawn, pawn);
            if (pawn.RaceProps.Humanlike)
            {
                string headGraphicPath = sourcePawn.story.HeadGraphicPath;
                pawn.story.melanin  = sourcePawn.story.melanin;
                pawn.story.crownType = sourcePawn.story.crownType;
                pawn.story.bodyType= sourcePawn.story.bodyType;
                pawn.story.hairColor = sourcePawn.story.hairColor;
                typeof (Pawn_StoryTracker).GetField("headGraphicPath", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(pawn.story, headGraphicPath);
                pawn.story.hairDef = sourcePawn.story.hairDef;
                GenerateZombieBioFromSource(pawn, sourcePawn);
                foreach (Trait allTrait in sourcePawn.story.traits.allTraits)
                {
                    pawn.story.traits.GainTrait(allTrait);
                }
            }
            GenerateZombieApparelFromSource(pawn, sourcePawn);
            GenerateAgeFromSource(sourcePawn, pawn);
            return pawn as ZombiePawn;
        }

        private static void GenerateAgeFromSource(Pawn sourcePawn, Pawn pawn)
        {
            pawn.ageTracker.AgeBiologicalTicks = sourcePawn.ageTracker.AgeBiologicalTicks;
            pawn.ageTracker.BirthAbsTicks = sourcePawn.ageTracker.BirthAbsTicks;
            pawn.ageTracker.AgeChronologicalTicks = sourcePawn.ageTracker.AgeChronologicalTicks;
        }

        private static void GenerateSkillsFromSource(Pawn sourcePawn, Pawn pawn)
        {
            foreach (var record in pawn.skills.skills)
            {
                var sourceSkill = sourcePawn.skills.GetSkill(record.def);
                record.Level = sourceSkill.Level;
            }
        }

        private void GenerateZombieBioFromSource(Pawn zombie, Pawn sourcePawn)
        {
            NameTriple name = sourcePawn.Name as NameTriple;
            zombie.Name = new NameTriple(name.First, "* " + "Zombie".Translate() + " " + name.Nick + " *", name.Last);
            zombie.story.childhood = sourcePawn.story.childhood;
            zombie.story.adulthood = sourcePawn.story.adulthood;
        }

        private void GenerateZombieApparelFromSource(Pawn zombie, Pawn sourcePawn)
        {
            if (sourcePawn.apparel == null || sourcePawn.apparel.WornApparelCount == 0)
                return;
            foreach (Apparel apparel in sourcePawn.apparel.WornApparel)
            {
                Apparel newApparel = !apparel.def.MadeFromStuff ? (Apparel) ThingMaker.MakeThing(apparel.def, (ThingDef) null) : (Apparel) ThingMaker.MakeThing(apparel.def, apparel.Stuff);
                newApparel.DrawColor = new Color(apparel.DrawColor.r, apparel.DrawColor.g, apparel.DrawColor.b, apparel.DrawColor.a);
                zombie.apparel.Wear(newApparel, true);
            }
        }
    }
}