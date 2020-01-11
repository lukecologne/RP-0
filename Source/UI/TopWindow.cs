using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RP0.Crew;
using RP0.UI;
using RP0.Unity.Interfaces;
using RP0.Unity.Unity;

namespace RP0
{
    [KSPAddon(KSPAddon.Startup.FlightEditorAndKSC, false)]
    public class TopWindow : MonoBehaviour, IRP1_MainPanel
    {

        private bool windowGenerated;
        private bool windowGenerating;
        private bool positionSet;
        private RP1_MainPanel UIWindow;
        private Rect windowPos = new Rect(500, 500, 0, 0);

        private Coroutine _repeatingWorkerAvionics;

        private Coroutine _repeatingWorkerCourses;

        #region Instance

        private static TopWindow Instance;

        public static TopWindow _Instance
        {
            get { return Instance; }
        }

        #endregion

        public bool IgnoreScale
        {
            get { return false; }
            set{}
        }

        private bool _isVisible;
        public bool IsVisible
        {
            get { return _isVisible; }
        }

        public bool isCareerMode
        {
            get
            {
                if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
                    return true;
                
                return false;
            }
        }

        public bool PixelPerfect
        {
            get { return true; }
            set{}
        }

        private int _currentScene;
        public int currentScene
        {
            get { return _currentScene; }

        }

        public float MasterScale
        {
            get { return 1; }
        }

        public float Scale
        {
            get { return 1; }
            set{}
        }

        public string Version
        {
            get { return "v1.2.1"; }
        }

        public string newActiveCourseDuration
        {
            get
            {
                if(newCourse != null)
                    return KSPUtil.PrintDateDeltaCompact(newCourse.GetTime(), false, false);
                else
                    return "";
            }
        }

        public string newActiveCourseEndTime
        {
            get
            {
                if (newCourse != null)
                    return KSPUtil.PrintDate(newCourse.CompletionTime(), false);
                else
                    return "";
            } 
        }

        #region AstronautFields

        private List<IRP1_Astronaut> crewMembers = new List<IRP1_Astronaut>();

        private List<IRP1_Course> courses = new List<IRP1_Course>();

        public IList<IRP1_Astronaut> getAstronauts
        {
            get { return crewMembers; }
        }

        public IList<IRP1_Course> getCourses
        {
            get{ return new List<IRP1_Course>(courses.ToArray());}
        }

        private Dictionary<ProtoCrewMember, ActiveCourse> activeMap = new Dictionary<ProtoCrewMember, ActiveCourse>(); 

        #endregion

        #region MaintenanceFields

        #region Main maintenance

        public double facilityUpkeep
        {
            get => MaintenanceHandler.Instance.facilityUpkeep;
        }

        public double integrationUpkeep
        {
            get => MaintenanceHandler.Instance.integrationUpkeep;
        }

        public double researchUpkeep
        {
            get => MaintenanceHandler.Instance.researchUpkeep;
        }

        public double nautTotalUpkeep
        {
            get => MaintenanceHandler.Instance.nautTotalUpkeep;
        }

        public double totalUpkeep
        {
            get => MaintenanceHandler.Instance.totalUpkeep + MaintenanceHandler.Instance.settings.maintenanceOffset;
        }

        #endregion

        #region Facilities Maintenance

        public double LaunchPadTotalCost
        {
            get => MaintenanceHandler.Instance.padCost;
            
        }

        public string[] LaunchPadLevels
        {
            get
            {
                string[] toReturn = new string[1];
                for (int i = 0; i < toReturn.Length; i++)
                {
                    if (MaintenanceHandler.Instance.padCosts[i] == 0d)
                        continue;
                    toReturn[i] = String.Format("Level {0} × {1}", i + 1, MaintenanceHandler.Instance.kctPadCounts[i]);
                }

                return toReturn;
            }
        }

        public double[] LauchPadLevelsCost
        {
            get
            {
                return MaintenanceHandler.Instance.padCosts;
            }
        }

        public double RunwayTotalCost
        {
            get => MaintenanceHandler.Instance.runwayCost;
        }

        public double VABTotalCost
        {
            get => MaintenanceHandler.Instance.vabCost;
        }

        public double SPHTotalCost
        {
            get => MaintenanceHandler.Instance.sphCost;
        }

        public double RnDTotalCost
        {
            get => MaintenanceHandler.Instance.rndCost;
        }

        public double MissionControlTotalCost
        {
            get => MaintenanceHandler.Instance.mcCost;
        }

        public double TrackingStationTotalCost
        {
            get => MaintenanceHandler.Instance.tsCost;
        }

        public double AstronautComplexTotalCost
        {
            get => MaintenanceHandler.Instance.acCost;
        }

