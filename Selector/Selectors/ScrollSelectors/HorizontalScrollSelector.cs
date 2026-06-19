using System.Text;

namespace Selector
{
    using Resources;
    using Defaults;

    /// <summary>
    /// Horizontal Selector supporting any number of options passed into it.
    /// Any options not visible inside the console can be revealed by scrolling left or right in the menu
    /// </summary>
    /// <typeparam name="TOption">Type of the data being selected</typeparam>
    public class HorizontalScrollSelector<TOption> : HorizontalSelectorBase<TOption, HorizontalScrollSlot<TOption>> where TOption : IOption
    {
        /// <summary>
        /// Color of the symbols signifying more options are available in a particular direction.
        /// </summary>
        public ConsoleColor ScrollSymbolColor { get; set; } = SelectorDefaults.ScrollForegroundColor;

        /// <summary>
        /// String that appears when more options are available when scrolling left when selecting
        /// </summary>
        public string ScrollLeftSymbol { get; set; } = SelectorDefaults.ScrollLeftSymbol;

        /// <summary>
        /// String that appears when more options are available when scrolling right when selecting
        /// </summary>
        public string ScrollRightSymbol { get; set; } = SelectorDefaults.ScrollRightSymbol;

        /// <summary>
        /// Prepopulated string containing every option formatted by the selector.
        /// Each option is separated by a number of spaces which is passed into the constructor.
        /// </summary>
        public string SelectionText { get; private set; }
        
        /// <summary>
        /// The initial console column the selector prints its menu
        /// </summary>
        public int InitialColumn { get; private set;}

        /// <summary>
        /// The max number of characters the selector menu can take up inside the console.
        /// Caps at the console window's current width
        /// </summary>
        public int MaxSelectorWidth
        {
            get => maxSelectorWidth;
            set
            {
                maxSelectorWidth = Math.Clamp(value, ScrollLeftSymbol.Length + ScrollRightSymbol.Length + Model.format.Length - SelectorDefaults.ExcludedFormatCharacters, Console.WindowWidth);
            }
        }
        int maxSelectorWidth;

        int availableSpace; // available space for options to be printed. Value excludes scroll symbols
        int leftBoundOptionIndex, rightBoundOptionIndex; // indexes defining the section of options displayed to the console
        int characterAmountDisplayed; // total characters from SelectionText being displayed into the console
        
        /// <summary>
        /// Initialize a horizontal scroll selector. Creates and stores printed menu internally.
        /// <c>selectorOptions</c> must be of length > 0.
        /// </summary>
        /// <param name="selectorOptions">Read only. Options selectable by the user</param>
        /// <param name="optionSpacing">Number of spaces between each option</param>
        /// <param name="format">String format each option uses when displayed in the console. Requires {0}</param>
        /// <exception cref="ArgumentException">selectorOptions must not have a length of 0</exception>
        public HorizontalScrollSelector(
            IReadOnlyList<TOption> selectorOptions,
            int optionSpacing = SelectorDefaults.OptionSpacing,
            char fill = SelectorDefaults.FillCharacter,
            string format = SelectorDefaults.Format)
            : base(selectorOptions, optionSpacing, fill, format, false)
        {
            MaxSelectorWidth = Console.WindowWidth;

            options = PopulateOptions(selectorOptions);

            string spacing = new string(fill, optionSpacing);
            int estimateCapacity = (options[0].formattedName.Length + optionSpacing) * options.Count; 
            StringBuilder stringBuilder = new StringBuilder(options[0].formattedName, estimateCapacity);

            options[0].selectionIndex = 0; // base class throws if options.Length is 0
            for (int i = 1; i < options.Count; i++) {
                stringBuilder.Append(spacing);
                options[i].selectionIndex = stringBuilder.Length;
                stringBuilder.Append(options[i].formattedName);
            }

            SelectionText = stringBuilder.ToString();
        }

        protected override IObjectSource<HorizontalScrollSlot<TOption>> PopulateOptions(IReadOnlyList<TOption> selectorOptions) {
            var source = new HorizontalScrollSlotSource<TOption>(this, selectorOptions);
            source.InitializeAll();
            return source;
        }

