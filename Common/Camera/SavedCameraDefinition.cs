using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;

namespace Medical
{
    public class SavedCameraDefinition
    {
        public Vector3 Position { get; set; }

        public Vector3 LookAt { get; set; }

        public String Name { get; set; }

        public bool Save { get; set; }

        public SavedCameraDefinition(String name, Vector3 position, Vector3 lookAt)
        {
            this.Name = name;
            this.Position = position;
            this.LookAt = lookAt;
            this.Save = true;
        }

        public SavedCameraDefinition(String name, Vector3 position, Vector3 lookAt, bool save)
        {
            this.Name = name;
            this.Position = position;
            this.LookAt = lookAt;
            this.Save = save;
        }
    }
}
