using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Link
{
    protected Settings settings;
    [SerializeField] protected Controller actor;
    [SerializeField] protected Controller reactor;

    public Link(Controller actor, Controller reactor, Settings settings)
    {
        this.settings = settings;
        this.actor = actor;
        this.reactor = reactor;

        //Request Actor and Reactor to Data
        actor.ActionLinks.Add(this);
        reactor.ReactionLinks.Add(this);
    }

    public void Unlink()
    {
        if(actor.ActionLinks.Contains(this))
            actor.ActionLinks.Remove(this);

        if (reactor.ReactionLinks.Contains(this))
            reactor.ReactionLinks.Remove(this);
    }
}
