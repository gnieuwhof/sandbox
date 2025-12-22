namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Input;
    using System;

    public class SecretsGeneratorPage : Page
    {
        private readonly Database database;


        public override string Title => "Secrets Generator";


        public SecretsGeneratorPage(Page returnPage, Database database)
            : base(returnPage)
        {
            this.database = database;
        }


        public override Page Show()
        {
            var lengthInput = new InputVal<int>(this.database, "Number of characters")
            {
                Default = 40,
                Validator = Validate
            };

            while (true)
            {
                bool result = Inputs.Get(this.database, lengthInput);

                if (result)
                {
                    break;
                }

                return this.ReturnPage;
            }

            Console.WriteLine("Generate Secret: empty, Back otherwise");

            while (true)
            {
                string input = Console.ReadLine();
                if (input != "")
                {
                    break;
                }

                string secret = Generate(lengthInput.Value);

                Console.WriteLine(secret);
            }

            return this.ReturnPage;
        }

        private static (bool, string) Validate(object obj)
        {
            bool isValid;
            string msg = "Invalid input (input must be inrage 1-100)";

            if (obj is int num)
            {
                isValid = (num > 0) && (num <= 100);
            }
            else
            {
                throw new InvalidOperationException();
            }

            return (isValid, msg);
        }

        private static string Generate(int length)
        {
            string characters =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                "abcdefghijklmnopqrstuvwxyz" +
                "0123456789" +
                "-_.~";

            var random = new Random(Guid.NewGuid().GetHashCode());

            string result = "";

            for (int i = 0; i < length; ++i)
            {
                int index = random.Next(characters.Length);

                char chr = characters[index];

                result += chr;
            }

            return result;
        }
    }
}
