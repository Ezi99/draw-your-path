namespace OctoXR
{
    public enum UpdateRun
    {
        Update = 1,
        LateUpdate = 2,
        FixedUpdate = 4,
        UpdateAndLateUpdate = Update | LateUpdate,
        UpdateAndFixedUpdate = Update | FixedUpdate,
        LateAndFixedUpdate = LateUpdate | FixedUpdate,
        AllUpdates = Update | LateUpdate | FixedUpdate,
    }
}
