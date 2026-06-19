using System.Collections;

namespace Selector.Resources
{
    /// <summary>
    /// Implements base functionality into IObjectSource methods
    /// Guarantee calls Init() for each Slot initialized
    /// </summary>
    /// <typeparam name="TOption">Type of the data being selected</typeparam>
    /// <typeparam name="TSlot">Slot type encapsulating data to be used for selectors</typeparam>
    /// <typeparam name="TSelector">Selector type using the object source type</typeparam>
    public class SlotSource<TOption, TSlot, TSelector> : IObjectSource<TSlot>
        where TOption : IOption
        where TSlot : Slot<TOption>, new()
        where TSelector : Selector<TOption, TSlot>
    {
        public int Count => options.Count;

        protected readonly IReadOnlyList<TOption> options;
        protected readonly TSlot[] loadedSlots;
        protected readonly TSelector selector;

        /// <summary>
        /// Returns the slot at <c>index</c>. If slot is null, then it's initialized immediately.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TSlot this[int index] {
            get
            {
                loadedSlots[index] ??= InitializeSlot(index);
                return loadedSlots[index];
            }
        }

        public SlotSource(TSelector selector, IReadOnlyList<TOption> options) {
            this.selector = selector;
            this.options = options;
            loadedSlots = new TSlot[options.Count];
        }

        public void InitializeRange(int begin, int end) {
            for (int i = begin; i < end; i++) {
                loadedSlots[i] ??= InitializeSlot(i);
            }
        }
        
        public void InitializeAll() {
            InitializeRange(0, options.Count);
        }

        public IEnumerator<TSlot> GetEnumerator() {
            for (int i = 0; i < Count; i++) {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        protected virtual TSlot InitializeSlot(int index) {
            var slot = new TSlot();
            slot.Init(selector.Model, options[index], string.Format(selector.Model.format, options[index].GetName()));
            return slot;
        }
    }
}