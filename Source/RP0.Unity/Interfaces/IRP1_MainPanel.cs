using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngineInternal;

namespace RP0.Unity.Interfaces
{
    public interface IRP1_MainPanel
    {
        bool IgnoreScale { get; set; }

        bool PixelPerfect { get; set; }

        bool isCareerMode { get; }

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

        string[] LaunchPadLevels { get; }

        double[] LauchPadLevelsCost { get; }

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


        float Scale { get; set; }

        float MasterScale { get; }

        string Version { get; }

        IList<IRP1_Astronaut> getAstronauts { get; }

        IList<IRP1_Course> getCourseButtons { get; }

        void SetWindowPosition(Rect r);

        void startNewCourse();
    }
}
