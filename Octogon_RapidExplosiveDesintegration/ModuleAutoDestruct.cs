using System;
using KSP.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Octogon
{

    // This class provides a part module to run the abort action group on certain conditions
    // and destroy the vessel the part is attached to. 

    public class ModuleAutoDestruct : PartModule
    {

        // state of the module variables
        private double last_warning = 0;
        private bool trigger_auto_destruct = false;
        private double time_triggered = 0;
        private bool auto_destruct_active = false;

        // the tweakable settings of the part - and GUI
        [KSPField(isPersistant = true, guiActive = true, guiName = "#autoLOC_OCTO_RED_IGNITION_DELAY", guiUnits = "sec")]
        [UI_FloatRange(maxValue = 10, minValue = 0, stepIncrement = 1)]
        public float ignition_delay = 5;

        [KSPField(isPersistant = true, guiActive = true, guiName = "#autoLOC_OCTO_RED_ALTITUDE_LIMIT", guiUnits = "m")]
        [UI_FloatRange(maxValue = 100000, minValue = 0, stepIncrement = 500)]
        public float shutdown_altitude = 15000;

        [KSPField(isPersistant = true, guiActive = true, guiName = "#autoLOC_OCTO_RED_AOA_LIMIT", guiUnits="°")]
        [UI_FloatRange(maxValue = 180, minValue = 0, stepIncrement = 1)]
        public float aoa_limit = 60;

        [KSPField(isPersistant = true, guiActive=true, guiName="#autoLOC_OCTO_RED_ENFORCE_AOA")]
        [UI_Toggle(disabledText= "#autoLOC_OCTO_DISABLED", enabledText= "#autoLOC_OCTO_ENABLED", invertButton = true)]
        public Boolean destroy_on_AoA = true;

        [KSPField(isPersistant = true, guiActive = true, guiName = "#autoLOC_OCTO_RED_ENFORCE_NO_CONTROL")]
        [UI_Toggle(disabledText = "#autoLOC_OCTO_DISABLED", enabledText = "#autoLOC_OCTO_ENABLED", invertButton = true)]
        public Boolean destroy_on_uncontrolable = true;

        // during game cycle check for vessel state - handle abort if triggered
        public override void OnUpdate()
        {
            base.OnUpdate();
            if (vessel.terrainAltitude > shutdown_altitude) return;

            if (vessel.IsControllable != true && destroy_on_uncontrolable) trigger_auto_destruct = true;
            if (checkAttitude() > aoa_limit && destroy_on_AoA) trigger_auto_destruct = true;

            if (trigger_auto_destruct)
            {
                executeAbort();
            }

            if (auto_destruct_active)
            {
                autodestructVessel();
            }
        }

        // executes the abort action group and write event to log
        public void executeAbort()
        {
            if (auto_destruct_active) return;

            string log_entry = Localizer.GetStringByTag("#autoLOC_OCTO_RED_SELF_DESTRUCT_ACTIVATED");
            FlightLogger.eventLog.Add(log_entry);

            ActionGroupList groups = vessel.ActionGroups;
            groups.SetGroup(KSPActionGroup.Abort, true);
        }

        // Self destruct action for the action groups 
        [KSPAction(guiName = "#autoLOC_OCTO_RED_SELF_DESTRUCT", actionGroup = KSPActionGroup.Abort)]
        public void startSelfDestruct(KSPActionParam param)
        {
            ScreenMessages.PostScreenMessage(
                "#autoLOC_OCTO_RED_SELF_DESTRUCT_ACTIVATED", 
                10.0f, ScreenMessageStyle.UPPER_CENTER
            );
            time_triggered = Planetarium.GetUniversalTime();
            auto_destruct_active = true;
        }

        // calculate angle of attack and generate warnings if limits are reached!
        public double checkAttitude()
        {
            // Do not check AoA below 15 m/s
            if (vessel.srfSpeed < 15) {
                return 0;
            }

            Vector3d prograde = vessel.srf_velocity;
            Vector3d ship = vessel.GetTransform().up;
            double AoA = Vector3d.Angle(ship, prograde);

            // dangerous values reached issue a warning!
            if (AoA > aoa_limit * .25)
            {
                double now = Planetarium.GetUniversalTime();
                string warning = "#autoLOC_OCTO_RED_AOA_WARN";
                if (AoA > aoa_limit * .5)
                { 
                    warning = "#autoLOC_OCTO_RED_AOA_CRIT";
                }
            
                if (now > last_warning + 2) { 
                    ScreenMessage myInfo = ScreenMessages.PostScreenMessage(
                        warning,
                        2f, ScreenMessageStyle.UPPER_CENTER
                    );
                    last_warning = now;
                }
            }
            return AoA;
        }

        // This actually destroys the vessel ... 
        public void autodestructVessel()
        {
            double now = Planetarium.GetUniversalTime();
            if (now < time_triggered + ignition_delay) return;

            // critically overheat each part, let the engine handle the rest
            foreach (Part part in vessel.parts)
            {
                int blast_temperature = 10000;
                part.temperature = blast_temperature;
            }
            vessel.state = Vessel.State.DEAD;
        }
    }
}