using UnityEngine;

namespace Cookie.HealthSystem
{
    [CreateAssetMenu(fileName = "AttackData", menuName = "Cookie Utils/Attack data", order = 0)]
    public class AttackData : ScriptableObject
    {
        [Tooltip("The mask used for hit detection\n Set to int.MaxValue to pass all checks")]
        public int mask;

        [Tooltip("The damage dealt by the Hitbox")]
        public int damage = 20;

        [Tooltip("The I-Frames invoked by the Hitbox")]
        public float iframes = 0.2f;

        [Tooltip("Whether the Hitbox has a limited pierce")]
        public bool hasPierce = false;

        [Tooltip(
            "The amount of pierce the Hitbox has until it can no longer attack\n Only used if hasPierce is true"
        )]
        public int pierce = 1;

        [Tooltip("Whether to destroy the object when pierce runs out")]
        public bool destroyOnOutOfPierce = true;

        [Tooltip("Whether to destroy the parent GameObject")]
        public bool destroyParent = true;

        [Tooltip("The delay to destroy the GameObject when pierce runs out")]
        public float destroyDelay;

        [Tooltip("The direction type used by the hitbox")]
        public Hitbox.DirectionType directionType;
    }
}
