using Domain.Entities.Contracts;
using Domain.Enums;
using FluentAssertions;

namespace Domain.UnitTests.Entities;

public class ContractTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _templateId = Guid.NewGuid();
    private const string ValidContractNumber = "CTR-2025-001";

    private Contract CreateDraftContract()
        => new(_userId, _templateId, ValidContractNumber, 100, 0.20m);

    private Contract CreatePendingSignatureContract()
    {
        var contract = CreateDraftContract();
        contract.SendForSignature();
        return contract;
    }

    private Contract CreateActiveContract()
    {
        var contract = CreatePendingSignatureContract();
        contract.Sign("Signed content");
        return contract;
    }

    #region Constructor

    [Fact]
    public void Constructor_ValidInput_ShouldCreateDraftContract()
    {
        var contract = new Contract(_userId, _templateId, ValidContractNumber, 100, 0.15m, "content");

        contract.UserId.Should().Be(_userId);
        contract.TemplateId.Should().Be(_templateId);
        contract.ContractNumber.Should().Be(ValidContractNumber);
        contract.AiQuotaLimit.Should().Be(100);
        contract.PlatformCommissionRate.Should().Be(0.15m);
        contract.SignedContent.Should().Be("content");
        contract.Status.Should().Be(ContractStatus.Draft);
        contract.SignedDate.Should().BeNull();
        contract.ScannedDocumentUrl.Should().BeNull();
        contract.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Constructor_DefaultOptionalParams_ShouldUseDefaults()
    {
        var contract = new Contract(_userId, _templateId, ValidContractNumber);

        contract.AiQuotaLimit.Should().Be(0);
        contract.PlatformCommissionRate.Should().Be(0m);
        contract.SignedContent.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyContractNumber_ShouldThrow(string? invalidNumber)
    {
        var act = () => new Contract(_userId, _templateId, invalidNumber!);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("contractNumber");
    }

    #endregion

    #region SendForSignature

    [Fact]
    public void SendForSignature_DraftContract_ShouldTransitionToPendingSignature()
    {
        var contract = CreateDraftContract();

        contract.SendForSignature();

        contract.Status.Should().Be(ContractStatus.PendingSignature);
        contract.UpdatedAt.Should().NotBeNull();
    }

    [Theory]
    [InlineData(ContractStatus.PendingSignature)]
    [InlineData(ContractStatus.Active)]
    [InlineData(ContractStatus.Expired)]
    [InlineData(ContractStatus.Terminated)]
    [InlineData(ContractStatus.Cancelled)]
    public void SendForSignature_NonDraftStatus_ShouldThrow(ContractStatus status)
    {
        var contract = CreateContractWithStatus(status);

        var act = () => contract.SendForSignature();

        act.Should().Throw<InvalidOperationException>();
    }

    #endregion

    #region Sign

    [Fact]
    public void Sign_PendingSignatureContract_ShouldActivate()
    {
        var contract = CreatePendingSignatureContract();

        contract.Sign("Signed by user", "https://scanned.pdf");

        contract.Status.Should().Be(ContractStatus.Active);
        contract.SignedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        contract.SignedContent.Should().Be("Signed by user");
        contract.ScannedDocumentUrl.Should().Be("https://scanned.pdf");
    }

    [Fact]
    public void Sign_WithNullParams_ShouldPreserveExistingValues()
    {
        var contract = CreatePendingSignatureContract();
        contract.UploadScannedDocument("https://original-scan.pdf");

        contract.Sign();

        contract.Status.Should().Be(ContractStatus.Active);
        contract.ScannedDocumentUrl.Should().Be("https://original-scan.pdf");
    }

    [Fact]
    public void Sign_DraftContract_ShouldThrow()
    {
        var contract = CreateDraftContract();

        var act = () => contract.Sign();

        act.Should().Throw<InvalidOperationException>();
    }

    #endregion

    #region UploadScannedDocument

    [Fact]
    public void UploadScannedDocument_PendingSignature_ShouldSetUrl()
    {
        var contract = CreatePendingSignatureContract();

        contract.UploadScannedDocument("https://storage/doc.pdf");

        contract.ScannedDocumentUrl.Should().Be("https://storage/doc.pdf");
        contract.Status.Should().Be(ContractStatus.PendingSignature);
    }

    [Fact]
    public void UploadScannedDocument_DraftContract_ShouldThrow()
    {
        var contract = CreateDraftContract();

        var act = () => contract.UploadScannedDocument("https://storage/doc.pdf");

        act.Should().Throw<InvalidOperationException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UploadScannedDocument_EmptyUrl_ShouldThrow(string? invalidUrl)
    {
        var contract = CreatePendingSignatureContract();

        var act = () => contract.UploadScannedDocument(invalidUrl!);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("scannedDocumentUrl");
    }

    #endregion

    #region Expire

    [Fact]
    public void Expire_ActiveContract_ShouldTransitionToExpired()
    {
        var contract = CreateActiveContract();

        contract.Expire();

        contract.Status.Should().Be(ContractStatus.Expired);
    }

    [Fact]
    public void Expire_DraftContract_ShouldThrow()
    {
        var contract = CreateDraftContract();

        var act = () => contract.Expire();

        act.Should().Throw<InvalidOperationException>();
    }

    #endregion

    #region Terminate

    [Fact]
    public void Terminate_ActiveContract_ShouldTransitionToTerminated()
    {
        var contract = CreateActiveContract();

        contract.Terminate();

        contract.Status.Should().Be(ContractStatus.Terminated);
    }

    [Fact]
    public void Terminate_DraftContract_ShouldThrow()
    {
        var contract = CreateDraftContract();

        var act = () => contract.Terminate();

        act.Should().Throw<InvalidOperationException>();
    }

    #endregion

    #region Cancel

    [Fact]
    public void Cancel_DraftContract_ShouldTransitionToCancelled()
    {
        var contract = CreateDraftContract();

        contract.Cancel();

        contract.Status.Should().Be(ContractStatus.Cancelled);
    }

    [Fact]
    public void Cancel_PendingSignatureContract_ShouldTransitionToCancelled()
    {
        var contract = CreatePendingSignatureContract();

        contract.Cancel();

        contract.Status.Should().Be(ContractStatus.Cancelled);
    }

    [Theory]
    [InlineData(ContractStatus.Active)]
    [InlineData(ContractStatus.Expired)]
    [InlineData(ContractStatus.Terminated)]
    public void Cancel_FinalizedContract_ShouldThrow(ContractStatus status)
    {
        var contract = CreateContractWithStatus(status);

        var act = () => contract.Cancel();

        act.Should().Throw<InvalidOperationException>();
    }

    #endregion

    #region Update

    [Fact]
    public void Update_DraftContract_ShouldUpdateCommercialTerms()
    {
        var contract = CreateDraftContract();
        var newTemplateId = Guid.NewGuid();

        contract.Update(newTemplateId, 500, 0.30m);

        contract.TemplateId.Should().Be(newTemplateId);
        contract.AiQuotaLimit.Should().Be(500);
        contract.PlatformCommissionRate.Should().Be(0.30m);
    }

    [Fact]
    public void Update_ActiveContract_ShouldThrow()
    {
        var contract = CreateActiveContract();

        var act = () => contract.Update(Guid.NewGuid(), 200, 0.10m);

        act.Should().Throw<InvalidOperationException>();
    }

    #endregion

    #region Lifecycle

    [Fact]
    public void FullLifecycle_DraftToExpired_ShouldTransitionCorrectly()
    {
        var contract = CreateDraftContract();
        contract.Status.Should().Be(ContractStatus.Draft);

        contract.SendForSignature();
        contract.Status.Should().Be(ContractStatus.PendingSignature);

        contract.Sign("Signed");
        contract.Status.Should().Be(ContractStatus.Active);

        contract.Expire();
        contract.Status.Should().Be(ContractStatus.Expired);
    }

    [Fact]
    public void FullLifecycle_DraftToTerminated_ShouldTransitionCorrectly()
    {
        var contract = CreateDraftContract();

        contract.SendForSignature();
        contract.Sign();
        contract.Terminate();

        contract.Status.Should().Be(ContractStatus.Terminated);
    }

    #endregion

    private Contract CreateContractWithStatus(ContractStatus targetStatus)
    {
        var contract = CreateDraftContract();

        return targetStatus switch
        {
            ContractStatus.Draft => contract,
            ContractStatus.PendingSignature => CreatePendingSignatureContract(),
            ContractStatus.Active => CreateActiveContract(),
            ContractStatus.Expired => ExpireContract(),
            ContractStatus.Terminated => TerminateContract(),
            ContractStatus.Cancelled => CancelContract(),
            _ => contract
        };

        Contract ExpireContract() { var c = CreateActiveContract(); c.Expire(); return c; }
        Contract TerminateContract() { var c = CreateActiveContract(); c.Terminate(); return c; }
        Contract CancelContract() { var c = CreateDraftContract(); c.Cancel(); return c; }
    }
}