        public override void PrintSelection() {
            PrintedRow = Console.CursorTop;
            InitialColumn = Console.CursorLeft;
            CurrentIndex = 0;

            UpdateSelector();

            int currentSize = options[CurrentIndex].nameLength;

            leftBoundOptionIndex = CurrentIndex;
            rightBoundOptionIndex = CurrentIndex + 1;

            int outcome = IterateRight(ref rightBoundOptionIndex, ref currentSize, availableSpace);
            // if we iterated right as far as we could (1), or failed on the first index (-1), iterate left
            if (outcome != 0) {
                if (leftBoundOptionIndex == 0) {
                    rightBoundOptionIndex = 0;
                } else {
                    leftBoundOptionIndex--;
                    outcome = IterateLeft(ref leftBoundOptionIndex, ref currentSize, availableSpace);
                    // if we failed immediately, return back to CurrentIndex
                    if (outcome == -1) {
                        leftBoundOptionIndex = CurrentIndex;
                    }
                }
            }
            characterAmountDisplayed = currentSize;

            UpdatePrintedText();

            selectionPrinted = true;
        }

        // Assumes we either incremented or decremented CurrentIndex by 1
        protected override void UpdateSelection() {
            if (PreviousIndex == CurrentIndex)
                return;
            // If scrolling isn't necessary, update as normal
            if (CurrentIndex >= leftBoundOptionIndex && CurrentIndex <= rightBoundOptionIndex) {
                base.UpdateSelection();
                return;
            }

            options[PreviousIndex].highlighted = false;

            int surplus = availableSpace - characterAmountDisplayed; // Always expected to be negative
            int clearanceAmount = surplus - options[CurrentIndex].nameLength - optionSpacing; // outstanding characters needing to be freed from the console
            // Scroll left: find size of newly selected option, then from right to left, remove options until we can fit the new option.
            if (CurrentIndex < PreviousIndex) {
                leftBoundOptionIndex = CurrentIndex;
                for (; rightBoundOptionIndex >= 0 && clearanceAmount < 0; rightBoundOptionIndex--) {
                    int optionSize = optionSpacing + options[rightBoundOptionIndex].nameLength;
                    clearanceAmount += optionSize;
                }

                leftBoundOptionIndex--;

                int includedCharacters = 0;
                int outcome = IterateLeft(ref leftBoundOptionIndex, ref includedCharacters, clearanceAmount);
                clearanceAmount -= includedCharacters;
                // if we overloaded characters immediately, return back to original index.
                if (outcome == -1) {
                    leftBoundOptionIndex = CurrentIndex;
                }

            }
            // Scroll right: find size of newly selected option, then from left to right, remove options until we can fit the new option.
            else {
                rightBoundOptionIndex = CurrentIndex;
                for (; leftBoundOptionIndex < options.Count && clearanceAmount < 0; leftBoundOptionIndex++) {
                    int optionSize = optionSpacing + options[leftBoundOptionIndex].nameLength;
                    clearanceAmount += optionSize;
                }

                rightBoundOptionIndex++;

                int includedCharacters = 0;
                int outcome = IterateRight(ref rightBoundOptionIndex, ref includedCharacters, clearanceAmount);
                clearanceAmount -= includedCharacters;
                // if we overloaded characters immediately, return back to original index.
                if (outcome == -1) {
                    rightBoundOptionIndex = CurrentIndex;
                }
            }

            // clearanceAmount are the extra characters we removed from the other options. If == 0, then we cleanly replaced one option with another.
            // If < 0, we have extra space in the buffer
            characterAmountDisplayed -= clearanceAmount - surplus;

            UpdatePrintedText();

            options[CurrentIndex].Highlight();
        }

        // Expects option indexes to be in the correct place
        void UpdatePrintedText() {
            int emptySpace = Math.Max(availableSpace - characterAmountDisplayed, 0);
            int leftPadding = emptySpace / 2;
            int firstOptionColumn = InitialColumn + ScrollLeftSymbol.Length + leftPadding;

            // calculate columms for selected options
            UpdateOptionColumns(firstOptionColumn);

            Console.SetCursorPosition(InitialColumn + ScrollLeftSymbol.Length, PrintedRow);
            // left padding
            Console.Write(new string(' ', leftPadding));

            WriteSelectionText();
            // right padding
            Console.Write(new string(' ', emptySpace - leftPadding));
            
            WriteScrollSymbols();
        }
        
        // Updating option column positions so we can select or deselect them in the console
        void UpdateOptionColumns(int startColumn) {
            // must manually calculate each column position since option name lengths may vary
            options[leftBoundOptionIndex].Column = startColumn;
            for (int i = leftBoundOptionIndex + 1; i <= rightBoundOptionIndex; i++) {
                var prevOption = options[i - 1];
                options[i].Column = prevOption.Column + prevOption.nameLength + optionSpacing;
            }
        }

