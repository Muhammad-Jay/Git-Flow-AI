namespace GitFlowAi.Utilities
{
    public class LineSeparator
    {
        public static void Line()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(new string('-', Console.WindowWidth));
            Console.ResetColor();
        }
    }
}