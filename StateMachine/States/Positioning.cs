using System;
using System.Drawing;

namespace ACManager.StateMachine.States
{
    /// <summary>
    /// State that is used to place the bot in the desired (X, Y) coordinate location in the world.
    /// </summary>
    class Positioning : StateBase<Positioning>, IState
    {
        private double TempHeading { get; set; }
        private double LastHeading { get; set; }
        private double LastLocationX { get; set; }
        private double LastLocationY { get; set; }

        public void Enter(Machine machine)
        {

        }

        public void Exit(Machine machine)
        {

        }

        public void Process(Machine machine)
        {
            if (machine.Enabled)
            {
                if (machine.EnablePositioning)
                {
                    if (machine.DesiredLandBlock.Equals(0) && machine.DesiredBotLocationX.Equals(0) && machine.DesiredBotLocationY.Equals(0))
                    {
                        machine.DesiredLandBlock = machine.Core.Actions.Landcell;
                        machine.DesiredBotLocationX = machine.Core.Actions.LocationX;
                        machine.DesiredBotLocationY = machine.Core.Actions.LocationY;
                        Debug.ToChat("Bot location set to current location since one was not set previously.");
                    }
                    else if (!machine.InPosition())
                    {
                        TempHeading = TargetHeading(machine.Core.Actions.Landcell, machine.DesiredLandBlock, machine.Core.Actions.LocationX, machine.Core.Actions.LocationY, machine.DesiredBotLocationX, machine.DesiredBotLocationY);

                        if (!CorrectHeading(machine.Core.Actions.Heading))
                        {
                            if (AutoRunning(machine))
                            {
                                machine.Core.Actions.SetAutorun(false);
                            }
                            else if (!IsTurning(machine))
                            {
                                machine.Core.Actions.FaceHeading(TempHeading, false);
                            }
                        }
                        else if (!IsTurning(machine))
                        {
                            if (!AutoRunning(machine))
                            {
                                machine.Core.Actions.SetAutorun(true);
                            }
                        }
                    }
                    else
                    {
                        if (AutoRunning(machine))
                        {
                            machine.Core.Actions.SetAutorun(false);
                        }

                        if (!machine.CorrectHeading() && !IsTurning(machine))
                        {
                            machine.Core.Actions.FaceHeading(machine.NextHeading, false);
                        }
                        else
                        {
                            machine.NextState = Idle.GetInstance;
                        }
                    }
                }
                else if (!machine.CorrectHeading())
                {
                    // Positioning is disabled, but still need to face the correct direction for portals
                    if (!IsTurning(machine))
                    {
                        machine.Core.Actions.FaceHeading(machine.NextHeading, false);
                    }
                }
                else
                {
                    machine.NextState = Idle.GetInstance;
                }
                LastLocationX = machine.Core.Actions.LocationX;
                LastLocationY = machine.Core.Actions.LocationY;
                LastHeading = machine.Core.Actions.Heading;
            }
            else
            {
                machine.NextState = Idle.GetInstance;
            }
        }

        private bool AutoRunning(Machine machine)
        {
            return !(machine.Core.Actions.LocationX == LastLocationX && machine.Core.Actions.LocationY == LastLocationY);
        }

        private bool IsTurning(Machine machine)
        {
            return !machine.Core.Actions.Heading.Equals(LastHeading);
        }

        private bool CorrectHeading(double currentHeading)
        {
            return currentHeading <= TempHeading + 1 && currentHeading >= TempHeading - 1;
        }

        private double TargetHeading(int currentLB, int targetLB, double currentX, double currentY, double targetX, double targetY)
        {
            int currentLB_EW, currentLB_NS, targetLB_EW, targetLB_NS, headingModifier;

            currentLB_EW = EWLandblockToInt(currentLB);
            currentLB_NS = NSLandblockToInt(currentLB);

            targetLB_EW = EWLandblockToInt(targetLB);
            targetLB_NS = NSLandblockToInt(targetLB);

            if (LandblockToInt(currentLB).Equals(LandblockToInt(targetLB)))
            {
                if (currentX < targetX)
                {
                    headingModifier = 90;
                }
                else
                {
                    headingModifier = 270;
                }
            }
            else if (currentLB_EW < targetLB_EW)
            {
                headingModifier = 90;
            }
            else
            {
                headingModifier = 270;
            }
            return headingModifier - RadToDegrees(Math.Atan(CalculateCoordDiff(currentLB_NS, targetLB_NS, currentY, targetY) / CalculateCoordDiff(currentLB_EW, targetLB_EW, currentX, targetX)));
        }

        private double RadToDegrees(double radians)
        {
            return radians * (180 / Math.PI);
        }

        private int LandblockToInt(int landblock)
        {
            return Convert.ToInt32(landblock.ToString("X").Substring(0, 3), 16);
        }

        private int EWLandblockToInt(int landblock)
        {
            return Convert.ToInt32(landblock.ToString("X").Substring(0, 2), 16);
        }

        private int NSLandblockToInt(int landblock)
        {
            return Convert.ToInt32(landblock.ToString("X").Substring(2, 2), 16);
        }

        private double CalculateCoordDiff(int currentLBEW_NS, int targetLBEW_NS, double currentXY, double targetXY)
        {
            return currentXY + ((currentLBEW_NS - targetLBEW_NS) * 192) - targetXY;
        }

        public override string ToString()
        {
            return nameof(Positioning);
        }
    }
}
