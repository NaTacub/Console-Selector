# Console-Selector
Summary
A prebuilt selector class that allows user to select from a set of options within the console. The following is available:
- A selector that displays options into the console both horizontally within the same line, or vertically where each option is contained in its own line.
- A selector that supports scrolling if the number of options passed in succeeds the amount of space available in the console window.
View the SelectorDemo.cs file inside of the Demo folder to understand how to properly use the Selectors.

Class Details
Selectors provides the functionality of querying users inside the console without needing to type answers into the console.
The user is forced to select from a set of options which bypasses the need of validating user input.

For any selector class, its constructor must be sent a list of options of type IReadOnlyList<T>. This includes both Arrays and Lists but not IEnumerables.
This list of option must implement the interface IOption, which requires both the GetName() and GetColor() methods defined.
One more thing, this list cannot be modified past initialization.

Different selectors will have different features available to style its menu display. Common features include:
- option formatting: defined in constructor, how each option displays into the console. The default is set to "[{0}]"
- highlighted foreground color: modifiable through accessing the selector's Model instance. Foreground color of the text when highlighted/selected.
- highlighted background color: modifiable through accessing the selector's Model instance. Background color of the text when highlighted/selected.

Call QuerySelection() from any selector to print the options into the console and require the user to select one.
