using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class AttackLink : Link //Other Link channels
    {
        private const float KNOCKBACK_DELAY_SECONDS = .2f;
        private const int LIFE_STEAL = 35;  

        public AttackLink(Controller actor, Controller reactor, Settings attack) : base(actor, reactor, attack)
		{
			//Link Features
            CombatEntity actorCombat = actor as CombatEntity;
            CombatEntity reactorCombat = reactor as CombatEntity;
            Rigidbody reactorRigidbody = reactor.gameObject.GetComponent<Rigidbody>();
            Furry furry = actor.SearchFeature<Furry>();
            FurryEntity furryEntity = actor as FurryEntity;

            CombatReactions actorReaction = actor.SearchFeature<CombatReactions>();
            Life reactorLife = reactor.SearchFeature<Life>();
            Life actorLife = actor.SearchFeature<Life>();
            SoundLibrary actorLibrary = actor.GetComponent<SoundLibrary>();
            SoundLibrary reactorLibrary = reactor.GetComponent<SoundLibrary>();

            Combat reactorCombatController = reactor.SearchFeature<Combat>();

            if (reactorLife == null)
            {
                Unlink();
                return;
            }

            if (reactorCombat == null)
            {
                Unlink();
                return;
            }

            int damage = actorCombat.attack;



            if(attack != null)
            {
                bool? proporcentualDamage = attack.Search("proportionalHealthDamage");
                if(proporcentualDamage.HasValue)
                {
                    if(proporcentualDamage.Value == true) damage = ((reactorLife.maxHealth - reactorLife.CurrentHealth) * damage) / reactorLife.maxHealth;
                }

                float? attackPotential = attack.Search("attackPotential");

                if (attackPotential.HasValue)
                {
                    damage = (int) (attackPotential * damage);
                }

                int? attackExtra = attack.Search("attackExtra");

                if (attackExtra.HasValue)
                {
                    damage += (int)attackExtra;
                }


                
            }

            //bool damaged = reactorCombat != null ? !reactorCombat.block && !reactorCombat.parry : true;

            if (!reactorCombat.block && !reactorCombat.parry)
            {
                Debug.Log("Life:" + reactorLife.CurrentHealth + " / " + reactorLife.maxHealth + "   |  " + damage + " damage made to " + reactor.name);
                reactorLife.Health(-damage);              
                
                if (furry != null) furry.IncreaseFurryCount();
                if (furryEntity != null) furryEntity.furryCombo++;

                if (AudioManager.instance)
                {
                    /*if(furry != null && furry.furryCount > furry.furryMax * 0.7)
                    {
                        actorLibrary.CallAudioManager("ImpactoFurro");
                        reactorLibrary.CallAudioManager("DañoFurro");
                    }
                    else
                    {
                        actorLibrary.CallAudioManager("Impacto");
                        reactorLibrary.CallAudioManager("Daño");
                    }*/

                    string attackSFX = attack.Search("attackSFX");
                    
                    if(attackSFX != null)
                    actorLibrary.CallAudioManager(attack.Search("attackSFX"));
                    reactorLibrary.CallAudioManager("Daño");
                }
                
                //A�adir efectos de ataque

                if (attack != null)
                {
                    int? attackImpact = attack.Search("attackImpact");

                    if(attackImpact.HasValue)
                    {
                        if(reactorCombatController != null) reactorCombatController.PriorityBasedCancelAttack(attackImpact.Value);
                    }

                    float? overrideStun = attack.Search("stunAplierValue");
                    if (overrideStun.HasValue)
                    {
                        if (reactor != null) reactor.SearchFeature<Stun>().StunSomeTime(overrideStun.Value);
                    }
                }

                if (actorReaction != null) actorReaction.PassTurn();

                if (attack != null)
                {
                    Vector3? attackKnockback = attack.Search("attackKnockback");

                    if (attackKnockback.HasValue)
                    {
                        Vector3 direction = reactor.transform.position - actor.transform.position;
                        direction.y = 0;
                        direction.Normalize();

                        AddAttackKnockback(reactorRigidbody, attackKnockback.Value, direction);
                    }
                }

                //Efectos al matar enemigo
                if (reactorLife.CurrentHealth <= 0 && actorLife != null)
                {
                    if(attack != null)
                    {
                        int? lifeSteal = attack.Search("attackLifeSteal");

                        if (lifeSteal.HasValue)
                        {
                            actorLife.HealthPercentual(lifeSteal.Value, true);
                            
                            if(AudioManager.instance)
                            AudioManager.instance.PlaySound("AbsorcionVida");                            
                        }
                    }
                }
            
            }
            else
            {
                if(reactorCombat.parry)
                {
                    //Debug.Log("TestCombate");
                    actor.SearchFeature<Combat>().StopAttack();
                    
                    reactor.SearchFeature<Friction>().ToggleActive(true);
                    //reactor.SearchFeature<Block>().block = false;
                    //reactor.SearchFeature<Block>().InvokeEnd();

                    Camera_System.instance.CameraShake("Parry");
                    reactor.SearchFeature<Combat>().attackCooldownTimer = 0;
                    reactor.CallFeature<CombatAnimator>(new Setting("combatCondition", "attack-counter", Setting.ValueType.String));
                    actor.SearchFeature<Stun>().StunSomeTime(reactor.settings.Search("parryStunDuration"));

                    if(reactor.SearchFeature<Furry>().furryCount > 80)
                    {
                        reactor.SearchFeature<Shield>().HalfShield();
                    }
                }
                else
                {
                    reactorLife.Health((int)(-damage * reactor.SearchFeature<Block>().blockingDamageMultiplier));
                }
            }

            Unlink();
        }

        private void AddAttackKnockback(Rigidbody reactorRigidbody, Vector3 attackKnockback, Vector3 direction)
        {
            if (reactorRigidbody == null || attackKnockback == Vector3.zero || direction == Vector3.zero) return;

            Vector3 knockbackInDirection = Vector3.Cross(direction, Vector3.up) * attackKnockback.x + Vector3.up * attackKnockback.y + direction * attackKnockback.z;

            reactorRigidbody.isKinematic = false;
            reactorRigidbody.AddForce(knockbackInDirection, ForceMode.VelocityChange);
        }
    }
}