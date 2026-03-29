namespace AuthDatabaseManager.Pages
{
    using System;

    public class InfoPage : Page
    {
        public override string Title => "Info";


        public InfoPage(Page returnPage) : base(returnPage)
        {
        }


        public override Page Show()
        {
            Console.WriteLine("Private encrypted key:");
            Write.Green("openssl genpkey -aes256 -out private.pem -pass file:passphrase.txt -algorithm RSA");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Public key:");
            Write.Cyan("openssl rsa -in private.pem -passin file:passphrase.txt -pubout -out public.key");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Fingerprint:");
            Write.Cyan("openssl pkey -pubin -in public.key -outform DER | openssl dgst -md5");
            Write.Warning("openssl x509 -in cert.crt -noout -fingerprint");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Public certificate:");
            Write.Warning("openssl req -new -x509 -key private.pem -passin file:passphrase.txt -out cert.crt -days 365");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Private certificate:");
            Write.Warning("openssl pkcs12 -export -out cert.pfx -inkey private.pem -passin file:pass.txt -in cert.crt");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("(any key to continue)");
            Console.ReadKey();

            return this.ReturnPage;
        }
    }
}
