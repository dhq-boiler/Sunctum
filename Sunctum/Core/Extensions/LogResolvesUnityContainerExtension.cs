using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Builder;
using Unity.Extension;
using Unity.Strategies;

namespace Sunctum.Core.Extensions
{
    public class LogResolvesUnityContainerExtension : UnityContainerExtension
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        protected override void Initialize()
        {
            Context.Strategies.Add(new LoggingStrategy(Logger), UnityBuildStage.PreCreation);
        }

        private class LoggingStrategy : BuilderStrategy
        {
            private readonly ILogger _logger;

            public LoggingStrategy(ILogger logger)
            {
                _logger = logger;
            }

            public override void PreBuildUp(ref BuilderContext context)
            {
                // Be aware that for Singleton Resolving this log message will only be logged once, when the Singleton is first resolved. After that, there is no buildup and it is just returned from a cache.

                var registrationType = context.RegistrationType;
                var registrationName = context.Name;
                var resolvedType = context.Type;

                var registrationNameWithParenthesesOrNothing = string.IsNullOrEmpty(registrationName) ? "" : $"({registrationName})";
                _logger.Debug($"Resolving [{registrationType}{registrationNameWithParenthesesOrNothing}] => [{resolvedType}]");
            }
        }
    }
}
