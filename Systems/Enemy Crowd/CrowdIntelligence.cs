using Features;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CrowdIntelligence<T> : MonoBehaviour where T : Controller
{
    [Serializable]
    public struct Unit
    {
        public bool defeated;
        public bool hostile;
        public bool available;
        public bool conscious;
        public MovementIntelligence intel;
    }

    [Header("Crowd")]
    [SerializeField] protected List<T> units;
    [SerializeField] protected bool crowdAlerted = false;
    public bool CrowdAlerted { get => crowdAlerted; }
    protected Dictionary<T, Unit> crowd;
    [SerializeField] protected List<Unit> crowdList;

    [Header("Crowd Aggro Settings")]
    [SerializeField] protected int hostileTokens;
    [SerializeField] protected int availableTokens;

    protected void Start()
    {
        crowd = new Dictionary<T, Unit>();

        foreach (T unit in units)
        {
            AddUnitToCrowd(unit);
        }
    }

    protected void Update()
    {
        ManageCrowd();
        crowdList = crowd.Values.ToList();
    }

    protected void ManageCrowd()
    {
        int consciousCount = GetConscious().Count();

        if (consciousCount <= 0) return;

        AdjustHostiles();
        AdjustAvailables();

    }

    protected void AdjustHostiles()
    {
        var hostiles = GetHostile();
        var availables = GetAvailable();
        int hostileCount = hostiles.Count();
        int availableCount = availables.Count();

        if (hostileCount == hostileTokens) return;

        int diff;

        if(hostileCount < hostileTokens)
        {
            if (availableCount <= 0) return;

            diff = hostileTokens - hostileCount;
            for(int i = 0; i < diff && i < availableCount; i++)
            {
                SetUnitHostile(availables.Keys.ToList()[i]);
            }

            return;
        }

        diff = hostileCount - hostileTokens;
        for(int i = 0; i < diff; i++)
        {
            SetUnitAvailable(hostiles.Keys.ToList()[i]);
        }
    }

    protected void AdjustAvailables()
    {
        var availables = GetAvailable();
        int availableCount = availables.Count();
        var outOffBattle = GetOutOfBattle();
        int outOffBattleCount = outOffBattle.Count();

        if (availableCount == availableTokens) return;

        int diff;

        if (availableCount < availableTokens)
        {
            if (outOffBattleCount <= 0) return;

            diff = availableTokens - availableCount;
            for (int i = 0; i < diff && i < outOffBattleCount; i++)
            {
                SetUnitAvailable(outOffBattle.Keys.ToList()[i]);
            }

            return;
        }

        diff = availableCount - availableTokens;
        for (int i = 0; i < diff; i++)
        {
            SetUnitOutOfBattle(availables.Keys.ToList()[i]);
        }
    }

    public virtual void AddUnitToCrowd(T unit)
    {
        if (unit == null) return;

        FollowEntity follow = unit as FollowEntity;

        if (follow == null) return;

        Life unitLife = unit.SearchFeature<Life>();
        MovementIntelligence unitIntel = unit.SearchFeature<MovementIntelligence>();

        Unit unitData = new Unit
        {
            defeated = unitLife.CurrentHealth <= 0,
            hostile = unitIntel.Hostile,
            available = unitIntel.Available,
            conscious = follow.target != null,
            intel = unitIntel
        };

        unit.SearchFeature<Life>().OnDeath += () => SetUnitDefeated(unit);
        if(!crowd.ContainsKey(unit)) crowd.Add(unit, unitData);

    }

    public void CrowdAlert()
    {
        if(crowdAlerted) return;

        crowdAlerted = true;

        var crowdCopy = new Dictionary<T, Unit>(crowd);

        foreach (var pair in crowdCopy)
        {
            if (pair.Key == null || pair.Value.defeated) continue;

            pair.Key.CallFeature<Follow>(new Setting("entityTargeted", true, Setting.ValueType.Bool));
        }
    }

    public void SetUnitHostile(T unit)
    {
        if(unit == null || !crowd.ContainsKey(unit)) return;
    
        Unit unitData = crowd[unit];

        if (unitData.defeated || !unitData.available || !unitData.conscious) return;

        unitData.hostile = true;

        crowd[unit] = unitData;

        if (unitData.intel == null) return;

        unitData.intel.SetActionState(MovementIntelligence.ActionState.Hostile);
    }

    public void SetUnitAvailable(T unit)
    {
        if (unit == null || !crowd.ContainsKey(unit)) return;

        Unit unitData = crowd[unit];

        if (unitData.defeated || !unitData.conscious) return;

        unitData.hostile = false;
        unitData.available = true;

        crowd[unit] = unitData;

        if (unitData.intel == null) return;

        unitData.intel.SetActionState(MovementIntelligence.ActionState.Available);
    }

    public void SetUnitOutOfBattle(T unit)
    {
        if (unit == null || !crowd.ContainsKey(unit)) return;

        Unit unitData = crowd[unit];

        if (unitData.defeated || !unitData.conscious) return;

        unitData.hostile = false;
        unitData.available = false;

        crowd[unit] = unitData;

        if (unitData.intel == null) return;

        unitData.intel.SetActionState(MovementIntelligence.ActionState.OutOfBattle);
    }

    public void SetUnitDefeated(T unit)
    {
        if (unit == null || !crowd.ContainsKey(unit)) return;

        Unit unitData = crowd[unit];

        unitData.hostile = false;
        unitData.available = false;
        unitData.defeated = true;

        crowd[unit] = unitData;

        if (unitData.intel == null) return;

        unitData.intel.SetActionState(MovementIntelligence.ActionState.OutOfBattle);
    }

    public void SetUnitConscious(T unit)
    {
        if (unit == null || !crowd.ContainsKey(unit)) return;

        Unit unitData = crowd[unit];

        if (unitData.conscious || unitData.defeated) return;

        unitData.conscious = true;

        crowd[unit] = unitData;
    }

    public Dictionary<T, Unit> GetHostile()
    {
        var hostile = crowd.Where(x => x.Value.hostile && !x.Value.defeated && x.Value.conscious).ToDictionary(x => x.Key, x => x.Value);

        return hostile;
    }

    public Dictionary<T, Unit> GetAvailable()
    {
        var hostile = crowd.Where(x => x.Value.available && !x.Value.hostile && !x.Value.defeated && x.Value.conscious).ToDictionary(x => x.Key, x => x.Value);

        return hostile;
    }

    public Dictionary<T, Unit> GetOutOfBattle()
    {
        var hostile = crowd.Where(x => !x.Value.available && !x.Value.defeated && x.Value.conscious).ToDictionary(x => x.Key, x => x.Value);

        return hostile;
    }

    public Dictionary<T, Unit> GetDefeated()
    {
        var hostile = crowd.Where(x => x.Value.defeated).ToDictionary(x => x.Key, x => x.Value);

        return hostile;
    }

    public Dictionary<T, Unit> GetConscious()
    {
        var hostile = crowd.Where(x => x.Value.conscious).ToDictionary(x => x.Key, x => x.Value);

        return hostile;
    }
}
