using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cookie.HealthSystem.Editor
{
    [CustomEditor(typeof(HealthData))]
    public class HealthDataEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset inspector;

        public override VisualElement CreateInspectorGUI() {
            VisualElement root = new();

            var health = (HealthData)target;

            inspector.CloneTree(root);

            var healCurve = root.Q<PropertyField>("RegenCurve");
            var destroyDelay = root.Q<VisualElement>("DestroyDelay");
            var editMask = root.Q<Button>("EditMask");
            var hasRegen = root.Q<PropertyField>("HasRegen");
            var maxHealth = root.Q<PropertyField>("MaxHealth");
            var startHealth = root.Q<PropertyField>("StartHealth");
            var destroyOnDeath = root.Q<Toggle>("DestroyOnDeath");
            var maskInput = root.Q<MaskField>("HitMask");

            UpdateChoices();

            maskInput.RegisterCallback<FocusEvent>(_ => UpdateChoices());

            editMask.RegisterCallback<ClickEvent>(_ => HealthSettings.OpenWindow());

            hasRegen.RegisterValueChangeCallback(_ =>
                healCurve.style.display = health.hasRegen ? DisplayStyle.Flex : DisplayStyle.None
            );

            maxHealth.RegisterValueChangeCallback(_ => ClampStartHealth());

            startHealth.RegisterValueChangeCallback(_ => ClampStartHealth());

            destroyOnDeath.RegisterValueChangedCallback(_ => {
                    destroyDelay.style.display =
                        health.destroyOnDeath ? DisplayStyle.Flex : DisplayStyle.None;
                }
            );

            return root;

            void ClampStartHealth() {
                health.startHealth = Mathf.Clamp(health.startHealth, 1, health.maxHealth);
            }

            void UpdateChoices() {
                maskInput.choices = HealthSettings.Get().masks;
            }
        }
    }
}