// This file handles all the console display stuff
// It keeps the UI code separate from the business logic
// All methods are static so we don't need to create an object to use them

namespace InventoryManagementSystem.Services
{
    public static class UIHelper
    {
        // shows a green success message
        public static void PrintSuccess(string message)
        {
            int w = Math.Max(message.Length + 8, 50);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  ╔══ SUCCESS " + new string('═', w - 11) + "╗");
            Console.WriteLine($"  ║  ✔  {message.PadRight(w - 6)}║");
            Console.WriteLine("  ╚" + new string('═', w + 2) + "╝");
            Console.ResetColor();
        }

        // shows a red error message
        public static void PrintError(string message)
        {
            int w = Math.Max(message.Length + 8, 50);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  ╔══ ERROR " + new string('═', w - 9) + "╗");
            Console.WriteLine($"  ║  ✘  {message.PadRight(w - 6)}║");
            Console.WriteLine("  ╚" + new string('═', w + 2) + "╝");
            Console.ResetColor();
        }

        // shows a yellow warning message
        public static void PrintWarning(string message)
        {
            int w = Math.Max(message.Length + 8, 50);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  ╔══ WARNING " + new string('═', w - 11) + "╗");
            Console.WriteLine($"  ║  ⚠  {message.PadRight(w - 6)}║");
            Console.WriteLine("  ╚" + new string('═', w + 2) + "╝");
            Console.ResetColor();
        }

        // shows a simple info message
        public static void PrintInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("  ℹ  ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        // prints the shop name banner at the top of the screen
        // both lines are centered inside the box
        public static void PrintBanner()
        {
            string line1 = "GLOW BEAUTY SHOP";
            string line2 = "Makeup & Skincare Inventory System";
            int inner = line2.Length + 8;

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("  ╔" + new string('═', inner) + "╗");

            // center line1
            int pad1L = (inner - line1.Length) / 2;
            int pad1R = inner - line1.Length - pad1L;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"  ║{new string(' ', pad1L)}{line1}{new string(' ', pad1R)}║");

            // center line2
            int pad2L = (inner - line2.Length) / 2;
            int pad2R = inner - line2.Length - pad2L;
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine($"  ║{new string(' ', pad2L)}{line2}{new string(' ', pad2R)}║");

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("  ╚" + new string('═', inner) + "╝");
            Console.ResetColor();
        }

        // prints a section header with a double-line box
        public static void PrintHeader(string title, ConsoleColor color = ConsoleColor.Cyan)
        {
            int w = Math.Max(title.Length + 10, 50);
            int padL = (w - title.Length) / 2;
            int padR = w - title.Length - padL;
            Console.WriteLine();
            Console.ForegroundColor = color;
            Console.WriteLine("  ╔" + new string('═', w) + "╗");
            Console.WriteLine($"  ║{new string(' ', padL)}{title.ToUpper()}{new string(' ', padR)}║");
            Console.WriteLine("  ╚" + new string('═', w) + "╝");
            Console.ResetColor();
        }

