# FlexiMail

**FlexiMail** is a flexible email client library designed to integrate seamlessly into your .NET applications. It
provides an easy-to-use interface for sending emails and managing various email-related tasks.

## Features

- **Easy Integration**: Seamlessly integrate FlexiMail into your .NET projects.
- **Multiple Email Providers**: Support for multiple email service providers. [Call for contribution]
- **Asynchronous Operations**: Perform email sending and processing asynchronously.
- **Testable Components**: Easily write unit and integration tests for your email functionality.

## Installation

To install **FlexiMail**, you can add the project to your solution manually.

### Manual Installation

1. Clone the repository:
    ```bash
    git clone https://github.com/yourusername/FlexiMail.git
    ```
2. Add the `FlexiMail` project to your solution.

3. Reference the `FlexiMail` project in your main application project.

## Usage

To use FlexiMail, follow the steps below:

### Basic Example

```csharp
using FlexiMail;

public class EmailService
{
    private readonly IFlexiMailClient flexiMailClient;

    public EmailService(IFlexiMailClient flexiMailClient) =>
        this.flexiMailClient = flexiMailClient;

    public async Task SendWelcomeEmail(string recipientEmail)
    {
        var flexiMessage = new FlexiMessage()
            {
                Subject = "FlexiMail is a cool library",
                To = [recipientEmail],
                Body = new FlexiBody
                {
                    Content = "<h3>Welcome to FlexiMail</h3><p>Bonjour tout le monde!</p>",
                    ContentType = BodyContentType.Html
                }
            };

        await this.flexiMailClient.SendAndSaveCopyAsync(flexiMessage);
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
      "scope1",
      "scope2",
      "scope3"
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

## Testing

Unit tests are provided to ensure the reliability of the **FlexiMail** library. The tests are located in the
`FlexiMail.Tests.Unit` and `FlexiMail.Test.Integration` projects.

### Running Tests

To run the tests, use the following command in the terminal:

```bash
dotnet test
```

Ensure that all dependencies are restored before running the tests.

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