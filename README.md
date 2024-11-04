Overview:
This project serves as a sample integration for handling online payments through FawryPay, a popular payment gateway. The API is designed to create a secure payment request for products or services using FawryPay’s SDK.

Features:
ASP.NET Core API with a PostFawry endpoint for initiating FawryPay payment requests.
Signature Generation: Ensures data integrity by generating a SHA-256 hash signature for secure payment requests.
Frontend Integration: Example HTML page with JavaScript to initiate the payment request and handle the FawryPay response.
Dynamic Language Support: Supports both Arabic and English based on user-selected language in the request.
Installation:
Clone the repository:

bash
 
git clone https://github.com/your-username/FawryPayIntegration.git
Configure Settings: Update appsettings.json with your FawryPay MerchantCode and SecretKey.

Run the Application:

In Visual Studio or via CLI:
bash
 
dotnet run
Testing: Use Postman or the included frontend HTML to test the integration.

Usage:
Backend API Endpoint:

Endpoint: /FawryPayController/PostFawry
Method: POST
Body Example:
json
 
{
    "order_reference": "ORDER123",

    "lang": "EN",

    "customer_phone": "01012345678",
    "customer_email": "customer@example.com",
    "customer_name": "John Doe",
    "package_id": 1,
    "package_price": 100.00,
    "decoder_id": 2,
    "hardware_price": 50.00
}
Note: Ensure all fields match the expected data types.
Frontend Usage:

Include fawrypay-payments.js in your HTML to access FawryPay’s checkout function.
Initiate the payment with:

javascript
 
fetch('https://localhost:7014/FawryPayController/PostFawry', 

{
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(orderData)
})
.then(response => response.json())
.then(data => {
    const configuration = { locale: 'en', mode: 'POPUP' };
    FawryPay.checkout(data, configuration);
})
.catch(error => console.error('Error:', error));

Additional Notes:

Testing: This setup uses a test environment URL for FawryPay (fawrystaging). For production, update the URLs to the live environment.
Security: Avoid hardcoding sensitive information like SecretKey directly in code for production.
Error Handling: Add error handling for edge cases such as network errors or missing order data
