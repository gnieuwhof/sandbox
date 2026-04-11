namespace TokenTest
{
    using System;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.Json;

    /*
    
    e.g.

    TokenValidator validator = TokenValidator.Create(header);

    string publicPem = File.ReadAllText("public.pem");

    string error = validator.Validate(
        issuer: "https://issuer.com",
        audience: "api.client.com",
        lifetime: true,
        scope: "entity.read",
        publicPem: publicPem
        );

     */

    public class TokenValidator
    {
        private readonly string message;
        private readonly string signature;


        public Header Header { get; }

        public Payload Payload { get; set; }


        public TokenValidator(string message, Header header,
            Payload payload, string signature)
        {
            this.message = message ??
                throw new ArgumentNullException(nameof(message));

            this.Header = header ??
                throw new ArgumentNullException(nameof(header));

            this.Payload = payload ??
                throw new ArgumentNullException(nameof(payload));

            this.signature = signature ??
                throw new ArgumentNullException(nameof(signature));
        }


        public static TokenValidator Create(AuthenticationHeaderValue authHeader)
        {
            string parameter = authHeader.Parameter;

            string[] parts = $"{parameter}".Split('.');

            if (parts.Length != 3)
            {
                return null;
            }

            string headerBase64 = parts[0];
            string payloadBase64 = parts[1];
            string signatureBase64 = parts[2];

            string message = $"{headerBase64}.{payloadBase64}";

            string headerJson = Base64UrlToJson(headerBase64);
            string payloadJson = Base64UrlToJson(payloadBase64);

            var header = JsonSerializer.Deserialize<Header>(headerJson);
            var payload = JsonSerializer.Deserialize<Payload>(payloadJson);

            var validator = new TokenValidator(
                message, header, payload, signatureBase64);

            return validator;
        }

        // Validators

        public bool ValidateIssuer(string issuer)
        {
            bool result = (this.Payload.Issuer == issuer);

            return result;
        }

        public bool ValidateAudience(string audience)
        {
            bool result = ($"{this.Payload.Audience}" == audience);

            return result;
        }

        public bool ValidateLifetime()
        {
            long unixEpoch = DateTimeOffset.Now.ToUnixTimeSeconds();

            bool result = (unixEpoch < this.Payload.Expires);

            return result;
        }

        public bool ValidateScope(string requiredScope)
        {
            string scope = this.Payload.Scopes;

            string[] scopes = scope.Split(' ');

            bool result = scopes.Contains(requiredScope);

            return result;
        }

        public bool ValidateSignature(string publicPem)
        {
            byte[] data = Encoding.UTF8.GetBytes(this.message);
            byte[] signature = FromBase64Url(this.signature);

            using var rsa = RSA.Create();

            // This handles -----BEGIN PUBLIC KEY-----
            rsa.ImportFromPem(publicPem);

            bool result = rsa.VerifyData(
                data,
                signature,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            );

            return result;
        }

        public string Validate(
            string issuer = null,
            string audience = null,
            bool lifetime = false,
            string scope = null,
            string publicPem = null
            )
        {
            if (!string.IsNullOrWhiteSpace(issuer)
                && !this.ValidateIssuer(issuer))
            {
                return $"Token issuer {this.Payload.Issuer} is invalid " +
                    $"(expected {issuer}).";
            }

            if (!string.IsNullOrWhiteSpace(audience)
                && !this.ValidateAudience(audience))
            {
                return $"Token audience {this.Payload.Audience} is invalid " +
                    $"(expected {audience}).";
            }

            if (lifetime && !this.ValidateLifetime())
            {
                return "The token has expired.";
            }

            if (!string.IsNullOrWhiteSpace(scope)
                && !this.ValidateScope(scope))
            {
                return $"Token does not contain required scope {scope} " +
                    $"(token scopes {this.Payload.Scopes}).";
            }

            if (!string.IsNullOrWhiteSpace(publicPem)
                && !this.ValidateSignature(publicPem))
            {
                return "Invalid signature.";
            }

            return null;
        }

        //

        private static byte[] FromBase64Url(string base64Url)
        {
            string padded = base64Url
                .Replace('-', '+')
                .Replace('_', '/');

            // fix padding
            switch (padded.Length % 4)
            {
                case 2: padded += "=="; break;
                case 3: padded += "="; break;
            }

            byte[] bytes = Convert.FromBase64String(padded);

            return bytes;
        }

        private static string Base64UrlToJson(string base64uRL)
        {
            byte[] bytes = FromBase64Url(base64uRL);

            string json = Encoding.UTF8.GetString(bytes);

            return json;
        }
    }
}
