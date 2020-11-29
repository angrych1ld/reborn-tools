using RebornTools.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RebornTools
{
    public class TooltipService
    {
        public event Action<Tooltip> TooltipChanged;

        private Tooltip currentTooltip;

        public void Display(Tooltip tooltip)
        {
            if (currentTooltip != tooltip)
            {
                currentTooltip = tooltip;
                TooltipChanged?.Invoke(tooltip);
            }
        }

        public void Hide(Tooltip tooltip)
        {
            if (currentTooltip == tooltip)
            {
                currentTooltip = null;
                TooltipChanged?.Invoke(currentTooltip);
            }
        }
    }

    public enum TooltipType
    {
        None = 0,
        Text = 1,
        Raidboss = 2
    }
}
