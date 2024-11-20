using Microsoft.AspNetCore.Mvc;
using BasedGram.WebUI.Controllers;
using BasedGram.WebUI.DTO;
using TechTalk.SpecFlow;
using Xunit;

namespace E2E.Steps
{
    [Binding]
    public class LoginSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private _2FAController _controller;
        private IActionResult _result;

        public LoginSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"the authentication controller is initialized")]
        public void GivenTheAuthenticationControllerIsInitialized()
        {
            _controller = new _2FAController();
        }

        [When(@"the user logs in with email ""(.*)"" and password ""(.*)""")]
        public void WhenTheUserLogsInWithEmailAndPassword(string email, string password)
        {
            var loginResult = _controller.Login(new Login2FA_DTO(email, password, null));
            _scenarioContext["LoginResult"] = loginResult;
        }

        [Then(@"a verification code is sent to the email")]
        public void ThenAVerificationCodeIsSentToTheEmail()
        {
            var loginResult = (IActionResult)_scenarioContext["LoginResult"];
            var viewResult = Assert.IsType<OkObjectResult>(loginResult);
            Assert.Equal("Verification code sent to email.", viewResult.Value);
        }

        [When(@"the user enters the verification code ""(.*)"" for email ""(.*)""")]
        public void WhenTheUserEntersTheVerificationCodeForEmail(string code, string email)
        {
            var verifyResult = _controller.VerifyLogin(new Login2FA_DTO(email, null, code));
            _scenarioContext["VerifyResult"] = verifyResult;
        }

        [Then(@"the account is verified successfully")]
        public void ThenTheAccountIsVerifiedSuccessfully()
        {
            var verifyResult = (IActionResult)_scenarioContext["VerifyResult"];
            var viewResult = Assert.IsType<OkObjectResult>(verifyResult);
            Assert.Equal("Account verified successfully.", viewResult.Value);
        }
    }
}
