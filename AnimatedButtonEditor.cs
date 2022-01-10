using Game.Common.UI;
using Game.Components.UI;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Editor.Components.UI
{
    [CustomEditor(typeof(AnimatedButton), true)]
    [CanEditMultipleObjects]
    public class AnimatedButtonEditor : ButtonEditor
    {
        private SerializedProperty _targetScaleTransform;

        private SerializedProperty _scaleAnimation;
        private SerializedProperty _shaderAnimation;
        private SerializedProperty _pressCurve;
        private SerializedProperty _customReleaseCurve;
        private SerializedProperty _releaseCurve;
        private SerializedProperty _pressScale;
        private SerializedProperty _pressDuration;
        private SerializedProperty _releaseDuration;
        private SerializedProperty _beginScale;

        private SerializedProperty _spriteAnimation;
        private SerializedProperty _targetImage;
        private SerializedProperty _normalSprite;
        private SerializedProperty _pressedSprite;

        private SerializedProperty _enableDisableEffect;
        private SerializedProperty _enableDisableEffectsList;


        protected override void OnEnable()
        {
            base.OnEnable();

            _targetScaleTransform = serializedObject.FindProperty("targetScaleTransform");
            
            _scaleAnimation = serializedObject.FindProperty("scaleAnimation");
            _shaderAnimation = serializedObject.FindProperty("shaderAnimation");
            _pressCurve = serializedObject.FindProperty("pressCurve");
            _customReleaseCurve = serializedObject.FindProperty("customReleaseCurve");
            _releaseCurve = serializedObject.FindProperty("releaseCurve");
            _pressScale = serializedObject.FindProperty("pressScale");
            _releaseDuration = serializedObject.FindProperty("releaseDuration");
            _pressDuration = serializedObject.FindProperty("pressDuration");
            _beginScale = serializedObject.FindProperty("beginScale");
            
            _spriteAnimation = serializedObject.FindProperty("spriteAnimation");
            _targetImage = serializedObject.FindProperty("targetImage");
            _normalSprite = serializedObject.FindProperty("normalSprite");
            _pressedSprite = serializedObject.FindProperty("pressedSprite");
            
            _enableDisableEffect = serializedObject.FindProperty("enableDisableEffect");
            _enableDisableEffectsList = serializedObject.FindProperty("enableDisableEffectsList");

        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            AnimatedButton animatedButton = (AnimatedButton) target;
            
            EditorGUILayout.Space();
            
            EditorGUILayout.PropertyField(_scaleAnimation);
            EditorGUILayout.PropertyField(_shaderAnimation);

            if (_scaleAnimation.boolValue)
            {
                EditorGUILayout.PropertyField(_targetScaleTransform);
                
                EditorGUI.BeginChangeCheck();
 
                animatedButton.PressCurve = EditorGUILayout.CurveField("PressCurve", animatedButton.PressCurve);
 
                if (EditorGUI.EndChangeCheck()) 
                    _pressCurve.animationCurveValue = animatedButton.PressCurve ;
                
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(_customReleaseCurve);

                if (_customReleaseCurve.boolValue)
                {
                    EditorGUI.BeginChangeCheck();
 
                    animatedButton.ReleaseCurve = EditorGUILayout.CurveField("ReleaseCurve", animatedButton.ReleaseCurve);
 
                    if (EditorGUI.EndChangeCheck()) 
                        _releaseCurve.animationCurveValue = animatedButton.ReleaseCurve ;
                }

                EditorGUILayout.PropertyField(_pressScale);
                EditorGUILayout.PropertyField(_beginScale);
                EditorGUILayout.PropertyField(_pressDuration);
                EditorGUILayout.PropertyField(_releaseDuration);
            }

            EditorGUILayout.PropertyField(_spriteAnimation);

            if (_spriteAnimation.boolValue)
            {
                EditorGUILayout.PropertyField(_targetImage);
                EditorGUILayout.PropertyField(_normalSprite);
                EditorGUILayout.PropertyField(_pressedSprite);
            }

            EditorGUILayout.PropertyField(_enableDisableEffect);

            if (_enableDisableEffect.boolValue)
            {
                EditorGUILayout.PropertyField(_enableDisableEffectsList, true);
            }

            serializedObject.SetIsDifferentCacheDirty();
            serializedObject.ApplyModifiedProperties();
        }
        
        
        [MenuItem("CONTEXT/Button/Convert To AnimatedButton", true)]
        static bool ConvertToAnimatedButtonValidator(MenuCommand command)
        {
            return CanConvertTo<AnimatedButton>(command.context);
        }
        
        [MenuItem("CONTEXT/Button/Convert To AnimatedButton", false)]
        static void ConvertToAnimatedButton(MenuCommand command)
        {
            ConvertTo<AnimatedButton>(command.context);
        }

        
        [MenuItem("CONTEXT/Button/Convert To Button", true)]
        static bool ConvertToButtonValidator(MenuCommand command)
        {
            return CanConvertTo<Button>(command.context);
        }

        
        [MenuItem("CONTEXT/Button/Convert To Button", false)]
        static void ConvertToButton(MenuCommand command)
        {
            ConvertTo<Button>(command.context);
        }
        
        protected static bool CanConvertTo<T>(Object context) where T : Button => context && context.GetType() != typeof(T);

        protected static void ConvertTo<T>(Object context) where T : Button
        {
            Button target = context as Button;
            if (target == null)
            {
                return;
            }
            
            SerializedObject so = new SerializedObject(target);
            so.Update();

            bool enabled = target.enabled;
            bool interactable = target.interactable;
            Button.ButtonClickedEvent clickedEvent = target.onClick;
            target.enabled = false;

            foreach (MonoScript script in Resources.FindObjectsOfTypeAll<MonoScript>())
            {
                if (script.GetClass() != typeof(T))
                {
                    continue;
                }

                so.FindProperty("m_Script").objectReferenceValue = script;
                so.ApplyModifiedProperties();
                break;
            }

            Button newButton = so.targetObject as Button;
            if (newButton == null)
            {
                return;
            }
            
            newButton.enabled = enabled;
            newButton.interactable = interactable;
            newButton.onClick = clickedEvent;
        }
    }
}
