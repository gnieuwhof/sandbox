namespace TokenTool
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading.Tasks;

    public class Program
    {
        public const string VERSION = "1";


        private static async Task<int> Main(string[] args)
        {
            if (args.Length != 2)
            {
                PrintUsage();

                return ExitCode.InvalidArgumentsCount;
            }

            string configFile = args[0];
            string secretOrPassword = args[1];

            Write($"TokenTool v{VERSION}");
            Write($"Config file: {configFile}");

            if (!File.Exists(configFile))
            {
                PrintUsage();

                return ExitCode.ConfigNotFound;
            }

            Config config = null;
            try
            {
                Write("Loading config file");
                string configJson = File.ReadAllText(configFile);

                Write("Deserializing config file");
                config = Serializer.Deserialize<Config>(configJson);
            }
            catch (Exception ex)
            {
                Error("There was an error deserializing the configuration:");
                Error(ex.Message);
                return ExitCode.ConfigError;
            }

            Write("Validating config file");
            bool validConfig = ValidateConfig(config);
            if (!validConfig)
            {
                return ExitCode.ConfigError;
            }

            Write($"Configuration:");
            Write($"Authorization URL: {config.AuthorizeUrl}");
            Write($"Client ID: {config.ClientId}");
            Write($"Certificate file: {config.CertificateFile}");
            Write($"Token encryption: {config.TokenEncryption ?? false}");
            Write($"YMD encryption key: {config.YmdEncryptionKey ?? false}");
            Write($"Token file: {config.TokenFile}");

            int result;

            try
            {
                result = await Process(config, secretOrPassword);
            }
            catch (Exception ex)
            {
                Error("There was an error processing the configuration:");
                Error(ex.Message);
                return ExitCode.UnknownError;
            }

            return result;
        }

        private static void PrintUsage()
        {
            Console.WriteLine($"TokenTool {VERSION}");
            Console.WriteLine("expected 2 startup arguments.");
            Console.WriteLine();
            Console.WriteLine("usage: token.exe <config> <sensitive>");
            Console.WriteLine();
            Console.WriteLine("e.g.");
            Console.WriteLine("token.exe config.json Sup3r5ecR3t");
            Console.WriteLine();
            Console.WriteLine("arguments:");
            Console.WriteLine("    <config> config file location");
            Console.WriteLine("    <sensitive> client secret/password");
            Console.WriteLine();
        }

        private static bool ValidateConfig(Config config)
        {
            bool result = true;

            if (string.IsNullOrWhiteSpace(config.AuthorizeUrl))
            {
                Error("No AuthorizeUrl found in config");
            }

            if (string.IsNullOrWhiteSpace(config.ClientId))
            {
                Error("No ClientId found in config");
            }

            if (string.IsNullOrWhiteSpace(config.TokenFile))
            {
                Error("No TokenFile found in config");
            }

            if (!string.IsNullOrWhiteSpace(config.CertificateFile) &&
                !File.Exists(config.CertificateFile))
            {
                Error($"CertificateFile {config.CertificateFile} does not exists");
            }

            return result;
        }

        public static void Error(string message)
        {
            Write(ConsoleColor.Red, message);
        }

        public static void Write(string message = "") =>
            Write(ConsoleColor.Gray, message);

        public static void Write(ConsoleColor color, params string[] message)
        {
            Console.ForegroundColor = color;

            foreach (string msg in message)
            {
                Console.WriteLine(msg);
            }

            Console.ResetColor();
        }

        private static async Task<int> Process(Config config, string secretOrPassword)
        {
            var httpClient = new HttpClient();

            string authUrl = config.AuthorizeUrl;
            if (!authUrl.Contains("://"))
            {
                authUrl = $"https://{authUrl}";
            }
            var authUri = new Uri(authUrl);

            string clientId = config.ClientId;

            string scope = config.Scope;

            HttpResponseMessage response;
            byte[] certificateBytes = null;

            string certificateFile = config.CertificateFile;
            if (!string.IsNullOrWhiteSpace(certificateFile))
            {
                Write($"Certificate file: {certificateFile}");
                Write("Loading certificate file");
                certificateBytes = File.ReadAllBytes(certificateFile);

                Write("Get token using certificate/password");
                response = await OAuth.GetCCAuthResponse(
                    httpClient, authUri, clientId, scope, certificateBytes, secretOrPassword);
            }
            else
            {
                Write("Get token using client ID/secret");
                response = await OAuth.GetCSAuthResponse(
                    httpClient, authUri, clientId, secretOrPassword, scope);
            }

            if (!response.IsSuccessStatusCode)
            {
                OAuthError oAuthError = await OAuth.GetError(response);
                Error("There was an error getting a token:");
                Error(oAuthError.Error);
                Error(oAuthError.ErrorDescription);
                return ExitCode.OAuthError;
            }

            var header = await OAuth.GetAuthenticationHeader(response);

            string content = $"{header}";
            if (config.TokenEncryption == true)
            {
                string postFix = "";

                if (config.YmdEncryptionKey == true)
                {
                    postFix = DateTime.UtcNow.Date.ToString("yyyyMMdd");
                }

                byte[] encryptionKey;
                if (certificateBytes == null)
                {
                    string toHash = $"{secretOrPassword}{postFix}";

                    encryptionKey = Encryptor.GetSha256Hash(toHash);
                }
                else
                {
                    var certificate =
                        new X509Certificate2(certificateBytes, secretOrPassword);

                    RSA rsaPrivateKey = certificate.GetRSAPrivateKey();

                    string toSign = $"{certificate.Thumbprint}{postFix}";

                    byte[] signBytes = rsaPrivateKey.SignData(
                        Encoding.UTF8.GetBytes(toSign),
                        HashAlgorithmName.SHA256,
                        RSASignaturePadding.Pkcs1
                        );

                    encryptionKey = Encryptor.GetSha256Hash(signBytes);
                }

                Write("Encrypting token");
                content = Encryptor.Encrypt(encryptionKey, content);
            }
            else
            {
                content = $"{header}";
            }

            string tokenFile = config.TokenFile;

            Write("Saving token to file");
            File.WriteAllText(tokenFile, content);

            return ExitCode.OK;
        }
    }
}
