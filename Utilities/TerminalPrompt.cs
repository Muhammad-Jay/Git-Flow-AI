using System;
using System.Collections.Generic;
using System.Linq;

namespace GitFlowAi.Utilities
{
    public static class TerminalPrompt
    {
        public static int PromptForSelection(List<string> options, string? promptMessage = null)
        {
            if (options == null || options.Count == 0)
            {
                throw new ArgumentException("Option list cannot be empty.");
            }
            int currentSelection = 0;
            ConsoleKey key;
            
            int startRow =  Console.CursorTop;
            int menuHeight = options.Count + 1;
            do
            {
                Console.Clear();
                Console.SetCursorPosition(0, startRow);
                Console.CursorVisible = false;
                
                Console.Write(new string(' ', Console.WindowWidth) + "\r");
                Console.WriteLine(promptMessage);

                for (int i = 0; i < options.Count; i++)
                {
                    bool isSelected = (i == currentSelection);
                    
                    Console.ForegroundColor = isSelected ? ConsoleColor.Cyan : ConsoleColor.Gray;
                    string lineContent = (isSelected ? " > " : "   ") + options[i];
                    Console.WriteLine(lineContent.PadRight(Console.WindowWidth - 1));
                }
                Console.ResetColor();
                
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                key = keyInfo.Key;

                if (key == ConsoleKey.UpArrow && currentSelection > 0)
                {
                    currentSelection--;
                }
                else if (key == ConsoleKey.DownArrow && currentSelection < options.Count - 1)
                {
                    currentSelection++;
                }
                else if (keyInfo.KeyChar == 'q' || keyInfo.KeyChar == 'Q')
                {
                    currentSelection = options.Count - 1;
                    key = ConsoleKey.Enter;
                }
            } while (key != ConsoleKey.Enter);

            Console.CursorVisible = true;
            
            Console.SetCursorPosition(0, startRow);
            // for (int i = 0; i < menuHeight; i++)
            // {
            //     Console.WriteLine(new string(' ', Console.WindowWidth));
            // }
            Console.SetCursorPosition(0, startRow);
            return currentSelection;
        }
    }
}

