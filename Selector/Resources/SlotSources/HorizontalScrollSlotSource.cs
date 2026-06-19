namespace Selector.Resources
{
    /// <summary>
    /// Slot source specific to the HorizontalScrollSelector class
    /// </summary>
    public class HorizontalScrollSlotSource<TOption> : SlotSource<TOption, HorizontalScrollSlot<TOption>, HorizontalScrollSelector<TOption>> where TOption : IOption
    {
        readonly HorizontalScrollSelector<TOption> scrollSelector;

        public HorizontalScrollSlotSource(HorizontalScrollSelector<TOption> selector, IReadOnlyList<TOption> optionArray) : base(selector, optionArray) {
            scrollSelector = selector;
        }

        protected override HorizontalScrollSlot<TOption> InitializeSlot(int index) {
            var slot = new HorizontalScrollSlot<TOption>(scrollSelector);
            slot.Init(scrollSelector.Model, options[index], string.Format(scrollSelector.Model.format, options[index].GetName()));
            return slot;
        }
    }
}