using System;

namespace ACManager.StateMachine.States
{
    /// <summary>
    /// State that is used to place the bot in the desired (X, Y) coordinate location in the world.
    /// </summary>
    class Positioning : StateBase<Positioning>, IState
    {
        private double TempHeading { get; set; }
        private bool AutoRunEnabled { get; set; }
        private DateTime LastHeadingCheck { get; set; }
        private bool IsTurning { get; set; }

        public void Enter(Machine machine)
        {
            IsTurning = false;
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
                        TempHeading = GetHeading(machine.Core.Actions.Landcell, machine.DesiredLandBlock, machine.Core.Actions.LocationX, machine.Core.Actions.LocationY, machine.DesiredBotLocationX, machine.DesiredBotLocationY);

                        if (machine.Core.Actions.Heading <= TempHeading + 2 && machine.Core.Actions.Heading >= TempHeading - 2)
                        {
                            if (!AutoRunEnabled)
                            {
                                IsTurning = false;
                                AutoRunEnabled = !AutoRunEnabled;
                                machine.Core.Actions.SetAutorun(AutoRunEnabled);
                            }
                        }
                        else
                        {
                            if (AutoRunEnabled)
                            {
                                AutoRunEnabled = !AutoRunEnabled;
                                machine.Core.Actions.SetAutorun(AutoRunEnabled);
                            }
                            else if (!IsTurning)
                            {
                                IsTurning = !IsTurning;
                                machine.Core.Actions.FaceHeading(TempHeading, false);
                            }
                        }
                    }
                    else
                    {
                        if (AutoRunEnabled)
                        {
                            AutoRunEnabled = !AutoRunEnabled;
                            machine.Core.Actions.SetAutorun(AutoRunEnabled);
                        }
                        else if (!machine.CorrectHeading())
                        {
                            Turn(machine);
                        }
                        else
                        {
                            machine.NextState = Idle.GetInstance;
                        }
                    }
                }
                else if (!machine.CorrectHeading())
                {
                    Turn(machine);
                }
                else
                {
                    machine.NextState = Idle.GetInstance;
                }
            }
            else
            {
                machine.NextState = Idle.GetInstance;
            }
        }

        public override string ToString()
        {
            return nameof(Positioning);
        }

        private void Turn(Machine machine)
        {
            if (!IsTurning)
            {
                LastHeadingCheck = DateTime.Now;
                IsTurning = !IsTurning;
                machine.Core.Actions.FaceHeading(machine.NextHeading, false);
            }
            else if (DateTime.Now - LastHeadingCheck > TimeSpan.FromMilliseconds(1500) && !machine.Core.Actions.Heading.Equals(machine.NextHeading))
            {
                LastHeadingCheck = DateTime.Now;
                machine.Core.Actions.FaceHeading(machine.NextHeading, false);
            }
        }

        private double GetHeading(int currentLB, int targetLB, double currentX, double currentY, double targetX, double targetY)
        {
            int currentLBint, targetLBint, currentLB_EW, currentLB_NS, targetLB_EW, targetLB_NS, headingModifier;

            currentLBint = LandblockToInt(currentLB);
            targetLBint = LandblockToInt(targetLB);

            currentLB_EW = EWLandblockToInt(currentLB);
            currentLB_NS = NSLandblockToInt(currentLB);

            targetLB_EW = EWLandblockToInt(targetLB);
            targetLB_NS = NSLandblockToInt(targetLB);

            if (currentLBint.Equals(targetLBint))
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

    }
}
