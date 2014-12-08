using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Pose.Commands
{
    /// <summary>
    /// A single action for a pose command.
    /// </summary>
    public interface PoseCommandAction
    {
        /// <summary>
        /// Called when posing starts.
        /// </summary>
        void posingEnded();

        /// <summary>
        /// Called when posing is completed.
        /// </summary>
        void posingStarted();
    }
}
