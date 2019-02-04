using System;
using System.Collections.Generic;
using System.Linq;

namespace ScreenScrappingAzureFunctionDemo.Services.Extensions
{
	public static class ExceptionExtensions
	{

		/// <summary>
		/// Returns a list of all the exception messages from the top-level
		/// exception down through all the inner exceptions.
		/// </summary>
		public static IEnumerable<string> Messages(this Exception ex)
		{
			// return an empty sequence if the provided exception is null
			if (ex == null)
			{
				yield break;
			}

			// first return this exception's message at the beginning of the list
			if (ex.StackTrace == null)
			{
				yield return ex.Message;
			}
			else
			{
				yield return string.Concat(ex.Message, Environment.NewLine, ex.StackTrace);
			}

			// then get all the lower-level exception messages recursively (if any)
			var innerExceptions = Enumerable.Empty<Exception>();
		    if (ex is AggregateException aggEx)
			{
				if (aggEx.InnerExceptions.Any())
				{
					innerExceptions = aggEx.InnerExceptions;
				}
			}
			else if (ex.InnerException != null)
			{
				innerExceptions = new[]
                {
	                ex.InnerException
                };
			}
			foreach (var message in innerExceptions.SelectMany(innerEx => innerEx.Messages()))
			{
				yield return message;
			}
		}
	}
}