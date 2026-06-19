namespace Selector {
    /// <summary>
    /// Expected functionality of any Selector class
    /// </summary>
    /// <typeparam name="TOption">Type of the data being selected</typeparam>
    public interface ISelector<TOption> where TOption : IOption
    {
        /// <summary>
        /// Data applied to all slots initialized by the selector.
        /// </summary>
        SelectorModel Model { get; }
        /// <summary>
        /// Returns the current option being selected by the Selector
        /// </summary>
        TOption? GetCurrent();
        /// <summary>
        /// True if the selector has no options in it.
        /// </summary>
        bool IsEmpty();
        /// <summary>
        /// Prints out a menu of options
        /// </summary>
        void PrintSelection();
        
        /// <summary>
        /// Clear the printed menu from the console
        /// </summary>
        void ClearSelection();
        /// <summary>
        /// Prints a menu of options and queries the user to select between them.
        /// </summary>
        /// <returns>The option selected</returns>
        TOption? QuerySelection();
    }

    /// <summary>
    /// Contains data that applies to all slots of a selector
    /// </summary>
    public class SelectorModel
    {
        public ConsoleColor highlightedBackgroundColor;
        public ConsoleColor highlightedForegroundColor;
        public readonly string format;
        public SelectorModel(string format) {
            this.format = format;
        }
    }
}