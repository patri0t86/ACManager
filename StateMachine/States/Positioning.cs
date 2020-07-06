using System;

namespace ACManager.StateMachine.States
{
    /// <summary>
    /// State that is used to place the bot in the desired (X, Y) coordinate location in the world.
    /// </summary>
    class Positioning : StateBase<Positioning>, IState
    {
        double tempHeading;
        bool autoRunEnabled = false;

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
                if (!(Math.Abs(machine.Core.Actions.LocationX - machine.DesiredBotLocationX) < 1 && Math.Abs(machine.Core.Actions.LocationY - machine.DesiredBotLocationY) < 1 && machine.Core.Actions.Landcell == machine.DesiredLandBlock))
                {
                    tempHeading = GetHeading(machine.Core.Actions.Landcell, machine.DesiredLandBlock, machine.Core.Actions.LocationX, machine.Core.Actions.LocationY, machine.DesiredBotLocationX, machine.DesiredBotLocationY);

                    if (machine.Core.Actions.Heading <= tempHeading + 2 && machine.Core.Actions.Heading >= tempHeading - 2)
                    {
                        if (!autoRunEnabled)
                        {
                            machine.Core.Actions.SetAutorun(true);
                            autoRunEnabled = !autoRunEnabled;
                        }
                    }
                    else
                    {
                        if (autoRunEnabled)
                        {
                            machine.Core.Actions.SetAutorun(false);
                            autoRunEnabled = !autoRunEnabled;
                        }
                        machine.Core.Actions.Heading = tempHeading;

                    }
                }
                else // Have made it to the desired location
                {
                    if (autoRunEnabled)
                    {
                        machine.Core.Actions.SetAutorun(false);
                        autoRunEnabled = !autoRunEnabled;
                    }
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
