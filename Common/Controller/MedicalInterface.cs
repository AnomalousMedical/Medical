using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public interface MedicalInterface
    {
        void initialize(MedicalController controller);

        void destroy();
    }
}
