namespace ACManager.StateMachine.States
{
    /// <summary>
    /// --- DO NOT USE THIS ---
    /// This is a template to copy/paste into a new state for ease of implementation/development.
    /// </summary>
    class StateTemplate : StateBase<StateTemplate>, IState
    {
        public void Enter(Machine machine)
        {
            // Do this stuff only once when entering the state
        }

        public void Exit(Machine machine)
        {
            // Do this stuff only once when exiting the state
        }

        public void Process(Machine machine)
        {
            if (machine.Enabled)
            {
                // Do stuff in here
            }
            else
            {
                machine.NextState = Idle.GetInstance;
            }
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
