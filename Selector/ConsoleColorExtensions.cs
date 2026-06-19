namespace Selector
{
    public static class ConsoleColorExtensions
    {
        public static readonly Dictionary<ConsoleColor, string> asciiDictionary = new()
        {
            {ConsoleColor.Red, "\u001b[91m"},
            {ConsoleColor.DarkRed, "\u001b[31m"},
            {ConsoleColor.Yellow, "\u001b[93m"},
            {ConsoleColor.DarkYellow, "\u001b[33m"},
            {ConsoleColor.Green, "\u001b[92m"},
            {ConsoleColor.DarkGreen, "\u001b[32m"},
            {ConsoleColor.Cyan, "\u001b[96m"},
            {ConsoleColor.DarkCyan, "\u001b[36m"},
            {ConsoleColor.Blue, "\u001b[94m"},
            {ConsoleColor.DarkBlue, "\u001b[34m"},
            {ConsoleColor.Magenta, "\u001b[95m"},
            {ConsoleColor.DarkMagenta, "\u001b[35m"},
            {ConsoleColor.Black, "\u001b[30m"},
            {ConsoleColor.Gray, "\u001b[90m"},
            {ConsoleColor.DarkGray, "\u001b[37m"},
            {ConsoleColor.White, "\u001b[97m"}
        };

        /// <summary>
        /// Shortcut for converting any ConsoleColor enum into ANSI code
        /// </summary>
        public static string ToANSIForegroundColor(this ConsoleColor color) {
            return asciiDictionary[color];
        }
    }
}
/* Sources about ANSI mark-up
 * @https://chrisyeh96.github.io/2020/03/28/terminal-colors.html
 * @https://en.wikipedia.org/wiki/ANSI_escape_code#Colors
 * @https://www.ditig.com/256-colors-cheat-sheet
 * @https://stackoverflow.com/questions/23975735/what-is-this-u001b9-syntax-of-choosing-what-color-text-appears-on-console
 */