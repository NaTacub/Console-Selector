namespace Selector.Resources
{
    /// <summary>
    /// Slot specific for the VerticalScrollSelector class
    /// </summary>
    /// <typeparam name="TOption">Type of the data being selected</typeparam>
    public class VerticalScrollSlot<TOption> : Slot<TOption> where TOption : IOption
    {
        // all vertical slots begin at cursor position 0
        public override int Column => 0;

        /// <summary>
        /// Index of an option string this slot begins
        /// </summary>
        public int selectionIndex;

        public VerticalScrollSlot() { }

        public override void Init(SelectorModel model, TOption instance, string formattedName) {
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