        public double TotalFacilityCostCost
        {
            get => MaintenanceHandler.Instance.facilityUpkeep;
        }

        #endregion

        #region Integration Maintenance

        public string[] integrationSites
        {
            get
            {
                string[] toReturn = new string[MaintenanceHandler.Instance.kctBuildRates.Count];
                int i = 0;
                foreach (string site in MaintenanceHandler.Instance.kctBuildRates.Keys)
                {
                    toReturn[i] = site;
                    i++;
                }

                return toReturn;
            }
        }

        public double[] integrationRates
        {
            get
            {
                double[] toReturn = new double[MaintenanceHandler.Instance.kctBuildRates.Count];
                int i = 0;
                foreach (string site in MaintenanceHandler.Instance.kctBuildRates.Keys)
                {
                    toReturn[i] = MaintenanceHandler.Instance.kctBuildRates[site] * MaintenanceHandler.Instance.settings.kctBPMult;
                    i++;
                }

                return toReturn;
            }
        }

        #endregion

        #region Astronauts Maintenance

        public int CorpsSize
        {
            get => HighLogic.CurrentGame.CrewRoster.GetActiveCrewCount();
        }

        public double AstronautBaseCost
        {
            get => MaintenanceHandler.Instance.nautBaseUpkeep;
        }

        public double AstronautOperationCost
        {
            get => MaintenanceHandler.Instance.nautInFlightUpkeep;
        }

        public double AstronautTrainingCost
        {
            get => MaintenanceHandler.Instance.trainingUpkeep;
        }

        #endregion

        #endregion

        #region Courses Fields

        private List<IRP1_Course> coursesList = new List<IRP1_Course>();

        public IList<IRP1_Course> getCourseButtons
        {
            get { return new List<IRP1_Course>(coursesList.ToArray()); }
        }

        private ActiveCourse newCourse = null;

        #endregion


        #region General Methods

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            GameEvents.onGameSceneSwitchRequested.Add(OnSceneChange);
        }

        private void start()
        {
           
        }

        private void onDestroy()
        {
            if (Instance != this)
                return;

            Close();

            Instance = null;

            if (UIWindow != null)
            {
                Destroy(UIWindow.gameObject);
            }

            if (_repeatingWorkerAvionics != null)
            {
                StopCoroutine(_repeatingWorkerAvionics);
                _repeatingWorkerAvionics = null;
            }

            if (_repeatingWorkerCourses != null)
            {
                StopCoroutine(_repeatingWorkerCourses);
                _repeatingWorkerCourses = null;
            }
        }

        public void SetWindowPosition(Rect r)
        {
            windowPos = r;
        }

        public void SetPosition()
        {
            positionSet = true;

            UIWindow.setPosition(windowPos);
        }

        public void evaluateScene()
        {
            if (HighLogic.LoadedSceneIsEditor)
                _currentScene = 0;
            else if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
                _currentScene = 1;
        }

        public void Open()
        {
            if (!windowGenerated && !windowGenerating)
                StartCoroutine(GenerateWindow());

            StartCoroutine(WaitForOpen());
        }

        private IEnumerator WaitForOpen()
        {
            while (!windowGenerated)
                yield return null;

            if (UIWindow == null)
                yield break;

            if (_repeatingWorkerAvionics != null)
                StopCoroutine(_repeatingWorkerAvionics);

            if (_repeatingWorkerCourses != null)
                StopCoroutine(_repeatingWorkerCourses);

            _repeatingWorkerAvionics = StartCoroutine(RepeatingWorkerAvionics(0.25f, workerMethodAvionics));

            _repeatingWorkerCourses = StartCoroutine(RepeatingWorkerCourses(1f, workerMethodCourses));

            _isVisible = true;

            UIWindow.Open();
        }

        public void Close()
        {
            if (UIWindow == null)
                return;

            if (_repeatingWorkerAvionics != null)
            {
                StopCoroutine(_repeatingWorkerAvionics);
                _repeatingWorkerAvionics = null;
            }

            _isVisible = false;

            UIWindow.Close();
        }

        private IEnumerator GenerateWindow()
        {
            if (RP1Loader.WindowPrefab == null)
                yield break;

            if (UIWindow != null)
                yield break;

            evaluateScene();

            generateAstronautList();

            generateCoursTemplateList();

            //UIWindow.CreateNauts(crewMembers);
            
            windowGenerating = true;

            UIWindow = Instantiate(RP1Loader.WindowPrefab, DialogCanvasUtil.DialogCanvasRect, false).GetComponent<RP1_MainPanel>();

            yield return StartCoroutine(UIWindow.setWindow(this));

            //SetPosition(); //todo doesn't work right, window disappears instantly

            Close();

            windowGenerated = true;
            windowGenerating = false;
        }

        #endregion

