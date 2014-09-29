using Engine;
using Engine.Attributes;
using Engine.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.Animation.Proxy
{
    class ProxyBone : BehaviorInterface
    {
        [Editable]
        private String proxyRootSimObjectName = "Pelvis";

        [Editable]
        private String proxyRootName = "SpineRoot";

        [DoNotCopy]
        [DoNotSave]
        ProxyRoot proxyRoot;

        protected override void link()
        {
            base.link();

            var spineRootSimObject = Owner.getOtherSimObject(proxyRootSimObjectName);
            if (spineRootSimObject == null)
            {
                blacklist("Cannot find parent Spine Root SimObject '{0}'.", proxyRootSimObjectName);
            }

            proxyRoot = spineRootSimObject.getElement(proxyRootName) as ProxyRoot;
            if (proxyRoot == null)
            {
                blacklist("Cannot find SpineRoot '{0}' on SimObject '{1}'.", proxyRootName, proxyRootSimObjectName);
            }
        }

        protected override void positionUpdated()
        {
            proxyRoot.alertChainMoved();
        }
    }
}
