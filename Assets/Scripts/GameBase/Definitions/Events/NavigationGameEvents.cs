using Flowbit.Utilities.Core.Events;

namespace Flowbit.GameBase.Definitions
{
    /// <summary>
    /// Event called when we go to a new scene
    /// </summary>
    public sealed class OnNextScene : IEvent
    {
    }

    /// <summary>
    /// Event called when we go back to a previous scene
    /// </summary>
    public sealed class OnPreviousScene : IEvent
    {
    }

    /// <summary>
    /// Event called when we open a popup
    /// </summary>
    public sealed class OnPopupOpen : IEvent
    {
    }

    /// <summary>
    /// Event called when we close a popup
    /// </summary>
    public sealed class OnPopupClose : IEvent
    {
    }
}