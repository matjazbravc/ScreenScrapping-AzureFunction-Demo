using System;
using System.Runtime.CompilerServices;

namespace ScreenScrappingAzureFunctionDemo.Services.Logging {
    public interface ILog
    {
        bool IsDebugEnabled { get; }

        bool IsErrorEnabled { get; }

        bool IsFatalEnabled { get; }

        bool IsInfoEnabled { get; }

        bool IsWarnEnabled { get; }

        void Debug(string message = null, [CallerMemberName] string memberName = "");

        void Debug(Func<string> messageFactory);

        void Debug(string message, Exception exception);

        void DebugFormat(string format, params object[] args);

        void DebugFormat(Exception exception, string format, params object[] args);

        void DebugFormat(IFormatProvider formatProvider, string format, params object[] args);

        void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args);

        void Error(string message, bool throwException = true, bool includeDebugData = true, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0);

        void Error(Func<string> messageFactory);

        void Error(string message, Exception exception, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0);

        void Error(Exception exception, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0);

        void ErrorFormat(string format, params object[] args);

        void ErrorFormat(Exception exception, string format, params object[] args);

        void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args);

        void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args);

        void Fatal(string message);

        void Fatal(Func<string> messageFactory);

        void Fatal(string message, Exception exception);

        void FatalFormat(string format, params object[] args);

        void FatalFormat(Exception exception, string format, params object[] args);

        void FatalFormat(IFormatProvider formatProvider, string format, params object[] args);

        void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args);

        void Info(string message);

        void Info(Func<string> messageFactory);

        void Info(string message, Exception exception);

        void InfoFormat(string format, params object[] args);

        void InfoFormat(Exception exception, string format, params object[] args);

        void InfoFormat(IFormatProvider formatProvider, string format, params object[] args);

        void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args);

        void Warn(string message);

        void Warn(Func<string> messageFactory);

        void Warn(string message, Exception exception);

        void WarnFormat(string format, params object[] args);

        void WarnFormat(Exception exception, string format, params object[] args);

        void WarnFormat(IFormatProvider formatProvider, string format, params object[] args);

        void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args);
    }
}