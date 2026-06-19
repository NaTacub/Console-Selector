namespace Selector.Resources
{
    /// <summary>
    /// Slot specific for the HorizontalScrollSelector class.
    /// </summary>
    /// <typeparam name="TOption">Type of the data being selected</typeparam>
    public class HorizontalScrollSlot<TOption> : Slot<TOption> where TOption : IOption
    {
        readonly HorizontalScrollSelector<TOption> selector = null!;

        /// <summary>
        /// length of the option's name, excluding any ASCII characters
        /// </summary>
        public int nameLength;

        // all horizontal slots share the same row
        public override int Row  => selector?.PrintedRow ?? -1;

        /// <summary>
        /// Index of an option string this slot begins
        /// </summary>
        public int selectionIndex;

        public HorizontalScrollSlot() { }
        public HorizontalScrollSlot(HorizontalScrollSelector<TOption> selector) {
            this.selector = selector;
        }

        public override void Init(SelectorModel model, TOption instance, string formattedName) {
            nameLength = instance.GetName().Length;
            base.Init(model, instance, string.Concat(instance.GetColor().ToANSIForegroundColor(), formattedName, "\u001b[0m"));
        }

#pragma warning disable CS8602
        protected override void WriteAsHighlighted() {
            var prevBackgroundColor = Console.BackgroundColor;
            var prevForegroundColor = Console.ForegroundColor;

            Console.BackgroundColor = model.highlightedBackgroundColor;
            Console.ForegroundColor = model.highlightedForegroundColor;
            Console.Write(string.Format(model.format, instance.GetName()));

            Console.BackgroundColor = prevBackgroundColor;
            Console.ForegroundColor = prevForegroundColor;
        }
#pragma warning restore CS8602
    }
}