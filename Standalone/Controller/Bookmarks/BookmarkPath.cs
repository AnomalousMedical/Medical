using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class BookmarkPath
    {
        public BookmarkPath()
        {

        }

        /// <summary>
        /// The display name for this path.
        /// </summary>
        public String DisplayName { get; internal set; }

        /// <summary>
        /// The parent path for this path.
        /// </summary>
        public BookmarkPath Parent { get; internal set; }

        /// <summary>
        /// The full path of this path.
        /// </summary>
        internal String Path { get; set; }
    }
}
