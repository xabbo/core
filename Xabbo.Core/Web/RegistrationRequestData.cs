using System;
using System.Text.Json.Serialization;

namespace Xabbo.Core.Web
{
    public class RegistrationRequestData
    {
        [JsonPropertyName("captchaToken")]
        public string CaptchaToken { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("passwordRepeated")]
        public string PasswordRepeated { get; set; }

        [JsonPropertyName("birthdate")]
        public Birthdate Birthdate { get; set; }

        [JsonPropertyName("termsOfServiceAccepted")]
        public bool TermsOfServiceAccepted { get; set; }

        public RegistrationRequestData()
        {
            TermsOfServiceAccepted = true;
        }

        public RegistrationRequestData(string email, string password, int birthYear, int birthMonth, int birthDay)
        {
            Email = email;
            Password = PasswordRepeated = password;
            Birthdate = new Birthdate(birthYear, birthMonth, birthDay);
            TermsOfServiceAccepted = true;
        }
    }

    public class Birthdate
    {
        [JsonPropertyName("day")]
        public int Day { get; set; }

        [JsonPropertyName("month")]
        public int Month { get; set; }

        [JsonPropertyName("year")]
        public int Year { get; set; }

        public Birthdate() { }

        public Birthdate(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
        }
    }
}
