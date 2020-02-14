using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using KSP.UI;
using UnityEngine;
using RP0.Crew;
using RP0.Tooling;
using RP0.Unity.Interfaces;
using RP0.Unity.Unity;
using Smooth.Slinq;
using Debug = UnityEngine.Debug;

namespace RP0.UI
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

        public bool astronautTrainingEnabled
        {
            get => HighLogic.CurrentGame.Parameters.CustomParams<RP0Settings>().IsTrainingEnabled;
        }

        public bool astronautRetirementEnabled
        {
            get => HighLogic.CurrentGame.Parameters.CustomParams<RP0Settings>().IsRetirementEnabled;
        }

        public bool KACAPIReady
        {
            get => KACWrapper.APIReady;
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
            get => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
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

        public int addedCrewCount
        {
            get
            {
                if (newCourse != null)
                    return newCourse.Students.Count;
                else
                    return -1;
            } 
        }

        #region Maintenance Properties

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

        public RP1_Maintenance.padLevelsAndCosts[] LaunchPadLevelsAndCosts
        {
            get
            {
                RP1_Maintenance.padLevelsAndCosts[] toReturn = new RP1_Maintenance.padLevelsAndCosts[MaintenanceHandler.padLevels];
                for (int i = 0; i < MaintenanceHandler.padLevels; i++)
                {
                    toReturn[i] = new RP1_Maintenance.padLevelsAndCosts(i + 1,
                        MaintenanceHandler.Instance.kctPadCounts[i], MaintenanceHandler.Instance.padCosts[i]);
                }

                return toReturn;
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

        #region Astronaut Fields

        private List<IRP1_Astronaut> crewMembers = new List<IRP1_Astronaut>();

        public IList<IRP1_Astronaut> getAstronauts
        {
            get { return crewMembers; }
        }

        private Dictionary<ProtoCrewMember, ActiveCourse> activeMap = new Dictionary<ProtoCrewMember, ActiveCourse>(); 

        #endregion

        #region Courses Fields

        private List<GUICourseTemplate> coursesList = new List<GUICourseTemplate>();

        public IList<IRP1_Course> getCourseButtons
        {
            get { return new List<IRP1_Course>(coursesList.ToArray()); }
        }

        private ActiveCourse newCourse = null;

        #endregion

        #region Tooling Fields

        private List<IRP1_Tooling> untooledPartsList = new List<IRP1_Tooling>();

        public IList<IRP1_Tooling> getUntooledParts
        {
            get => untooledPartsList;
        }


        private struct untooledPart : IRP1_Tooling
        {
            public string name;
            public float toolingCost;
            public float untooledMultiplier;
            public float totalCost;

            public string partName
            {
                get => name;
            }

            public float partToolingCost
            {
                get => toolingCost;
            }

            public float partTooledCost
            {
                get => totalCost;
            }

            public float partUntooledCost
            {
                get => totalCost - GetUntooledExtraCost(this);
            }
        }

        private float _allTooledCost;
        public float allTooledCost
        {
            get => _allTooledCost;
        }

        public IList<string> getExistingToolingList
        {
            get => new List<string>(ToolingDatabase.toolings.Keys); 
        }

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

        private void Start()
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

        public void clampWindow(RectTransform rect)
        {
            UIMasterController.ClampToScreen(rect, Vector2.zero);
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

            generateCourseTemplateList();

            windowGenerating = true;

            UIWindow = Instantiate(RP1Loader.WindowPrefab, UIMasterController.Instance.appCanvas.transform, false).GetComponent<RP1_MainPanel>();

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
            updateAstronautList();

            updateUntooledParts();
            UIWindow.updateToolingMainMenu(untooledPartsList);
            //UIWindow.updateMaintenanceWindow();
        }

        #endregion

        #region Astronaut Methods

        public void generateAstronautList()
        {
            updateActiveMap();

            if(crewMembers.Count != 0)
                crewMembers.Clear();

            foreach (ProtoCrewMember pcm in HighLogic.CurrentGame.CrewRoster.Crew)
            {
                ActiveCourse pcmActiveCourse = null;

                if (activeMap.ContainsKey(pcm))
                    pcmActiveCourse = activeMap[pcm];

                GUICrewMember member = new GUICrewMember(pcm, pcmActiveCourse, this);

                crewMembers.Add(member);
            }
        }

        public void updateAstronautList()
        {
            updateActiveMap();

            if(crewMembers.Count == 0)
                return;

            foreach (GUICrewMember naut in crewMembers)
            {
                naut.course = null;

                if (activeMap.ContainsKey(naut.self))
                    naut.course = activeMap[naut.self];
            }

            //UIWindow.updateAllNauts(UIWindow.astronauts);
            //UIWindow.updateAllNauts(UIWindow.courseStartListAstronauts);
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

        #endregion

        #region Courses Methods

        //TODO: Update the courses List periodically
        public void generateCourseTemplateList()
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
            Debug.Log("[RP-1] New course prepared " + newCourse.name);
        }

        public void addCrewToNewCourse(ProtoCrewMember toAdd)
        {
            newCourse?.AddStudent(toAdd);
        }

        public void removeCrewFromNewCourse(ProtoCrewMember toRemove)
        {
            newCourse?.RemoveStudent(toRemove);
        }

        public void startNewCourse()
        {
            if (newCourse.Students.Count > 0)
            {
                if(newCourse.StartCourse())
                {
                    //Debug stuff
                    foreach (ProtoCrewMember pcm in newCourse.Students)
                    {
                        Debug.Log("[RP-1] " + pcm.name + " Added to course " + newCourse.name);
                    }

                    if(newCourse.Students.Count == 0)
                        return;

                    CrewHandler.Instance.ActiveCourses.Add(newCourse);
                    CourseTemplate temporaryTemplate = newCourse;
                    newCourse = new ActiveCourse(temporaryTemplate);
                    MaintenanceHandler.Instance.UpdateUpkeep();
                    updateAstronautList();
                    UIWindow.updateCourseStartPanel();
                }
            }
        }

        public bool doesStudentMeetCourseReqs(ProtoCrewMember pcm)
        {
            if (newCourse == null)
                return false;
            

            return newCourse.MeetsStudentReqs(pcm);;
        }

        #endregion

        #region Tooling Methods

        public void updateUntooledParts()
        {
            if(!HighLogic.LoadedSceneIsEditor)
                return;

            untooledPartsList.Clear();
            float totalUntooledExtraCost = 0;

            if (EditorLogic.fetch != null && EditorLogic.fetch.ship != null && EditorLogic.fetch.ship.Parts.Count > 0) {
                for (int i = EditorLogic.fetch.ship.Parts.Count; i-- > 0;) {
                    Part p = EditorLogic.fetch.ship.Parts[i];
                    for (int j = p.Modules.Count; j-- > 0;) {
                        if (p.Modules[j] is ModuleTooling mT && !mT.IsUnlocked()) {
                            untooledPart uP;
                            uP.name = $"{p.partInfo.title} ({mT.ToolingType}) {mT.GetToolingParameterInfo()}";
                            uP.toolingCost = mT.GetToolingCost();
                            uP.untooledMultiplier = mT.untooledMultiplier;
                            uP.totalCost = p.GetModuleCosts(p.partInfo.cost) + p.partInfo.cost;
                            totalUntooledExtraCost += GetUntooledExtraCost(uP);
                            untooledPartsList.Add(uP);
                        }
                    }
                }
            }

            _allTooledCost = EditorLogic.fetch.ship.GetShipCosts(out _, out _) - totalUntooledExtraCost;
        }

        private static float GetUntooledExtraCost(untooledPart uP)
        {
            return uP.toolingCost * uP.untooledMultiplier;
        }

        public void toolAllParts()
        { 
            var untooledParts = EditorLogic.fetch.ship.Parts.Slinq().SelectMany(p => p.FindModulesImplementing<ModuleTooling>().Slinq())
                                                                            .Where(mt => !mt.IsUnlocked())
                                                                            .ToList();

            float totalToolingCost = ModuleTooling.PurchaseToolingBatch(untooledParts, isSimulation: true);
            bool canAfford = Funding.Instance.Funds >= totalToolingCost;
            PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new MultiOptionDialog(
                    "ConfirmAllToolingsPurchase",
                    $"Tooling for all untooled parts will cost {totalToolingCost:N0} funds.",
                    "Tooling Purchase",
                    HighLogic.UISkin,
                    new Rect(0.5f, 0.5f, 150f, 60f),
                    new DialogGUIFlexibleSpace(),
                    new DialogGUIVerticalLayout(
                        new DialogGUIFlexibleSpace(),
                        new DialogGUIButton(canAfford ? "Purchase All Toolings" : "Can't Afford",
                            () =>
                            {
                                if (canAfford)
                                {
                                    ModuleTooling.PurchaseToolingBatch(untooledParts);
                                    untooledParts.ForEach(mt =>
                                    {
                                        mt.Events["ToolingEvent"].guiActiveEditor = false;
                                    });
                                    GameEvents.onEditorShipModified.Fire(EditorLogic.fetch.ship);
                                }
                            }, 140.0f, 30.0f, true),
                        new DialogGUIButton("Close", () => { }, 140.0f, 30.0f, true)
                        )),
                false,
                HighLogic.UISkin);
        }


        private string[] GetTypeHeadings(Parameter[] parameters)
        {
            string[] toReturn = new string[parameters.Length];

            toReturn[0] = parameters[0].Title;
            for (int i = 1; i < parameters.Length; ++i)
            {
                toReturn[i] = parameters[i].Title;
            }

            return toReturn;
        }

        private string[] DisplayRow(float[] values, Parameter[] parameters)
        {
            string[] toReturn = new string[values.Length];

            toReturn[0] = $"{values[0]:F3} {parameters[0].Unit}";
            for (int i = 1; i < values.Length; ++i)
            {
                toReturn[i] = $"{values[i]:F3} {parameters[i].Unit}";
            }

            return toReturn;
        }

        public List<string[]> DisplayTypeTab(string currentToolingType)
        {
            List<string[]> toReturn = new List<string[]>();
            var parameters = Parameters.GetParametersForToolingType(currentToolingType);
            toReturn.Add(GetTypeHeadings(parameters));
            
            var entries = ToolingDatabase.toolings[currentToolingType];
            var values = new float[parameters.Length];
            DisplayRows(entries, 0, values, parameters, toReturn);

            return toReturn;
        }

        private void DisplayRows(List<ToolingEntry> entries, int parameterIndex, float[] values, Parameter[] parameters, List<string[]> toReturn)
        {
            if(parameterIndex == parameters.Length)
            {
                toReturn.Add(DisplayRow(values, parameters));
                return;
            }

            foreach (var toolingEntry in entries)
            {
                values[parameterIndex] = toolingEntry.Value;
                DisplayRows(toolingEntry.Children, parameterIndex + 1, values, parameters, toReturn);
            }
        }

        #endregion
    }
}

