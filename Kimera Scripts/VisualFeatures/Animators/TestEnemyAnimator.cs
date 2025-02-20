using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Features
{
    public class TestEnemyAnimator : EntityAnimator
    {
        public override void SetAnimator(Controller controller)
        {
            KineticEntity kinetic = controller as KineticEntity;
            TerrainEntity terrain = controller as TerrainEntity;
            StunEntity stun = controller as StunEntity;
            CombatEntity combatEntity = controller as CombatEntity;
            Combat combat = controller.SearchFeature<Combat>();

            if (stun != null) cmp_animator.SetBool("Stun", stun.isStunned);

            if (kinetic == null || terrain == null || combat == null) return;

            Vector3 speed = kinetic.speed;
            cmp_animator.SetFloat("AttackFactor", combat.attackSpeedMultiplier);
            cmp_animator.SetFloat("UpSpd", speed.y);
            speed.y = 0;
            speed.Normalize();
            cmp_animator.SetFloat("HorSpd", speed.x);
            cmp_animator.SetFloat("VerSpd", speed.z);
            cmp_animator.SetFloat("Speed", .5f + speed.magnitude / kinetic.maxSpeed);

            cmp_animator.SetBool("Ground", terrain.onGround);
        }
    }
}
