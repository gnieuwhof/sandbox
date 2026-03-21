namespace TokenConsole
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    public static class OAuth
    {
        // These sample application registration values are available for all online instances.
        public const string CLIENT_ID = "51f81489-12ee-4a9e-aaae-a2591f45987d";


        public static async Task<HttpResponseMessage> GetUPAuthResponse(HttpClient httpClient,
            Uri authorizationUrl, string username, string password, string scope)
        {
            string postData =
                $"&client_id={CLIENT_ID}" +
                $"&grant_type=password" +
                $"&username={username}" +
                $"&password={password}" +
                $"&scope={scope}";

            HttpResponseMessage response = await GetAuthResponse(httpClient, authorizationUrl, postData);

            return response;
        }

        public static async Task<HttpResponseMessage> GetCSAuthResponse(HttpClient httpClient,
            Uri authorizationUrl, string clientId, string secret, string scope)
        {
            string postData =
                $"&client_id={clientId}" +
                $"&grant_type=client_credentials" +
                $"&client_secret={secret}" +
                $"&scope={scope}" +
                $"";

            HttpResponseMessage response = await GetAuthResponse(httpClient, authorizationUrl, postData);

            return response;
        }

        public static async Task<HttpResponseMessage> GetCCAuthResponse(
            HttpClient httpClient,
            Uri authorizationUrl,
            string clientId,
            string scope,
            byte[] certificateBytes,
            string password = null
            )
        {
            X509Certificate2 certificate = string.IsNullOrEmpty(password)
                ? new X509Certificate2(certificateBytes)
                : new X509Certificate2(certificateBytes, password);

            RSA rsaPrivateKey = certificate.GetRSAPrivateKey();

            byte[] hash = certificate.GetCertHash();
            string x5t = Base64UrlEncode(hash);

            HttpResponseMessage response = await GetCCAuthResponse(
                httpClient, authorizationUrl, clientId, scope, rsaPrivateKey, x5t);

            return response;
        }

        public static async Task<HttpResponseMessage> GetCCAuthResponse(
            HttpClient httpClient,
            Uri authorizationUrl,
            string clientId,
            string scope,
            RSA rsaPrivateKey,
            string x5t
            )
        {
            string assertion = Assertion(authorizationUrl, clientId, rsaPrivateKey, x5t);

            string postData =
                $"&client_id={clientId}" +
                $"&grant_type=client_credentials" +
                $"&client_assertion_type=urn:ietf:params:oauth:client-assertion-type:jwt-bearer" +
                $"&client_assertion={assertion}" +
                $"&scope={scope}" +
                $"";

            HttpResponseMessage response = await GetAuthResponse(httpClient, authorizationUrl, postData);

            return response;
        }

        private static async Task<HttpResponseMessage> GetAuthResponse(
            HttpClient httpClient, Uri authorizationUrl, string postData)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, authorizationUrl);
            request.Content = new StringContent(postData, Encoding.UTF8);
            request.Content.Headers.Remove("Content-Type");
            request.Content.Headers.TryAddWithoutValidation("Content-Type", $"application/x-www-form-urlencoded");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;

            HttpResponseMessage response = await httpClient.SendAsync(request);

            return response;
        }


        public static async Task<AuthenticationHeaderValue> GetAuthenticationHeader(
            HttpResponseMessage authResponse)
        {
            string jsonResponse = await authResponse.Content.ReadAsStringAsync();

            var jsonContent = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse);

            object token = jsonContent["access_token"];

            var header = new AuthenticationHeaderValue("Bearer", $"{token}");

            return header;
        }

        public static async Task<OAuthError> GetError(HttpResponseMessage authResponse)
        {
            string content = await authResponse.Content.ReadAsStringAsync();

            OAuthError oAuthError = JsonSerializer.Deserialize<OAuthError>(content);

            return oAuthError;
        }

        //
        // using var rsaPrivateKey = RSA.Create();
        // rsa.ImportFromEncryptedPem(pem, "password_here");
        // 
        // string kid = "1a2b3c4d5e6f7a8b9c0d1e2f3a4b5c6d7e8f9a0b";
        // 
        // // kid -> x5t
        // byte[] kidBytes = Convert.FromHexString(kid);
        // string x5t = Base64UrlEncode(kidBytes);
        // 
        private static string Assertion(Uri authorizationUrl,
            string clientId, RSA rsaPrivateKey, string x5t)
        {
            // x5t -> kid
            byte[] x5tBytes = FromBase64Url(x5t);
            string kid = Convert.ToHexString(x5tBytes);

            // Create JWT header
            var header = new
            {
                alg = "RS256",
                typ = "JWT",
                x5t,
                kid
            };

            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long exp = now + 600; // 10 minutes

            // Create JWT payload
            var payload = new
            {
                aud = $"{authorizationUrl}",
                exp,
                iss = clientId,
                jti = $"{Guid.NewGuid()}",
                nbf = now,
                sub = clientId
            };

            string headerBase64 = Base64UrlEncodeObj(header);
            string payloadBase64 = Base64UrlEncodeObj(payload);

            string signatureInput = $"{headerBase64}.{payloadBase64}";

            // Sign with certificate's private key
            byte[] signatureBytes = rsaPrivateKey.SignData(
                Encoding.UTF8.GetBytes(signatureInput),
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
                );

            string signature = Base64UrlEncode(signatureBytes);

            return $"{signatureInput}.{signature}";
        }

        private static string Base64UrlEncodeObj(object obj)
        {
            string json = JsonSerializer.Serialize(obj);

            byte[] bytes = Encoding.UTF8.GetBytes(json);

            string base64 = Base64UrlEncode(bytes);

            return base64;
        }

        private static string Base64UrlEncode(byte[] input)
        {
            string base64 = Convert.ToBase64String(input);

            // Convert to base64url
            base64 = base64
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');

            return base64;
        }

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
    }
}
