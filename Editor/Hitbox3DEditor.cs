using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cookie.HealthSystem.Editor
{
    [CustomEditor(typeof(Hitbox3D))]
    public class Hitbox3DEditor : HitboxEditor
    {
        public override VisualElement CreateInspectorGUI() {
            VisualElement root = base.CreateInspectorGUI();

            var hitbox3D = (Hitbox3D)target;

            VisualElement panel3D = new();
            panel3D.AddToClassList("panel");

            Foldout title3D = new() {
                text = "3D",
                viewDataKey = "title3D",
            };

            title3D.AddToClassList("title");

            PropertyField overrideTrigger = new() {
                bindingPath = "overrideTrigger",
                label = "Override trigger",
            };

            ObjectField triggerOverride = new() {
                bindingPath = "trigger",
                objectType = typeof(Collider),
                label = "Trigger",
            };

            overrideTrigger.RegisterValueChangeCallback(_ =>
                triggerOverride.style.display = hitbox3D.overrideTrigger ? DisplayStyle.Flex : DisplayStyle.None
            );

            title3D.Add(overrideTrigger);
            title3D.Add(triggerOverride);

            panel3D.Add(title3D);

            root.Add(panel3D);

            return root;
        }
    }
}