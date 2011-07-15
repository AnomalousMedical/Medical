using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    /// <summary>
    /// This interface provides a way for the DocumentController to be extended with additional file types.
    /// </summary>
    public interface DocumentHandler
    {
        /// <summary>
        /// Return true from this function if this DocumentHandler can handle
        /// the type of file being seen.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        bool canReadFile(String filename);

        /// <summary>
        /// Process a file. Return true to add this file to the recent documents
        /// list.
        /// </summary>
        /// <param name="filename">The filename to read.</param>
        /// <returns>True to add to the recent documents list. False to keep it off.</returns>
        bool processFile(String filename);

        /// <summary>
        /// Get a nicely formatted name for this type of file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        string getPrettyName(string filename);

        /// <summary>
        /// Get an icon resource name for the file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        string getIcon(string filename);
    }
}
