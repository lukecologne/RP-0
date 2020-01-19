using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RP0.Unity.Interfaces
{
    public interface IRP1_Astronaut
    {
        bool isInCourse { get; }

        bool meetsCourseReqs { get; }

        string crewMemberName { get; }

        string type { get; }

        int level { get; }

        string courseName { get; }

        string education { get; }

        string completeTime { get; }

        string retireTime { get; }

        //IRP1_Course getCurrentCourse { get; }

        void onLeaveButtonPressed();

        void addSelfToNewCourse();

        void removeSelfFromNewCourse();
    }
}
