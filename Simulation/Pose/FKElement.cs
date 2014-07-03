using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    /// <summary>
    /// A single element in an FK Chain, could be a link or a root.
    /// </summary>
    public interface FKElement
    {
        /// <summary>
        /// Add a child link.
        /// </summary>
        /// <param name="child">The child link to add.</param>
        void addChild(FKElement child);

        /// <summary>
        /// Remove a child link.
        /// </summary>
        /// <param name="child">The child link to remove.</param>
        void removeChild(FKElement child);

        /// <summary>
        /// The translation of this element.
        /// </summary>
        Vector3 Translation { get; }

        /// <summary>
        /// The rotation of this element.
        /// </summary>
        Quaternion Rotation { get; }

        /// <summary>
        /// Add this element's properties to a chain state.
        /// </summary>
        /// <param name="chain">The chain to add information to.</param>
        void addToChainState(FKChainState chain);

        /// <summary>
        /// Apply a chain state's settings to this element.
        /// </summary>
        /// <param name="chain">The chain to apply.</param>
        void applyChainState(FKChainState chain);
    }
}
