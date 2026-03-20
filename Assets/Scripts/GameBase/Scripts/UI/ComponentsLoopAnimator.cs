using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flowbit.GameBase.UI
{
    /// <summary>
    /// Adds a subtle looping scale and rotation animation to a set of targets.
    /// Each target receives small random variations so the motion feels more alive
    /// and less synchronized.
    /// </summary>
    public sealed class ComponentsLoopAnimator : MonoBehaviour
    {
        [Header("Targets")]
        [SerializeField]
        private List<Transform> targets_ = new();

        [Header("Scale Animation")]
        [SerializeField]
        [Min(0f)]
        private float scaleAmplitude_ = 0.04f;

        [SerializeField]
        [Min(0.01f)]
        private float scaleFrequency_ = 1.2f;

        [Header("Rotation Animation")]
        [SerializeField]
        [Min(0f)]
        private float rotationAmplitudeDegrees_ = 4f;

        [SerializeField]
        [Min(0.01f)]
        private float rotationFrequency_ = 0.9f;

        [Header("Randomization")]
        [SerializeField]
        [Min(0f)]
        private float amplitudeRandomness_ = 0.25f;

        [SerializeField]
        [Min(0f)]
        private float frequencyRandomness_ = 0.2f;

        [SerializeField]
        [Min(0f)]
        private float phaseRandomness_ = 10f;

        [Header("Motion Style")]
        [SerializeField]
        [Range(0f, 1f)]
        private float secondaryWaveBlend_ = 0.35f;

        [SerializeField]
        [Range(0f, 1f)]
        private float unscaledTime_ = 0f;

        [SerializeField]
        private bool animateOnEnable_ = true;

        private readonly Dictionary<Transform, AnimatedTargetState> states_ = new();

        private void Awake()
        {
            RebuildStates();
        }

        private void OnEnable()
        {
            if (animateOnEnable_)
            {
                RebuildStates();
            }
        }

        private void OnDisable()
        {
            RestoreAllTargets();
        }

        private void OnValidate()
        {
            scaleFrequency_ = Mathf.Max(0.01f, scaleFrequency_);
            rotationFrequency_ = Mathf.Max(0.01f, rotationFrequency_);
            scaleAmplitude_ = Mathf.Max(0f, scaleAmplitude_);
            rotationAmplitudeDegrees_ = Mathf.Max(0f, rotationAmplitudeDegrees_);
            amplitudeRandomness_ = Mathf.Max(0f, amplitudeRandomness_);
            frequencyRandomness_ = Mathf.Max(0f, frequencyRandomness_);
            phaseRandomness_ = Mathf.Max(0f, phaseRandomness_);
        }

        private void Update()
        {
            float time = unscaledTime_ > 0.5f ? Time.unscaledTime : Time.time;

            CleanupMissingTargets();

            foreach (KeyValuePair<Transform, AnimatedTargetState> pair in states_)
            {
                Transform target = pair.Key;
                AnimatedTargetState state = pair.Value;

                if (target == null)
                {
                    continue;
                }

                ApplyAnimation(target, state, time);
            }
        }

        /// <summary>
        /// Adds a target to the animation list.
        /// If the target is already present, nothing happens.
        /// </summary>
        /// <param name="target">The transform to animate.</param>
        public void AddTarget(Transform target)
        {
            if (target == null || states_.ContainsKey(target))
            {
                return;
            }

            targets_.Add(target);
            states_.Add(target, CreateState(target));
        }

        /// <summary>
        /// Removes a target from the animation list and restores its original transform values.
        /// </summary>
        /// <param name="target">The transform to remove.</param>
        public void RemoveTarget(Transform target)
        {
            if (target == null)
            {
                return;
            }

            if (states_.TryGetValue(target, out AnimatedTargetState state))
            {
                RestoreTarget(target, state);
                states_.Remove(target);
            }

            targets_.Remove(target);
        }

        /// <summary>
        /// Clears all animated targets and restores their original transform values.
        /// </summary>
        public void ClearTargets()
        {
            RestoreAllTargets();
            states_.Clear();
            targets_.Clear();
        }

        /// <summary>
        /// Rebuilds the internal animation state for all current targets.
        /// Use this after changing the target list at runtime from external code.
        /// </summary>
        public void RefreshTargets()
        {
            RestoreAllTargets();
            RebuildStates();
        }

        private void RebuildStates()
        {
            states_.Clear();

            for (int i = targets_.Count - 1; i >= 0; i--)
            {
                Transform target = targets_[i];

                if (target == null)
                {
                    targets_.RemoveAt(i);
                    continue;
                }

                if (states_.ContainsKey(target))
                {
                    continue;
                }

                states_.Add(target, CreateState(target));
            }
        }

        private AnimatedTargetState CreateState(Transform target)
        {
            float scaleAmplitudeMultiplier = GetRandomMultiplier(amplitudeRandomness_);
            float rotationAmplitudeMultiplier = GetRandomMultiplier(amplitudeRandomness_);
            float scaleFrequencyMultiplier = GetRandomMultiplier(frequencyRandomness_);
            float rotationFrequencyMultiplier = GetRandomMultiplier(frequencyRandomness_);

            return new AnimatedTargetState
            {
                BaseScale = target.localScale,
                BaseRotation = target.localRotation,
                ScaleAmplitude = scaleAmplitude_ * scaleAmplitudeMultiplier,
                RotationAmplitudeDegrees = rotationAmplitudeDegrees_ * rotationAmplitudeMultiplier,
                ScaleFrequency = scaleFrequency_ * scaleFrequencyMultiplier,
                RotationFrequency = rotationFrequency_ * rotationFrequencyMultiplier,
                ScalePhase = UnityEngine.Random.Range(0f, Mathf.PI * 2f) + UnityEngine.Random.Range(-phaseRandomness_, phaseRandomness_),
                RotationPhase = UnityEngine.Random.Range(0f, Mathf.PI * 2f) + UnityEngine.Random.Range(-phaseRandomness_, phaseRandomness_),
                SecondaryPhaseOffset = UnityEngine.Random.Range(0f, Mathf.PI * 2f)
            };
        }

        private void ApplyAnimation(Transform target, AnimatedTargetState state, float time)
        {
            float primaryScaleWave = Mathf.Sin(time * state.ScaleFrequency * Mathf.PI * 2f + state.ScalePhase);
            float secondaryScaleWave = Mathf.Sin(time * state.ScaleFrequency * Mathf.PI * 4f + state.SecondaryPhaseOffset);
            float scaleWave = Mathf.Lerp(primaryScaleWave, secondaryScaleWave, secondaryWaveBlend_ * 0.35f);

            float primaryRotationWave = Mathf.Sin(time * state.RotationFrequency * Mathf.PI * 2f + state.RotationPhase);
            float secondaryRotationWave = Mathf.Sin(time * state.RotationFrequency * Mathf.PI * 3f + state.SecondaryPhaseOffset);
            float rotationWave = Mathf.Lerp(primaryRotationWave, secondaryRotationWave, secondaryWaveBlend_);

            float scaleOffset = scaleWave * state.ScaleAmplitude;
            float rotationOffset = rotationWave * state.RotationAmplitudeDegrees;

            target.localScale = state.BaseScale * (1f + scaleOffset);
            target.localRotation = state.BaseRotation * Quaternion.Euler(0f, 0f, rotationOffset);
        }

        private void RestoreAllTargets()
        {
            foreach (KeyValuePair<Transform, AnimatedTargetState> pair in states_)
            {
                if (pair.Key == null)
                {
                    continue;
                }

                RestoreTarget(pair.Key, pair.Value);
            }
        }

        private static void RestoreTarget(Transform target, AnimatedTargetState state)
        {
            target.localScale = state.BaseScale;
            target.localRotation = state.BaseRotation;
        }

        private void CleanupMissingTargets()
        {
            bool removedAny = false;

            List<Transform> nullTargets = null;

            foreach (KeyValuePair<Transform, AnimatedTargetState> pair in states_)
            {
                if (pair.Key != null)
                {
                    continue;
                }

                nullTargets ??= new List<Transform>();
                nullTargets.Add(pair.Key);
            }

            if (nullTargets != null)
            {
                for (int i = 0; i < nullTargets.Count; i++)
                {
                    states_.Remove(nullTargets[i]);
                    removedAny = true;
                }
            }

            for (int i = targets_.Count - 1; i >= 0; i--)
            {
                if (targets_[i] != null)
                {
                    continue;
                }

                targets_.RemoveAt(i);
                removedAny = true;
            }

            if (removedAny)
            {
                RemoveDuplicateTargets();
            }
        }

        private void RemoveDuplicateTargets()
        {
            HashSet<Transform> uniqueTargets = new();

            for (int i = targets_.Count - 1; i >= 0; i--)
            {
                Transform target = targets_[i];

                if (target == null || uniqueTargets.Add(target))
                {
                    continue;
                }

                targets_.RemoveAt(i);
            }
        }

        private static float GetRandomMultiplier(float randomness)
        {
            if (randomness <= 0f)
            {
                return 1f;
            }

            return UnityEngine.Random.Range(1f - randomness, 1f + randomness);
        }

        [Serializable]
        private struct AnimatedTargetState
        {
            public Vector3 BaseScale;
            public Quaternion BaseRotation;
            public float ScaleAmplitude;
            public float RotationAmplitudeDegrees;
            public float ScaleFrequency;
            public float RotationFrequency;
            public float ScalePhase;
            public float RotationPhase;
            public float SecondaryPhaseOffset;
        }
    }
}