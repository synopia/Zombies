using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Zombies
{
    [StaticConstructorOnStartup]
    public class JobGiver_ZombieFeed : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.Downed)
                return null;

            Predicate<Thing> predicate1 = (Predicate<Thing>) (t =>
            {
                if (t != pawn && t.Spawned)
                    return t is Building_Turret;
                return false;
            });
            ZombiePawn zombiePawn = pawn as ZombiePawn;
            float maxDistance = zombiePawn.RaidingAttackRange;
            if (zombiePawn.IsRaiding)
            {
                maxDistance = 999f;
            }
            Thing thing1 = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), PathEndMode.ClosestTouch, TraverseParms.For((Pawn) zombiePawn, Danger.Deadly, TraverseMode.PassDoors, true), maxDistance, predicate1, (IEnumerable<Thing>) null, 5, false);
            if (thing1 != null)
            {
                Job job = new Job(JobDefOf.AttackMelee, thing1);
                job.maxNumMeleeAttacks = 1;
                int num2 = Rand.Range(420, 900);
                job.expiryInterval = num2;
                return job;
            }
            Predicate<Thing> predicate2 = (Predicate<Thing>) (t =>
            {
                if (t == pawn)
                    return false;
                Pawn pawn1 = t as Pawn;
                if (pawn1 == null || pawn1.Downed  || (!t.Spawned || pawn1 is ZombiePawn))
                    return false;
                if (!pawn1.RaceProps.Humanlike)
                    return !pawn1.RaceProps.IsFlesh;
                return true;
            });
            Thing thing2 = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, TraverseParms.For((Pawn) zombiePawn, Danger.Deadly, TraverseMode.PassDoors, false), maxDistance, predicate2, (IEnumerable<Thing>) null, 50, true);
            if (thing2 != null)
            {
                Thing thing3;
                using (PawnPath path = pawn.Map.pathFinder.FindPath(pawn.Position, thing2.Position, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.PassDoors, false), PathEndMode.OnCell))
                {
                    IntVec3 cellBefore;
                    thing3 = path.FirstBlockingBuilding(out cellBefore, pawn);
                }
                if (thing3 != null)
                {
                    Job job = new Job(JobDefOf.AttackMelee, thing3);
                    job.maxNumMeleeAttacks = 1;
                    int num2 = Rand.Range(420, 900);
                    job.expiryInterval = num2;
                    int num3 = 2;
                    job.locomotionUrgency = (LocomotionUrgency) num3;
                    return job;
                }
                if (thing2 != null)
                {
                    Job job = new Job(JobDefOf.AttackMelee, thing2);
                    job.maxNumMeleeAttacks = 1;
                    int num2 = Rand.Range(420, 900);
                    job.expiryInterval = num2;
                    int num3 = 2;
                    job.locomotionUrgency = (LocomotionUrgency) num3;
                    return job;
                }
            }
            return (Job) null;
        }
    }
}