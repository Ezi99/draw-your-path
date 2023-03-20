using System;
using UnityEngine;

namespace OctoXR.KinematicInteractions
{
    [Serializable]
    public class SoundPlayer
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private float audioThreshold = 0.05f;

        public void PlayAudio(ScaleableGrabbableModifier modifier)
        {
            if (!audioSource) return;

            var currentFrameScale = modifier.transform.localScale.magnitude;
            var velocity = (currentFrameScale - modifier.LastFrameScale) * 5;

            if ((velocity > audioThreshold || velocity < -audioThreshold) && !audioSource.isPlaying)
            {
                audioSource.volume = 1 * Mathf.Clamp(Mathf.Abs(velocity), 0.3f, 1f);
                audioSource.Play();
            }

            modifier.LastFrameScale = currentFrameScale;
        }
    }
}