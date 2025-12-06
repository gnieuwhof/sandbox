namespace AuthDatabaseManager.Pages
{
    using System;

    public class InfoPage : ReturnBase
    {
        public override string Title => "Info";


        public InfoPage(Page returnPage)
        {
            this.ReturnPage = returnPage;
        }


        public override Page Show()
        {
            Console.WriteLine("Private encrypted key:");
            Write.Warning("openssl genpkey -aes256 -out private.pem -pass file:passphrase.txt -algorithm RSA");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Public key:");
            Write.Warning("openssl rsa -in private.pem -passin file:passphrase.txt -pubout -out public.key");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Fingerprint:");
            Write.Warning("openssl pkey -pubin -in public.key -outform DER | openssl dgst -md5");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("(any key to continue)");
            Console.ReadKey();

            return this.ReturnPage;
        }
    }
}
