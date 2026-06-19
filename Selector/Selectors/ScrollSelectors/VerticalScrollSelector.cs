using System.Text;

namespace Selector
{
    using Resources;
    using Defaults;

    /// <summary>
    /// Vertical Selector supporting any number of options passed into it.
    /// Any options not visible inside the console can be revealed by scrolling up or down in the menu
    /// </summary>
    /// <typeparam name="TOption">Type of the data being selected</typeparam>
    public class VerticalScrollSelector<TOption> : VerticalSelectorBase<TOption, VerticalScrollSlot<TOption>> where TOption : IOption
    {
        /// <summary>
        /// Color of the symbols signifying more options are available in a particular direction.
        /// </summary>
        public ConsoleColor ScrollSymbolColor { get; set; } = SelectorDefaults.ScrollForegroundColor;

        /// <summary>
        /// String that appears when more options are available when scrolling upward when selecting
        /// </summary>
        public string ScrollUpSymbol
        {
            get => scrollUpSymbol;
            set
            {
                scrollUpSymbol = value;
                UpdateSelector();
            }
        }
        string scrollUpSymbol = SelectorDefaults.ScrollUpSymbol;

        
        /// <summary>
        /// String that appears when more options are available when scrolling downward when selecting
        /// </summary>
        public string ScrollDownSymbol
        {
            get => scrollDownSymbol;
            set
            {
                scrollDownSymbol = value;
                UpdateSelector();
            }
        }
        string scrollDownSymbol = SelectorDefaults.ScrollDownSymbol;

        /// <summary>
        /// Prepopulated string containing every option formatted by the selector.
        /// Each option is separated by a new line '\n'
        /// </summary>
        public string SelectionText { get; private set; }

        int upSymbolHeight, downSymbolHeight;
        int leftBoundOptionIndex, rightBoundOptionIndex;

        /// <summary>
        /// Max number of options that can be displayed to the console. Rejects any values less than 1.
        /// </summary>
        public int MaxDisplayOptions
        {
            get => maxOptionsDisplayed;
            set
            {
                if (value < 1)
                    return;

                maxOptionsDisplayed = Math.Min(value, options.Count);
            }
        }
        int maxOptionsDisplayed;

        /// <summary>
        /// Initialize a vertical scroll selector. Creates and stores printed menu internally.
        /// <c>selectorOptions</c> must be of length > 0
        /// </summary>
        /// <param name="selectorOptions">Read only. Options selectable by the user</param>
        /// <param name="format">String format each option uses when displayed in the console. Requires {0}</param>
        /// <exception cref="ArgumentException">selectorOptions must not have a length of 0</exception>
        public VerticalScrollSelector(IReadOnlyList<TOption> selectorOptions, string format = SelectorDefaults.Format) : base(selectorOptions, format) {
            MaxDisplayOptions = SelectorDefaults.MaxOptions;

            int estimateCapacity = selectorOptions[0].GetName().Length * (selectorOptions.Count + 1); 
            StringBuilder stringBuilder = new StringBuilder(estimateCapacity);

            stringBuilder.Append(options[0].formattedName + new string(' ', Console.WindowWidth - options[0].formattedName.Length - 2));
            for (int i = 1; i < options.Count; i++) {
                stringBuilder.Append('\n');
                options[i].selectionIndex = stringBuilder.Length;
                stringBuilder.Append(options[i].formattedName + new string(' ', Console.WindowWidth - options[i].formattedName.Length - 2));
            }

            SelectionText = stringBuilder.ToString();
        }

        protected override IObjectSource<VerticalScrollSlot<TOption>> PopulateOptions(IReadOnlyList<TOption> selectorOptions) {
            UpdateSelector();
            return base.PopulateOptions(selectorOptions);
        }

        public override void PrintSelection() {
            int linesBelow = Console.WindowHeight - Console.CursorTop - 1; // number of lines below the cursor
            int requiredHeight = maxOptionsDisplayed + upSymbolHeight + downSymbolHeight + 1; // + 1 for extra empty space below
            CurrentIndex = 0;

            // if the required space to print the selection menu surpasses the amount of space available at the current cursor position,
            // move the cursor upward to create more space.
            if (linesBelow < requiredHeight) {
                int newCursorTop = Console.WindowHeight - requiredHeight - 1;
                // if the console does not provide enough space for us, lower number of options we can display
                if (newCursorTop < 0) {
                    maxOptionsDisplayed += newCursorTop;
                    requiredHeight += newCursorTop;
                    newCursorTop = 0;
                }
                Console.SetCursorPosition(0, newCursorTop);
            }

            FirstRow = Console.CursorTop;
            LastRow = FirstRow + requiredHeight - 2;
            nextEmptyLine = LastRow + 1;
            
            leftBoundOptionIndex = CurrentIndex;
            rightBoundOptionIndex = CurrentIndex + maxOptionsDisplayed - 1;
            // adjust leftBound if we cannot display anymore options on the right
            if (rightBoundOptionIndex > options.Count - 1) {
                rightBoundOptionIndex = options.Count - 1;
                leftBoundOptionIndex = rightBoundOptionIndex - maxOptionsDisplayed - 1; 
            }

            UpdatePrintedText();

            selectionPrinted = true;
        }

