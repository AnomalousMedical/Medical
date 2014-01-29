using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Medical
{
    public interface ResourceProvider
    {
        /// <summary>
        /// Open a stream to read a file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        Stream openFile(String filename);

        /// <summary>
        /// Open a stream to write to a file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        Stream openWriteStream(String filename);

        /// <summary>
        /// Add a file to this resource provider, if supported.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="targetDirectory"></param>
        void addFile(String path, string targetDirectory);

        /// <summary>
        /// Delete a file from this resource provider, if supported.
        /// </summary>
        /// <param name="filename"></param>
        void delete(String filename);

        /// <summary>
        /// List the files matching a pattern.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        IEnumerable<String> listFiles(String pattern);

        /// <summary>
        /// List the files matching a pattern.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        IEnumerable<String> listFiles(String pattern, String directory, bool recursive);

        /// <summary>
        /// List the directories matching a pattern.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        IEnumerable<String> listDirectories(String pattern, String directory, bool recursive);

        /// <summary>
        /// Determine if a directory is empty.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool directoryHasEntries(String path);

        /// <summary>
        /// Determine if a file exsits at path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool fileExists(String path);

        /// <summary>
        /// Determine if a directory exists at path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool directoryExists(String path);

        /// <summary>
        /// Determine if a file or directory exists at path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool exists(String path);

        /// <summary>
        /// Get the real path of the file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        String getFullFilePath(String filename);

        /// <summary>
        /// Create a directory if supported.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="directoryName"></param>
        void createDirectory(string path, string directoryName);

        /// <summary>
        /// Returns true if path is a directory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool isDirectory(String path);

        /// <summary>
        /// The location that this resource provider is wrapping, not always the filesystem.
        /// </summary>
        String BackingLocation { get; }

        /// <summary>
        /// Move a file from the old path to the new path.
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newPath"></param>
        void move(string oldPath, string newPath);

        /// <summary>
        /// Make a copy of the file at from at to.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        void copyFile(string from, string to);

        /// <summary>
        /// Make a copy of the directory and all its contents at from at to.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        void copyDirectory(string from, string to);

        /// <summary>
        /// Clone the physical backend of this provider to the new given location.
        /// This should actually copy all the files provided by this resource provider
        /// to the new location.
        /// </summary>
        /// <param name="destination"></param>
        void cloneProviderTo(String destination);
    }

    public static class ResourceProviderExtensions
    {
        public static void cloneTo(ResourceProvider source, ResourceProvider destination)
        {
            foreach (String dir in source.listDirectories("*", "", true))
            {
                destination.createDirectory("", dir);
            }
            foreach (String file in source.listFiles("*", "", true))
            {
                using (Stream writeStream = destination.openWriteStream(file))
                {
                    using (Stream readStream = source.openFile(file))
                    {
                        readStream.CopyTo(writeStream);
                    }
                }
            }
        }
    }
}
