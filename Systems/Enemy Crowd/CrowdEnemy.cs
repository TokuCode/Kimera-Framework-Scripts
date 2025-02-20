using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Features;
using UnityEngine.AI;

public class CrowdEnemy : CrowdIntelligence<Features.Enemy>
{
    public override void AddUnitToCrowd(Features.Enemy unit)
    {
        CombatReactions unitCombatReactions = unit.SearchFeature<CombatReactions>();
        Follow unitFollow = unit.SearchFeature<Follow>();

        if (unitCombatReactions != null) unitCombatReactions.enemyCrowd = this;
        if (unitFollow != null) unitFollow.enemyCrowd = this;

        base.AddUnitToCrowd(unit);

        if(crowdAlerted) SetUnitConscious(unit);

        MovementModeSelector unitMove = unit.SearchFeature<MovementModeSelector>();

        StartCoroutine(FixAgent(unitMove.agent));
    }

    public IEnumerator FixAgent(NavMeshAgent agent)
    {
        agent.enabled = false;

        yield return null;

        agent.enabled = true;
    }
}
