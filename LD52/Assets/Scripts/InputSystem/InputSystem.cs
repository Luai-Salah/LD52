namespace LD52.InputSystem
{
    public static class InputSystem
    {
        public static InputActions.PlayerActions Player { get; private set; }

        private static InputActions s_Instance;

        public static void Init()
        {
            s_Instance = new InputActions();
            Player = s_Instance.Player;
        }

        public static void Shutdown()
        {
            s_Instance.Disable();
        }
    }
}
