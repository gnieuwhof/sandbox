namespace AuthDatabaseManager.Pages
{
    using System;

    public abstract class Page
    {
        public class Line { }

        protected static readonly Line LINE = new();


        public abstract string Title { get; }

        public string Subtitle { get; protected set; }

        public Page ReturnPage { get; set; }

        public abstract Page Show();


        protected Page ExceptionRetry(Page returnPage, Func<Page> action)
        {
            while (true)
            {
                try
                {
                    returnPage = action?.Invoke() ?? returnPage;

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
