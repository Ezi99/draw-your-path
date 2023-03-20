namespace OctoXR.Input
{
    public enum InputConfidenceTriggerEvaluationRun
    {
        Update = 1,
        FixedUpdate = 2,
        UpdateAndFixedUpdate = Update | FixedUpdate,
    }
}
