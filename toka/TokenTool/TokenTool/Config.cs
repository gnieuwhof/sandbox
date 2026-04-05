namespace TokenTool
{
    /*    
    {
	    "AuthorizeUrl": "orthrus.azurewebsites.net/2d000c7d-2cc3-4289-8277-f06424a3d396/v1.0/token",
	    "ClientId": "a3d594ac-fa9d-4708-85e3-637d9fc66db0",
	    "CertificateFile": null,
	    "Scope": "msg.read",
	    "TokenFile": "auth.token",
	    "TokenEncryption": true,
	    "YmdEncryptionKey": true
    }
     */

    public class Config
    {
        public string AuthorizeUrl { get; set; }

        public string ClientId { get; set; }

        public string CertificateFile { get; set; }

        public string Scope { get; set; }

        public string TokenFile { get; set; }

        public bool? TokenEncryption { get; set; }

        public bool? YmdEncryptionKey { get; set; }
    }
}
