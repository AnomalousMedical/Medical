using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Pose.Commands
{
    public interface PoseCommandAction
    {
        void posingEnded();

        void posingStarted();
    }
}
