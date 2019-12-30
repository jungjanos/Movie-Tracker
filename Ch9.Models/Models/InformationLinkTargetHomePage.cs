namespace Ch9.Models
{
    /// <summary>
    /// encodes the setting of the target homepage to which information links will refer.
    /// e.g. when opening a detailed Person info page via browser
    /// enum. Invalid=0 is placeholder only, should NOT be used ! 
    /// </summary>
    public enum InformationLinkTargetHomePage
    {
        Invalid = 0,
        IMDb = 1,
        TMDb = 2
    }
}
