using Verse;

namespace Zombies
{
    public class ZombiesDef : Def
    {
        public float InfectChanceOnBite;
        public float BrainHitChance;
        public float DamageMultiplierOnBrainHit;
        public long MinTicksBeforeReanimate;
        public float ChancePerTickToReanimate;

        public int MinTicksBeforeFirstRaid;
        public int MinRaidTicksBase;
        public int MaxRaidTicksBase;
    }
}