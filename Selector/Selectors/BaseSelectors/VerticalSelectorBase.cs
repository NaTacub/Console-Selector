namespace Selector
{
    using Resources;
    using Defaults;

    /// <summary>
    /// Base class for vertical selectors where options display from top to bottom.
    /// Uses arrow keys to move along the selector menu
    /// </summary>
    /// <typeparam name="TOption">Type of the data being selected</typeparam>
    /// <typeparam name="TSlot">Slot type encapsulating data to be used for selectors</typeparam>
    public abstract class VerticalSelectorBase<TOption, TSlot> : Selector<TOption, TSlot>
        where TOption : IOption
        where TSlot : Slot<TOption>, new()
    {
        public override ConsoleKey IncrementKey => ConsoleKey.DownArrow;
        public override ConsoleKey DecrementKey => ConsoleKey.UpArrow;

        public override int NextEmptyLine => nextEmptyLine;
        protected int nextEmptyLine;

        /// <summary>
        /// First row of the printed menu within the console
        /// </summary>
        public int FirstRow { get; protected set; }

        /// <summary>
        /// Last row of the printed menu within the console
        /// </summary>
        public int LastRow { get; protected set; }

        /// <param name="selectorOptions">Read only. Options selectable by the user</param>
        /// <param name="format">String format each option uses when displayed in the console. Requires {0}</param>
        /// <param name="populateOptions">Whether the constructor should call PopulateOptions(...) or lend the task to another class</param>
        public VerticalSelectorBase(IReadOnlyList<TOption> selectorOptions, string format = SelectorDefaults.Format, bool populateOptions = true) : base(selectorOptions, format, populateOptions) { }

        /// <summary>
        /// Prints out a menu of options. Forces cursor to column position 0.
        /// </summary>
        public override void PrintSelection() {
            if (options.Count == 0)
                return;

            FirstRow = Console.CursorTop;

            // Can be modified where all options share the same column instead of
            // being set to column 0. That means wherever you begin querying the user,
            // options will be printed directly below the current cursor position.
            Console.SetCursorPosition(0, FirstRow);

            for (int i = 0; i < options.Count; i++) {
                options[i].Print();
                Console.WriteLine();
            }

            LastRow = Console.CursorTop - 1;
            nextEmptyLine = Console.CursorTop;
            
            selectionPrinted = true;
        }

        public override void ClearSelection() {
            if (!selectionPrinted)
                return;

            for (int row = FirstRow; row <= LastRow; row++) {
                ClearConsoleLine(row, false);
            }
            Console.SetCursorPosition(0, FirstRow);
            nextEmptyLine = LastRow = FirstRow;

            selectionPrinted = false;
        }
    }
}