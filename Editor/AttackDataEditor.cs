using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cookie.HealthSystem.Editor
{
    [CustomEditor(typeof(AttackData))]
    public class AttackDataEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset inspector;

        public override VisualElement CreateInspectorGUI() {
            VisualElement root = new();

            var hitbox = (AttackData)target;

            inspector.CloneTree(root);

            var editMask = root.Q<Button>("EditMask");
            var hasPierce = root.Q<PropertyField>("HasPierce");
            var destroyOnOutOfPierce = root.Q<PropertyField>("DestroyOnNoPierce");
            var hideIfNoPierce = root.Q<VisualElement>("HideIfNoPierce");
            var hideIfNoDestroy = root.Q<VisualElement>("HideIfNoDestroy");
            var maskInput = root.Q<MaskField>("HitMask");

            UpdateChoices();

            maskInput.RegisterCallback<FocusEvent>(_ => UpdateChoices());

            editMask.RegisterCallback<ClickEvent>(_ => HealthSettings.OpenWindow());

            hasPierce.RegisterValueChangeCallback(_ =>
                hideIfNoPierce.style.display = hitbox.hasPierce ? DisplayStyle.Flex : DisplayStyle.None
            );

            destroyOnOutOfPierce.RegisterValueChangeCallback(_ =>
                hideIfNoDestroy.style.display = hitbox.destroyOnOutOfPierce ? DisplayStyle.Flex : DisplayStyle.None
            );

            return root;

            void UpdateChoices() {
                maskInput.choices = HealthSettings.Get().masks;
            }
        }
    }
}