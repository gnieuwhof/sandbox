namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Input;
    using System;

    public class Hex2Base64Page : ReturnBase
    {
        public override string Title => "Convert HEX 2 BASE64";


        public Hex2Base64Page(Page returnPage)
        {
            this.ReturnPage = returnPage;
        }


        public override Page Show()
        {
            Page result = ExceptionRetry(this.ReturnPage, ConvertHex);

            return result;
        }

        private Page ConvertHex()
        {
            while (true)
            {
                Console.WriteLine("Enter hexadecimal string.");
                Console.WriteLine();
                Console.WriteLine("e.g.");
                Console.WriteLine("a5600d23947a0ed5526b01ac53d8ab5a");
                Console.WriteLine("A5 60 0D 23 94 7A 0E D5 52 6B 01 AC 53 D8 AB 5A");
                Console.WriteLine("a5:60:0d:23:94:7a:0e:d5:52:6b:01:ac:53:d8:ab:5a");
                Console.WriteLine("0xA5 0x60 0x0D 0x23 0x94 0x7A 0x0E 0xD5 0x52 0x6B 0x01 0xAC 0x53 0xD8 0xAB 0x5A");
                Console.WriteLine();
                Console.WriteLine("(c to Cancel)");
                var hexInput = new InputString("Input:");

                string input = hexInput.GetValue();

                if (input == "c")
                {
                    return null;
                }

                if (string.IsNullOrWhiteSpace(input))
                {
                    Write.Error("No input.");

                    Page retryResult = this.Retry("1 Retry (y/N)", 'y');

                    return retryResult;
                }

                input = $"{input}";
                input = input.Replace(" ", "");
                input = input.Replace("0x", "");
                input = input.Replace(":", "");

                try
                {
                    byte[] converted = Convert.FromHexString(input);

                    string base64 = Convert.ToBase64String(converted);

                    base64 = UrlSafe(base64);

                    Console.WriteLine();
                    Console.WriteLine("BASE64:");
                    Write.Warning(base64);
                    Console.WriteLine();

                    Page retyResult = this.Retry("Another (y/N)\n\r(y clears the screen)", 'y');

                    return retyResult;
                }
                catch (Exception ex)
                {
                    Write.Error(ex.Message);

                    Page retryResult = this.Retry("3Retry (y/N)", 'y');

                    return retryResult;
                }
            }
        }

        private Page Retry(string message, char nonDefaultChar)
        {
            Console.WriteLine(message);

            string input = Console.ReadLine();

            if (input != $"{nonDefaultChar}")
            {
                return this.ReturnPage;
            }

            return this;
        }

        private static string UrlSafe(string input)
        {
            string result = $"{input}";

            result = result.Replace("+", "-");
            result = result.Replace("/", "_");
            result = result.Replace("=", "");

            return result;
        }
    }
}
