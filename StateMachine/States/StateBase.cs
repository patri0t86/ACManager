namespace ACManager.StateMachine.States
{
    internal abstract class StateBase<T>
        where T : StateBase<T>, new()
    {
        public static T GetInstance { get; } = new T();

        /// <summary>
        /// Overrides the ToString method for nicer output.
        /// </summary>
        /// <returns></returns>
        public abstract override string ToString();
    }
}
