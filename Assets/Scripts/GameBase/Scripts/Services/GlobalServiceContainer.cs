using Flowbit.Utilities.Unity.Navigation;
using Flowbit.Utilities.Core.Events;
using Flowbit.Utilities.Core.Services;
using UnityEngine;

namespace Flowbit.GameBase.Services
{
    /// <summary>
    /// Service container for global context
    /// </summary>
    static public class GlobalServiceContainer
    {
        static private ServiceInitializer serviceInitializer_;

        /// <summary>
        /// Gets a service container with the initialized services
        /// </summary>
        public static ServiceContainer ServiceContainer
        {
            get
            {
                if (serviceInitializer_ == null)
                {
                    serviceInitializer_ = new ServiceInitializer();
                }

                return serviceInitializer_.ServiceContainer;
            }
        }
    }
}