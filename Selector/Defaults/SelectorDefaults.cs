namespace Selector.Defaults
{
    /// <summary>
    /// Default configuration values for selectors. Can be overridden when
    ///  calling selector constructors or assigning to selector properties
    /// </summary>
    public static class SelectorDefaults
    {
#region Fundamentals
        /// <summary>
        /// Default format selectors apply to options passed into them.
        /// </summary>
        public const string Format = "[{0}]";
        /// <summary>
        /// Number of characters needing to be excluded from the format string.
        /// Normally would be -3 to exclude "{0}" from any string calculations
        /// </summary>
        public const int ExcludedFormatCharacters = 3;
        /// <summary>
        /// The color of the background of a printed option inside the console when highlighted.
        /// </summary>
        public const ConsoleColor HighlightedBackgroundColor = ConsoleColor.Yellow;
        /// <summary>
        /// The color of printed option text inside the console when highlighted.
        /// </summary>
        public const ConsoleColor HighlightedForegroundColor = ConsoleColor.Black;

#endregion // Fundamentals

#region Horizontal Selector
        /// <summary>
        /// Default symbol that is displayed in the console when more options
        /// are available when scrolling left within a horizontal selector
        /// </summary>
        public const string ScrollLeftSymbol = "<... ";
        /// <summary>
        /// Default symbol that is displayed in the console when more options
        /// are available when scrolling right within a horizontal selector
        /// </summary>
        public const string ScrollRightSymbol = " ...>";
        /// <summary>
        /// Default amount of spacing mandatory when separating each option
        /// within a horizontal selector
        /// </summary>
        public const int OptionSpacing = 5;
        /// <summary>
        /// Default character to fill in the gap between options within a
        /// horizontal selector
        /// </summary>
        public const char FillCharacter = ' ';
#endregion // Horizontal Selector

#region Vertical Selector
        /// <summary>
        /// Default symbol that is displayed in the console when more options
        /// are available when scrolling up within a vertical selector
        /// </summary>
        public const string ScrollUpSymbol = " ^ \n...";
        /// <summary>
        /// Default symbol that is displayed in the console when more options
        /// are available when scrolling down within a vertical selector
        /// </summary>
        public const string ScrollDownSymbol = "...\n v ";
        /// <summary>
        /// Default cap on the number of options that is allowed to be displayed
        /// into the console by a vertical selector.
        /// </summary>
        public const int MaxOptions = int.MaxValue;
#endregion // Vertical Selector

#region Scroll Selectors
        public const ConsoleColor ScrollForegroundColor = ConsoleColor.Green;
#endregion
    }
}