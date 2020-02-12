namespace RP0.Unity.Interfaces
{
    public interface IRP1_Course
    {


        string courseName { get; }

        string description { get; }

        bool isTemporary { get; }

        int seatMax { get; }

        int seatMin { get; }

        //IList<IRP1_Astronaut> getStudents { get; }

        //void StartCourse();

        void prepareCourse();
    }
}
