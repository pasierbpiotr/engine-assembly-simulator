/// <summary>
/// Wyliczenie reprezentujące możliwe stany montażu części.
/// </summary>
public enum AssemblyState
{
    /// <summary>
    /// Część jest zablokowana i nie można jej używać ani montować.
    /// </summary>
    Locked,

    /// <summary>
    /// Część jest odblokowana i można ją montować.
    /// </summary>
    Unlocked,

    /// <summary>
    /// Część została zamontowana i nie można już jej edytować ani przesuwać.
    /// </summary>
    Assembled
}
