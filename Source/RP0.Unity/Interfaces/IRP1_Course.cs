using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RP0.Unity.Unity;

namespace RP0.Unity.Interfaces
{
    public interface IRP1_Course
    {
        string name { get; }

        string description { get; }

        bool isTemporary { get; }

        int seatMax { get; }

        int seatMin { get; }

        //IList<IRP1_Astronaut> getStudents { get; }

        //void StartCourse();
    }
}
