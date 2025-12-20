using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Cookie.HealthSystem
{
    /// <summary>
    ///     Abstract class for representing a dimensionless hurtbox
    /// </summary>
    [PublicAPI]
    public abstract class Hurtbox : MonoBehaviour
    {
        /// <summary>
        ///     Whether the Health component should be overriden explicitly<br />
        ///     If set to false, GetComponentInParent searching for Health is called
        /// </summary>
        public bool overrideHealth;

        /// <summary>
        ///     The Health component this Hurtbox is bound to
        /// </summary>
        public Health health;

        protected readonly List<Hitbox> HitboxesInRange = new();
        protected int HitboxesLayer;
        protected LayerMask WallMask;

        protected virtual void Awake()
        {
            if (!overrideHealth)
                health = GetComponentInParent<Health>();
            WallMask = HealthSettings.Get().wallMasks;
            HitboxesLayer = LayerMask.NameToLayer("Hitboxes");
        }

        protected void FixedUpdate()
        {
            for (int i = HitboxesInRange.Count - 1; i >= 0; i--)
            {
                if (i >= HitboxesInRange.Count)
                    return; // avoid weird stuff with destroying
                Hitbox hitbox = HitboxesInRange[i];
                OnHit(hitbox);
            }
        }

        /// <summary>
        ///     Should be called when a Hitbox is detected
        /// </summary>
        /// <param name="hitbox">The detected Hitbox</param>
        protected virtual void OnHit(Hitbox hitbox)
        {
            if (hitbox.data.hasPierce && hitbox.pierceLeft <= 0)
                return;

            if (IsSameGameObject(hitbox.transform))
                return;

            (bool hitWall, Vector3 hitPoint) wallCheck = WallCheck(hitbox.transform.position);

            if (wallCheck.hitWall)
                return;

            Hitbox.HitboxInfo hitboxInfo = hitbox.GetInfo();
            Health.AttackInfo attackInfo = new(hitboxInfo, wallCheck.hitPoint);

            if (health.TryGetHit(hitbox.GetInstanceID(), attackInfo))
                hitbox.OnAttack();
        }

        protected bool IsSameGameObject(Transform resultTransform)
        {
            if (!resultTransform)
                return false;

            if (resultTransform == transform)
                return true;

            if (resultTransform.parent)
            {
                if (resultTransform.parent == transform)
                    return true;

                if (transform.parent && resultTransform.parent == transform.parent)
                    return true;
            }

            return false;
        }

        protected abstract (bool hitWall, Vector3 hitPoint) WallCheck(Vector3 position);
    }
}
