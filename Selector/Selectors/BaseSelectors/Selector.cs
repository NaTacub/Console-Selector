namespace Selector
{
    using Resources;
    using Defaults;

    /// <summary>
    /// Base class for any selector. Defines helper methods and common input variables for convenience.
    /// </summary>
    /// <typeparam name="TOption">Type of the data being selected</typeparam>
    /// <typeparam name="TSlot">Slot type encapsulating data to be used for selectors</typeparam>
    public abstract class Selector<TOption, TSlot> : ISelector<TOption>
        where TOption : IOption
        where TSlot : Slot<TOption>, new()
    {
        /// <summary>
        /// Shared data across all Slot instances inside the Selector. Use to configure highlight color
        /// </summary>
        public SelectorModel Model { get; private set; }
        /// <summary>
        /// User input which selects the currently selected option
        /// </summary>
        public virtual ConsoleKey ConfirmationKey { get; set; } = ConsoleKey.Enter;
        /// <summary>
        /// User input to move to the next available option
        /// </summary>
        public abstract ConsoleKey IncrementKey { get; }
        /// <summary>
        /// User input to move to a previous option
        /// </summary>
        public abstract ConsoleKey DecrementKey { get; }
        /// <summary>
        /// The next console line unused by the selector
        /// </summary>
        public abstract int NextEmptyLine { get; }

        
        /// <summary>
        /// Get the currently highlighted index. Clamps any assigned value to a valid index.
        /// </summary>
        public int CurrentIndex
        {
            get => currentIndex;
            protected set 
            {
                if (value == currentIndex)
                    return;

                PreviousIndex = currentIndex;
                currentIndex = options.Count > 0 ? Math.Clamp(value, 0, options.Count - 1) : 0;
            }
        }
        private int currentIndex;

        /// <summary>
        /// The previously highlighted index
        /// </summary>
        public int PreviousIndex { get; private set; }

        protected IObjectSource<TSlot> options = null!;

        protected bool selectionPrinted = false;

        /// <param name="selectorOptions">Read only. Options selectable by the user</param>
        /// <param name="format">String format each option uses when displayed in the console. Requires {0}</param>
        /// <param name="populateOptions">Whether the constructor should call PopulateOptions(...) or lend the task to another class</param>
        /// <exception cref="ArgumentException">selectorOptions must not have a length of 0</exception>
        public Selector(IReadOnlyList<TOption> selectorOptions, string format = SelectorDefaults.Format, bool populateOptions = true) {
            if (selectorOptions.Count == 0) {
                throw new ArgumentException("Cannot pass selectorOptions of length 0 into a type " + nameof(Selector));
            }

            Model = new SelectorModel(format) {
                highlightedBackgroundColor = SelectorDefaults.HighlightedBackgroundColor,
                highlightedForegroundColor = SelectorDefaults.HighlightedForegroundColor
            };

            if (populateOptions) {
                options = PopulateOptions(selectorOptions);
            }
        }
        /// <summary>
        /// Single use method called inside of the constructor. Wraps all options into usable slots for the selector.
        /// </summary>
        protected virtual IObjectSource<TSlot> PopulateOptions(IReadOnlyList<TOption> selectorOptions) {
            var source = new SlotSource<TOption, TSlot, Selector<TOption, TSlot>>(this, selectorOptions);
            source.InitializeAll();
            return source;
        }

        public TOption? GetCurrent() => options.Count == 0 || CurrentIndex >= options.Count ? default : options[CurrentIndex].instance;
        public bool IsEmpty() => options.Count == 0;

        public abstract void PrintSelection();
        public abstract void ClearSelection();

        // Updates the menu's appearance if any selector values were changed
        protected virtual void UpdateSelection() {
            if (PreviousIndex == currentIndex)
                return;

            options[PreviousIndex].Unhighlight();
            options[CurrentIndex].Highlight();
        }

        public virtual TOption? QuerySelection() {
            if (IsEmpty()) {
                return default;
            }
            Console.CursorVisible = false;
            
            PrintSelection(); // print current cursor menu

            options[CurrentIndex].Highlight();

            // loop until we select an option
            while (true) {
                // read a key press from the user
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                // move to next option
                if (keyInfo.Key == IncrementKey)
                    CurrentIndex++;
                // move to previous option
                else if (keyInfo.Key == DecrementKey)
                    CurrentIndex--;
                // select at CurrentIndex
                else if (keyInfo.Key == ConfirmationKey)
                    break;
                // update menu after user input
                UpdateSelection();
            }
            // cursor positon set to the line under the selection area
            Console.SetCursorPosition(0, NextEmptyLine);

            Console.CursorVisible = true;

            return options[CurrentIndex].instance;
        }


        /// <summary>
        /// Replaces an entire row with spaces
        /// </summary>
        /// <param name="rowIndex">Console row index</param>
        /// <param name="saveOriginalCursorPosition">preserve the original cursor position before method was called. If <c>false</c>, cursor position will be at the beginning of the cleared row</param>
        public static void ClearConsoleLine(int rowIndex, bool saveOriginalCursorPosition)
        {
            // default cursor position to the beginning of the current row
            int previousCol = 0;
            int previousRow = rowIndex;
            // save the current cursor position
            if (saveOriginalCursorPosition) {
                previousCol = Console.CursorLeft;
                previousRow = Console.CursorTop;
            }
            // fill the row with spaces
            Console.SetCursorPosition(0, rowIndex);
            Console.Write(new string(' ', Console.WindowWidth));
            // move to previous or new cursor position
            Console.SetCursorPosition(previousCol, previousRow);
        }
    }
}