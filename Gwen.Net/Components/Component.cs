using System;
using System.Linq;
using Gwen.Net.Control;

namespace Gwen.Net.Components
{
    /// <summary>
    ///     Base class for all components. A component is a group of Gwen controls that handles user input of them.
    /// </summary>
    public abstract class Component : IDisposable
    {
        /// <summary>
        ///     Constructor for implementing the component from a Gwen control.
        /// </summary>
        /// <param name="view">Gwen control that will be a view of the component.</param>
        protected Component(ControlBase view)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));
            
            view.Component = this;

            View = view;
        }

        /// <summary>
        ///     Get the view of the component. View is a group of child controls or components that implements
        ///     the visual part of the component.
        /// </summary>
        public ControlBase View { get; private set; }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Create a new instance of component. Component must be created with Create function, not calling constructor.
        /// </summary>
        /// <typeparam name="T">Type of component.</typeparam>
        /// <param name="parent">Parent Gwen control of the component.</param>
        /// <param name="data">Optional data for a component constructor.</param>
        /// <returns>Created instance of the component.</returns>
        public static T Create<T>(ControlBase parent, params object[] data) where T : Component
        {
            return Create(typeof(T), parent, data) as T;
        }

        /// <summary>
        ///     Create a new instance of component by type. Component must be created with Create function, not calling
        ///     constructor.
        /// </summary>
        /// <param name="type">Type of component.</param>
        /// <param name="parent">Parent Gwen control of the component.</param>
        /// <param name="data">Optional data for a component constructor.</param>
        /// <returns>Created instance of the component.</returns>
        public static Component Create(Type type, ControlBase parent, params object[] data)
        {
            Component component;

            if (data.Length > 0)
            {
                component = Activator.CreateInstance(
                    type,
                    new object[]
                    {
                        parent
                    }.Concat(data).ToArray()) as Component;
            }
            else
            {
                component = Activator.CreateInstance(type, parent) as Component;
            }

            if (component == null)
            {
                throw new InvalidOperationException("Unable to create a component. Component is null.");
            }
            
            if (component.View == null)
            {
                throw new InvalidOperationException("Unable to create a component. Component contains no view.");
            }

            component.OnCreated();

            return component;
        }

        /// <summary>
        ///     Called when the view of the component is created. This is the right place to do initialization of the component
        ///     because Gwen controls are created at this point.
        /// </summary>
        /// <remarks>No need to call the base implementation.</remarks>
        protected virtual void OnCreated() {}

        /// <summary>
        ///     This function is called for every child control of the component. Child control in this context is a control
        ///     that is defined in the XML as a child element of the component, not child controls that implement the component.
        /// </summary>
        /// <param name="child">Child control.</param>
        /// <remarks>No need to call the base implementation.</remarks>
        protected virtual void OnChildAdded(ControlBase child) {}

        /// <summary>
        ///     Get a child control or component of this component.
        /// </summary>
        /// <typeparam name="T">Type of the control or component.</typeparam>
        /// <param name="name">Name.</param>
        /// <returns>Control or component.</returns>
        public T GetControl<T>(string name) where T : class
        {
            return GetControl(name) as T;
        }

        /// <summary>
        ///     Get a child control or component of this component.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <returns>Control or component.</returns>
        public object GetControl(string name)
        {
            if (View == null)
            {
                throw new InvalidOperationException("Unable to get a control. Component contains no view.");
            }

            ControlBase control = View.Name == name ? View : View.FindChildByName(name, recursive: true);

            if (control == null)
            {
                return null;
            }

            if (control.Component != null)
            {
                if (control.Component != this)
                {
                    return control.Component;
                }

                return control;
            }

            return control;
        }

        /// <summary>
        ///     Override this function if you want to handle all XML based events in the same function.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="handlerName">Element handler defined in the XML.</param>
        /// <param name="sender">Event sender control.</param>
        /// <param name="args">Event arguments.</param>
        /// <returns>True if the event was handled, false otherwise.</returns>
        /// <remarks>No need to call the base implementation.</remarks>
        public virtual bool HandleEvent(string eventName, string handlerName, ControlBase sender, EventArgs args)
        {
            return false;
        }

        /// <summary>
        ///     Get the type of the property value. Override this function if you want to implement all properties in the same
        ///     function.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">
        ///     Type of the property value. Return null if you want to handle string values from XML attributes by
        ///     yourself without conversion.
        /// </param>
        /// <returns>True if the property is a valid property, false otherwise.</returns>
        /// <remarks>No need to call the base implementation.</remarks>
        public virtual bool GetValueType(string name, out Type type)
        {
            type = null;

            return false;
        }

        /// <summary>
        ///     Get the value of the property. Override this function if you want to implement all property getters in the same
        ///     function.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="value">Value of the property.</param>
        /// <returns>True if the property value was successfully evaluated, false otherwise.</returns>
        /// <remarks>No need to call the base implementation.</remarks>
        public virtual bool GetValue(string name, out object value)
        {
            value = null;

            return false;
        }

        /// <summary>
        ///     Set the value of the property. Override this function if you want to implement all property setters in the same
        ///     function.
        ///     This is also used in the XML parser to set the property value if no dedicated property was found.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="value">Value of the property.</param>
        /// <returns>True if the property value was successfully set, false otherwise.</returns>
        /// <remarks>No need to call the base implementation. If you implement this, implement also <see cref="ValueType" />.</remarks>
        public virtual bool SetValue(string name, object value)
        {
            return false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (View == null) return;

            View.Component = null;
            View.Dispose();
            View = null;
        }
    }
}
