using Homura.ORM;
using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.ViewModels;
using System;

namespace Sunctum.Domain.Logic.Async
{
    public class ThumbnailRecording : AsyncTaskMakerBase, IThumbnailRecording
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        public ThumbnailViewModel Target { get; set; }

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Begin ThumbnailRecording"));
        }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            sequence.Add(() => RecordThumbnail(Target));
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish ThumbnailRecording"));
        }

        private static void RecordThumbnail(ThumbnailViewModel thumbnail, DataOperationUnit dataOpUnit = null)
        {
            try
            {
                ThumbnailFacade.InsertOrReplace(thumbnail, dataOpUnit);
                s_logger.Debug($"Recorded Thumbnail into database. {thumbnail.ToString()}");
            }
            catch (Exception e)
            {
                s_logger.Error(e);
            }
        }
    }
}