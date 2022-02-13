using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Logging;

namespace TransformalizeModule.Services {

   /// <summary>
   /// Combined Orchard Core Logging (NLog) and Logging to Process itself
   /// The MemoryLogger is only used for it's IsEnabled (per level) Methods, the idea is to log to the process itself
   /// </summary>
   /// <typeparam name="T"></typeparam>
   public class CombinedLogger<T> : IPipelineLogger {

      private readonly ILogger<T> _siteLogger;
      private readonly IContext _context;
      private readonly MemoryLogger _logger;

      public Transformalize.Contracts.LogLevel LogLevel => _logger.LogLevel;
      public List<LogEntry> Log => _logger.Log;

      public CombinedLogger(ILogger<T> siteLogger, MemoryLogger logger) {
         _siteLogger = siteLogger;
         _context = new PipelineContext(this, new Process { Name = Common.ModuleName });
         _logger = logger;
      }

      public void Clear() {
         _logger.Clear();
      }

      public void Debug(IContext context, Func<string> message) {
         if (message == null) {
            return;
         }
         if (_siteLogger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug) || _logger.DebugEnabled) {
            var msg = message();
            _siteLogger.LogDebug(msg);
            _logger.Debug(context, ()=>msg);
         }
      }

      public void Error(IContext context, string message, params object[] args) {
         if(message == null) {
            return;
         }
         if (_siteLogger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error) || _logger.ErrorEnabled) {
            var msg = args == null ? message : string.Format(message, args);
            _siteLogger.LogError(msg);
            _logger.Error(context, msg);
         }
      }

      public void Error(IContext context, Exception exception, string message, params object[] args) {
         if (message == null) {
            return;
         }
         if (_siteLogger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error) || _logger.ErrorEnabled) {
            var msg = args == null ? message : string.Format(message, args);
            _siteLogger.LogError(exception, msg);
            _logger.Error(context, exception, msg);
         }
      }

      public void Info(IContext context, string message, params object[] args) {
         if (message == null) {
            return;
         }
         if (_siteLogger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information) || _logger.InfoEnabled) {
            var msg = args == null ? message : string.Format(message, args);
            _siteLogger.LogInformation(msg);
            _logger.Info(context, msg);
         }
      }

      public void Warn(IContext context, string message, params object[] args) {
         if (message == null) {
            return;
         }
         if (_siteLogger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Warning) || _logger.WarnEnabled) {
            var msg = args == null ? message : string.Format(message, args);
            _siteLogger.LogWarning(msg);
            _logger.Warn(context, msg);
         }
      }

      public void SuppressConsole() {
         _logger.SuppressConsole();
      }

      // methods for when you don't have a context handy
      public void Debug(Func<string> message) {
         if (_siteLogger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug) || _logger.DebugEnabled) {
            Debug(_context, message);
         }
      }

      public void Info(Func<string> message) {
         if (_siteLogger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information) || _logger.InfoEnabled) {
            Info(_context, message());
         }
      }

      public void Warn(Func<string> message) {
         if (_siteLogger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Warning) || _logger.WarnEnabled) {
            Warn(_context, message());
         }
      }

      public void Error(Func<string> message) {
         if (_siteLogger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error) || _logger.ErrorEnabled) {
            Error(_context, message());
         }
      }

      public void Error(Exception exception, Func<string> message) {
         if (_siteLogger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Error) || _logger.ErrorEnabled) {
            Error(_context, exception, message());
         }
      }


   }
}
