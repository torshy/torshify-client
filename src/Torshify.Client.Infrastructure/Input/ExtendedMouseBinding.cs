using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Markup;

namespace Torshify.Client.Infrastructure.Input
{
    public class ExtendedMouseBinding : MouseBinding
    {
        [ValueSerializer(typeof(MouseGestureValueSerializer)), TypeConverter(typeof(ExtendedMouseGestureConverter))]
        public override InputGesture Gesture
        {
            get
            {
                return base.Gesture;
            }
            set
            {
                base.Gesture = value;
            }
        }
    }
}