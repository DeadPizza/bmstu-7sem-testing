using Microsoft.AspNetCore.Mvc;
using TechTalk.SpecFlow;
using BasedGram.WebUI.Controllers;
using BasedGram.WebUI.DTO;
using Xunit;

namespace E2E.Steps;

[Binding]
public class ResetPasswordSteps
{
    private readonly ScenarioContext _scenarioContext;
    private _2FAController _controller;
    private IActionResult _result;

    public ResetPasswordSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given(@"the authentication controller is initialized for reset")]
    public void GivenTheAuthenticationControllerIsInitialized()
    {
        _controller = new _2FAController();
    }

    [When(@"the user logs in with email ""(.*)"" and password ""(.*)"" for reset")]
    public void WhenTheUserLogsInWithEmailAndPassword(string email, string password)
    {
        var loginResult = _controller.ResetPassword(new Login2FA_DTO(email, password, null));
        _scenarioContext["ResetPasswordResult"] = loginResult;
    }

    [Then(@"a verification code is sent to the email for reset")]
    public void ThenAVerificationCodeIsSentToTheEmail()
    {
        var loginResult = (IActionResult)_scenarioContext["ResetPasswordResult"];
        var viewResult = Assert.IsType<OkObjectResult>(loginResult);
        Assert.Equal("Verification code sent to email.", viewResult.Value);
    }

    [When(
        @"the user enters the verification code ""(.*)"" for email ""(.*)"" and sends password ""(.*)"" for reset"
    )]
    public void WhenTheUserEntersTheVerificationCodeForEmail(
        string code,
        string email,
        string password
    )
    {
        var verifyResult = _controller.ResetPasswordVerify(
            new Login2FA_DTO(email, password, code)
        );
        _scenarioContext["ResetPasswordVerifyResult"] = verifyResult;
    }

    [Then(@"the password is reset successfully")]
    public void ThenTheAccountIsVerifiedSuccessfully()
    {
        var verifyResult = (IActionResult)_scenarioContext["ResetPasswordVerifyResult"];
        var viewResult = Assert.IsType<OkObjectResult>(verifyResult);
        Assert.Equal("Password reset successfully.", viewResult.Value);
    }
}
