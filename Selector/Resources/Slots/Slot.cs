namespace Selector.Resources
{
    /// <summary>
    /// Holds a reference to a passed in piece of data.
    /// Acts as a wrapper class that adds extra functionality specifically for Selector classes
    /// </summary>
    /// <typeparam name="TOption">Type of the data being selected</typeparam>
    public class Slot<TOption> where TOption : IOption
    {   
        /// <summary>
        /// Instance data the slot points to
        /// </summary>
        public TOption? instance;

        /// <summary>
        /// Shared data across all slots
        /// </summary>
        public SelectorModel? model;

        /// <summary>
        /// Name already formmatted for the selector
        /// </summary>
        public string formattedName = "";

        /// <summary>
        /// Current state of the slot printed into the console
        /// </summary>
        public bool highlighted = false;

        /// <summary>
        /// Initial column within the console window
        /// </summary>
        public virtual int Column { get; set; }

        /// <summary>
        /// Initial row within the console window
        /// </summary>
        public virtual int Row { get; set; }

        protected bool initialized = false;

        protected const string errorMessage = "Must call Init before calling any other methods within Slot instance";

        public Slot(SelectorModel model, TOption instance, string formattedName) {
            Init(model, instance, formattedName);
        }

        public Slot() {
            instance = default;
            model = default;
        }

        /// <summary>
        /// Mandatory call before using any of the instance's methods
        /// </summary>
        /// <param name="model">Data all slots instances share</param>
        /// <param name="instance">the data the slot points to</param>
        /// <param name="formattedName">name specially formatted for selectors</param>
        public virtual void Init(SelectorModel model, TOption instance, string formattedName) {
            this.model = model;
            this.instance = instance;
            this.formattedName = formattedName;
            initialized = true;
        }
        /// <summary>
        /// Prints the word into the console. Its appearance varies based on its highlighted state.
        /// </summary>
        /// <exception cref="Exception">Must call Init() before calling any class methods</exception>
        public void Print() {
            if (!initialized)
                throw new Exception(errorMessage);

            // store console position of the text
            Column = Console.CursorLeft;
            Row = Console.CursorTop;

            if (highlighted) {
                WriteAsHighlighted();
            } else {
                Write();
            }
        }
        /// <summary>
        /// Rewrites the word within the buffer to be unhighlighted
        /// </summary>
        /// <exception cref="Exception">Must call Init() before calling any class methods</exception>
        public void Unhighlight() {
            if (!initialized)
                throw new Exception(errorMessage);

            if (!highlighted)
                return;

            highlighted = false;

            int prevColumn = Console.CursorLeft;
            int prevRow = Console.CursorTop;

            Console.SetCursorPosition(Column, Row);
            Write();
            
            Console.SetCursorPosition(prevColumn, prevRow);
        }
        /// <summary>
        /// Rewrites the word within the buffer to be highlighted
        /// </summary>
        /// <exception cref="Exception">Must call Init() before calling any class methods</exception>
        public void Highlight() {
            if (!initialized)
                throw new Exception(errorMessage);

            if (highlighted)
                return;

            highlighted = true;

            int prevColumn = Console.CursorLeft;
            int prevRow = Console.CursorTop;

            Console.SetCursorPosition(Column, Row);
            WriteAsHighlighted();
            
            Console.SetCursorPosition(prevColumn, prevRow);
        }
#pragma warning disable CS8602
        // Write the name as its set color into the console
        protected virtual void Write() {
            var prevColor = Console.ForegroundColor;

            Console.ForegroundColor = instance.GetColor();
            Console.Write(formattedName);
            Console.ForegroundColor = prevColor;
        }
        // Write the name as highlighted into the console
        protected virtual void WriteAsHighlighted() {
            
            var prevBackgroundColor = Console.BackgroundColor;
            var prevForegroundColor = Console.ForegroundColor;

            Console.BackgroundColor = model.highlightedBackgroundColor;
            Console.ForegroundColor = model.highlightedForegroundColor;
            Console.Write(formattedName);

            Console.BackgroundColor = prevBackgroundColor;
            Console.ForegroundColor = prevForegroundColor;
        }
    }
#pragma warning restore CS8602
}