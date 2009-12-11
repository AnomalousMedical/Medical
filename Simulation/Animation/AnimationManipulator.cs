using System;

namespace Medical
{
    public interface AnimationManipulator
    {
        AnimationManipulatorStateEntry createStateEntry();
        
        float DefaultPosition { get; }
        
        float Position { get; set; }
        
        String UIName { get; }
    }
}
