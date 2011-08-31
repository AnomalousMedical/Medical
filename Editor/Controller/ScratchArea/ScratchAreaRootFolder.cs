using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    class ScratchAreaRootFolder : ScratchAreaFolder
    {
        private String fileSystemPath;

        public ScratchAreaRootFolder(String folderName, String fileSystemPath)
            :base(folderName)
        {
            this.fileSystemPath = fileSystemPath;
        }

        protected override void findPath(StringBuilder sb)
        {
            sb.Append(fileSystemPath);
            sb.Append("//");
            sb.Append(folderName);
        }
    }
}
