using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Editor
{
    public interface ProjectType
    {
        void deleteProject(String name);

        void ensureProjectExists(String name);

        ResourceProvider openProject(String name);

        String Extension { get; }

        bool doesProjectExist(string name);
    }
}
