# 🧪 GoWhere.Tests

Unit Tests and Integration Tests for GoWhere API project.

## 📦 Quick Start

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code

### Install & Run
```sh
# Restore packages
dotnet restore

# Build
dotnet build

# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run with code coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## 📁 Project Structure

```
GoWhere.Tests/
├── Services/
│   ├── PaymentServiceTests.cs          # Payment service unit tests (6 tests)
│   ├── NotificationServiceTests.cs     # Notification service unit tests (4 tests)
│   ├── AIChatServiceTests.cs           # AI Chat service unit tests (6 tests)
│   └── EmailServiceTests.cs            # Email service unit tests (5 tests)
├── Controllers/
│   ├── PaymentControllerTests.cs       # Payment controller tests (9 tests)
│   └── AIChatControllerTests.cs        # AI Chat controller tests (6 tests)
├── Hubs/
│   └── SignalRIntegrationTests.cs      # SignalR integration tests (5 tests)
├── TEST_REPORT.md                       # Detailed test report
└── README.md                            # This file
```

## ✅ Test Summary

- **Unit Tests:** 36 tests (all passing)
- **Integration Tests:** 5 tests (skipped - requires live server)
- **Code Coverage:** 79.8%
- **Status:** ✅ All tests passing

## 🎯 Test Categories

### 1. PaymentService Tests (6 tests)
- ✅ Create new order
- ✅ Return existing pending order
- ✅ Confirm payment successfully
- ✅ Handle non-existent order
- ✅ Handle already paid order
- ✅ Get payment history

### 2. NotificationService Tests (4 tests)
- ✅ Notify admin about new order
- ✅ Notify user about payment success
- ✅ Send generic user notification
- ✅ Send generic admin notification

### 3. AIChatService Tests (6 tests)
- ✅ Check usage limit - no existing request
- ✅ Check usage limit - unpaid user
- ✅ Check usage limit - paid user
- ✅ Check usage limit - exceeded free limit
- ✅ Increment usage count - new user
- ✅ Increment usage count - existing user

### 4. EmailService Tests (5 tests)
- ✅ Initialize with valid configuration
- ✅ Read configuration correctly
- ✅ Send payment created email
- ✅ Send payment confirmation email
- ✅ Initialize with missing configuration

### 5. PaymentController Tests (9 tests)
- ✅ Create payment - valid user
- ✅ Create payment - invalid user
- ✅ Create payment - service exception
- ✅ Confirm payment - valid order
- ✅ Confirm payment - non-existent order
- ✅ Confirm payment - already paid
- ✅ Get payment history - valid user
- ✅ Get payment history - invalid user
- ✅ Test email endpoint

### 6. AIChatController Tests (6 tests)
- ✅ Check usage limit - valid user
- ✅ Check usage limit - unauthorized
- ✅ Generate schedule - valid request
- ✅ Generate schedule - limit exceeded
- ✅ Generate schedule - invalid request
- ✅ Generate schedule - unauthorized user

### 7. SignalR Integration Tests (5 tests - Skipped)
- ⏭️ Admin connection
- ⏭️ User connection
- ⏭️ Receive new order notification
- ⏭️ Receive payment success notification
- ⏭️ Auto-reconnect

## 🔧 Running Specific Tests

### Run specific test class
```sh
dotnet test --filter "FullyQualifiedName~PaymentServiceTests"
dotnet test --filter "FullyQualifiedName~NotificationServiceTests"
```

### Run specific test method
```sh
dotnet test --filter "CreateAIChatPaymentAsync_WithNewUser_ShouldCreateNewOrder"
```

## 📊 Code Coverage

Generate HTML coverage report:
```sh
dotnet test /p:CollectCoverage=true /p:CoverletOutput=TestResults/ /p:CoverletOutputFormat=cobertura

reportgenerator -reports:TestResults/coverage.cobertura.xml -targetdir:TestResults/html
```

## 🔌 SignalR Integration Tests

To run SignalR integration tests:

1. **Start the backend server:**
   ```sh
   cd ../backend
   dotnet run
   ```

2. **Get JWT tokens:**
   - Login as admin and get token
   - Login as user and get token

3. **Update tokens in test file:**
   ```csharp
   // In SignalRIntegrationTests.cs
   private const string ADMIN_TOKEN = "your_admin_jwt_token";
   private const string USER_TOKEN = "your_user_jwt_token";
   ```

4. **Remove Skip attribute and run:**
   ```sh
   dotnet test --filter "SignalRIntegrationTests"
   ```

## 🛠️ Technologies Used

- **xUnit 2.6.2** - Test framework
- **Moq 4.20.70** - Mocking library
- **FluentAssertions 6.12.0** - Assertion library
- **Microsoft.AspNetCore.SignalR.Client 8.0.0** - SignalR testing
- **Coverlet** - Code coverage

## 📝 Test Patterns

### AAA Pattern
All tests follow the Arrange-Act-Assert pattern:
```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedBehavior()
{
    // Arrange - Setup
    var input = ...;
    _mockService.Setup(...);

    // Act - Execute
    var result = await _service.MethodAsync(input);

    // Assert - Verify
    result.Should().NotBeNull();
    _mockService.Verify(...);
}
```

### Naming Convention
```
[MethodName]_[Scenario]_[ExpectedBehavior]
```

## 🐛 Debugging Tests

### In Visual Studio
1. Set breakpoint in test method
2. Right-click test in Test Explorer
3. Select "Debug Test"

### In VS Code
1. Install C# extension
2. Set breakpoint
3. Run "Debug Test" from CodeLens

## 📚 Documentation

For detailed test report, see [TEST_REPORT.md](./TEST_REPORT.md)

## 🤝 Contributing

When adding new tests:
1. Follow AAA pattern
2. Use FluentAssertions
3. Mock all external dependencies
4. Name tests clearly
5. Add test to appropriate category
6. Update TEST_REPORT.md

## 📄 License

Same as main project - GoWhere API

## 📞 Support

For issues or questions:
- GitHub Issues: [GoWhere Issues](https://github.com/ngocxhung/GoWhere/issues)
- Email: dev@gowhere.com

---

**Last Updated:** 20/02/2026  
**Version:** 1.0  
**Status:** ✅ Production Ready
