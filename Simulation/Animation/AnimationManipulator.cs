using System;

namespace Medical
{
    public interface AnimationManipulator
    {
        BoneManipulatorStateEntry createStateEntry();
        
        float DefaultPosition { get; }
        
        float Position { get; set; }
        
        String UIName { get; }
    }
}
