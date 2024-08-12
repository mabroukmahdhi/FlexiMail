<p align="center">
  <img  src="https://github.com/mabroukmahdhi/FlexiMail/blob/main/FlexiMail/icmail.png">
</p>

# FlexiMail

**FlexiMail** is a flexible email client library designed to integrate seamlessly into your .NET applications. It
provides an easy-to-use interface for sending emails and managing various email-related tasks.

[![Nuget](https://img.shields.io/nuget/v/FlexiMail)](https://www.nuget.org/packages/FlexiMail/)
[![Nuget](https://img.shields.io/nuget/dt/FlexiMail)](https://www.nuget.org/packages/FlexiMail/)
![.NET 8](https://img.shields.io/badge/.NET_8-COMPATIBLE-2ea44f)

## Features

- **Easy Integration**: Seamlessly integrate FlexiMail into your .NET projects.
- **Multiple Email Providers**: Support for multiple email service providers. [Call for contribution]
- **Asynchronous Operations**: Perform email sending and processing asynchronously.
- **Testable Components**: Easily write unit and integration tests for your email functionality.

## Installation

You can install **FlexiMail** via NuGet or by adding the project to your solution manually.

### NuGet Installation

To install FlexiMail using NuGet, run the following command in the NuGet Package Manager Console:

```bash
Install-Package FlexiMail
```
Or, you can find the package on [NuGet](https://www.nuget.org/packages/FlexiMail). 

### .NET CLI Installation
To install FlexiMail using the .NET CLI, use the following command:
```bash
dotnet add package FlexiMail
```

### Manual Installation

1. Clone the repository:
    ```bash
    git clone https://github.com/mabroukmahdhi/FlexiMail
    ```
2. Add the `FlexiMail` project to your solution.

3. Reference the `FlexiMail` project in your main application project.

## Usage

To use **FlexiMail**, follow the steps below:

### Basic Example

```csharp
using FlexiMail.Models.Configurations;
using FlexiMail.Models.Foundations.Bodies;
using FlexiMail.Models.Foundations.Messages;

namespace FlexiMail.Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configurations = new ExchangeConfigurations
            {
                ClientId = "your-client-id",
                ClientSecret = "your-client-secret",
                TenantId = "your-tenant-id", 
                Authority = "https://login.microsoftonline.com/{your-tenant-id}",
                Scopes = ["https://outlook.office365.com/.default"],
                SmtpAddress = "your-sender-email"
            };

            var flexiMailClient = new FlexiMailClient(configurations);

            var nessage = new FlexiMessage
            {
                To = ["email@domain.com"],
                Cc = ["other-email@domain.com"],
                Subject = "Keep testing FlexiMail",
                Body = new FlexiBody
                {
                    Content = "This is the message body. It can be a plain text or HTML.",
                    ContentType = BodyContentType.PlainText
                }
            };

            await flexiMailClient.SendAndSaveCopyAsync(nessage);

            Console.ReadKey();
        }
    }
}
```

## Configuration

**FlexiMail** can be configured to use different email service providers. Configure the desired provider in your app's
configuration file:

```json
{
   "ExchangeConfigurations": {
      "ClientId": "your-client-id",
      "ClientSecret": "your-client-secret",
      "TenantId": "your-tenant-id",
      "SmtpAddress": "your-smtp-address",
      "Sid": "your-sid",
      "PrincipalName": "your-principal-name",
      "Authority": "your-authority",
      "Scopes": [
         "https://outlook.office365.com/.default"
      ]
   }
}

```

Note: You can use `SmtpAddress`, `Sid` or `PrincipalName` in order to configure your exchange service.

## Architecture

**FlexiMail** is organized into the following components according
to [The-Standard](https://github.com/hassanhabib/The-Standard):

- **Brokers**: Handle communication with external email services.
- **Models**: Contain the data structures used across the library.
- **Services**: Core logic for sending and managing emails.
- **Extensions**: Helper methods and extensions for common tasks.

The main class, `FlexiMailClient`, implements the `IFlexiMailClient` interface and serves as the entry point for most
operations.

## Contributing

Contributions to **FlexiMail** are welcome! Please follow these steps to contribute:

1. Fork the repository.
2. Create a new branch (`git checkout -b users/your-github-id/YourFeature`).
3. Commit your changes (`git commit -m 'Add new feature'`).
4. Push to the branch (`git push origin users/your-github-id/YourFeature`).
5. Open a Pull Request.

## License

This project is licensed under the MIT License - see
the [LICENSE](https://github.com/mabroukmahdhi/FlexiMail/blob/main/LICENSE) file for details.

## Contact
For more information or questions, feel free to contact me at [contact@mahdhi.com](mailto:contact@mahdhi.com).