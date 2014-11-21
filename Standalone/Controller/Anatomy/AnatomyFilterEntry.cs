using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    /// <summary>
    /// This interface represents a facet entry that can be filtered.
    /// </summary>
    public interface AnatomyFilterEntry
    {
        /// <summary>
        /// The caption for this entry.
        /// </summary>
        String Caption { get; }

        /// <summary>
        /// The name of the facet for this entry.
        /// </summary>
        String FacetName { get; }

        /// <summary>
        /// An enumerator over the items for this facet.
        /// </summary>
        IEnumerable<String> FilterableItems { get; }

        /// <summary>
        /// Get the top level items for this filter entry.
        /// </summary>
        IEnumerable<Anatomy> TopLevelItems { get; }

        /// <summary>
        /// Build a group selection for the given anatomy.
        /// </summary>
        /// <param name="anatomy">The anatomy to build a group for.</param>
        /// <returns>An Anatomy instance with the group for the specified anatomy.</returns>
        Anatomy buildGroupSelectionFor(AnatomyIdentifier anatomy);
    }
}
