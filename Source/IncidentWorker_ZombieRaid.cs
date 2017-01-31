using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Zombies
{
    [StaticConstructorOnStartup]
    public class IncidentWorker_ZombieRaid : IncidentWorker_RaidEnemy
    {
        protected override string GetLetterText(IncidentParms parms, List<Pawn> list)
        {

            string str;
            if ( parms.points <= 175.0)
                str = "SmallHorde".Translate();
            else if (parms.points <= 350.0)
                str = "MediumHorde".Translate();
            else if (parms.points <= 700.0)
                str = "LargeHorde".Translate();
            else if (parms.points <= 1400.0)
                str = "MassiveHorde".Translate();
            else
                str = "OverwhelmingHorde".Translate();

            return str;
        }

        protected override string GetRelatedPawnsInfoLetterText(IncidentParms parms)
        {
            return "LetterRelatedPawnsRaidFriendly".Translate((object) parms.faction.def.pawnsPlural);
        }

        protected override string GetLetterLabel(IncidentParms parms)
        {
            return parms.raidStrategy.letterLabelEnemy;
        }

        protected override LetterType GetLetterType()
        {
            return LetterType.BadUrgent;
        }

        protected override void ResolveRaidStrategy(IncidentParms parms)
        {
            if (parms.raidStrategy != null)
                return;
            parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
        }

        protected override bool TryResolveRaidFaction(IncidentParms parms)
        {
            parms.faction = Find.FactionManager.FirstFactionOfDef(FactionDef.Named("Zombies"));
            return parms.faction != null;
        }

        protected override void ResolveRaidArriveMode(IncidentParms parms)
        {
            parms.raidArrivalMode = PawnsArriveMode.EdgeWalkIn;
        }
    }
}