       protected override void UpdateSelection() {
            if (PreviousIndex == CurrentIndex)
                return;
            // If scrolling isn't necessary, update as normal
            if (CurrentIndex >= leftBoundOptionIndex && CurrentIndex <= rightBoundOptionIndex) {
                base.UpdateSelection();
                return;
            }

            options[PreviousIndex].highlighted = false;

            // Scroll up
            if (CurrentIndex < PreviousIndex) {
                leftBoundOptionIndex = CurrentIndex;
                rightBoundOptionIndex = Math.Max(rightBoundOptionIndex - 1, 0);
            }
            // Scroll down
            else {
                leftBoundOptionIndex = Math.Min(leftBoundOptionIndex + 1, options.Count - 1);
                rightBoundOptionIndex = CurrentIndex;
            }

            UpdatePrintedText();

            options[CurrentIndex].Highlight();
        }

        // Expects option indexes to be in the correct place
        void UpdatePrintedText() {
            // calculate rows for selected options
            int firstOptionRow = FirstRow + upSymbolHeight;

            UpdateOptionRows(firstOptionRow);

            Console.SetCursorPosition(0, firstOptionRow);
            WriteSelectionText();

            WriteScrollSymbols();
        }

        // Updating option row positions so we can select or deselect them in the console
        void UpdateOptionRows(int startRow) {
            for (int i = leftBoundOptionIndex; i <= rightBoundOptionIndex; i++) {
                options[i].Row = startRow++;
            }
        }

        // Write the current set of options into the console. Expects option indexes to be in the correct place 
        void WriteSelectionText() {
            // set bounds of a substring. Use formattedName.Length over nameLength since the string doesn't hide ASCII escape characters
            int leftBoundTextIndex = options[leftBoundOptionIndex].selectionIndex;
            int rightBoundTextIndex = options[rightBoundOptionIndex].selectionIndex + options[rightBoundOptionIndex].formattedName.Length;
            Console.Write(SelectionText.AsSpan(leftBoundTextIndex, rightBoundTextIndex - leftBoundTextIndex));
            // Remove any trailing characters left behind from the last element
            Console.Write(new string(' ', Console.WindowWidth - options[rightBoundOptionIndex].formattedName.Length - 2));
        }
        // Write or clear the up and down scroll symbols
        void WriteScrollSymbols() {
            int col = Console.CursorLeft;
            int row = Console.CursorTop;

            if (leftBoundOptionIndex != 0) {
                Console.SetCursorPosition(0, FirstRow);
                WriteUpSymbol();
            } else {
                for (int i = FirstRow; i < FirstRow + upSymbolHeight; i++) {
                    ClearConsoleLine(i, false);
                }
            }

            int targetRow = LastRow - downSymbolHeight + 1;

            if (rightBoundOptionIndex != options.Count - 1) {
                Console.SetCursorPosition(0, targetRow);
                WriteDownSymbol();
            } else {
                for (int i = targetRow; i < targetRow + downSymbolHeight; i++) {
                    ClearConsoleLine(i, false);
                }
            }
            // return to original cursor position
            Console.SetCursorPosition(col, row);
        }

        void WriteUpSymbol() {
            var prevColor = Console.ForegroundColor;

            Console.ForegroundColor = ScrollSymbolColor;
            Console.Write(ScrollUpSymbol);
            Console.ForegroundColor = prevColor;
        }

        void WriteDownSymbol() {
            var prevColor = Console.ForegroundColor;

            Console.ForegroundColor = ScrollSymbolColor;
            Console.Write(ScrollDownSymbol);
            Console.ForegroundColor = prevColor;
        }
        
        // Update precalculated symbol height values
        void UpdateSelector() {
            upSymbolHeight = GetLineAmountOf(ScrollUpSymbol);
            downSymbolHeight = GetLineAmountOf(ScrollDownSymbol);
        }

        // Get the number of lines a string will take up. Does not account for
        // text wrapping inside the console window if a string is too long.
        static int GetLineAmountOf(string current) {
            return current.Count(c => c == '\n') + 1;
        }
    }

}