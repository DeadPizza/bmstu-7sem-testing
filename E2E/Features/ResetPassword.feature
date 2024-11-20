Feature: ResetPassword
  Scenario: Resetting a user's password
    Given the authentication controller is initialized for reset
    When the user logs in with email "trehsmeh@gmail.com" and password "1337" for reset
    Then a verification code is sent to the email for reset
    When the user enters the verification code "123456" for email "trehsmeh@gmail.com" and sends password "123" for reset
    Then the password is reset successfully