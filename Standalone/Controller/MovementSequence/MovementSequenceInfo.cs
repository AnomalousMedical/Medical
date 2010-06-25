using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller
{
    public class MovementSequenceInfo : IDisposable
    {
        public void Dispose()
        {
            //if (Thumbnail != null)
            //{
            //    Thumbnail.Dispose();
            //}
        }

        public String Name { get; set; }

        public String FileName { get; set; }

        //public Bitmap Thumbnail { get; set; }
    }
}
