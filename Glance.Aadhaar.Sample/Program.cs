using System.Security.Cryptography.X509Certificates;
using Glance.Aadhaar.Sample;
using Glance.Aadhaar.Security;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Hello, World! - Aadhaar Authentication Sample App For GlanceVillageAPI");

var options = new AadhaarOptions();
new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build()
    .GetSection("Aadhaar")
    .Bind(options);

var signerAndVerifier = new XmlSignature
{
    Signer = new X509Certificate2(options.AuaSignatureKeyPath, "public", X509KeyStorageFlags.Exportable),
    Verifier = new X509Certificate2(options.UidaiSignatureKeyPath)
};

Auth.Options = options;
Auth.Signer = signerAndVerifier;
Auth.Verifier = signerAndVerifier;

await Auth.AuthenticateAsync();

Console.WriteLine("Press any key to exit...");

Console.ReadKey();

signerAndVerifier.Dispose();