using BEPUikPlugin;
using Engine;
using Medical.Pose.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Pose
{
    public interface PoseHandler
    {
        /// <summary>
        /// Add an action to this pose handler in the given mode.
        /// </summary>
        /// <param name="action">The action to add.</param>
        /// <param name="mode">The mode to add the action to.</param>
        void addPoseCommandAction(PoseCommandAction action, String mode);

        /// <summary>
        /// Remove an action to this pose handler in the given mode.
        /// </summary>
        /// <param name="action">The action to add.</param>
        /// <param name="mode">The mode to add the action to.</param>
        void removePoseCommandAction(PoseCommandAction action, String mode);

        /// <summary>
        /// Run the posingStarted events. Pass an enumerator over the active modes which will also be run if
        /// this handler supports them.
        /// </summary>
        /// <param name="modes">An enumerator over the active modes to run.</param>
        void posingStarted(IEnumerable<String> modes);

        /// <summary>
        /// Run the posingStarted events. Pass an enumerator over the active modes which will also be run if
        /// this handler supports them. This should be the same as what was passed to posingStarted.
        /// </summary>
        /// <param name="modes">An enumerator over the active modes to run. Should be the same as what was passed to posingStarted.</param>
        void posingEnded(IEnumerable<String> modes);

        /// <summary>
        /// Probably temporary, will investigate moving the actual posing behind this interface as well.
        /// </summary>
        BEPUikBone Bone { get; }
    }
}
