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
            Write.Dark("openssl genpkey -algorithm RSA \\");
            Write.Dark("  -pkeyopt rsa_keygen_bits:2048 -pkeyopt rsa_keygen_pubexp:65537 \\");
            Write.Dark("  -aes-256-cbc -out private.pem -pass file:passphrase.txt");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Public key:");
            Write.Cyan("openssl rsa -in private.pem -passin file:passphrase.txt -pubout -out public.key");
            Write.Dark("openssl rsa -inform PEM -outform PEM -in private.pem -passin file:passphrase.txt \\");
            Write.Dark("  -pubout -out public.key");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Fingerprint:");
            Write.Cyan("openssl pkey -pubin -in public.key -outform DER | openssl dgst -md5");
            Write.Warning("openssl x509 -in cert.crt -noout -fingerprint");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Public certificate:");
            Write.Warning("openssl req -new -x509 -key private.pem -passin file:passphrase.txt -out cert.crt -days 365");
            Write.Dark("openssl req -new -x509 -key private.pem -passin file:passphrase.txt \\");
            Write.Dark("  -out cert.crt -days 365 -sha256");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Private certificate:");
            Write.Warning("openssl pkcs12 -export -out cert.pfx -inkey private.pem -passin file:pass.txt -in cert.crt");
            Write.Dark("openssl pkcs12 -export -out cert.pfx -inkey private.pem -passin file:pass.txt \\");
            Write.Dark("  -in cert.crt -keypbe AES-256-CBC -certpbe AES-256-CBC -macalg SHA256 -iter 2048");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("(any key to continue)");
            Console.ReadKey();

            return this.ReturnPage;
        }
    }
}
