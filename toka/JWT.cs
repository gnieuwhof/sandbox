namespace Test
{
    using System.Text.Json.Serialization;

    public class Header
    {
        /*
        {
          "alg": "RS256",
          "typ": "JWT"
        }
         */
        //public const string BASE64 = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9";


        /*
        ”alg” Param Value	Digital Signature or MAC Algorithm	Implementation Requirements
        HS256	HMAC using SHA-256	Required
        HS384	HMAC using SHA-384	Optional
        HS512	HMAC using SHA-512	Optional
        RS256	RSASSA-PKCS1-v1_5 using SHA-256	Recommended
        RS384	RSASSA-PKCS1-v1_5 using SHA-384	Optional
        RS512	RSASSA-PKCS1-v1_5 using SHA-512	Optional
        ES256	ECDSA using P-256 and SHA-256	Recommended+
        ES384	ECDSA using P-384 and SHA-384	Optional
        ES512	ECDSA using P-521 and SHA-512	Optional
        PS256	RSASSA-PSS using SHA-256 and MGF1 with SHA-256	Optional
        PS384	RSASSA-PSS using SHA-384 and MGF1 with SHA-384	Optional
        PS512	RSASSA-PSS using SHA-512 and MGF1 with SHA-512	Optional
        none	No digital signature or MAC performed	Optional
         */
        [JsonPropertyName("alg")]
        public string Algorithm { get; set; }


        [JsonPropertyName("typ")]
        public string Type { get; set; }


        /*
         * kid: A hint for the server to identify which key was used to sign the token. 
         * jwk: A JSON object containing the public key used to sign the token. 
         * jku: A URL from which the server can retrieve the public key set. 
         * x5t: The SHA-1 hash of an X.509 certificate.
         * x5t#s256: The SHA-256 hash of an X.509 certificate.
         * x5c: A chain of X.509 certificates used to sign the JWT.
         */

        [JsonPropertyName("kid")]
        public string KID { get; set; }

        [JsonPropertyName("x5t")]
        public string X5t { get; set; }
    }

    public class Payload
    {
        [JsonPropertyName("iss")]
        public string Issuer { get; set; }

        [JsonPropertyName("sub")]
        public string Subject { get; set; }

        [JsonPropertyName("aud")]
        public object Audience { get; set; }

        [JsonPropertyName("exp")]
        public int? Expires { get; set; }

        [JsonPropertyName("nbf")]
        public int? NotBefore { get; set; }

        [JsonPropertyName("iat")]
        public int? IssuedAt { get; set; }

        [JsonPropertyName("jti")]
        public string JwtID { get; set; }

        [JsonPropertyName("scopes")]
        public string Scopes { get; set; }
    }
}
