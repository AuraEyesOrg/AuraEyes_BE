using AURA.Tests.Integration.Fixtures;
using AURA.Tests.Integration.Helpers;
using AURA.Tests.Integration.Seed;
using Infrastructure.Persistence;

namespace AURA.Tests.Integration.Tests.Commands;

[Collection("Integration")]
public sealed class RequestConsultationTests
{
    private readonly IntegrationTestFixture _fixture;

    public RequestConsultationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task RequestConsultation_ShouldCreateAggregate_AndAssignOphthalmologist()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var token = await AuthHelper.LoginPatientAsync(client);
        AuthHelper.SetBearerToken(client, token);

        Guid patientId;
        Guid ophthalmologistId;
        Guid aiScreeningId;

        using (var scope = _fixture.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var patient = await TestDataSeeder.GetSeededPatientAsync(dbContext);
            var ophthalmologist = await TestDataSeeder.GetSeededOphthalmologistAsync(dbContext);
            var screening = await TestDataSeeder.CreateAiScreeningWithResultAsync(dbContext, patient.Id);

            patientId = patient.Id;
            ophthalmologistId = ophthalmologist.Id;
            aiScreeningId = screening.Screening.Id;
        }

        var response = await client.PostAsJsonAsync("/api/consultation-sessions/verification", new
        {
            patientId,
            aiScreeningId,
            price = 250000m,
            ophthalmologistId
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var payload = await HttpClientHelper.ReadApiResponseAsync<Guid>(response);
        payload.Success.Should().BeTrue();

        var consultationId = payload.Data;
        consultationId.Should().NotBeEmpty();

        using var assertScope = _fixture.CreateScope();
        var dbAssert = assertScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var created = await dbAssert.ConsultationSessions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == consultationId);

        created.Should().NotBeNull();
        created!.OphthalmologistId.Should().Be(ophthalmologistId);
    }

    [Fact]
    public async Task PaymentFlow_ShouldCreateDeposit_AndMarkAsPaid()
    {
        await _fixture.ResetDatabaseAsync();
        using var client = _fixture.CreateClient();

        var token = await AuthHelper.LoginPatientAsync(client);
        AuthHelper.SetBearerToken(client, token);

        var createDepositResponse = await client.PostAsJsonAsync("/api/wallets/deposit", new
        {
            amountVnd = 100000m,
            paymentMethod = 0,
            description = "Integration payment",
            returnUrl = "https://localhost/success",
            cancelUrl = "https://localhost/cancel"
        });

        createDepositResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var createJson = await HttpClientHelper.ReadJsonDocumentAsync(createDepositResponse);
        var orderCode = createJson.RootElement
            .GetProperty("data")
            .GetProperty("orderCode")
            .GetString();

        orderCode.Should().NotBeNullOrWhiteSpace();

        var verifyPaymentResponse = await client.PostAsJsonAsync("/api/wallets/verify-payment", new
        {
            orderCode
        });

        verifyPaymentResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var walletResponse = await client.GetAsync("/api/wallets");
        walletResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var walletJson = await HttpClientHelper.ReadJsonDocumentAsync(walletResponse);
        var balance = walletJson.RootElement.GetProperty("data").GetProperty("balance").GetDecimal();
        balance.Should().BeGreaterThan(0m);

        _fixture.ApiFactory.PaymentMockServer.LogEntries.Should().NotBeEmpty();
    }
}
