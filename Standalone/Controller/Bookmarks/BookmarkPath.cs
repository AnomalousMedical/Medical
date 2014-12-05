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

        public String DisplayName { get; internal set; }

        public BookmarkPath Parent { get; internal set; }

        internal String Path { get; set; }

        internal String BackingPath { get; set; }
    }
}
