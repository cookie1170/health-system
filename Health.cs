using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CookieUtils.Debugging;
using CookieUtils.ObjectPooling;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace Cookie.HealthSystem
{
    /// <summary>
    ///     A MonoBehaviour class used for tracking health and death
    /// </summary>
    [PublicAPI]
    public class Health : MonoBehaviour, IDebugDrawer, IPoolCallbackReceiver
    {
        /// <summary>
        ///     The types of I-Frames that can be used by a hurtbox
        /// </summary>
        public enum IframeType
        {
            /// <summary>
            ///     I-Frames based on the hitbox, each hitbox has its own I-Frames value
            /// </summary>
            Local,

            /// <summary>
            ///     I-Frames based on the hurtbox, while they're active no hitbox can damage the hurtbox
            /// </summary>
            Global,
        }

        [Tooltip("A HealthData scriptable object used for the data of this object")]
        public HealthData data;

        [Tooltip("The event invoked on hit, not invoked for fatal hits")]
        public UnityEvent<AttackInfo> onHit;

        [Tooltip("The event invoked on death")]
        public UnityEvent<AttackInfo> onDeath;

        /// <summary>
        ///     Used for tracking local I-Frames
        /// </summary>
        protected readonly Dictionary<int, float> LocalIframes = new();

        /// <summary>
        ///     Used for tracking global I-Frames
        /// </summary>
        protected float GlobalIframes;

        /// <summary>
        ///     The time since the object last got hit, used for tracking regeneration
        /// </summary>
        public float TimeSinceHit { get; protected set; }

        [field: Tooltip("The current amount of health. Use Hit or Regen to edit externally")]
        public float HealthAmount { get; protected set; } = 100;

        [Tooltip("Is the object dead\nUse Kill to set it externally")]
        public bool IsDead { get; protected set; }

        protected virtual void Awake() {
            if (!data) {
                Debug.LogError($"{name}'s Health has no data object!");
                Destroy(this);

                return;
            }

            HealthAmount = data.startHealth;
        }

        protected virtual void Update() {
            if (data.hasRegen) HandleRegen();

            switch (data.iframeType) {
                case IframeType.Local: {
                    int[] keys = LocalIframes.Keys.ToArray();

                    foreach (int hitboxId in keys) LocalIframes[hitboxId] -= Time.deltaTime;

                    for (int i = LocalIframes.Keys.Count - 1; i >= 0; i--) {
                        int key = keys[i];
                        float time = LocalIframes[key];

                        if (time <= 0) LocalIframes.Remove(key);
                    }

                    break;
                }

                case IframeType.Global: {
                    GlobalIframes -= Time.deltaTime;

                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetUpDebugUI(IDebugUI_BuilderProvider provider) {
            IDebugUI_Group foldout = provider.GetFor(this)
                .FoldoutGroup("Health");

            foldout.IntField("Amount", () => (int)HealthAmount, val => HealthAmount = val);
            foldout.StringField("Mask", () => Convert.ToString(data.mask, 2));
            foldout.FloatField("Time since hit", () => TimeSinceHit);
        }

        public void OnGet() {
            LocalIframes.Clear();
            GlobalIframes = 0;
            HealthAmount = data.maxHealth;
        }

        public void OnRelease() { }

        /// <summary>
        ///     Used to regenerate the object's health by amount
        /// </summary>
        /// <param name="amount">Amount to regenerate the health by</param>
        public virtual void Regen(float amount) {
            HealthAmount += amount;
            HealthAmount = Mathf.Clamp(HealthAmount, 0, data.maxHealth);
        }

        /// <summary>
        ///     Used to deal damage to the object
        /// </summary>
        /// <param name="info">The HitboxInfo used for the hit</param>
        public virtual void Hit(AttackInfo info) {
            if (IsDead) return;

            TimeSinceHit = 0f;

            HealthAmount -= info.HitboxInfo.Damage;
            if (HealthAmount <= 0) {
                Kill(info);

                return;
            }

            onHit?.Invoke(info);
        }

        /// <summary>
        ///     Used to kill the object, can be called prematurely
        /// </summary>
        /// <param name="info">The HitboxInfo used for the death</param>
        public virtual void Kill(AttackInfo info) {
            if (IsDead) return;

            IsDead = true;
            HealthAmount = 0;
            onDeath?.Invoke(info);

            if (data.destroyOnDeath) StartCoroutine(DestroyWithDelay());
        }

        private IEnumerator DestroyWithDelay() {
            yield return new WaitForSeconds(data.destroyDelay);
            gameObject.ReleaseOrDestroy();
        }

        /// <summary>
        ///     Handles the passive heath regeneration, only called if hasRegen is true
        /// </summary>
        protected virtual void HandleRegen() {
            TimeSinceHit += Time.deltaTime;
            float healAmount = data.regenCurve.Evaluate(TimeSinceHit) * Time.deltaTime;
            Regen(healAmount);
        }

        /// <summary>
        ///     Called by a Hurtbox when it detects a Hitbox
        /// </summary>
        /// <param name="instanceId">The instance ID of the Hitbox</param>
        /// <param name="info">The HitboxInfo of the Hitbox</param>
        /// <returns>Whether the hit check passed or not</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when iframeType is out of range</exception>
        public virtual bool TryGetHit(int instanceId, AttackInfo info) {
            if (!CheckMask(info.HitboxInfo.Mask)) return false;

            switch (data.iframeType) {
                case IframeType.Local: {
                    if (CheckLocalIframes(instanceId)) {
                        LocalIframes[instanceId] = info.HitboxInfo.Iframes * data.iframeMult;
                        Hit(info);

                        return true;
                    }

                    break;
                }
                case IframeType.Global: {
                    if (GlobalIframes <= 0) {
                        GlobalIframes = info.HitboxInfo.Iframes * data.iframeMult;
                        Hit(info);

                        return true;
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }

        public bool CheckMask(int mask) => (data.mask & mask) != 0;

        public bool CheckLocalIframes(int instanceId) {
            float iframes = LocalIframes.GetValueOrDefault(instanceId, 0f);

            return iframes <= 0;
        }

        public class AttackInfo
        {
            public Vector3 ContactPoint;
            public Hitbox.HitboxInfo HitboxInfo;

            public AttackInfo(Hitbox.HitboxInfo hitboxInfo, Vector3 contactPoint) {
                HitboxInfo = hitboxInfo;
                ContactPoint = contactPoint;
            }
        }
    }
}