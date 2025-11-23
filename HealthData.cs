using UnityEngine;

namespace Cookie.HealthSystem
{
    [CreateAssetMenu(fileName = "HealthData", menuName = "Cookie Utils/Health data", order = 0)]
    public class HealthData : ScriptableObject
    {
        [Tooltip("The bitwise mask used for hit comparison\n Set to int.MaxValue to pass all checks")]
        public int mask;

        [Min(1)] [Tooltip("The maximum amount of health it can reach")]
        public int maxHealth = 100;

        [Min(1)] [Tooltip("The starting amount of health")]
        public int startHealth = 100;

        [Tooltip("Whether the object can regenerate health passively")]
        public bool hasRegen = false;

        [Tooltip(
            "The curve used for passive health regeneration<br/> The curve\'s value at a certain time is the amount it regenerates per second"
        )]
        public AnimationCurve regenCurve = AnimationCurve.EaseInOut(0, 0, 5, 5);

        [Tooltip("The type of I-Frames used\nLocal - per hitbox, \nGlobal - per hurtbox")]
        public Health.IframeType iframeType = Health.IframeType.Local;

        [Min(0.1f)] [Tooltip("The multiplier applied to the I-Frames")]
        public float iframeMult = 1f;

        [Tooltip("Whether to destroy the GameObject on death")]
        public bool destroyOnDeath;

        [Tooltip("The delay for destroying the GameObject on death")]
        public float destroyDelay;
    }
}