namespace Selector
{
    using Resources;
    using Defaults;

    /// <summary>
    /// Vertical Selector with a fixed number of options displayed to the console.
    /// Ignores any options that cannot fit into the console at the current cursor position.
    /// </summary>
    /// <typeparam name="TOption">Type of the data being selected</typeparam>
    /// <typeparam name="TSlot">Slot type encapsulating data to be used for selectors</typeparam>
    public class VerticalFixedSelector<TOption> : VerticalSelectorBase<TOption, Slot<TOption>> where TOption : IOption
    {
        /// <summary>
        /// Initialize a vertical fixed selector.
        /// Recommend ensuring all options can fit within the given console space at the current cursor position.
        /// </summary>
        /// <param name="selectorOptions">Read only. Options selectable by the user</param>
        /// <param name="format">String format each option uses when displayed in the console. Requires {0}</param>
        /// <exception cref="ArgumentException">selectorOptions must not have a length of 0</exception>
        public VerticalFixedSelector(IReadOnlyList<TOption> selectorOptions, string format = SelectorDefaults.Format) : base(selectorOptions, format, false) {
            options = PopulateOptions(selectorOptions);
        }

        protected override IObjectSource<Slot<TOption>> PopulateOptions(IReadOnlyList<TOption> selectorOptions) {
            int availableLines = Console.WindowHeight - Console.CursorTop - 1;
            if (availableLines < selectorOptions.Count) {
                var truncated = new TOption[availableLines];
                for (int i = 0; i < availableLines; i++) {
                    truncated[i] = selectorOptions[i];
                }
                selectorOptions = truncated;
            }
            return base.PopulateOptions(selectorOptions);
        }
    }
}