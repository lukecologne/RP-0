using System.Collections.Generic;
using RP0.Unity.Unity;
using UnityEngine;

namespace RP0.Unity.Interfaces
{
    public interface IRP1_MainPanel
    {
        bool IgnoreScale { get; set; }

        bool PixelPerfect { get; set; }

        bool isCareerMode { get; }

        bool KACAPIReady { get; }

        int currentScene { get; }

        #region MaintenanceMain

        double facilityUpkeep { get; }
        
        double integrationUpkeep { get; }
        
        double researchUpkeep { get; }
        
        double nautTotalUpkeep { get; }

        double totalUpkeep { get; }

        #endregion

        #region MaintenanceFacilities

        double LaunchPadTotalCost { get; }

        RP1_Maintenance.padLevelsAndCosts[] LaunchPadLevelsAndCosts { get; }

        double RunwayTotalCost { get; }

        double VABTotalCost { get; }

        double SPHTotalCost { get; }

        double RnDTotalCost { get; }

        double MissionControlTotalCost { get; }

        double TrackingStationTotalCost { get; }

        double AstronautComplexTotalCost { get; }

        double TotalFacilityCostCost { get; } 

        #endregion

        #region MaintenanceIntegration

        string[] integrationSites { get; }

        double[] integrationRates { get; }

        #endregion

        #region MaintenanceAstronauts

        int CorpsSize { get; }

        double AstronautBaseCost { get; }

        double AstronautOperationCost { get; }

        double AstronautTrainingCost { get; }

        #endregion

        string newActiveCourseDuration { get; }

        string newActiveCourseEndTime { get; }

        int addedCrewCount { get; }

        //Tooling
        float allTooledCost { get; }

        IList<string> getExistingToolingList { get; }

        List<string[]> DisplayTypeTab(string currentToolingType);

        void toolAllParts();


        float Scale { get; set; }

        float MasterScale { get; }

        string Version { get; }

        IList<IRP1_Astronaut> getAstronauts { get; }

        IList<IRP1_Course> getCourseButtons { get; }

        IList<IRP1_Tooling> getUntooledParts { get; }

        void SetWindowPosition(Rect r);

        void startNewCourse();

        void clampWindow(RectTransform rect);
    }
}
