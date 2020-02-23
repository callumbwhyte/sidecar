using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Umbraco.Core;
using Umbraco.Core.Events;

namespace Our.Umbraco.Sidecar.BackgroundTasks
{
    public class AnyTask
    {
        public void Main([SidecarEvent("ContentService.Saved")] ContentSavedEventArgs args)
        {
            
        }
    }




    public class HandleEventTasks
    {
        public void Main()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var methods = assemblies
                .SelectMany(x => x.GetTypes())
                .SelectMany(x => x.GetMethods());

            var attributeMethods = methods.Where(x => x.GetAttributes<SidecarEventAttribute>().Any() == true);

            var paramMethods = methods.Where(x => x.GetParameterAttributes<SidecarEventAttribute>().Any() == true);
        }
    }






    public static class TypeExtensions
    {
        public static ParameterInfo[] GetParameters(this EventInfo eventInfo)
        {
            var handler = eventInfo.EventHandlerType;

            var method = handler.GetMethod("Invoke");

            return method.GetParameters();
        }

        public static IEnumerable<T> GetAttributes<T>(this MethodInfo method)
            where T : Attribute
        {
            return method
                .GetCustomAttributes<T>()
                .Where(x => x != null);
        }

        public static IEnumerable<T> GetParameterAttributes<T>(this MethodInfo method)
            where T : Attribute
        {
            return method
                .GetParameters()
                .Select(x => x.GetCustomAttribute<T>())
                .Where(x => x != null);
        }
    }





    [AttributeUsage(AttributeTargets.Parameter)]
    public class SidecarEventAttribute : Attribute
    {
        public SidecarEventAttribute(string eventName)
        {
            EventName = eventName;
        }

        public string EventName { get; set; }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class SidecarTimerAttribute : Attribute
    {
        public SidecarTimerAttribute(int interval)
        {
            Interval = interval;
        }

        public int Delay { get; set; }

        public int Interval { get; set; } = 60 * 1000;
    }
}