        // Write the current set of options into the console. Expects option indexes to be in the correct place 
        void WriteSelectionText() {
            // set bounds of a substring. Use formattedName.Length over nameLength since the string doesn't hide ASCII escape characters
            int leftBoundTextIndex = options[leftBoundOptionIndex].selectionIndex;
            int rightBoundTextIndex = options[rightBoundOptionIndex].selectionIndex + options[rightBoundOptionIndex].formattedName.Length;

            Console.Write(SelectionText.AsSpan(leftBoundTextIndex, rightBoundTextIndex - leftBoundTextIndex));
        }
        // Write or clear the up and down scroll symbols
        void WriteScrollSymbols() {
            if (leftBoundOptionIndex != 0) {
                WriteLeftSymbol();
            } else {
                WriteTextAndPreserveCursor(new string(' ', ScrollLeftSymbol.Length), InitialColumn);
            }
            if (rightBoundOptionIndex != options.Count - 1) {
                WriteRightSymbol();
            } else {
                WriteTextAndPreserveCursor(new string(' ', ScrollRightSymbol.Length), InitialColumn + MaxSelectorWidth - ScrollRightSymbol.Length);
            }
        }

        void WriteLeftSymbol() {
            var prevColor = Console.ForegroundColor;

            Console.ForegroundColor = ScrollSymbolColor;
            WriteTextAndPreserveCursor(ScrollLeftSymbol, InitialColumn);
            Console.ForegroundColor = prevColor;
        }

        void WriteRightSymbol() {
            var prevColor = Console.ForegroundColor;

            Console.ForegroundColor = ScrollSymbolColor;
            WriteTextAndPreserveCursor(ScrollRightSymbol, InitialColumn + MaxSelectorWidth - ScrollRightSymbol.Length);
            Console.ForegroundColor = prevColor;
        }

        void WriteTextAndPreserveCursor(string text, int column) {
            int currCol = Console.CursorLeft, currRow = Console.CursorTop;
            Console.SetCursorPosition(column, PrintedRow);
            Console.Write(text);
            Console.SetCursorPosition(currCol, currRow);
        }
        // Updates the selector's max width and option text's available space
        void UpdateSelector() {
            // availableSpace must be smaller or equal to what's available in the current line within the buffer
            int windowSpace = Console.WindowWidth - InitialColumn;
            availableSpace = MaxSelectorWidth - ScrollLeftSymbol.Length - ScrollRightSymbol.Length;
            if (availableSpace > windowSpace) {
                MaxSelectorWidth = windowSpace;
                availableSpace = MaxSelectorWidth - ScrollLeftSymbol.Length - ScrollRightSymbol.Length;
            }
        }

        /// <summary>
        /// Iterates right in the options without breaching the max number of characters we can display.
        /// </summary>
        /// <returns>
        /// <c>-1</c>: we ended the loop immediately.
        /// <c>0</c>: characters was filled before reaching characterMax.
        /// <c>1</c>: we visited the rightmost option available.
        /// </returns>
        int IterateRight(ref int index, ref int characters, int characterMax) {
            int startIndex = index;
            for (; index < options.Count; index++) {
                int optionSize = optionSpacing + options[index].nameLength;
                if (characters + optionSize > characterMax) {
                    if (index != startIndex) {
                        index--;
                        return 0;
                    }
                    return -1;
                }
                characters += optionSize;
            }
            index--;
            // If we visited the last option available
            return 1; 
        }

        /// <summary>
        /// Iterates left in the options without breaching the max number of characters we can display,
        /// </summary>
        /// <returns>
        /// <c>-1</c>: we ended the loop immediately.
        /// <c>0</c>: characters was filled before reaching characterMax.
        /// <c>1</c>: we visited the leftmost option available.
        /// </returns>
        int IterateLeft(ref int index, ref int characters, int characterMax) {
            int startIndex = index;
            for (; index >= 0; index--) {
                int optionSize = optionSpacing + options[index].nameLength;
                if (characters + optionSize > characterMax) {
                    if (index != startIndex) {
                        index++;
                        return 0;
                    }
                    return -1;
                }
                characters += optionSize;
            }
            index++;
            // if we visited the last last option available
            return 1;
        }
    }

}