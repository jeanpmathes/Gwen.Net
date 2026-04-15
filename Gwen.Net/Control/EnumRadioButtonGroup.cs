using System;

namespace Gwen.Net.Control
{
    public class EnumRadioButtonGroup<T> : RadioButtonGroup where T : struct
    {
        public EnumRadioButtonGroup(ControlBase parent) : base(parent)
        {
            if (!typeof(T).IsEnum)
            {
                throw new Exception("T must be an enumerated type!");
            }

            for (var i = 0; i < Enum.GetValues(typeof(T)).Length; i++)
            {
                String name = Enum.GetNames(typeof(T))[i];
                LabeledRadioButton lrb = AddOption(name);
                lrb.UserData = Enum.GetValues(typeof(T)).GetValue(i);
            }
        }

        public T SelectedValue
        {
            get => (T) Selected.UserData;
            set
            {
                foreach (ControlBase child in Children)
                {
                    if (child is LabeledRadioButton && child.UserData.Equals(value))
                    {
                        (child as LabeledRadioButton).RadioButton.Press();
                    }
                }
            }
        }
    }
}
