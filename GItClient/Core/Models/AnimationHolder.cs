using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace GItClient.Core.Models
{
    internal class AnimationHolder
    {
        public DoubleAnimation Height;
        public DoubleAnimation Width;
    
        public AnimationHolder(DoubleAnimation h, DoubleAnimation w)
        {
            Height = h;
            Width = w;
        }
    }
}
