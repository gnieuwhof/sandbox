namespace AuthDatabaseManager.Pages
{
    using System;

    public abstract class Page
    {
        public class Line { }

        protected static readonly Line LINE = new();


        public abstract string Title { get; }

        public abstract Page Show();


        protected Page ExceptionRetry(Page returnPage, Action action)
        {
            while (true)
            {
                try
                {
                    action?.Invoke();

                    break;
                }
                catch (Exception e)
                {
                    Write.Error(e.Message);
                    Console.WriteLine("Try again? (Y/n)");

                    string input = Console.ReadLine();

                    if (input == "n")
                    {
                        break;
                    }
                }
            }

            return returnPage;
        }
    }
}
