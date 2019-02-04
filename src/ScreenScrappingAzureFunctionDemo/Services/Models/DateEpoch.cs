using System;
using System.Runtime.Serialization;

namespace ScreenScrappingAzureFunctionDemo.Services.Models
{
    // inspired by: https://blogs.msdn.microsoft.com/documentdb/2014/11/17/working-with-dates-in-azure-documentdb/
    [DataContract, Serializable]
    public class DateEpoch
    {
        public DateEpoch()
        {
        }

        public DateEpoch(DateTime date)
        {
            Date = date;
        }

        [DataMember(Name = "date")]
        public DateTime Date { get; set; }

        [DataMember(Name = "epoch")]
        public long Epoch => ToUnixEpochDate(Date);

        /// <summary>
        ///     Get this datetime as a Unix epoch timestamp (seconds since Jan 1, 1970, midnight UTC).
        /// </summary>
        /// <param name="date">The date to convert.</param>
        /// <returns>Seconds since Unix epoch.</returns>
        private static long ToUnixEpochDate(DateTime date)
        {
            var timeStart = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
            if (date < timeStart)
            {
                return 0;
            }
            return (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
        }
    }
}