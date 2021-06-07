namespace ACManager.StateMachine.States
{
    /// <summary>
    /// --- DO NOT USE THIS ---
    /// This is a template to copy/paste into a new state for ease of implementation/development.
    /// </summary>
    class StateTemplate : StateBase<StateTemplate>, IState
    {
        public void Enter(Machine machine) {}

        public void Exit(Machine machine) {}

        public void Process(Machine machine)
        {
            if (!machine.Enabled)
            {
                machine.NextState = Idle.GetInstance; 
            }
        }

        public override string ToString()
        {
            return nameof(StateTemplate);
        }
    }
}
