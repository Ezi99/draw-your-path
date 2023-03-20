namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// Interface used to give an object Attach and Detach methods - primarily used by grabbables.
    /// </summary>
    /// <typeparam name="T"></typeparam>    
    public interface IAttachable<T>
    {
        public void Attach(T attachParent);

        public void Detach();
    }
}