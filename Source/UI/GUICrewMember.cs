using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using RP0.Crew;
using RP0.UI;
using RP0.Unity.Interfaces;
using UnityEngine;

namespace RP0.UI
{
    public class GUICrewMember : IRP1_Astronaut
    {
        public string crewMemberName
        {
            get => _self.name;
        }

        public string type
        {
            get => _self.trait;
        }

        public int level
        {
            get => _self.experienceLevel;
        }

        public string courseName
        {
            get
            {
                if (course == null)
                {
                    if (_self.rosterStatus == ProtoCrewMember.RosterStatus.Assigned)
                    {
                        return "(in-flight)";
                    }
                    else if (_self.inactive)
                    {
                        return "(inactive)";
                    }
                    else
                    {
                        return "(free)";
                    }
                }
                else
                {
                    return course.name;
                }
            }
        }

        public string education
        {
            get => CrewHandler.Instance.GetTrainingString(self);
        }

        public string completeTime
        {
            get
            {
                if (course == null)
                {
                    if (_self.rosterStatus == ProtoCrewMember.RosterStatus.Assigned)
                    {
                        return KSPUtil.PrintDate(_self.inactiveTimeEnd, false);
                    }
                    else if (_self.inactive)
                    {
                        return KSPUtil.PrintDate(_self.inactiveTimeEnd, false);
                    }
                    else
                    {
                        return "(n/a)";
                    }
                }
                else
                {
                    return KSPUtil.PrintDate(course.CompletionTime(), false);
                }
            }
        }

        public string retireTime
        {
            get
            {
                if (CrewHandler.Instance.kerbalRetireTimes.ContainsKey(_self.name))
                {
                    return CrewHandler.Instance.retirementEnabled ? KSPUtil.PrintDate(CrewHandler.Instance.kerbalRetireTimes[_self.name], false) : "(n/a)";
                }
                else
                {
                    return "(unknown)";
                }
            }
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

        public bool meetsCourseReqs
        {
            get
            {
                if (topWindow == null)
                    return false;
                
                return topWindow.doesStudentMeetCourseReqs(_self);
            } 
        }

        private ProtoCrewMember _self;
        public ProtoCrewMember self
        {
            get => _self;
        }

        public ActiveCourse course = null;

        private TopWindow topWindow;

        public GUICrewMember(ProtoCrewMember pcm, ActiveCourse ac, TopWindow pTopWindow)
        {
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
                    new MultiOptionDialog("RP1ConfirmStudentDropCourse", "Are you sure you want "+ _self.name + " to drop this course?","Drop Course?",
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
