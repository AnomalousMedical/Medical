using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Reflection;
using Engine.Editing;

namespace Engine
{
    /// <summary>
    /// This is the default MemberScanner for Behaviors. It will allow all
    /// fields/properties both public and nonpublic. marked with the Editable
    /// attribute through as long as those classes extend the BehaviorObjectBase
    /// class or they are avaliable as ReflectedVariables.
    /// </summary>
    public class TimelineGUIEditMemberScanner : MemberScannerFilter
    {
        private static MemberScanner scanner;
        private readonly string BEHAVIOR_OBJECT_INTERFACE = typeof(BehaviorObjectBase).FullName;

        /// <summary>
        /// Staic constructor.
        /// </summary>
        static TimelineGUIEditMemberScanner()
        {
            scanner = new MemberScanner(new TimelineGUIEditMemberScanner());
        }

        /// <summary>
        /// The MemberScanner instance.
        /// </summary>
        public static MemberScanner Scanner
        {
            get
            {
                return scanner;
            }
        }

        /// <summary>
        /// Hide constructor.
        /// </summary>
        private TimelineGUIEditMemberScanner()
        {

        }

        /// <summary>
        /// This is the test function. It will return true if the member should
        /// be accepted.
        /// </summary>
        /// <param name="wrapper">The MemberWrapper with info about the field/property being scanned.</param>
        /// <returns>True if the member should be included in the results. False to omit it.</returns>
        public bool allowMember(MemberWrapper wrapper)
        {
            return wrapper.getCustomAttributes(typeof(EditableAttribute), true).Length > 0;
        }

        /// <summary>
        /// This function determines if the given type should be scanned for
        /// members. It will return true if the member should be accepted.
        /// </summary>
        /// <param name="type">The type to potentially scan for members.</param>
        /// <returns>True if the type should be scanned.</returns>
        public bool allowType(Type type)
        {
            return true;
        }
    }
}
