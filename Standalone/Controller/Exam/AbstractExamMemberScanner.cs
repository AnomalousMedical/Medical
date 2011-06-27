using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Reflection;

namespace Medical
{
    class AbstractExamMemberScanner : MemberScannerFilter
    {
        static MemberScanner memberScanner;

        static AbstractExamMemberScanner()
        {
            memberScanner = new MemberScanner(new AbstractExamMemberScanner());
            memberScanner.ProcessFields = false;
            memberScanner.ProcessNonPublicFields = false;
            memberScanner.ProcessNonPublicProperties = false;
        }

        public static MemberScanner MemberScanner
        {
            get
            {
                return memberScanner;
            }
        }

        private AbstractExamMemberScanner()
        {

        }

        public bool allowMember(MemberWrapper wrapper)
        {
            return wrapper.getCustomAttributes(typeof(HiddenAttribute), true).Length == 0;
        }

        public bool allowType(Type type)
        {
            return true;
        }
    }
}
