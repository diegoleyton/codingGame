using System.Collections;

namespace Flowbit.Utilities.Unity.Navigation
{
    public interface IAnimatedScreen : IScreen
    {
        IEnumerator PlayEnter();
        IEnumerator PlayExit();
    }
}