using System;
using UnityEngine;

namespace Cookie.HealthSystem
{
    /// <summary>
    ///     A 2D implementation of a Hitbox
    /// </summary>
    public class Hitbox2D : Hitbox
    {
        [Tooltip(
            "Whether the trigger collider should be overriden explicitly\n If set to false, GetComponent searching for Collider2D is called"
        )]
        public bool overrideTrigger = false;

        [Tooltip("The trigger collider this Hitbox is bound to")]
        public Collider2D trigger;

        private Vector2 _lastPos;

        protected override void Awake()
        {
            base.Awake();
            if (!overrideTrigger)
                trigger = GetComponent<Collider2D>();
            trigger.isTrigger = true;
            gameObject.layer = LayerMask.NameToLayer("Hitboxes");
        }

        private void FixedUpdate()
        {
            if (((Vector2)transform.position - _lastPos).sqrMagnitude > 0.025f)
                _lastPos = transform.position;
        }

        protected override Vector3 GetDirection()
        {
            return data.directionType switch
            {
                DirectionType.Transform => ((Vector2)transform.position - _lastPos).normalized,
                DirectionType.Manual => direction,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}
