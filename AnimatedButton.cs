using DG.Tweening;
using System;
using System.Collections.Generic;
using Game.Utils.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Components.UI 
 {
    public class AnimatedButton : Button
    {
        private const string AlphaMultiplierProperty = "_AlphaMultiplier";
        
        [Serializable]
        class EnableDisableEffect
        {
            public GameObject gameObject;
            public bool isNeedDisable;

            public void StartEffect() => gameObject.SetActive(!isNeedDisable);

            public void StopEffect() => gameObject.SetActive(isNeedDisable);
        }

        private static readonly int _additivePropertyId = Shader.PropertyToID(AlphaMultiplierProperty);

        public RectTransform RectTransform => (RectTransform)transform;

        [SerializeField] private bool scaleAnimation = true;
        [SerializeField] private bool shaderAnimation = true;
        [SerializeField] private Transform targetScaleTransform;
        [SerializeField] private AnimationCurve pressCurve = null;
        [SerializeField] private bool customReleaseCurve = true;
        [SerializeField] private AnimationCurve releaseCurve = null;
        [SerializeField] private float pressScale = 0.8f;
        [SerializeField] private float beginScale = 1f;
        [SerializeField] private float pressDuration = 0.1f;
        [SerializeField] private float releaseDuration = 0.1f;

        [SerializeField] private bool spriteAnimation = false;
        [SerializeField] private Image targetImage = null;
        [SerializeField] private Sprite normalSprite = null;
        [SerializeField] private Sprite pressedSprite = null;

        [SerializeField] private bool enableDisableEffect = false;
        [SerializeField] private List<EnableDisableEffect> enableDisableEffectsList = new List<EnableDisableEffect>();

        private bool _isButtonPressed;
        private Material _imageMaterial;

        public AnimationCurve PressCurve
        {
            get => pressCurve ??= AnimationCurve.EaseInOut(0, 0, 1, 1);
            set => pressCurve = value;
        }
        
        public AnimationCurve ReleaseCurve
        {
            get => releaseCurve ??= AnimationCurve.Linear(0, 0, 1, 1);
            set => releaseCurve = value;
        }

        public float AdditiveValue
        {
            get => _imageMaterial == null ? 0.0f : _imageMaterial.GetFloat(_additivePropertyId);
            set
            {
                if (_imageMaterial == null)
                {
                    return;
                }

                if (_imageMaterial.HasProperty(_additivePropertyId))
                {
                    _imageMaterial.SetFloat(_additivePropertyId, value);
                }
                else
                {
                    Debug.LogWarning($"Шейдер материала {_imageMaterial} {_imageMaterial.shader} " +
                                     $"не имеет проперти {AlphaMultiplierProperty}");
                }
            }
        }
        
        protected override void Awake()
        {
            base.Awake();

            if (!customReleaseCurve)
            {
                releaseCurve = new AnimationCurve(pressCurve.keys);
            }

            if (!shaderAnimation)
            {
                return;
            }

            var target = targetGraphic;
            
            if (target == null)
            {
                return;
            }
            
            _imageMaterial = new Material(target.material);
            target.material = _imageMaterial;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            DOTween.Kill(targetScaleTransform ? targetScaleTransform : transform);
            
            _imageMaterial.SafeDestroy();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (!IsPressed())
            {
                _isButtonPressed = false;
                ReleaseEffect();
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
                       
            _isButtonPressed = true; 
            PressEffect();            
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            
            if (_isButtonPressed)
            {
                ReleaseEffect(); 
                _isButtonPressed = false;
            }
        }

        private void PressEffect()
        {
            TryUseShaderPressEffect();
            TryUseScalePressEffect();
            TryUseChangeSpritePressEffect();
            TryUseEnableDisablePressEffect();
        }

        private void ReleaseEffect()
        {
            TryUseShaderReleaseEffect();
            TryUseScaleReleaseEffect();
            TryUseChangeSpriteReleaseEffect();
            TryUseEnableDisableReleaseEffect();
        }

        private void TryUseScalePressEffect()
        {
            if (scaleAnimation)
            {
                if (targetScaleTransform)
                {
                    DOTween.Kill(targetScaleTransform);
                    targetScaleTransform.DOScale(pressScale, pressDuration).SetEase(pressCurve).SetId(this).SetUpdate(true);  
                }
                else
                {
                    DOTween.Kill(transform);
                    transform.DOScale(pressScale, pressDuration).SetEase(pressCurve).SetId(this).SetUpdate(true);   
                }
            }
        }

        private void TryUseShaderPressEffect()
        {
            if (shaderAnimation)
            {
                DOTween.To(() => AdditiveValue, value => AdditiveValue = value, 1.2f, 0.2f);
            }
        }
        
        private void TryUseShaderReleaseEffect()
        {
            if (shaderAnimation)
            {
                DOTween.To(() => AdditiveValue, value => AdditiveValue = value, 1.0f, 0.2f);
            }
        }

        private void TryUseScaleReleaseEffect()
        {
            if (scaleAnimation)
            {
                if (targetScaleTransform)
                {
                    DOTween.Kill(targetScaleTransform);
                    targetScaleTransform.DOScale(beginScale * Vector3.one, releaseDuration).SetEase(releaseCurve).SetId(this).SetUpdate(true); 
                }
                else
                {
                    DOTween.Kill(this);
                    transform.DOScale(beginScale * Vector3.one, releaseDuration).SetEase(releaseCurve).SetId(this).SetUpdate(true);   
                }
            }
        }

        private void TryUseChangeSpritePressEffect()
        {
            if (spriteAnimation)
            {
                if (pressedSprite == null)
                {
                    targetImage.enabled = false;
                }
                else
                {
                    targetImage.enabled = true;
                    targetImage.sprite = pressedSprite;
                }
            }
        }

        private void TryUseChangeSpriteReleaseEffect()
        {
            if (spriteAnimation)
            {
                if (normalSprite == null)
                {
                    targetImage.enabled = false;
                }
                else
                {
                    targetImage.enabled = true;
                    targetImage.sprite = normalSprite;
                }
            }
        }

        private void TryUseEnableDisablePressEffect()
        {
            if (enableDisableEffect)
            {
                for (int i = 0, n = enableDisableEffectsList.Count; i < n; i++)
                {
                    enableDisableEffectsList[i].StartEffect();
                }
            }
        }

        private void TryUseEnableDisableReleaseEffect()
        {
            if (enableDisableEffect)
            {
                for (int i = 0, n = enableDisableEffectsList.Count; i < n; i++)
                {
                    enableDisableEffectsList[i].StopEffect();
                }
            }
        }
    }
}