        // prints a small label to divide parts of a screen
        public static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"  ┌─ {title} " + new string('─', Math.Max(0, 65 - title.Length)) + "┐");
            Console.ResetColor();
        }

        // prints a horizontal line
        public static void PrintThinDivider()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  " + new string('─', 76));
            Console.ResetColor();
        }

        // prints one menu item with a colored key badge
        public static void PrintMenuItem(string key, string label, string? hint = null)
        {
            Console.Write("    ");
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($" {key} ");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"  {label}");
            if (hint != null)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($"  —  {hint}");
            }
            Console.ResetColor();
            Console.WriteLine();
        }

        // prints a group label above menu items
        public static void PrintMenuGroup(string label)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine($"    {label.ToUpper()}");
            Console.ResetColor();
        }

        // prints the top border and header row of a table
        public static void PrintTableHeader(params (string Label, int Width)[] cols)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;

            // top border
            Console.Write("  ┌");
            for (int i = 0; i < cols.Length; i++)
                Console.Write(new string('─', cols[i].Width + 2) + (i < cols.Length - 1 ? "┬" : "┐"));
            Console.WriteLine();

            // header labels
            Console.Write("  │");
            foreach (var col in cols)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write($" {col.Label.ToUpper().PadRight(col.Width)} ");
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write("│");
            }
            Console.WriteLine();

            // separator line under headers
            Console.Write("  ├");
            for (int i = 0; i < cols.Length; i++)
                Console.Write(new string('─', cols[i].Width + 2) + (i < cols.Length - 1 ? "┼" : "┤"));
            Console.WriteLine();
            Console.ResetColor();
        }

        // prints one data row - wraps long text instead of cutting it off
        public static void PrintTableRow(ConsoleColor textColor,
                                         params (string Value, int Width)[] cols)
        {
            // wrap each column's text into lines that fit the column width
            var wrappedCols = new List<List<string>>();
            int maxLines = 1;

            foreach (var col in cols)
            {
                var lines = WrapText(col.Value, col.Width);
                wrappedCols.Add(lines);
                if (lines.Count > maxLines) maxLines = lines.Count;
            }

            // make all columns have the same number of lines
            for (int i = 0; i < wrappedCols.Count; i++)
                while (wrappedCols[i].Count < maxLines)
                    wrappedCols[i].Add("");

            // print each line of the row
            for (int ln = 0; ln < maxLines; ln++)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("  │");
                for (int c = 0; c < cols.Length; c++)
                {
                    Console.Write(" ");
                    Console.ForegroundColor = textColor;
                    Console.Write(wrappedCols[c][ln].PadRight(cols[c].Width));
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(" │");
                }
                Console.ResetColor();
                Console.WriteLine();
            }
        }

        // breaks text into lines that fit within the given width
        private static List<string> WrapText(string text, int width)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(text)) { result.Add(""); return result; }
            if (text.Length <= width)       { result.Add(text); return result; }

            var words = text.Split(' ');
            var current = "";

            foreach (var word in words)
            {
                if (current.Length == 0)
                {
                    // if word is longer than column, cut it
                    current = word.Length > width ? word[..width] : word;
                }
                else if (current.Length + 1 + word.Length <= width)
                {
                    current += " " + word;
                }
                else
                {
                    result.Add(current);
                    current = word;
                }
            }

            if (current.Length > 0) result.Add(current);
            return result;
        }

        // prints the bottom border of a table
        public static void PrintTableFooter(params int[] widths)
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("  └");
            for (int i = 0; i < widths.Length; i++)
                Console.Write(new string('─', widths[i] + 2) + (i < widths.Length - 1 ? "┴" : "┘"));
            Console.WriteLine();
            Console.ResetColor();
        }

        // prints a row of small summary cards side by side
        public static void PrintStatCards(
            params (string Label, string Value, ConsoleColor Color)[] cards)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            foreach (var _ in cards) Console.Write("  ┌──────────────────────┐");
            Console.WriteLine();

            // label row
            foreach (var c in cards)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("  │ ");
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write(c.Label.PadRight(20));
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(" │");
            }
            Console.WriteLine();

            // value row
            foreach (var c in cards)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("  │ ");
                Console.ForegroundColor = c.Color;
                string val = c.Value.Length > 20 ? c.Value[..20] : c.Value;
                Console.Write(val.PadRight(20));
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(" │");
            }
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            foreach (var _ in cards) Console.Write("  └──────────────────────┘");
            Console.WriteLine();
            Console.ResetColor();
        }

        // shows a text input prompt and returns what the user typed
        public static string PromptInput(string label)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write($"  ▸ {label}: ");
            Console.ForegroundColor = ConsoleColor.White;
            string val = Console.ReadLine()?.Trim() ?? "";
            Console.ResetColor();
            return val;
        }

        // asks for a number and keeps asking until a valid one is given
        public static int PromptInt(string label, int min = int.MinValue, int max = int.MaxValue)
        {
            while (true)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write($"  ▸ {label}: ");
                Console.ForegroundColor = ConsoleColor.White;
                string input = Console.ReadLine()?.Trim() ?? "";
                Console.ResetColor();

                if (int.TryParse(input, out int value) && value >= min && value <= max)
                    return value;

                PrintError($"Please enter a valid whole number between {min} and {max}.");
            }
        }

        // asks for a decimal number (for prices) and validates it
        public static decimal PromptDecimal(string label, decimal min = 0)
        {
            while (true)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write($"  ▸ {label}: ");
                Console.ForegroundColor = ConsoleColor.White;
                string input = Console.ReadLine()?.Trim() ?? "";
                Console.ResetColor();

                if (decimal.TryParse(input, out decimal value) && value >= min)
                    return value;

                PrintError($"Please enter a valid number (minimum {min}).");
            }
        }

        // asks a yes or no question and returns true if user types y or yes
        public static bool PromptConfirm(string message)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"  ▸ {message} ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("[y/n]: ");
            Console.ForegroundColor = ConsoleColor.White;
            string answer = Console.ReadLine()?.Trim().ToLower() ?? "n";
            Console.ResetColor();
            return answer == "y" || answer == "yes";
        }

        // reads a password without showing the characters on screen
        // shows a dot for each key pressed instead
        public static string PromptPassword(string label = "Password")
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write($"  ▸ {label}: ");
            Console.ForegroundColor = ConsoleColor.DarkGray;

            string password = "";
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true); // read key without displaying it
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("●"); // show dot instead of the actual character
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password[..^1];
                    Console.Write("\b \b"); // erase the dot
                }
            }
            while (key.Key != ConsoleKey.Enter);

            Console.ResetColor();
            Console.WriteLine();
            return password;
        }

        // shows the "enter your choice" prompt
        public static void PrintChoicePrompt()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("  Enter your choice ▸ ");
            Console.ForegroundColor = ConsoleColor.White;
        }

        // pauses the screen until user presses any key
        public static void PressAnyKey()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("  Press any key to continue ▸ ");
            Console.ResetColor();
            Console.ReadKey(true);
            Console.WriteLine();
        }

        // prints the top part of the login box
        public static void PrintLoginBox()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("  ┌" + new string('─', 44) + "┐");
            Console.WriteLine("  │" + "  SECURE LOGIN".PadRight(44) + "│");
            Console.WriteLine("  ├" + new string('─', 44) + "┤");
            Console.ResetColor();
        }

        // prints the bottom part of the login box
        public static void PrintLoginBoxBottom()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("  └" + new string('─', 44) + "┘");
            Console.ResetColor();
        }

        // clears the screen
        public static void ClearScreen() => Console.Clear();
    }
}