        private void OnSceneChange(GameEvents.FromToAction<GameScenes, GameScenes> fromTo)
        {
            UIWindow.Close();
        }

        #region Repeating Worker

        private IEnumerator RepeatingWorkerAvionics(float seconds, Action method)
        {
            WaitForSeconds wait = new WaitForSeconds(seconds);

            yield return wait;


            while (true)
            {
                method();

                yield return wait;
            }
        }

        void workerMethodAvionics()
        {
            if (_currentScene == 0)
            {
                float maxMass = 0f, vesselMass = 0f;
                bool isControlLocked = true;

                if (EditorLogic.fetch.ship?.Parts is List<Part> parts && parts.Count > 0)
                    isControlLocked = ControlLockerUtils.ShouldLock(parts, false, out maxMass, out vesselMass);

                UIWindow.updateAvionicsMenu(maxMass, vesselMass, isControlLocked);
            }
        }

        private IEnumerator RepeatingWorkerCourses(float seconds, Action method)
        {
            WaitForSeconds wait = new WaitForSeconds(seconds);

            yield return wait;

            while (true)
            {
                method();

                yield return wait;
            }
        }

        void workerMethodCourses()
        {
            UIWindow.updateAllNauts(UIWindow.astronauts);
            UIWindow.updateAllNauts(UIWindow.courseStartListAstronauts);
        }

        #endregion

        #region Astronaut Methods

        //TODO make the course list update every 2 seconds or so

        public void generateAstronautList()
        {
            updateActiveMap();

            foreach (ProtoCrewMember pcm in HighLogic.CurrentGame.CrewRoster.Crew)
            {
                string courseName, completeTime, retireTime;

                ActiveCourse pcmActiveCourse = null;

                generateAstronautValues(pcm, out courseName, out completeTime, out retireTime, out pcmActiveCourse);

                GUICrewMember member = new GUICrewMember(pcm.name, pcm.trait, pcm.experienceLevel, retireTime, pcm, pcmActiveCourse, this);

                member.courseName = courseName;
                member.completeTime = completeTime;
                member.retireTime = retireTime;

                crewMembers.Add(member);
            }
        }

        private void updateActiveMap()
        {
            activeMap.Clear();
            foreach (ActiveCourse course in CrewHandler.Instance.ActiveCourses) {
                foreach (ProtoCrewMember student in course.Students) {
                    activeMap[student] = course;
                }
            }
        }

        void generateAstronautValues(ProtoCrewMember pcm, out string courseName, out string completeTime, out string retireTime, out ActiveCourse currentCourse)
        {
            if (CrewHandler.Instance.kerbalRetireTimes.ContainsKey(pcm.name))
            {
                retireTime = CrewHandler.Instance.retirementEnabled ? KSPUtil.PrintDate(CrewHandler.Instance.kerbalRetireTimes[pcm.name], false) : "(n/a)";
            }
            else
            {
                retireTime = "(unknown)";
            }

            currentCourse = null;

            if (activeMap.ContainsKey(pcm))
                currentCourse = activeMap[pcm];

            if (currentCourse == null)
            {
                if (pcm.rosterStatus == ProtoCrewMember.RosterStatus.Assigned)
                {
                    courseName = "(in-flight)";
                    completeTime = KSPUtil.PrintDate(pcm.inactiveTimeEnd, false);
                }
                else if (pcm.inactive)
                {
                    courseName = "(inactive)";
                    completeTime = KSPUtil.PrintDate(pcm.inactiveTimeEnd, false);
                }
                else
                {
                    courseName = "(free)";
                    completeTime = "(n/a)";
                }
            }
            else
            {
                courseName = currentCourse.name;
                completeTime = KSPUtil.PrintDate(currentCourse.CompletionTime(), false);
            }
        }

        #endregion

        #region Courses Methods

        public void generateCoursTemplateList()
        {
            foreach (CourseTemplate course in CrewHandler.Instance.OfferedCourses) 
            {
                GUICourseTemplate guiCourse = new GUICourseTemplate(course, this);
                coursesList.Add(guiCourse);
            }
        }

        public void prepareNewCourse(CourseTemplate template)
        {
            newCourse = new ActiveCourse(template);
        }

        public void addCrewToNewCourse(ProtoCrewMember toAdd)
        {
            newCourse?.Students.Add(toAdd);
        }

        public void removeCrewFromNewCourse(ProtoCrewMember toRemove)
        {
            newCourse?.Students.Remove(toRemove);
        }

        public void startNewCourse()
        {
            CrewHandler.Instance.ActiveCourses.Add(newCourse);
            newCourse.Students.Clear();
            MaintenanceHandler.Instance.UpdateUpkeep();
            UIWindow.updateCourseStartPanel();
        }

        #endregion

    }
}

