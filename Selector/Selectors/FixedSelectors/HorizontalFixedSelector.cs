namespace Selector
{
    using Resources;
    using Defaults;

    /// <summary>
    /// Horizontal Selector with a fixed number of options displayed to the console.
    /// Ignores any options that cannot fit into the console at the current cursor position.
    /// </summary>
    /// <typeparam name="TOption">Type of the data being selected</typeparam>
    /// <typeparam name="TSlot">Slot type encapsulating data to be used for selectors</typeparam>
    public class HorizontalFixedSelector<TOption> : HorizontalSelectorBase<TOption, Slot<TOption>> where TOption : IOption
    {
        public bool ApplyPadding { get; set; } = true;
        int leftPadding;

        /// <summary>
        /// Initialize a horizontal fixed selector.
        /// Recommend ensuring all options can fit within the given console space at the current cursor position.
        /// </summary>
        /// <param name="selectorOptions">Read only. Options selectable by the user</param>
        /// <param name="format">String format each option uses when displayed in the console. Requires {0}</param>
        /// <exception cref="ArgumentException">selectorOptions must not have a length of 0</exception>
        public HorizontalFixedSelector(
            IReadOnlyList<TOption> selectorOptions,
            string format = SelectorDefaults.Format,
            char fill = SelectorDefaults.FillCharacter,
            int optionSpacing = SelectorDefaults.OptionSpacing)
            : base(selectorOptions, optionSpacing, fill, format, false)
        {
            // allows us to use the optionSpacing value initialized by HorizontalSelectorBase
            options = PopulateOptions(selectorOptions);
        }

        protected override IObjectSource<Slot<TOption>> PopulateOptions(IReadOnlyList<TOption> selectorOptions) {
            int formatCharacters = SelectorDefaults.Format.Length - SelectorDefaults.ExcludedFormatCharacters;
            // first option must fit within console window before attempting to display the rest
            int availableSpace = Console.WindowWidth - Console.CursorLeft - selectorOptions[0].GetName().Length - formatCharacters;
            if (availableSpace < 0)
                throw new Exception("First option's name cannot fit within current buffer size");
            // loop over each option to see how many can fit in console
            int index = 1;
            for (; index < selectorOptions.Count; index++) {
                int optionSize = selectorOptions[index].GetName().Length + formatCharacters + optionSpacing;
                if (availableSpace - optionSize < 0) 
                    break;
                availableSpace -= optionSize;
            }
            leftPadding = availableSpace / 2;

            if (index != selectorOptions.Count) {
                var truncated = new TOption[index];
                for (int i = 0; i < index; i++) {
                    truncated[i] = selectorOptions[i];
                }
                selectorOptions = truncated;
            }

            return base.PopulateOptions(selectorOptions);
        }

        public override void PrintSelection() {
            if (ApplyPadding) {
                Console.Write(new string(fill, leftPadding));
            }
            base.PrintSelection();
        }
    }
}