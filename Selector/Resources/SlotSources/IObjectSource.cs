namespace Selector.Resources
{
    /// <summary>
    /// Slot handler meant to properly initialize each slot of a specific selector.
    /// Can initialize a range of slots or whenever one is requested.
    /// </summary>
    /// <typeparam name="TOption">Type of the data being selected</typeparam>
    /// <typeparam name="TSlot">Slot type encapsulating data to be used for selectors</typeparam>
    /// <typeparam name="TSelector">Selector type using the object source type</typeparam>
    public interface IObjectSource<T> : IEnumerable<T>
    {
        /// <summary>
        /// Total options stored inside object source
        /// </summary>
        int Count { get; }

        T this[int index] { get; }

        /// <summary>
        /// Initializes elements of type <c>T</c> from index <c>begin</c> to index <c>end</c> - 1 only if needed.
        /// </summary>
        void InitializeRange(int begin, int end);

        /// <summary>
        /// Initialize all elements of type <c>T</c>
        /// </summary>
        void InitializeAll();
    }
}