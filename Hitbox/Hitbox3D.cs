using System;
using UnityEngine;

namespace Cookie.HealthSystem
{
    /// <summary>
    ///     A 3D implementation of a Hitbox
    /// </summary>
    public class Hitbox3D : Hitbox
    {
        [Tooltip(
            "Whether the trigger collider should be overriden explicitly\n If set to false, GetComponent searching for Collider is called"
        )]
        public bool overrideTrigger = false;

        [Tooltip("The trigger collider this Hitbox is bound to")]
        public Collider trigger;

        private Vector3 _lastPos;

        protected override void Awake()
        {
            base.Awake();
            if (!overrideTrigger)
                trigger = GetComponent<Collider>();
            trigger.isTrigger = true;
            gameObject.layer = LayerMask.NameToLayer("Hitboxes");
        }

        private void FixedUpdate()
        {
            if ((transform.position - _lastPos).sqrMagnitude > 0.025f)
                _lastPos = transform.position;
        }

        protected override Vector3 GetDirection()
        {
            switch (data.directionType)
            {
                case DirectionType.Transform:
                {
                    return (transform.position - _lastPos).normalized;
                }

                case DirectionType.Manual:
                {
                    return direction;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
