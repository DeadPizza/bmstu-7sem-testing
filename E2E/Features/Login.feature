Feature: Login

  Scenario: User logs in and verifies account
    Given the authentication controller is initialized
    When the user logs in with email "trehsmeh@gmail.com" and password "1337"
    Then a verification code is sent to the email
    When the user enters the verification code "123456" for email "trehsmeh@gmail.com"
    Then the account is verified successfully

