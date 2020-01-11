using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using RP0.UI;
using RP0.Unity.Interfaces;
using UnityEngine;

namespace RP0.Crew
{
    public class GUICrewMember : MonoBehaviour, IRP1_Astronaut
    {
        private string _name;
        public string name
        {
            get => _name;
            set => _name = value;
        }

        private string _type;
        public string type
        {
            get => _type;
            set => _type = value;
        }

        private int _level;
        public int level
        {
            get => _level;
            set => _level = value;
        }

        private string _courseName;
        public string courseName
        {
            get => _courseName;
            set => _courseName = value;
        }

        public string education
        {
            get => CrewHandler.Instance.GetTrainingString(self);
        }

        private string _completeTime;
        public string completeTime
        {
            get => _completeTime;
            set => _completeTime = value;
        }

        private string _retireTime;
        public string retireTime
        {
            get => _retireTime;
            set => _retireTime = value;
        }

        public bool isInCourse
        {
            get
            {
                if (course == null)
                    return false;
                else
                    return true;
            }
        }


        private ProtoCrewMember _self;
        public ProtoCrewMember self
        {
            get => _self;
        }

        private ActiveCourse course = null;

        private TopWindow topWindow;

        public GUICrewMember(string pName, string pType, int pLevel, string pRetireTime, ProtoCrewMember pcm, ActiveCourse ac, TopWindow pTopWindow)
        {
            name = pName;
            type = pType;
            level = pLevel;
            retireTime = pRetireTime;
            _self = pcm;
            course = ac;
            topWindow = pTopWindow;
        }

        public void addSelfToNewCourse()
        {
            topWindow.addCrewToNewCourse(self);
        }

        public void removeSelfFromNewCourse()
        {
            topWindow.removeCrewFromNewCourse(self);
        }

        public void onLeaveButtonPressed()
        {
            if (course != null)
            {
                PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    new MultiOptionDialog("RP1ConfirmStudentDropCourse", "Are you sure you want "+ name + " to drop this course?","Drop Course?",
                        HighLogic.UISkin, new Rect(0.5f, 0.5f, 150f, 60f),
                        new DialogGUIFlexibleSpace(),
                        new DialogGUIVerticalLayout(
                            new DialogGUIFlexibleSpace(),
                            new DialogGUIButton("No",
                                delegate{ }, 140.0f, 30.0f, true), 
                            new DialogGUIButton("Yes",
                                delegate
                                {
                                    course?.RemoveStudent(_self);
                                    if (course.Students.Count == 0 && course != null)
                                    {
                                        CrewHandler.Instance.ActiveCourses.Remove(course);
                                        MaintenanceHandler.Instance.UpdateUpkeep();
                                    }
                                }, 140.0f, 30.0f, true)
                        )),
                    false,
                    HighLogic.UISkin);
            }
        }
    }
}
