using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour, IActivable
{
    [Header("Settings")]
    public Settings settings;

    [Header("Control")]
    protected bool active;

    [Header("Components")]
    [SerializeField] protected List<GameObject> additionalGameObjectFeatures;
    public Dictionary<System.Type, IFeatureSetup> features;
    public List<IFeatureSetup> otherFeatures;

    [Header("Links")]
    [SerializeField] protected List<Link> actionLinks;
    public List<Link> ActionLinks { get { return actionLinks; } }
    [SerializeField] protected List<Link> reactionLinks;
    public List<Link> ReactionLinks { get { return reactionLinks; } }

    private void Awake()
    {
        Setup();
        ToggleActive(true);
    }

    protected virtual void Update()
    {
        if (!active) return;

        UpdateFeatures();
        UpdateLinks();
    }

    protected virtual void FixedUpdate()
    {
        if (!active) return;

        FixedUpdateFeatures();
        FixedUpdateLinks();
    }

    public virtual void Setup()
    {
        settings.AssemblySettings();
        List<IFeatureSetup> featureList = new List<IFeatureSetup>(GetComponents<IFeatureSetup>());

        additionalGameObjectFeatures.ForEach(go => featureList.AddRange(new List<IFeatureSetup>(go.GetComponents<IFeatureSetup>())));

        features = new Dictionary<System.Type, IFeatureSetup>();
        otherFeatures = new List<IFeatureSetup>();

        foreach (IFeatureSetup feature in featureList)
        {
            if (!features.ContainsKey(feature.GetType())) features.Add(feature.GetType(), feature);
            else otherFeatures.Add(feature);
            feature.SetupFeature(this);
        }

        actionLinks = new List<Link>();
        reactionLinks = new List<Link>();
    }

    public virtual void UpdateFeatures()
    {
        foreach (IFeatureSetup feature in features.Values)
        {
            IFeatureUpdate featureUpdate = feature as IFeatureUpdate;

            if (featureUpdate == null) continue;

            featureUpdate.UpdateFeature(this);
        }

        foreach (IFeatureSetup feature in otherFeatures)
        {
            IFeatureUpdate featureUpdate = feature as IFeatureUpdate;

            if (featureUpdate == null) continue;

            featureUpdate.UpdateFeature(this);
        }
    }

    public virtual void FixedUpdateFeatures()
    {
        foreach (IFeatureSetup feature in features.Values)
        {
            IFeatureFixedUpdate featureFixedUpdate = feature as IFeatureFixedUpdate;

            if (featureFixedUpdate == null) continue;

            featureFixedUpdate.FixedUpdateFeature(this);
        }

        foreach (IFeatureSetup feature in otherFeatures)
        {
            IFeatureFixedUpdate featureFixedUpdate = feature as IFeatureFixedUpdate;

            if (featureFixedUpdate == null) continue;

            featureFixedUpdate.FixedUpdateFeature(this);
        }
    }

    public T SearchFeature<T>() where T: IFeatureSetup
    {
        if (!features.ContainsKey(typeof(T))) return default(T);

        return (T)features[typeof(T)];
    }

    public void CallFeature<T>(params Setting[] settings) where T : IFeatureSetup
    {
        if (!active) return;

        IFeatureSetup feature = SearchFeature<T>();

        IFeatureAction featureAction = feature as IFeatureAction;

        if (featureAction == null) return;

        featureAction.FeatureAction(this, settings);
    }

    public void UpdateLinks()
    {
        var tempActionLinks = new List<Link>(actionLinks);
        var tempReactionLinks = new List<Link>(reactionLinks);

        foreach (Link link in tempActionLinks)
        {
            ILinkUpdate linkUpdate = link as ILinkUpdate;
            if (linkUpdate == null) continue;
            linkUpdate.RequestActorUpdate(this);
        }

        foreach (Link link in tempReactionLinks)
        {
            ILinkUpdate linkUpdate = link as ILinkUpdate;
            if (linkUpdate == null) continue;
            linkUpdate.RequestReactorUpdate(this);
        }
    }

    public void FixedUpdateLinks()
    {
        var tempActionLinks = new List<Link>(actionLinks);
        var tempReactionLinks = new List<Link>(reactionLinks);

        foreach (Link link in tempActionLinks)
        {
            ILinkFixedUpdate linkFixedUpdate = link as ILinkFixedUpdate;
            if (linkFixedUpdate == null) continue;
            linkFixedUpdate.RequestActorFixedUpdate(this);
        }

        foreach (Link link in tempReactionLinks)
        {
            ILinkFixedUpdate linkFixedUpdate = link as ILinkFixedUpdate;
            if (linkFixedUpdate == null) continue;
            linkFixedUpdate.RequestReactorFixedUpdate(this);
        }
    }

    public bool GetActive()
    {
        return active;
    }

    public void ToggleActive(bool active)
    {
        this.active = active;

        foreach (IFeatureSetup feature in features.Values)
        {
            IActivable activableFeature = feature as IActivable;

            if (activableFeature != null) activableFeature.ToggleActive(active && activableFeature.GetActive());
        }

        if(active) return;

        foreach (Link link in new List<Link>(actionLinks))
        {
            link.Unlink();
        }
    }
}
