using System;
using System.Reflection;

namespace Sidecar.TriggerApp
{
    public class DebugHook
    {
        public static void MyFunction(object sender, string name)
        {
            Console.WriteLine(name);
        }
    }

    public class ContentService
    {
        public static event EventHandler Saved;

        public void Run()
        {
            Saved(this, new EventArgs());
        }
    }



    class Program
    {
        static void Main(string[] args)
        {
            var targetEvent = typeof(ContentService).GetEvent("Saved");

            var eventHandler = targetEvent.EventHandlerType;

            // construct target
            Action<object, EventArgs> targetAction = (sender, eventArgs) => HandleEvent<DebugHook>("MyFunction", sender, "Hello world");

            // register event
            var handler = Delegate.CreateDelegate(eventHandler, null, targetAction.Method);

            targetEvent.AddEventHandler(new Program(), handler);

            // fire event
            new ContentService().Run();

            Console.ReadLine();
        }

        private static void HandleEvent<T>(string methodName, params object[] parameters)
            where T : class, new()
        {
            var targetMethod = typeof(T).GetMethod(methodName);

            // TODO: send only required params
            var targetParameters = parameters;

            targetMethod.Invoke(new T(), targetParameters);
        }
    }
}