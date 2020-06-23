namespace ACManager.StateMachine.States
{
    /// <summary>
    /// The interface all states must derive from, and implement.
    /// Each method receives the parent State Machine as a parameter. This is how state variables are modified as the bot operates.
    /// </summary>
    internal interface IState
    {
        /// <summary>
        /// This method is called by the state machine every time a new state is entered. This is called only once.
        /// </summary>
        /// <param name="machine"></param>
        void Enter(Machine machine);

        /// <summary>
        /// This method is called by the state machine every time a state is exited. This is called only once.
        /// </summary>
        /// <param name="machine"></param>
        void Exit(Machine machine);

        /// <summary>
        /// This is the "thinking" in the state. The repetitive actions/checks are implemented here.
        /// </summary>
        /// <param name="machine"></param>
        void Process(Machine machine);
    }
}
