using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Xabbo.Core.Web
{
    /* Example data
    {
        "captchaToken":"03AOLTBLQio7iqR4sMrdb4F3pgO53SHU1jZ6WBxBaJcb_QxO5tuxQ7fQmR-ktF977Wj4stV2nM_hjiMq7wiCk_8Pz5ECvfkltw7E4OnRvz30S3V9ThpdosZKG_ARXlClFekg3rIeLDy5VvOLelqDswwRA_lrUGTx2pjiWPbbUGkWnJe2kWXWlIQN-GJgEQN_cCezRbS-aREKzrvpZU_HXwtnxx5wdtJsdZFhATJs3gKW3Sqglft8cQo-F6D5u0tOSSGOPBWbx0wzcAM75vC5a1hl3h3wnqXG58h3ctm9fYi4KCSS7HdbpAKV0tHouidl8Fxja1agvtDCfmixkQup2lFX9tjn8u_UtYeLf173MERPfysiBAJtaLldlO1NWxtxzWCd2Oe_aHEnhYx7ewdaTt8q6q_sIwJ3n9uB-Mt_C5v07n-RCe8PMc8KWXAkTr-xyv8B5UcHYyGoeS0pEWOWYcdi_BDUimu2v7dO458TfEdGwjFAQEh2YXbuw0VX3YvFHs2tk65c4ut_9F",
        "email":"sPG3TLSyZ5vtioZpzViq79j8aUvNWo+0000@gmail.com",
        "password":"{(AA9J%k9Uc7tfU&o5x2",
        "passwordRepeated":"{(AA9J%k9Uc7tfU&o5x2",
        "birthdate":{"day":1,"month":1,"year":1990},
        "termsOfServiceAccepted":true
    }
    */

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
