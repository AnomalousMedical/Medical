using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI
{
    class PatientFileBuffer
    {
        private const int bufferSize = 200;
        private PatientDataFile[] dataFileBuffer = new PatientDataFile[bufferSize];
        private int currentPosition = 0;

        public PatientFileBuffer()
        {

        }

        public void reset()
        {
            currentPosition = 0;
        }

        public bool addPatient(PatientDataFile patient)
        {
            dataFileBuffer[currentPosition++] = patient;
            return currentPosition == bufferSize;
        }

        public IEnumerable<PatientDataFile> Files
        {
            get
            {
                for (int i = 0; i < currentPosition; ++i)
                {
                    yield return dataFileBuffer[i];
                }
            }
        }

        public bool HasResults
        {
            get
            {
                return currentPosition != 0;
            }
        }
    }
}
