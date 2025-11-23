using System.Collections.Generic;
using UnityEngine;
#if ZLINQ
using ZLinq;

#else
using System.Linq;
#endif

namespace Cookie.HealthSystem
{
    /// <summary>
    ///     A 3D implementation of a Hurtbox
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Hurtbox3D : Hurtbox
    {
        /// <summary>
        ///     Whether the trigger collider should be overriden explicitly<br />
        ///     If set to false, GetComponent searching for Collider is called
        /// </summary>
        public bool overrideTrigger;

        /// <summary>
        ///     The trigger collider this Hurtbox is bound to
        /// </summary>
        public Collider trigger;

        protected override void Awake() {
            base.Awake();
            if (!overrideTrigger) trigger = GetComponent<Collider>();

            if (!trigger) throw new UnityException("Hurtbox3D needs a trigger");
            trigger.isTrigger = true;
            trigger.excludeLayers = int.MaxValue - LayerMask.GetMask("Hitboxes");
        }

        protected void OnTriggerEnter(Collider other) {
            // could probably be optimized but i don't care
            if (other.TryGetComponent(out Hitbox hitbox)) HitboxesInRange.Add(hitbox);
        }

        protected void OnTriggerExit(Collider other) {
            // could probably be optimized but i don't care
            if (other.TryGetComponent(out Hitbox hitbox)) HitboxesInRange.Remove(hitbox);
        }

        protected override (bool hitWall, Vector3 hitPoint) WallCheck(Vector3 position) {
            int mask = LayerMask.GetMask("Hitboxes") | WallMask;
            float distance = Vector3.Distance(transform.position, position);
            RaycastHit[] results = Physics.RaycastAll(
                transform.position, (position - transform.position).normalized, distance, mask,
                QueryTriggerInteraction.Collide
            );

            List<RaycastHit> resultsList = results
                #if ZLINQ
                .AsValueEnumerable()
                #endif
                .Where(result => result.transform && !IsSameGameObject(result.transform))
                .ToList();

            if (resultsList.Count == 0) return (false, transform.position);

            int wallIndex = resultsList
                .FindIndex(hit => ((1 << hit.transform.gameObject.layer) & WallMask) != 0);

            int hitboxIndex = resultsList
                .FindIndex(hit => hit.transform.gameObject.layer == HitboxesLayer);

            bool isWall = wallIndex != -1;
            Vector3 hitPoint = resultsList[hitboxIndex != -1 ? hitboxIndex : 0].point;

            return (isWall, hitPoint);
        }
    }
}