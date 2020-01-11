using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RP0.Crew;
using RP0.Unity.Interfaces;
using RP0.Unity.Unity;
using UnityEngine;

namespace RP0.UI
{
    public class GUICourseTemplate : MonoBehaviour, IRP1_Course
    {
        private CourseTemplate courseTemplate;

        private TopWindow topWindow;

        public string name
        {
            get => courseTemplate.name;
        }

        public string description
        {
            get => courseTemplate.description;
        }

        public bool isTemporary
        {
            get => courseTemplate.isTemporary;
        }

        //public string getDuration
        //{
        //    get
        //    {
        //    }
        //}

        //public string getCompleteTime
        //{
        //    get => 
        //}

        public int seatMax
        {
            get => courseTemplate.seatMax;
        }

        public int seatMin
        {
            get => courseTemplate.seatMin;
        }

        public List<GUICrewMember> students = new List<GUICrewMember>();
        public IList<IRP1_Astronaut> getStudents
        {
            get
            {
                return new List<IRP1_Astronaut>(students.ToArray());
            }
        }

        public GUICourseTemplate(CourseTemplate pTemplate, TopWindow pTopWindow)
        {
            courseTemplate = pTemplate;
            topWindow = pTopWindow;
        }

        public void prepareCourse()
        {
            topWindow.prepareNewCourse(courseTemplate);
        }

        public void StartCourse()
        {
            if (courseTemplate != null)
            {
                ActiveCourse courseToAdd = new ActiveCourse(courseTemplate);

                List<ProtoCrewMember> studentsToAdd = new List<ProtoCrewMember>();
                foreach (var guiCrewMember in students)
                {
                    studentsToAdd.Add(guiCrewMember.self);
                }

                courseToAdd.Students = studentsToAdd;

                if(studentsToAdd.Count > 0)
                    CrewHandler.Instance.ActiveCourses.Add(courseToAdd);
            }
            
            MaintenanceHandler.Instance.UpdateUpkeep();
        }
    }
}
