namespace OctoXR.KinematicInteractions
{
    /// <summary>
    /// Interface used to give an object Interaction start, update and end methods. Primarily used by GrabController.
    /// </summary>
    public interface IInteractor
    {
        void StartInteraction();
        void UpdateInteraction();
        void EndInteraction();
    }
}
