using Decal.Adapter;
using System;

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
                if (Utility.BotSettings.BotPositioning)
                {
                    if (Utility.BotSettings.DesiredLandBlock.Equals(0) && Utility.BotSettings.DesiredBotLocationX.Equals(0) && Utility.BotSettings.DesiredBotLocationY.Equals(0))
                    {
                        Utility.BotSettings.DesiredLandBlock = CoreManager.Current.Actions.Landcell;
                        Utility.BotSettings.DesiredBotLocationX = CoreManager.Current.Actions.LocationX;
                        Utility.BotSettings.DesiredBotLocationY = CoreManager.Current.Actions.LocationY;
                        Debug.ToChat("Bot location set to current location since one was not set previously.");
                    }
                    else if (!machine.InPosition())
                    {
                        TempHeading = TargetHeading(CoreManager.Current.Actions.Landcell, Utility.BotSettings.DesiredLandBlock, CoreManager.Current.Actions.LocationX, CoreManager.Current.Actions.LocationY, Utility.BotSettings.DesiredBotLocationX, Utility.BotSettings.DesiredBotLocationY);

                        if (!CorrectHeading(CoreManager.Current.Actions.Heading))
                        {
                            if (AutoRunning())
                            {
                                CoreManager.Current.Actions.SetAutorun(false);
                            }
                            else if (!IsTurning())
                            {
                                CoreManager.Current.Actions.FaceHeading(TempHeading, false);
                            }
                        }
                        else if (!IsTurning())
                        {
                            if (!AutoRunning())
                            {
                                CoreManager.Current.Actions.SetAutorun(true);
                            }
                        }
                    }
                    else
                    {
                        if (AutoRunning())
                        {
                            CoreManager.Current.Actions.SetAutorun(false);
                        }

                        if (!machine.CorrectHeading() && !IsTurning())
                        {
                            CoreManager.Current.Actions.FaceHeading(machine.CurrentRequest.Heading, false);
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
                    if (!IsTurning())
                    {
                        CoreManager.Current.Actions.FaceHeading(machine.CurrentRequest.Heading, false);
                    }
                }
                else
                {
                    machine.NextState = Idle.GetInstance;
                }
                LastLocationX = CoreManager.Current.Actions.LocationX;
                LastLocationY = CoreManager.Current.Actions.LocationY;
                LastHeading = CoreManager.Current.Actions.Heading;
            }
            else
            {
                machine.NextState = Idle.GetInstance;
            }
        }

        private bool AutoRunning()
        {
            return !(CoreManager.Current.Actions.LocationX == LastLocationX && CoreManager.Current.Actions.LocationY == LastLocationY);
        }

        private bool IsTurning()
        {
            return !CoreManager.Current.Actions.Heading.Equals(LastHeading);
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
