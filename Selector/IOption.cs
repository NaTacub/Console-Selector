namespace Selector
{
    /// <summary>
    /// Interface required by every ISelector type
    /// </summary>
    public interface IOption
    {
        /// <summary>
        /// Display name when printed into the console through an ISelector
        /// </summary>
        string GetName();
        /// <summary>
        /// Display color when printed into the console through an ISelector
        /// </summary>
        ConsoleColor GetColor();
    }
}