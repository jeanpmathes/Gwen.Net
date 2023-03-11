using System;
using System.Linq;
using Gwen.Net.Control.Layout;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Radio button group.
    /// </summary>
    public class RadioButtonGroup : VerticalLayout
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RadioButtonGroup" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public RadioButtonGroup(ControlBase parent) : base(parent)
        {
            IsTabable = false;
            KeyboardInputEnabled = true;
        }

        /// <summary>
        ///     Selected radio button.
        /// </summary>
        public LabeledRadioButton Selected { get; private set; }

        /// <summary>
        ///     Internal name of the selected radio button.
        /// </summary>
        public string SelectedName => Selected.Name;

        /// <summary>
        ///     Text of the selected radio button.
        /// </summary>
        public string SelectedLabel => Selected.Text;

        /// <summary>
        ///     Index of the selected radio button.
        /// </summary>
        public int SelectedIndex => Children.IndexOf(Selected);

        /// <summary>
        ///     Invoked when the selected option has changed.
        /// </summary>
        public event GwenEventHandler<ItemSelectedEventArgs> SelectionChanged;

        /// <summary>
        ///     Adds a new option.
        /// </summary>
        /// <param name="text">Option text.</param>
        /// <param name="optionName">Internal name.</param>
        /// <param name="userData">User data.</param>
        /// <returns>Newly created control.</returns>
        public virtual LabeledRadioButton AddOption(string text, string optionName = null, object userData = null)
        {
            LabeledRadioButton lrb = new(this);
            lrb.Name = optionName;
            lrb.UserData = userData;
            lrb.Text = text;
            lrb.Checked += OnRadioClicked;
            lrb.Margin = new Margin(left: 0, top: 0, right: 0, bottom: 1);
            lrb.KeyboardInputEnabled = false;
            lrb.IsTabable = true;

            return lrb;
        }

        /// <summary>
        ///     Adds an option.
        /// </summary>
        /// <param name="lrb">Radio button.</param>
        public virtual void AddOption(LabeledRadioButton lrb)
        {
            lrb.Checked += OnRadioClicked;
            lrb.Margin = new Margin(left: 0, top: 0, right: 0, bottom: 1);
            lrb.KeyboardInputEnabled = false;
            lrb.IsTabable = true;
        }

        /// <summary>
        ///     Handler for the option change.
        /// </summary>
        /// <param name="fromPanel">Event source.</param>
        protected virtual void OnRadioClicked(ControlBase fromPanel, EventArgs args)
        {
            var @checked = fromPanel as RadioButton;

            foreach (LabeledRadioButton rb in Children.OfType<LabeledRadioButton>()) // todo: optimize
            {
                if (rb.RadioButton == @checked)
                {
                    Selected = rb;
                }
                else
                {
                    rb.RadioButton.IsChecked = false;
                }
            }

            OnChanged(Selected);
        }

        protected virtual void OnChanged(ControlBase NewTarget)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged.Invoke(this, new ItemSelectedEventArgs(NewTarget));
            }
        }

        /// <summary>
        ///     Selects the specified option.
        /// </summary>
        /// <param name="index">Option to select.</param>
        public void SetSelection(int index)
        {
            if (index < 0 || index >= Children.Count)
            {
                return;
            }

            (Children[index] as LabeledRadioButton).RadioButton.Press();
        }

        /// <summary>
        ///     Selects the specified option.
        /// </summary>
        /// <param name="name">Option name to select.</param>
        public void SetSelectionByName(string name)
        {
            ControlBase child = FindChildByName(name);

            if (child != null)
            {
                (child as LabeledRadioButton).RadioButton.Press();
            }
        }

        /// <summary>
        ///     Selects the specified option with the given user data it finds.
        /// </summary>
        /// <param name="userdata">
        ///     The UserData to look for. The equivalency check uses "param.Equals(item.UserData)".
        ///     If null is passed in, it will look for null/unset UserData.
        /// </param>
        public void SelectByUserData(object userdata)
        {
            ControlBase option = Children.Where(x => x.UserData.Equals(userdata)).FirstOrDefault();

            if (option != null)
            {
                (option as LabeledRadioButton).RadioButton.Press();
            }
        }
    }
}
