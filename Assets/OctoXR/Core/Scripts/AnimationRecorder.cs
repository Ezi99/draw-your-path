#if UNITY_EDITOR
using UnityEngine;
using UnityEditor.Animations;

namespace OctoXR
{
    public class AnimationRecorder : MonoBehaviour
    {
        public AnimationClip animationClip;
        private GameObjectRecorder recorder;
        private bool isRecording = false;

        private void Update()
        {
            bool isButtonPressed = UnityEngine.Input.GetKeyDown(KeyCode.R);

            if (isButtonPressed && !isRecording)
            {
                InitalizeRecording();
                isRecording = true;
            }
            else if (isButtonPressed && isRecording)
            {
                isRecording = false;
                SaveRecording();
            }
        }

        private void InitalizeRecording()
        {
            recorder = new GameObjectRecorder(gameObject);
            recorder.BindComponentsOfType<Transform>(gameObject, true);
        }

        private void LateUpdate()
        {
            if (animationClip == null) return;

            if (isRecording)
            {
                recorder.TakeSnapshot(Time.deltaTime);
            }
        }

        private void SaveRecording()
        {
            if (animationClip == null) return;

            if (recorder.isRecording)
            {
                recorder.SaveToClip(animationClip);
            }
        }
    }
}
#endif
