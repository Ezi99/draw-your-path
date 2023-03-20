namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IManipulable<T>
    {
        void Start(T grabbable);
        void Stop(T grabbable);
        void Update(T grabbable);
    }
}
