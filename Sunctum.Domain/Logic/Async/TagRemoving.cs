

using Ninject;
using NLog;
using Sunctum.Domain.Data.DaoFacade;
using Sunctum.Domain.Extensions;
using Sunctum.Domain.Models.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunctum.Domain.Logic.Async
{
    public class TagRemoving : AsyncTaskMakerBase, ITagRemoving
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        [Inject]
        public ITagManager TagManager { get; set; }

        public IEnumerable<string> TagNames { get; set; }

        public override void ConfigurePreTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Start TagRemoving"));
            sequence.Add(() => s_logger.Info($"      TagNames : {TagNames.ArrayToString()}"));
        }

        public override void ConfigureTaskImplementation(AsyncTaskSequence sequence)
        {
            foreach (var tagName in TagNames)
            {
                var itTargets = (from it in TagManager.Chains
                                 join t in TagNames on it.Tag.Name equals t
                                 select it).ToList();

                foreach (var target in itTargets)
                {
                    sequence.Add(new Task(() =>
                    {
                        TagManager.Chains.Remove(target);
                    }));
                }

                var tTargets = (from t in TagManager.Tags
                                join i in TagNames on t.Name equals i
                                select t).ToList();

                foreach (var target in tTargets)
                {
                    sequence.Add(new Task(() =>
                    {
                        TagManager.Tags.Remove(target);
                    }));
                }

                sequence.Add(new Task(() => ImageTagFacade.DeleteByTagName(tagName)));
                sequence.Add(new Task(() => TagFacade.DeleteByTagName(tagName)));
            }
        }

        public override void ConfigurePostTaskAction(AsyncTaskSequence sequence)
        {
            sequence.Add(() => s_logger.Info($"Finish TagRemoving"));
        }

        [Obsolete]
        public static List<Task> GenerateRemoveTagTasks(ITagManager tagManager, string[] tagNames)
        {
            List<Task> tasks = new List<Task>();

            foreach (var tagName in tagNames)
            {
                var itTargets = (from it in tagManager.Chains
                                 join t in tagNames on it.Tag.Name equals t
                                 select it).ToList();

                foreach (var target in itTargets)
                {
                    tasks.Add(new Task(() =>
                    {
                        tagManager.Chains.Remove(target);
                    }));
                }

                var tTargets = (from t in tagManager.Tags
                                join i in tagNames on t.Name equals i
                                select t).ToList();

                foreach (var target in tTargets)
                {
                    tasks.Add(new Task(() =>
                    {
                        tagManager.Tags.Remove(target);
                    }));
                }

                tasks.Add(new Task(() => ImageTagFacade.DeleteByTagName(tagName)));
                tasks.Add(new Task(() => TagFacade.DeleteByTagName(tagName)));
            }

            return tasks;
        }
    }
}