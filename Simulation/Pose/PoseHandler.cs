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
        void addPoseCommandAction(PoseCommandAction action);

        void removePoseCommandAction(PoseCommandAction action);

        void posingStarted();

        void posingEnded();

        /// <summary>
        /// Probably temporary, will investigate moving the actual posing behind this interface as well.
        /// </summary>
        BEPUikBone Bone { get; }
    }
}
