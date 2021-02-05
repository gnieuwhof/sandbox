namespace LangTest
{
    using Lexing;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    public class Program
    {
        private static void Main()
        {
            while (true)
            {
                Console.Write(">>> ");
                string input = Console.ReadLine();

                string source = "<stdin>";

                if(File.Exists(input))
                {
                    source = input;
                    input = File.ReadAllText(input);
                    Console.WriteLine(input);
                }

                var stopwatch = new Stopwatch();

                stopwatch.Start();

                var lexer = new Lexer(source, input);

                var tokens = lexer.GetTokens(out LexicalError error);

                stopwatch.Stop();

                Console.WriteLine();

                if (error != null)
                {
                    Console.WriteLine(error);
                }
                else
                {
                    var t = tokens;// Simplifier.Filter(tokens).ToList();
                    foreach(Token token in t)
                    {
                        Console.WriteLine(token);
                        if(token.Type == TokenType.ContentEnd)
                        {
                            break;
                        }
                    }
                }

                Console.WriteLine();
                Console.WriteLine($"Lexing time: {stopwatch.ElapsedMilliseconds} ms");
            }
        }
    }
}
