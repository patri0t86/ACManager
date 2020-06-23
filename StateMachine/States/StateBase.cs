namespace ACManager.StateMachine.States
{
    internal abstract class StateBase<T>
        where T : StateBase<T>, new()
    {
        private static readonly T instance = new T();
        public static T GetInstance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Overrides the ToString method for nicer output.
        /// </summary>
        /// <returns></returns>
        public abstract override string ToString();
    }
}
