namespace Selector
{
    using Resources;
    using Defaults;

    /// <summary>
    /// Base class for horizontal selectors where options display from left to right
    /// Uses arrow keys to move along the selector menu
    /// </summary>
    /// <typeparam name="TOption">Type of the data being selected</typeparam>
    /// <typeparam name="TSlot">Slot type encapsulating data to be used for selectors</typeparam>
    public abstract class HorizontalSelectorBase<TOption, TSlot> : Selector<TOption, TSlot>
        where TOption : IOption
        where TSlot : Slot<TOption>, new()
    {
        public override ConsoleKey IncrementKey => ConsoleKey.RightArrow;
        public override ConsoleKey DecrementKey => ConsoleKey.LeftArrow;

        public override int NextEmptyLine => PrintedRow + 1;

        /// <summary>
        /// The row of the selector's printed menu
        /// </summary>
        public int PrintedRow { get; protected set; }
        
        /// <summary>
        /// Total space between each printed option inside the console
        /// </summary>
        public readonly int optionSpacing;

        public readonly char fill;

        /// <param name="selectorOptions">Read only. Options selectable by the user</param>
        /// <param name="optionSpacing">Amount of spaces used to separate each option printed into the console</param>
        /// <param name="format">String format each option uses when displayed in the console. Requires {0}</param>
        /// <param name="populateOptions">Whether the constructor should call PopulateOptions(...) or lend the task to another class</param>
        public HorizontalSelectorBase(
            IReadOnlyList<TOption> selectorOptions,
            int optionSpacing = SelectorDefaults.OptionSpacing,
            char fill = SelectorDefaults.FillCharacter,
            string format = SelectorDefaults.Format,
            bool populateOptions = true)
            : base(selectorOptions, format, populateOptions)
        {
            this.optionSpacing = optionSpacing;
            this.fill = fill;
        }

        public override void PrintSelection() {
            if (options.Count == 0)
                return;

            PrintedRow = Console.CursorTop;
            foreach (var option in options) {
                option.Print();
                Console.Write(new string(fill, optionSpacing));
            }

            selectionPrinted = true;
        }

        public override void ClearSelection() {
            if (options.Count == 0 || !selectionPrinted)
                return;

            ClearConsoleLine(PrintedRow, false);
            Console.SetCursorPosition(0, PrintedRow);

            selectionPrinted = false;
        }
    }
}