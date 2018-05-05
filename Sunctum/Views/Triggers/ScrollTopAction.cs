

using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Sunctum.Views.Triggers
{
    public class ScrollTopAction : TargetedTriggerAction<ListView>
    {
        protected override void Invoke(object parameter)
        {
            if (Target.Items.Count > 0)
            {
                var svAutomation = UIElementAutomationPeer.CreatePeerForElement(Target);
                var scrollInterface = svAutomation.GetPattern(PatternInterface.Scroll) as IScrollProvider;
                scrollInterface.SetScrollPercent(scrollInterface.VerticalScrollPercent, 0.0);
                Target.SelectedIndex = 0;
            }
        }
    }
}
