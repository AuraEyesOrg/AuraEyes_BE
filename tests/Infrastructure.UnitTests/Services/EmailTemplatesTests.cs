using FluentAssertions;
using Infrastructure.Services.Email;

namespace Infrastructure.UnitTests.Services;

public class EmailTemplatesTests
{
    #region Subject Constants

    [Fact]
    public void EmailConfirmationSubject_ShouldNotBeEmpty()
    {
        EmailTemplates.EmailConfirmationSubject.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void PasswordResetSubject_ShouldNotBeEmpty()
    {
        EmailTemplates.PasswordResetSubject.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void WelcomeSubject_ShouldNotBeEmpty()
    {
        EmailTemplates.WelcomeSubject.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void OrganisationOnboardingSubject_ShouldNotBeEmpty()
    {
        EmailTemplates.OrganisationOnboardingSubject.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void OrganisationAccountProvisionedSubject_ShouldNotBeEmpty()
    {
        EmailTemplates.OrganisationAccountProvisionedSubject.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void AllSubjects_ShouldContainAura()
    {
        EmailTemplates.EmailConfirmationSubject.Should().ContainEquivalentOf("Aura");
        EmailTemplates.PasswordResetSubject.Should().ContainEquivalentOf("Aura");
        EmailTemplates.WelcomeSubject.Should().ContainEquivalentOf("Aura");
        EmailTemplates.OrganisationOnboardingSubject.Should().ContainEquivalentOf("AURA");
        EmailTemplates.OrganisationAccountProvisionedSubject.Should().ContainEquivalentOf("AURA");
    }

    #endregion

    #region GetEmailConfirmationBody

    [Fact]
    public void GetEmailConfirmationBody_ShouldReturnNonEmptyHtml()
    {
        var result = EmailTemplates.GetEmailConfirmationBody("https://app.auraeyes.vn/confirm?token=abc");

        result.Should().NotBeNullOrWhiteSpace();
        result.Should().Contain("<!DOCTYPE html>");
    }

    [Fact]
    public void GetEmailConfirmationBody_ShouldContainConfirmationLink()
    {
        const string link = "https://app.auraeyes.vn/confirm?token=xyz123";

        var result = EmailTemplates.GetEmailConfirmationBody(link);

        result.Should().Contain(link);
    }

    [Fact]
    public void GetEmailConfirmationBody_ShouldContainLogoUrlDerivedFromLink()
    {
        var result = EmailTemplates.GetEmailConfirmationBody("https://app.auraeyes.vn/confirm?token=abc");

        result.Should().Contain("https://app.auraeyes.vn/logo.png");
    }

    [Fact]
    public void GetEmailConfirmationBody_WithInvalidUri_ShouldFallbackToLocalhostLogo()
    {
        var result = EmailTemplates.GetEmailConfirmationBody("not-a-valid-url");

        result.Should().Contain("http://localhost:3000/logo.png");
    }

    [Fact]
    public void GetEmailConfirmationBody_ShouldContainVietnameseContent()
    {
        var result = EmailTemplates.GetEmailConfirmationBody("https://app.auraeyes.vn/confirm");

        result.Should().Contain("Xác nhận địa chỉ email");
        result.Should().Contain("Xác nhận email");
    }

    [Fact]
    public void GetEmailConfirmationBody_ShouldContainFooter()
    {
        var result = EmailTemplates.GetEmailConfirmationBody("https://app.auraeyes.vn/confirm");

        result.Should().Contain("AURA Healthcare System");
        result.Should().Contain("support@auraeyes.vn");
    }

    #endregion

    #region GetPasswordResetBody

    [Fact]
    public void GetPasswordResetBody_ShouldReturnNonEmptyHtml()
    {
        var result = EmailTemplates.GetPasswordResetBody("https://app.auraeyes.vn/reset?token=abc");

        result.Should().NotBeNullOrWhiteSpace();
        result.Should().Contain("<!DOCTYPE html>");
    }

    [Fact]
    public void GetPasswordResetBody_ShouldContainResetLink()
    {
        const string link = "https://app.auraeyes.vn/reset?token=xyz456";

        var result = EmailTemplates.GetPasswordResetBody(link);

        result.Should().Contain(link);
    }

    [Fact]
    public void GetPasswordResetBody_ShouldContainVietnameseContent()
    {
        var result = EmailTemplates.GetPasswordResetBody("https://app.auraeyes.vn/reset");

        result.Should().Contain("Đặt lại mật khẩu");
        result.Should().Contain("Thay đổi mật khẩu");
    }

    [Fact]
    public void GetPasswordResetBody_ShouldContain24HourWarning()
    {
        var result = EmailTemplates.GetPasswordResetBody("https://app.auraeyes.vn/reset");

        result.Should().Contain("24 giờ");
    }

    [Fact]
    public void GetPasswordResetBody_ShouldNotContainLogoImage()
    {
        var result = EmailTemplates.GetPasswordResetBody("https://app.auraeyes.vn/reset");

        result.Should().NotContain("<img");
    }

    #endregion

    #region GetWelcomeBody

    [Fact]
    public void GetWelcomeBody_ShouldReturnNonEmptyHtml()
    {
        var result = EmailTemplates.GetWelcomeBody("Nguyen Van A");

        result.Should().NotBeNullOrWhiteSpace();
        result.Should().Contain("<!DOCTYPE html>");
    }

    [Fact]
    public void GetWelcomeBody_ShouldContainFullName()
    {
        const string name = "Tran Thi B";

        var result = EmailTemplates.GetWelcomeBody(name);

        result.Should().Contain(name);
    }

    [Fact]
    public void GetWelcomeBody_WithNullOrWhitespaceName_ShouldFallbackToBan()
    {
        EmailTemplates.GetWelcomeBody("").Should().Contain("bạn");
        EmailTemplates.GetWelcomeBody("   ").Should().Contain("bạn");
    }

    [Fact]
    public void GetWelcomeBody_ShouldContainFeatureList()
    {
        var result = EmailTemplates.GetWelcomeBody("Test User");

        result.Should().Contain("lịch hẹn khám sàng lọc");
        result.Should().Contain("võng mạc");
        result.Should().Contain("hồ sơ y tế");
    }

    [Fact]
    public void GetWelcomeBody_ShouldContainWelcomeHeading()
    {
        var result = EmailTemplates.GetWelcomeBody("Test");

        result.Should().Contain("Chào mừng đến với Aura");
    }

    #endregion

    #region GetOrganisationOnboardingAdminBody

    [Fact]
    public void GetOrganisationOnboardingAdminBody_ShouldReturnNonEmptyHtml()
    {
        var result = EmailTemplates.GetOrganisationOnboardingAdminBody(
            "Bệnh viện Mắt TP.HCM", "Hospital", "Nguyen Van A",
            "admin@hospital.vn", "0901234567", "123 Nguyen Du", "GP-12345", "Test notes");

        result.Should().NotBeNullOrWhiteSpace();
        result.Should().Contain("<!DOCTYPE html>");
    }

    [Fact]
    public void GetOrganisationOnboardingAdminBody_ShouldContainAllRequiredFields()
    {
        var result = EmailTemplates.GetOrganisationOnboardingAdminBody(
            "Bệnh viện Mắt", "Clinic", "Dr. Tran",
            "dr.tran@clinic.vn", "0909999888", "456 Le Loi", "LIC-789", "Urgent request");

        result.Should().Contain("Bệnh viện Mắt");
        result.Should().Contain("Clinic");
        result.Should().Contain("Dr. Tran");
        result.Should().Contain("dr.tran@clinic.vn");
        result.Should().Contain("0909999888");
        result.Should().Contain("456 Le Loi");
        result.Should().Contain("LIC-789");
        result.Should().Contain("Urgent request");
    }

    [Fact]
    public void GetOrganisationOnboardingAdminBody_WithNullOptionalFields_ShouldShowDash()
    {
        var result = EmailTemplates.GetOrganisationOnboardingAdminBody(
            "Test Org", "Type", "Contact", "email@test.com",
            null, null, null, null);

        result.Should().Contain("—");
    }

    [Fact]
    public void GetOrganisationOnboardingAdminBody_ShouldContainAdminActionPrompt()
    {
        var result = EmailTemplates.GetOrganisationOnboardingAdminBody(
            "Org", "Type", "Contact", "email@test.com", null, null, null, null);

        result.Should().Contain("quản trị tổ chức");
    }

    #endregion

    #region GetOrganisationCredentialsBody

    [Fact]
    public void GetOrganisationCredentialsBody_ShouldReturnNonEmptyHtml()
    {
        var result = EmailTemplates.GetOrganisationCredentialsBody(
            "Dr. Nguyen", "Bệnh viện Mắt", "admin@hospital.vn", "TempP@ss123");

        result.Should().NotBeNullOrWhiteSpace();
        result.Should().Contain("<!DOCTYPE html>");
    }

    [Fact]
    public void GetOrganisationCredentialsBody_ShouldContainContactNameAndOrg()
    {
        var result = EmailTemplates.GetOrganisationCredentialsBody(
            "Dr. Le", "Phòng khám ABC", "le@abc.vn", "Pass123!");

        result.Should().Contain("Dr. Le");
        result.Should().Contain("Phòng khám ABC");
    }

    [Fact]
    public void GetOrganisationCredentialsBody_ShouldContainCredentials()
    {
        const string email = "org-admin@test.vn";
        const string password = "Secure#Temp$2024";

        var result = EmailTemplates.GetOrganisationCredentialsBody(
            "Admin", "Org", email, password);

        result.Should().Contain(email);
        result.Should().Contain(password);
    }

    [Fact]
    public void GetOrganisationCredentialsBody_ShouldContainPostLoginInstruction()
    {
        var result = EmailTemplates.GetOrganisationCredentialsBody(
            "Admin", "Org", "a@b.com", "pass");

        result.Should().Contain("ký hợp đồng");
    }

    #endregion

    #region Cross-cutting HTML Structure

    [Theory]
    [InlineData("GetEmailConfirmation")]
    [InlineData("GetPasswordReset")]
    [InlineData("GetWelcome")]
    [InlineData("GetOrganisationOnboarding")]
    [InlineData("GetOrganisationCredentials")]
    public void AllTemplates_ShouldContainHtmlStructure(string templateKey)
    {
        var html = templateKey switch
        {
            "GetEmailConfirmation" => EmailTemplates.GetEmailConfirmationBody("https://example.com/confirm"),
            "GetPasswordReset" => EmailTemplates.GetPasswordResetBody("https://example.com/reset"),
            "GetWelcome" => EmailTemplates.GetWelcomeBody("Test User"),
            "GetOrganisationOnboarding" => EmailTemplates.GetOrganisationOnboardingAdminBody(
                "Org", "Type", "Contact", "e@e.com", null, null, null, null),
            "GetOrganisationCredentials" => EmailTemplates.GetOrganisationCredentialsBody(
                "Contact", "Org", "e@e.com", "pass"),
            _ => throw new ArgumentException($"Unknown template key: {templateKey}")
        };

        html.Should().Contain("<html");
        html.Should().Contain("</html>");
        html.Should().Contain("AURA");
        html.Should().Contain("lang=\"vi\"");
    }

    #endregion
}
