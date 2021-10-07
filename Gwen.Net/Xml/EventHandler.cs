using System;
using System.Reflection;
using Gwen.Net.Control;

namespace Gwen.Net.Xml
{
    /// <summary>
    ///     XML based event handler.
    /// </summary>
    /// <typeparam name="T">Type of event arguments.</typeparam>
    public class XmlEventHandler<T> where T : EventArgs
    {
        private readonly string m_eventName;
        private readonly string m_handlerName;

        private Type[] m_paramsType =
        {
            typeof(ControlBase),
            typeof(T)
        };

        public XmlEventHandler(string handlerName, string eventName)
        {
            m_eventName = eventName;
            m_handlerName = handlerName;
        }

        public void OnEvent(ControlBase sender, T args)
        {
            ControlBase handlerElement = sender.Parent;

            if (sender is Window)
            {
                handlerElement = sender;
            }
            else if (sender is TreeNode)
            {
                handlerElement = ((TreeNode)sender).TreeControl.Parent;
            }

            while (handlerElement != null)
            {
                if (handlerElement.Component != null)
                {
                    if (handlerElement.Component.HandleEvent(m_eventName, m_handlerName, sender, args))
                    {
                        break;
                    }

                    Type type = handlerElement.Component.GetType();

                    MethodInfo methodInfo = null;

                    do
                    {
                        MethodInfo[] methods = type.GetMethods(
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                        foreach (MethodInfo mi in methods)
                        {
                            if (mi.Name != m_handlerName)
                            {
                                continue;
                            }

                            ParameterInfo[] parameters = mi.GetParameters();

                            if (parameters.Length != 2)
                            {
                                continue;
                            }

                            if (parameters[0].ParameterType != typeof(ControlBase) ||
                                (parameters[1].ParameterType != typeof(T) &&
                                 parameters[1].ParameterType != typeof(T).BaseType))
                            {
                                continue;
                            }

                            methodInfo = mi;

                            break;
                        }

                        if (methodInfo != null)
                        {
                            break;
                        }

                        type = type.BaseType;
                    } while (type != null);

                    if (methodInfo != null)
                    {
                        methodInfo.Invoke(
                            handlerElement.Component,
                            new object[]
                            {
                                sender,
                                args
                            });

                        break;
                    }
                }

                if (handlerElement is Menu)
                {
                    Menu menu = handlerElement as Menu;

                    if (menu.ParentMenuItem != null)
                    {
                        handlerElement = menu.ParentMenuItem;
                    }
                    else
                    {
                        handlerElement = handlerElement.Parent;
                    }
                }
                else
                {
                    handlerElement = handlerElement.Parent;
                }
            }
        }
    }
}