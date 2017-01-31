using HugsLib.Source.Detour;
using RimWorld;
using Verse;
using Verse.AI;

namespace Zombies
{
    [StaticConstructorOnStartup]
    public class ZombiePawn : Pawn
    {
        public bool IsRaiding = true;
        public float RaidingAttackRange = 15f;
        public bool Zombified;

        public ZombiePawn()
        {
            RaidingAttackRange = UnityEngine.Random.Range(15f, 250f);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.LookValue(ref IsRaiding, "IsRaiding", true);
            Scribe_Values.LookValue(ref RaidingAttackRange, "RaidingAttackRange", 100f);
        }


        public override void Tick()
        {
            base.Tick();
            if (!Zombified)
            {
                mindState.mentalStateHandler.neverFleeIndividual = true;
                Zombified = true;
            }
        }
    }
}