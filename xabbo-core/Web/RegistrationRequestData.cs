using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Xabbo.Core.Web
{
    public class RegistrationRequestData
    {
        [JsonProperty("captchaToken")]
        public string CaptchaToken { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("passwordRepeated")]
        public string PasswordRepeated { get; set; }

        [JsonProperty("birthdate")]
        public Birthdate Birthdate { get; set; }

        [JsonProperty("termsOfServiceAccepted")]
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
        [JsonProperty("day")]
        public int Day { get; set; }

        [JsonProperty("month")]
        public int Month { get; set; }

        [JsonProperty("year")]
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
