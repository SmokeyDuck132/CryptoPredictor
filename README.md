# Crypto Price Predictor

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-green.svg)
![GitHub Issues](https://img.shields.io/github/issues/SmokeyDuck132/CryptoPredictor.svg)
![GitHub Stars](https://img.shields.io/github/stars/SmokeyDuck132/CryptoPredictor.svg?style=social)

Crypto Price Predictor is a Windows Forms application developed in C# that allows users to fetch, display, and predict cryptocurrency prices using APIs from **Bybit** and **Gemini**. The application features live charting with technical indicators, theming options, and data export capabilities.

## Table of Contents

- [Features](#features)
- [Screenshots](#screenshots)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)
- [Support](#support)

## Features

- **API Integration**: Connect to Bybit and Gemini APIs to fetch and predict cryptocurrency data.
- **Data Fetching**: Retrieve historical and live candlestick (Kline) data based on user-specified trading pairs and time frames.
- **Data Visualization**: Display data in a `DataGridView` and an interactive live-updating chart with indicators like SMA, EMA, RSI, MACD, Bollinger Bands, etc.
- **Price Prediction**: Generate price predictions using Gemini's API with customizable input data points.
- **Theming**: Toggle between dark and light modes for a personalized user interface.
- **Export Functionality**: Export fetched data to Word or Excel formats.
- **Live Charting**: Real-time chart updates with interactive features such as zooming and panning.
- **Support Dev Feature**: Display an image to support the developer (e.g., donation QR code).

## Screenshots

[Main Interface]
![sc3](https://github.com/user-attachments/assets/6a1487f3-5abf-4bac-bf16-03a85c03d115)
![sc2](https://github.com/user-attachments/assets/f6dbc491-ab16-4f15-a490-76ca64bff511)
![sc1](https://github.com/user-attachments/assets/f95b85ab-37e7-47e5-8df4-7c984f3b7b7d)


*Main interface of the Crypto Price Predictor.*

Live Chart

![sc4](https://github.com/user-attachments/assets/2627c430-2e7e-4717-85ad-cfe4925bb1b7)

*Live chart displaying real-time data and indicators.*

## Prerequisites


- **Operating System**: Windows 10 or later
- **.NET Framework**: .NET Framework 8.0 or later
- **Development Environment**: Visual Studio 2019 or later (for building from source)
- **API Keys**:
  - **Bybit API Key & Secret**
  - **Gemini API Key**

> **⚠️ Important**: Ensure that you keep your API keys secure and **do not** expose them publicly. The application handles API keys securely, but always follow best practices for API key management.

## Installation

### Using the Executable

1. **Download the Latest Release**:
   - Navigate to the [Releases](https://github.com/SmokeyDuck132/CryptoPricePredictor/releases) section.
   - Download the latest `CryptoPricePredictor.exe` file.

2. **Run the Application**:
   - Double-click the downloaded executable to launch the application.

### Building from Source

1. **Clone the Repository**:

   ```bash
   git clone https://github.com/SmokeyDuck132/CryptoPricePredictor.git
   ```

2. **Open the Project**:
   - Navigate to the cloned directory.
   - Open `CryptoPricePredictor.sln` in Visual Studio.

3. **Restore Dependencies**:
   - Ensure all NuGet packages are restored. Visual Studio typically handles this automatically.

4. **Build the Project**:
   - Build the solution by selecting `Build > Build Solution` or pressing `Ctrl + Shift + B`.

5. **Run the Application**:
   - Start the application by pressing `F5` or selecting `Debug > Start Debugging`.


### Setting Up API Keys

1. **Bybit API Key & Secret**:
   - Obtain your API Key and Secret from your [Bybit account](https://www.bybit.com/app/user/wallet/api-management).
   - Open the application and enter your Bybit API Key and Secret in the designated fields.
   - Click on "Set Bybit API Key" to save the credentials.

2. **Gemini API Key**:
   - Obtain your API Key from your [Gemini account](https://gemini.com/account/api).
   - Enter your Gemini API Key in the designated field.
   - Click on "Set Gemini API Key" to save the credential.

> **Security Note**: The application does not store API keys persistently. Ensure you enter them each time you run the application or implement secure storage mechanisms if needed.

3. **Update README.md**:
   - Include instructions on setting environment variables for API keys (as shown above).

## Usage

1. **Select Trading Pair and Time Frame**:
   - Enter the desired trading pair (e.g., `BTCUSD`).
   - Select the time frame from the dropdown (e.g., 1, 5, 15, 60 minutes, or Daily).

2. **Fetch Data**:
   - Click on the "Fetch Data" button to retrieve historical data.
   - The data will populate the `DataGridView` and the live chart.

3. **View Live Chart**:
   - Navigate to the "Live Chart" tab.
   - Click on "Start Live Chart" to begin real-time data updates.

4. **Generate Predictions**:
   - Enter the number of data points to send for prediction in the "Number of lines" textbox.
   - Click on "Predict Prices" to generate predictions using Gemini's API.
   - Predictions will appear in the designated output textbox.

5. **Toggle Theme**:
   - Click on the moon/sun icon to switch between dark and light modes.

6. **Export Data**:
   - Click on the "Export" button to save data to Word or Excel formats.
   - *Note*: Export functionality is currently a placeholder and needs implementation.

7. **Support Developer**:
   - Click on the "Support Dev" button to view a support image (e.g., donation QR code).

## Contributing

Contributions are welcome! If you'd like to contribute to this project, please follow the guidelines.

## License

This project is licensed under the [MIT License](LICENSE). See the [LICENSE](LICENSE) file for details.

## Support

If you encounter any issues or have questions, feel free to [open an issue](https://github.com/SmokeyDuck132/CryptoPricePredictor/issues) or contact the maintainer.

---

### Security Considerations

Given that your application uses API keys for **Bybit** and **Gemini**, it's crucial to handle them securely to prevent unauthorized access and potential misuse.

**Best Practices:**

1. **Avoid Hardcoding API Keys**:
   - Do not hardcode API keys directly into your source code.
   - Use environment variables or secure storage solutions.

2. **Use Configuration Files Wisely**:
   - If you use configuration files (e.g., `appsettings.json`), ensure they are included in `.gitignore` to prevent accidental commits.
   - Provide a template file (e.g., `appsettings.template.json`) for users to fill in their API keys.

3. **Encrypt Sensitive Data**:
   - Consider encrypting API keys if you plan to store them persistently.
   - Utilize Windows Credential Manager or other secure storage mechanisms.

4. **Educate Users**:
   - Clearly instruct users not to expose their API keys.
   - Inform them about the importance of keeping their keys secure.

### Implementing Export Functionality

Currently, the export methods (`ExportToWord` and `ExportToExcel`) are placeholders. Consider implementing these features using appropriate libraries:

- **Export to Excel**:
  - Use libraries like [EPPlus](https://github.com/EPPlusSoftware/EPPlus) or [ClosedXML](https://github.com/ClosedXML/ClosedXML) to create and manipulate Excel files.
  
- **Export to Word**:
  - Use libraries like [DocX](https://github.com/xceedsoftware/DocX) or [Open XML SDK](https://github.com/OfficeDev/Open-XML-SDK) to create and manipulate Word documents.

## Acknowledgements

- [Bybit API](https://www.bybit.com/en-US/docs/)
- [Gemini API](https://docs.gemini.com/)
- [Skender.Stock.Indicators](https://github.com/ScottLogic/Stock.Indicators)
- [Windows Forms Documentation](https://docs.microsoft.com/en-us/dotnet/desktop/winforms/?view=netdesktop-7.0)

---

## Contact

For any questions or suggestions, feel free to [open an issue](https://github.com/SmokeyDuck132/CryptoPricePredictor/issues) or contact the maintainer directly at [SmokeyDuck132@gmail.com](mailto:SmokeyDuck132@gmail.com).

---

## License

This project is licensed under the [MIT License](LICENSE). See the [LICENSE](LICENSE) file for details.

---

*Happy Trading! 🚀*

---
