using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Engine.Saving.XMLSaver;

namespace Medical
{
    /// <summary>
    /// This class can read and write patient data files (.pdt) files. This file
    /// format is very simple. The first 4 bytes are an int specifying how big
    /// the index data is. From byte 4 to indexdata size is a header that
    /// describes the file. From indexdata to the end of the file is a
    /// SavedMedicalStates object with the actual data inside.
    /// </summary>
    public class PatientDataFile
    {
        private SavedMedicalStates savedStates;
        private XmlSaver xmlSaver = new XmlSaver();
        private long headerSize = -1;

        private const String PATIENT_HEADER = "PatientHeader";
        private const String FIRST_NAME = "FirstName";
        private const String LAST_NAME = "LastName";

        public PatientDataFile(String backingFile)
        {
            this.BackingFile = backingFile;
            DateModified = File.GetLastWriteTime(backingFile);
        }

        public void save()
        {
            String saveFolder = Path.GetDirectoryName(BackingFile);
            if (!Directory.Exists(saveFolder))
            {
                Directory.CreateDirectory(saveFolder);
            }
            using (MemoryStream headerStream = new MemoryStream())
            {
                using (XmlTextWriter headerWriter = new XmlTextWriter(headerStream, Encoding.Default))
                {
                    headerWriter.WriteStartElement(PATIENT_HEADER);
                    headerWriter.WriteElementString(FIRST_NAME, FirstName);
                    headerWriter.WriteElementString(LAST_NAME, LastName);
                    headerWriter.WriteEndElement();
                    headerWriter.Flush();
                    using (Stream fileStream = new BufferedStream(new FileStream(BackingFile, FileMode.Create, FileAccess.Write)))
                    {
                        using (BinaryWriter bw = new BinaryWriter(fileStream))
                        {
                            bw.Write(headerStream.Length + sizeof(long));
                            fileStream.Write(headerStream.GetBuffer(), 0, (int)headerStream.Length);
                            using (XmlTextWriter dataWriter = new XmlTextWriter(fileStream, Encoding.Default))
                            {
                                dataWriter.Formatting = Formatting.Indented;
                                xmlSaver.saveObject(savedStates, dataWriter);
                            }
                        }
                    }
                }
            }
            DateModified = File.GetLastWriteTime(BackingFile);
        }

        /// <summary>
        /// Load the header for the data file. Returns true on success.
        /// </summary>
        /// <returns>True if the header was loaded without any problems.</returns>
        public bool loadHeader()
        {
            try
            {
                using (Stream fileStream = new BufferedStream(new FileStream(BackingFile, FileMode.Open, FileAccess.Read)))
                {
                    using (BinaryReader br = new BinaryReader(fileStream))
                    {
                        headerSize = br.ReadInt64();
                        using (XmlTextReader xmlReader = new XmlTextReader(fileStream))
                        {
                            while (!isEndElement(xmlReader, PATIENT_HEADER))
                            {
                                if (isValidElement(xmlReader))
                                {
                                    if (xmlReader.Name == FIRST_NAME)
                                    {
                                        FirstName = xmlReader.ReadElementContentAsString();
                                    }
                                    else if (xmlReader.Name == LAST_NAME)
                                    {
                                        LastName = xmlReader.ReadElementContentAsString();
                                    }
                                    else
                                    {
                                        xmlReader.Read();
                                    }
                                }
                                else
                                {
                                    xmlReader.Read();
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Load the date for the file. Will return true if sucessful.
        /// </summary>
        /// <returns>True if data is loaded correctly.</returns>
        public bool loadData()
        {
            bool loaded = true;
            try
            {
                if (headerSize == -1)
                {
                    loaded = loadHeader();
                }
                if (loaded)
                {
                    using (Stream fs = new BufferedStream(new FileStream(BackingFile, FileMode.Open, FileAccess.Read)))
                    {
                        fs.Seek(headerSize, SeekOrigin.Begin);
                        using (XmlTextReader textReader = new XmlTextReader(fs))
                        {
                            savedStates = xmlSaver.restoreObject(textReader) as SavedMedicalStates;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                loaded = false;
            }
            return loaded;
        }

        public void closeData()
        {
            savedStates = null;
        }

        public String BackingFile { get; set; }

        public String BackingFileName
        {
            get
            {
                return Path.GetFileNameWithoutExtension(BackingFile);
            }
        }

        public String BackingFileDirectory
        {
            get
            {
                return Path.GetDirectoryName(Path.GetFullPath(BackingFile));
            }
        }

        public String FirstName { get; set; }

        public String LastName { get; set; }

        public DateTime DateModified { get; set; }

        public SavedMedicalStates SavedStates
        {
            get
            {
                return savedStates;
            }
            set
            {
                savedStates = value;
            }
        }

        private static bool isEndElement(XmlReader xmlReader, String elementName)
        {
            return xmlReader.Name == elementName && xmlReader.NodeType == XmlNodeType.EndElement;
        }

        private static bool isValidElement(XmlReader xmlReader)
        {
            return xmlReader.NodeType == XmlNodeType.Element && !xmlReader.IsEmptyElement;
        }
    }
}
