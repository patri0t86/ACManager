namespace ACManager.Settings
{
    public enum PortalType
    {
        Primary,
        Secondary
    }

    public class Portal
    {
        public PortalType Type;
        public string Keyword;
        public string Description;
    }
}
