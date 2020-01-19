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
    public class GUICourseTemplate : IRP1_Course
    {
        private CourseTemplate courseTemplate;
        public CourseTemplate CourseTemplate
        {
            get => courseTemplate;
        }

        private TopWindow topWindow;

        public string courseName
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

        public int seatMax
        {
            get => courseTemplate.seatMax;
        }

        public int seatMin
        {
            get => courseTemplate.seatMin;
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
    }
}
